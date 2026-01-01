using IntroSE.Kanban.Backend.Data_accsses_layer;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
[assembly: InternalsVisibleTo("BackendUnitTests")]


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class BoardBL
    {
        
        private const int Backlog = 0;
        private const int InProgress = 1;
        private readonly int Done = 2;
        private const int MaxTitleLength = 50;
        private const int MaxDescriptionLength = 300;
        private const int Unlimited = -1;
        public Dictionary<string, string> BoardMembers { get; private set; }

        public string BoardOwner { get; private set; }

        public int BoardId { get; private set; }
        public string BoardName { get; private set; }
        public ColumnBL BackLogColumn { get; protected set; }
        public ColumnBL InProgressColumn { get; protected set; }
        public ColumnBL DoneColumn { get; protected set; }
        public BoardDTO BoardDTO { get;private set; }

        private long taskID = 1;
        private AuthenticationFacade authFacade;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BoardBL(string email, string boardName,int boardId, AuthenticationFacade authFacade)
        {
            this.BoardName = boardName;
            this.BoardOwner = email;
            this.BoardId = boardId;
            this.BoardDTO = new BoardDTO(BoardId, boardName, email);
            this.BoardMembers = new Dictionary<string, string>();
            this.authFacade = authFacade;
            this.BackLogColumn = new ColumnBL("Backlog", Unlimited, authFacade);
            this.InProgressColumn = new ColumnBL("In Progress", Unlimited, authFacade);
            this.DoneColumn = new ColumnBL("Done", Unlimited, authFacade);
            BoardDTO.addColumn(BackLogColumn.ColumnDTO);
            BoardDTO.addColumn(InProgressColumn.ColumnDTO);
            BoardDTO.addColumn(DoneColumn.ColumnDTO);
        }

        /// <summary>
        /// This method initializes the task ID for the board by finding the highest existing task ID and incrementing it by one.
        /// </summary>
        private void finalIdTask()
        {
            log.Info($"Calculating final task ID for board {BoardId}");
            long finalId = 0;
            TaskController taskController = new TaskController();
            List<TaskDTO> taskDTO1 = taskController.SelectBoard(BoardId);
            foreach (TaskDTO task in taskDTO1)
            {
                if(finalId < task.Id)
                {
                    finalId = task.Id;
                }
            }
            taskID =  finalId + 1;
        }
        /// <summary>
        /// Loads board data for a specific board ID and initializes columns based on the retrieved data. It also logs
        /// the loading process.
        /// </summary>
        public void boardBLLoadData()
        {
            log.Info($"Loading board data for board ID {BoardId}");
            ColumnController columnController = new ColumnController();
            List<ColumnDTO> list = columnController.SelectByBoard(BoardId);
            foreach (ColumnDTO columnDTO in list)
            {
                ColumnBL columnBL = new ColumnBL(columnDTO, authFacade);

                switch (columnDTO.ColumnNum)
                {
                    case 0:
                        BackLogColumn = columnBL;
                        break;

                    case 1:
                        InProgressColumn = columnBL;
                        break;

                    case 2:
                        DoneColumn = columnBL;
                        break;
                }
            }
            finalIdTask();
        }
        /// <summary>
        /// Allows a user to join a board if they are not already a member.
        /// </summary>
        /// <param name="email">Specifies the email address of the user attempting to join the board.</param>
        /// <returns>Returns the current instance of the board after adding the new member.</returns>
        /// <exception cref="Exception">Thrown when the user is already a member of the board.</exception>
        public void JoinBoard(string email)
        {
            
            if (BoardMembers.ContainsKey(email))
            {
                log.Error($"User {email} is already a member of the board {BoardName}.");
                throw new Exception("User is alrady in the board");
            }
            
            BoardMembers.Add(email, email);
            BoardUserDTO boardUserDal = new BoardUserDTO(BoardId, email);
            boardUserDal.save();

            
           

        }
        /// <summary>
        /// This method adds a new task to the board.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">New title for the task</param>
        /// <param name="description">New description for the task</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TaskBL AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            log.Info($"Task {taskID} is being added by {email}");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                log.Error($"The title can't be empty or only spaces");
                throw new Exception("the title can not be empty or only spaces");
            }
            if (title.Length > MaxTitleLength)
            {
                log.Error($"The title can't be with more than 50 characters");
                throw new Exception("the title can't be with more than 50 characters");
            }
            if (description.Length > MaxDescriptionLength || (string.IsNullOrWhiteSpace(description) && description.Length!=0))
            {
                log.Error($"The description can't be with more than 300 characters or only spaces");
                throw new Exception("the description can not be with more than 300 characters");
            }

            if (dueDate < DateTime.Now)
            {
                log.Error($"Due date cannot be in the past");
                throw new Exception("Due date cannot be in the past");
            }

            TaskBL task = BackLogColumn.AddTask(email, taskID, Backlog, title, description, dueDate);
            this.taskID++;
            return task;


        }
        /// <summary>
        /// Handles the process of a user leaving a board, ensuring they are logged in and not the board owner.
        /// </summary>
        /// <param name="email">Identifies the user attempting to leave the board.</param>
        /// <exception cref="Exception">Thrown when the user is not logged in, is the board owner, or is not a member of the board.</exception>
        public void LeaveBoard(string email)
        {
            log.Info($"User {email} is leaving the board");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (BoardMembers.ContainsKey(email))
            {
                if(email.Equals(BoardOwner))
                {
                    log.Error($"The owner of the board can not leave the board");
                    throw new Exception("Owner cannot leave his board");
                }
                else
                {
                    BackLogColumn.LeaveBoard(email);
                    InProgressColumn.LeaveBoard(email); 
                    BoardMembers.Remove(email);
                    BoardUserController boardUserController = new BoardUserController();
                    boardUserController.Delete(BoardId,email);
                    log.Info($"User {email} has left the board successfully");
                }
                    
            }
            else
            {
                log.Error($"User {email} is not a member of the board");
                throw new Exception("The email not belong to the board");
            }

        }
        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="taskID">The task to be deleted identified task ID</param>
        /// <exception cref="Exception"></exception>
        public void deleteTask(string email, int columnOrdinal, long taskID)
        {
            log.Info($"Task {taskID} is being deleted by {email}");
           
            if (string.IsNullOrEmpty(email))
            {
                log.Error($"the email can't be empty");
                throw new Exception("the email can't be empty");
            }
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnOrdinal < Backlog || columnOrdinal >= Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            if (columnOrdinal == Backlog)
            {
                BackLogColumn.RemoveTask(email, taskID);
                return;
            }
            if (columnOrdinal == InProgress)
            {
                InProgressColumn.RemoveTask(email, taskID);
                return;
            }
        }

        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="TaskID">The task to be moved identified task ID</param>
        /// <exception cref="Exception"></exception>
        public void advanceTask(string email, int columnOrdinal, long TaskID)
        {
            log.Info($"Task {TaskID} is being advanced by {email}");
            if (columnOrdinal == Done)
            {
                log.Error($"the task is already in the last column");
                throw new Exception("the task is already in the last column");

            }
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnOrdinal < Backlog || columnOrdinal >= Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            if (GetTask(email, columnOrdinal, TaskID).Assignee == null || !GetTask(email, columnOrdinal, TaskID).Assignee.Equals(email))
            {
                log.Error($"user not assignee can not advanced task");
                throw new Exception("user not assignee can not advanced task");
            }
            if (columnOrdinal == Backlog)
            {
                TaskBL task = BackLogColumn.GetTask(TaskID);

                InProgressColumn.AddTask(email, task.TaskID, Backlog + 1, task.Title, task.Description, task.DueDate);
                InProgressColumn.AssignTask(TaskID, email, email);
                BackLogColumn.RemoveAdevancedTask(email, TaskID);
                task.TaskDTO.ColumnNumber = InProgress;
                log.Info($"Task {TaskID} was advanced successfully");
                return;
            }
            if (columnOrdinal == InProgress)
            {
                TaskBL task = InProgressColumn.GetTask(TaskID);
                TaskBL newTask = DoneColumn.AddTask(email, task.TaskID, InProgress + 1, task.Title, task.Description, task.DueDate);
                DoneColumn.AssignTask(TaskID, email, email);
                InProgressColumn.RemoveAdevancedTask(email, TaskID);
                newTask.TaskDTO.ColumnNumber = Done;
                log.Info($"Task {TaskID} was advanced successfully");
                return;
            }

        }

        /// <summary>
        /// This method limits the number of tasks in a column.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnNum"></param>
        /// <param name="limit"></param>
        /// <exception cref="Exception"></exception>
        public void LimitColumn(string email, int columnNum, int limit)
        {
            log.Info($"column {columnNum} is being limited by {limit}");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnNum < Backlog || columnNum > Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            if (columnNum == Backlog)
            {
                BackLogColumn.SetColumnLimit(limit);
            }
            else if (columnNum == InProgress)
            {
                InProgressColumn.SetColumnLimit(limit);
            }
            else
            {
                DoneColumn.SetColumnLimit(limit);
            }
        }

        /// <summary>
        /// This method gets the specific board column limits
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnNum"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetColumnLimit(string email, int columnNum)
        {
            log.Info($"column {columnNum} is being limited by ");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnNum < Backlog || columnNum > Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            log.Info($"column limit return successfully");
            if (columnNum == Backlog)
                return BackLogColumn.Limit;
            else if (columnNum == InProgress)
                return InProgressColumn.Limit;
            else
                return DoneColumn.Limit;
        }

        /// <summary>
        /// Retrieves the name of a column based on the user's email and the specified column index.
        /// </summary>
        /// <param name="email">Identifies the user to check if they are logged in.</param>
        /// <param name="columnOrdinal">Specifies which column name to retrieve based on its index.</param>
        /// <returns>The name of the column corresponding to the provided index.</returns>
        /// <exception cref="Exception">Thrown when the user is not logged in or the column index is out of the valid range.</exception>
        public string GetColumnName(string email, int columnOrdinal)
        {
            log.Info($"column {columnOrdinal} is being limited by {email}");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnOrdinal < Backlog || columnOrdinal > Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            log.Info($"column {columnOrdinal} return column name successfully");
            if (columnOrdinal == Backlog)
                return BackLogColumn.ColumnName;
            else if (columnOrdinal == InProgress)
                return InProgressColumn.ColumnName;
            else
                return DoneColumn.ColumnName;
        }
        /// <summary>
        /// Retrieves a list of tasks associated with a specific column for a logged-in user.
        /// </summary>
        /// <param name="email">Identifies the user whose tasks are being retrieved based on their login status.</param>
        /// <param name="columnOrdinal">Specifies the column from which tasks should be fetched.</param>
        /// <returns>A list of tasks that belong to the specified column.</returns>
        /// <exception cref="Exception">Thrown when the user is not logged in.</exception>
        public List<TaskBL> GetColumnTask(string email, int columnOrdinal)
        {
            if (columnOrdinal < Backlog || columnOrdinal > Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            log.Info($"column {columnOrdinal} return column tasks successfully");
            if (columnOrdinal == Backlog)
            {
                return BackLogColumn.GetTasks(email);
            }
            else if (columnOrdinal == InProgress)
            {
                return InProgressColumn.GetTasks(email);
            }
            else
            {
                return DoneColumn.GetTasks(email);
            }
        }

        /// <summary>
        /// Retrieves a specific task from a column based on the user's email, column index, and task ID.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TaskBL GetTask(string email, int columnOrdinal, long taskId)
        {
            log.Info($"Task {taskId} is being retrieved by {email}");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }
            if (columnOrdinal < Backlog || columnOrdinal > Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            if (columnOrdinal == Backlog)
            {
                return BackLogColumn.GetTask(taskId);
            }
            else if (columnOrdinal == InProgress)
            {
                return InProgressColumn.GetTask(taskId);
            }
            else
            {
                return DoneColumn.GetTask(taskId);
            }
        }
        /// <summary>
        /// this method transfer the owner of the board
        /// </summary>
        /// <param name="currOwner"></param>
        /// <param name="newOwner"></param>
        /// <exception cref="Exception"></exception>
        public void TransferOwnership(string currOwner, string newOwner)
        {
            if (!authFacade.IsLoginIn(currOwner))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }

            if (string.IsNullOrEmpty(currOwner) || string.IsNullOrEmpty(newOwner))
            {
                log.Error($"curr owner or new owner cannot be null or Empty");
                throw new Exception(" curr owner or new owner cannot be null or Empty");
            }
            if (currOwner != BoardOwner)
            {
                log.Error($"only the owner can transfer ownership");
                throw new Exception("only the owner can transfer ownership");
            }
            if (!BoardMembers.ContainsKey(newOwner))
            {
                log.Error($"cant transfer owner user if newOwner not in the board");
                throw new Exception("cant transfer owner user if newOwner not in the board");
            }
            BoardDTO.BoardOwner = newOwner;
            this.BoardOwner = newOwner;
        }
        /// <summary>
        /// This method assigns a task to a new assignee in the specified column.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="columnNum"></param>
        /// <param name="currAssignee"></param>
        /// <param name="newAssignee"></param>
        /// <exception cref="Exception"></exception>

        public void AssignTask(long taskId, int columnNum, string currAssignee, string newAssignee)
        {
            log.Info($"Task {taskId} is being assigned by {currAssignee} to {newAssignee}");
            if (!authFacade.IsLoginIn(currAssignee))
            {
                log.Error($"User is not logged in");
                throw new Exception("User is not logged in");
            }

            if (string.IsNullOrWhiteSpace(currAssignee) || string.IsNullOrWhiteSpace(newAssignee))
            {
                log.Error($"the new assignee can't be empty");
                throw new Exception("the new assignee can't be empty");
            }
            if (!BoardMembers.ContainsKey(newAssignee))
            {
                log.Error($"the new assignee is not a member of the board");
                throw new Exception("the new assignee is not a member of the board");
            }
            if (columnNum < Backlog || columnNum >= Done)
            {
                log.Error($"the column number is not valid");
                throw new Exception("the column number is not valid");
            }
            if (columnNum == Backlog)
            {
                BackLogColumn.AssignTask(taskId, currAssignee, newAssignee);
            }
            else
            {
                InProgressColumn.AssignTask(taskId, currAssignee, newAssignee);
            }

        }

    }
}
