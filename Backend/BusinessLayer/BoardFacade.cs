using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IntroSE.Kanban.Backend.Data_accsses_layer;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;




namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class BoardFacade
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int inProgressTask = 1;
        private const int Backlog = 0;
        private const int InProgress = 1;
        private readonly int Done = 2;
        public BoardController BoardCon {  get;private set; }

        public Dictionary<string, Dictionary<string, BoardBL>> Boards { get; private set; }
        public Dictionary<int, BoardBL> BoardId { get; private set; } 
        private AuthenticationFacade authFacade;
        private int boardId = 1;


        public BoardFacade(AuthenticationFacade authFacade)
        {
            this.authFacade = authFacade;
            this.BoardId = new Dictionary<int, BoardBL>();
            this.Boards = new Dictionary<string, Dictionary<string, BoardBL>>();
            this.BoardCon = new BoardController();

            
        }
        /// <summary>
        /// This method loads all the boards from the database and adds them to the Boards dictionary.
        /// </summary>
        public void LoadDataBoards()
        {
            log.Info("Loading boards from the database.");
            BoardController boardController = new BoardController();
            BoardUserController boardUserController = new BoardUserController();
            List<BoardDTO> boardDTOs = boardController.Select();
            foreach (BoardDTO boardDTO in boardDTOs)
            {
                BoardBL boardBL = new BoardBL(boardDTO.BoardOwner, boardDTO.BoardName, boardDTO.BoardId, authFacade);
                BoardId.Add(boardBL.BoardId, boardBL);
                List<string> members = boardUserController.SelectAllEmail(boardDTO.BoardId);
                foreach (string member in members)
                {
                    if (Boards.ContainsKey(member) == false)
                    {
                        Boards.Add(member, new Dictionary<string, BoardBL>());
                    }
                    Boards[member].Add(boardDTO.BoardName, boardBL);
                    boardBL.BoardMembers.Add(member, member);

                }
                boardBL.boardBLLoadData();                
            }
            finalIdBoard();
        }
        /// <summary>
        /// This method deletes all boards and their associated data from the database. It clears the Boards dictionary and removes all entries from the BoardId dictionary.
        /// </summary>
        public void deleteDataBoards()
        {
            BoardController boardController = new BoardController();
            BoardUserController boardUserController = new BoardUserController();
            ColumnController columnController = new ColumnController();
            TaskController taskController = new TaskController();
            boardController.DeleteAll();
            boardUserController.DeleteAll();
            columnController.DeleteAll();
            taskController.DeleteAll();
            BoardId.Clear();
            Boards.Clear();
            this.boardId=1; // Reset the boardId to 1 after deleting all boards
            log.Info("All boards and associated data have been deleted successfully.");

        }
        /// <summary>
        /// This method calculates the final board ID by iterating through all existing boards and finding the maximum ID.
        /// </summary>
        private void finalIdBoard()
        {
            log.Info("Calculating the final board ID.");
            int finalId = 0;
            BoardController boardController = new BoardController();
            List<BoardDTO> boardDTO1 = boardController.Select();
            foreach (BoardDTO board in boardDTO1)
            {
                if (finalId < board.BoardId)
                {
                    finalId = board.BoardId;
                    
                }
            }
            boardId = finalId + 1;
        }
        /// <summary>
        /// Adds a new task to a specified board for a user and returns the task details.
        /// </summary>
        /// <param name="email">Identifies the user for whom the task is being added.</param>
        /// <param name="boardName">Specifies the board where the task will be created.</param>
        /// <param name="title">Provides the title of the new task being added.</param>
        /// <param name="description">Gives a detailed explanation of the task's purpose.</param>
        /// <param name="dueDate">Indicates the deadline for completing the task.</param>
        /// <returns>Returns an object containing details of the newly created task.</returns>
        public TaskBL addTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            log.Info($"Adding task to board {boardName} for user {email}.");
            BoardBL board = GetBoard(email, boardName);
            TaskBL newTask = board.AddTask(email, boardName, title, description, dueDate);
            return newTask;
        }
        /// <summary>
        /// Updates the due date of a specified task for a user on a given board and returns the updated task details.
        /// </summary>
        /// <param name="email">Identifies the user whose task is being updated.</param>
        /// <param name="boardName">Specifies the board where the task is located.</param>
        /// <param name="columnOrdinal">Indicates the position of the task within the board's columns.</param>
        /// <param name="taskID">Represents the unique identifier of the task to be updated.</param>
        /// <param name="dueDate">Sets the new due date for the specified task.</param>
        /// <returns>Returns an object containing the updated task's details.</returns>
        public TaskBL UpdateTaskDueDate(string email, string boardName, int columnOrdinal, long taskID, DateTime dueDate)
        {
            log.Info($"Updating task due date for user {email} on board {boardName}.");
            BoardBL board = GetBoard(email, boardName);
            board.GetTask(email, columnOrdinal, taskID).updateTask(email, null, null, dueDate);
            TaskBL task = board.GetTask(email, columnOrdinal, taskID);
            return task;
        }
        /// <summary>
        /// Updates the title of a specified task within a board and returns the updated task details.
        /// </summary>
        /// <param name="email">Identifies the user making the request to access the board.</param>
        /// <param name="boardName">Specifies the name of the board containing the task to be updated.</param>
        /// <param name="columnOrdinal">Indicates the position of the column where the task is located.</param>
        /// <param name="taskID">Represents the unique identifier of the task that needs its title updated.</param>
        /// <param name="title">Contains the new title to be assigned to the specified task.</param>
        /// <returns>Returns an object containing the updated task's details.</returns>
        public TaskBL UpdateTaskTitle(string email, string boardName, int columnOrdinal, long taskID, string title)
        {
            BoardBL board = GetBoard(email, boardName);
            board.GetTask(email, columnOrdinal, taskID).updateTask(email, title, null, default(DateTime));

            TaskBL task = board.GetTask(email, columnOrdinal, taskID);
            return task;
        }
        /// <summary>
        /// Updates the description of a specific task on a board for a user and returns the updated task details.
        /// </summary>
        /// <param name="email">Identifies the user whose task description is being updated.</param>
        /// <param name="boardName">Specifies the name of the board where the task is located.</param>
        /// <param name="columnOrdinal">Indicates the position of the column containing the task.</param>
        /// <param name="taskID">Represents the unique identifier of the task being updated.</param>
        /// <param name="description">Contains the new description to be assigned to the task.</param>
        /// <returns>Returns an object containing the updated task's details.</returns>
        public TaskBL UpdateTaskDescription(string email, string boardName, int columnOrdinal, long taskID, string description)
        {
            log.Info($"Updating task description for user {email} on board {boardName}.");
            BoardBL board = GetBoard(email, boardName);
            board.GetTask(email, columnOrdinal, taskID).updateTask(email, null, description, default(DateTime));
            TaskBL task = board.GetTask(email, columnOrdinal, taskID);
            return task;
        }
        /// <summary>
        /// Removes a specified task from a user's board based on the task ID and column ordinal.
        /// </summary>
        /// <param name="email">Identifies the user associated with the task to be deleted.</param>
        /// <param name="boardName">Specifies the name of the board from which the task will be removed.</param>
        /// <param name="columnOrdinal">Indicates the position of the column where the task is located.</param>
        /// <param name="taskID">Represents the unique identifier of the task that is to be deleted.</param>
        public void DeleteTask(string email, string boardName, int columnOrdinal, long taskID)
        {
            log.Info($"Deleting task with ID {taskID} for user {email} on board {boardName}.");
            BoardBL board = GetBoard(email, boardName);
            board.deleteTask(email, columnOrdinal, taskID);
        }
        /// <summary>
        /// Assigns a task to a new assignee on a specified board and column.
        /// </summary>
        /// <param name="taskId">Identifies the specific task to be reassigned.</param>
        /// <param name="boardName">Specifies the name of the board where the task is located.</param>
        /// <param name="columnNum">Indicates the column number where the task is currently placed.</param>
        /// <param name="currAssignee">Denotes the current assignee of the task being reassigned.</param>
        /// <param name="newAssignee">Represents the new assignee to whom the task will be assigned.</param>
        public void AssignTask(long taskId, string boardName, int columnNum, string currAssignee, string newAssignee)
        {
            log.Info($"Assigning task with ID {taskId} on board {boardName} from {currAssignee} to {newAssignee}.");
            BoardBL board = GetBoard(currAssignee, boardName);
            board.AssignTask(taskId, columnNum, currAssignee, newAssignee);
        }
        /// <summary>
        /// Sets a limit on a specified column of a board for a user. It logs the action for tracking purposes.
        /// </summary>
        /// <param name="email">Identifies the user for whom the column limit is being set.</param>
        /// <param name="boardName">Specifies the name of the board where the column limit will be applied.</param>
        /// <param name="columnNum">Indicates which column on the board will have its limit adjusted.</param>
        /// <param name="limit">Defines the maximum number of items allowed in the specified column.</param>
        public void LimitColumn(string email, string boardName, int columnNum, int limit)
        {
            log.Info($"Setting limit for column {columnNum} on board {boardName} for user {email} to {limit}.");
            BoardBL board = GetBoard(email, boardName);
            board.LimitColumn(email, columnNum, limit);
        }
        /// <summary>
        /// Retrieves the limit for a specified column in a board for a given user.
        /// </summary>
        /// <param name="email">Identifies the user whose board access is being checked.</param>
        /// <param name="boardName">Specifies the name of the board from which to retrieve the column limit.</param>
        /// <param name="coulmnNum">Indicates the specific column number for which the limit is being requested.</param>
        /// <returns>Returns the limit as an integer for the specified column.</returns>
        public int GetColumnLimit(string email, string boardName, int coulmnNum)
        {
            BoardBL board = GetBoard(email, boardName);
            int limit = board.GetColumnLimit(email, coulmnNum);
            return limit;

        }
        /// <summary>
        /// Advances a task to a new column on a specified board for a user.
        /// </summary>
        /// <param name="email">Identifies the user associated with the task being advanced.</param>
        /// <param name="boardName">Specifies the name of the board where the task is located.</param>
        /// <param name="columnOrdinal">Indicates the target column's position for the task advancement.</param>
        /// <param name="taskID">Represents the unique identifier of the task to be advanced.</param>
        public void AdvanceTask(string email, string boardName, int columnOrdinal, long taskID)
        {
            log.Info($"Advancing task with ID {taskID} for user {email} on board {boardName}.");

            BoardBL board = GetBoard(email, boardName);
            board.advanceTask(email, columnOrdinal, taskID);
        }
        /// <summary>
        /// Retrieves a list of tasks that are currently in progress for a specified user.
        /// </summary>
        /// <param name="email">The user's email address is used to filter tasks associated with that user.</param>
        /// <returns>A list of task objects that represent the in-progress tasks for the specified user.</returns>
        public List<TaskBL> InProgressTasksService(string email)
        {
            List<TaskBL> tasks = InProgressTasks(email);
            return tasks;
        }
        /// <summary>
        /// Retrieves a list of board services associated with a specific user.
        /// </summary>
        /// <param name="email">Used to identify the user whose boards and tasks are being retrieved.</param>
        /// <returns>A list of board services containing tasks organized by columns.</returns>
        public List<BoardBL> getBoardsService(string email)
        {
            List<BoardBL> boards = getBoards(email);
            return boards;
        }
        /// <summary>
        /// Retrieves the name of a specific column for a user on a given board.
        /// </summary>
        /// <param name="email">Identifies the user for whom the column name is being retrieved.</param>
        /// <param name="boardName">Specifies the board from which the column name is requested.</param>
        /// <param name="columnOrdinal">Indicates the position of the column within the board.</param>
        /// <returns>Returns the name of the specified column.</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            log.Info($"Retrieving column name for user {email} on board {boardName} at column {columnOrdinal}.");
            BoardBL board = GetBoard(email, boardName);
            string columnName = board.GetColumnName(email, columnOrdinal);
            return columnName;
        }
        /// <summary>
        /// Retrieves the name of a board based on its identifier.
        /// </summary>
        /// <param name="boardId">The identifier used to locate the specific board.</param>
        /// <returns>The name of the board as a string.</returns>
        public string getBoardNameService(int boardId)
        {
            BoardBL board = getBoardsById(boardId);
            string boardName = board.BoardName;
            return boardName;
        }
        /// <summary>
        /// Transfers ownership of a board from the current owner to a new owner.
        /// </summary>
        /// <param name="boardName"></param>
        /// <param name="currOwner"></param>
        /// <param name="newOwner"></param>
        public void TransferOwnership(String boardName, String currOwner, String newOwner)
        {
            log.Info($"Transferring ownership of board {boardName} from {currOwner} to {newOwner}.");
            BoardBL board = GetBoard(currOwner, boardName);
            board.TransferOwnership(currOwner, newOwner);
        }
        /// <summary>
        /// Retrieves a list of tasks from a specified column in a board for a given user.
        /// </summary>
        /// <param name="email">Identifies the user whose tasks are being retrieved.</param>
        /// <param name="boardName">Specifies the name of the board from which tasks are fetched.</param>
        /// <param name="columnOrdinal">Indicates the specific column from which tasks should be retrieved.</param>
        /// <returns>A list of task objects containing details like ID, title, description, due date, and creation time.</returns>
        public List<TaskBL> GetColumnTask(string email, string boardName, int columnOrdinal)
        {
            BoardBL board = GetBoard(email, boardName);
            List<TaskBL> tasks = board.GetColumnTask(email, columnOrdinal);
            return tasks;
        }
       
        /// <summary>
        /// This method creates a new board for the user.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public BoardBL CreateBoard(String email, String name)
        {
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The email {email} does not exist");
                throw new Exception("The email not exist");
            }           
            if (Boards.ContainsKey(email) == false)
            {
                Boards.Add(email, new Dictionary<string, BoardBL>());
            }
            if (Boards[email].ContainsKey(name))
            {
                log.Error($"Board with name {name} already exists for user {email}.");
                throw new Exception("Board with this name already exists");
            }
            if(string.IsNullOrWhiteSpace(name))
            {
                log.Error($"Invalid board name: {name} for user {email}.");
                throw new Exception("invalid board name");
            }
            log.Info($"Creating board with ID {boardId} for user {email} with name {name}");
            BoardBL board = new BoardBL(email, name, boardId, authFacade);
            BoardId.Add(boardId, board);
            JoinBoard(board.BoardId, email);
            boardId = boardId + 1;
            board.BoardDTO.Save();
            return board;

        }

        /// <summary>
        /// Retrieves a board object based on its identifier. Throws an exception if the identifier is invalid.
        /// </summary>
        /// <param name="boardId">The identifier used to look up a specific board in the collection.</param>
        /// <returns>Returns the board object associated with the given identifier.</returns>
        /// <exception cref="Exception">Thrown when the provided identifier does not exist in the collection.</exception>
        public BoardBL getBoardsById(int boardId)
        {
            log.Info($"Retrieving board with ID {boardId}.");

            if (BoardId.ContainsKey(boardId) == false)
            {
                throw new Exception("InVaild boardID");
            }
            BoardBL userBoard = BoardId.GetValueOrDefault(boardId);
            return userBoard;
        }
        /// <summary>
        /// Allows a user to join a board if they are authenticated and the board exists.
        /// </summary>
        /// <param name="boardId">Identifies the specific board that the user is attempting to join.</param>
        /// <param name="email">Represents the user's email address for authentication and membership purposes.</param>
        /// <exception cref="Exception">Thrown when the user is not authenticated, the board ID is invalid, or the user is already a member of the
        /// board.</exception>
        public void JoinBoard(int boardId, string email)
        {
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The user with email {email} is not authenticated.");
                throw new Exception("the user is not exist");
                
            }
            BoardId.TryGetValue(boardId, out BoardBL board);
            if (board == null)
            {
                log.Error($"Invalid board ID: {boardId}.");
                throw new Exception("InVaild boardID");
            }
            else
            {
                 board.JoinBoard(email);

                if (!Boards.ContainsKey(email))
                {
                    log.Info($"Adding new board {board.BoardName} for user {email}.");
                    Boards[email] = new Dictionary<string, BoardBL>();
                }
                log.Info($"User {email} is joining board {board.BoardName} with ID {boardId}.");
                Boards[email].Add(board.BoardName, board);
                    
            }
            
        }

        /// <summary>
        /// This method allows a user to leave a board. It checks if the user is authenticated and if they are a member of the board before allowing them to leave.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="email"></param>
        /// <exception cref="Exception"></exception>
        public void LeaveBoard(int boardId, string email)
        {
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The user with email {email} is not authenticated.");
                throw new Exception("the user is not exist");

            }
            BoardBL board = getBoardsById(boardId); 
            if(board.BoardMembers.ContainsKey(email) == false)
            {
                log.Error($"User {email} is not a member of the board {board.BoardName}.");
                throw new Exception("User is not in the board");
            }
            if (email == board.BoardOwner)
            {
                log.Error($"User {email} is the owner of the board {board.BoardName} and cannot leave.");
                throw new Exception("Board owner cannot leave the board");
            }
            log.Info($"User {email} is leaving board {board.BoardName} with ID {boardId}.");
            board.LeaveBoard(email);
            Boards[email].Remove(board.BoardName);
           


        }



        /// <summary>
        ///     This method deletes a board for the user.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void DeleteBoard(String email, string name)
        {

            log.Info($"User {email} is trying to delete board {name}");
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The email {email} does not exist");
                throw new Exception("The email not exist");
            }
            if (Boards.ContainsKey(email) == false)
            {
                log.Error($"User {email} does not have this board");
                throw new Exception("User does not have this board");
            }
            if (Boards[email].ContainsKey(name) == false)
            {
                log.Error($"Board {name} does not exist for user {email}");
                throw new Exception("Board does not exist");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                log.Error($"Invalid board name: {name} for user {email}");
                throw new Exception("invalid board name");
            }
            if (email != GetBoard(email, name).BoardOwner)
            {
                log.Error($"Invalid member: {email} for delete {name}");
                throw new Exception("Only BoardOwner can delete the board");
            }


            BoardBL board2 = GetBoard(email, name);
            board2.BoardMembers.Remove(email);
            BoardId.Remove(board2.BoardId);
            Boards[email].Remove(name);


            BoardUserController boardUserController = new BoardUserController();
            boardUserController.Delete(board2.BoardId, email);
            TaskController TaskController= new TaskController();
            TaskController.DeleteByBoard(board2.BoardId);
            ColumnController columnController = new ColumnController();
            columnController.Delete(board2.BoardId);
            BoardController boardController = new BoardController();
            boardController.Delete(board2.BoardId);
            log.Info($"Board {name} deleted successfully for user {email}.");
        }

        /// <summary>
        ///     This method returns a list of all tasks that are in progress for the user.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public List<TaskBL> InProgressTasks(String email)
        {
            log.Info($"Retrieving in-progress tasks for user {email}");
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The email {email} does not exist");
                throw new Exception("The email not exist");
            }
            List<TaskBL> allTasks = new List<TaskBL>();
            if (Boards.ContainsKey(email) == false)
            {
                log.Error($"User {email} does not have any boards");
                throw new Exception("User does not have any boards");
            }
            Dictionary<string, BoardBL> userBoard = Boards.GetValueOrDefault(email);
            foreach (var board in userBoard)
            {
                allTasks.AddRange(board.Value.GetColumnTask(email, inProgressTask));
            }
            List<TaskBL> inProgressTasks = new List<TaskBL>();
            foreach (TaskBL task2 in allTasks)
            {
                if (task2.Assignee != null && task2.Assignee.Equals(email))
                {
                    inProgressTasks.Add(task2);
                }
            }
            log.Info($"Found {inProgressTasks.Count} in-progress tasks for user {email}");
            return inProgressTasks;

        }
        /// <summary>
        /// /// This method returns a list of all boards that the user has.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<BoardBL> getBoards(string email)
        {
            log.Info($"Retrieving boards for user {email}.");
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The email {email} does not exist.");
                throw new Exception("The email not exist");
            }
            List<BoardBL> userBoards = new List<BoardBL>();
            if (Boards.ContainsKey(email) == false)
            {
                return userBoards; // Return an empty list if the user has no boards
            }
            Dictionary<string, BoardBL> userBoard = Boards.GetValueOrDefault(email);
            foreach (var board in userBoard)
            {
                userBoards.Add(board.Value);
            }
            log.Info($"Retrieved {userBoards.Count} boards for user {email}.");
            return userBoards;
        }
        /// <summary>
        /// This method returns a list of all board IDs that the user has.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<int> getUserBoardsID(string email)
        {
            log.Info($"Retrieving board IDs for user {email}.");
            List<BoardBL> boards = getBoards(email);
            List<int> boardsIDs = new List<int>();
            foreach (BoardBL board in boards)
            {
                boardsIDs.Add(board.BoardId);
            }
            return boardsIDs;
        }

        /// <summary>
        /// This method returns a list of all board members that the board has.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <returns></returns>
        public List<string> GetBoardMembers(string email, string boardName)
        {
            log.Info($"Retrieving board members for board {boardName}.");
            BoardBL board = GetBoard(email, boardName);
            List<string> boardMembers = board.BoardMembers.Values.ToList();
            return boardMembers;
        }

        /// <summary>
        /// This method returns a specific board for the user.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public BoardBL GetBoard(string email, string boardName)
        {
            log.Info($"Getting board {boardName} for user {email}");
            if (this.authFacade.IsLoginIn(email) == false)
            {
                log.Error($"The email {email} does not exist");
                throw new KeyNotFoundException("The email not exist");
            }
            if (Boards.ContainsKey(email) == false)
            {
                log.Error($"User {email} does not have any boards");
                throw new Exception("User does not have any boards");
            }
            if (Boards[email].ContainsKey(boardName) == false)
            {
                log.Error($"Board {boardName} does not exist for user {email}");
                throw new KeyNotFoundException("Board does not exist");
            }
            if (string.IsNullOrWhiteSpace(boardName))
            {
                log.Error($"Invalid board name: {boardName}");
                throw new Exception("invalid board name");
            }
            log.Info($"Board {boardName} retrieved successfully for user {email}");
            return Boards[email][boardName];
        }
    }
}

