
﻿using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Backend.ServiceLayers
{
    public class BoardService
    {
        private const int Backlog = 0;
        private const int InProgress = 1;
        private readonly int Done = 2;

        private BoardFacade BoardFacade;
        internal BoardService(BoardFacade bf)
        {
            this.BoardFacade = bf;
        }

        /// <summary>
        /// This method creates a board for the given user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>An string response, unless an error occurs  </returns>
        public string CreateBoard(string email, string boardName)
        {

            try
            {
                BoardBL board = BoardFacade.CreateBoard(email, boardName);
                BoardSL<TaskSL> boardSL = new BoardSL<TaskSL>(board.BoardName, board.BoardId, null, board.BoardOwner, BoardFacade.GetBoardMembers(email,boardName));              
                Response<BoardSL<TaskSL>> response = new Response<BoardSL<TaskSL>>(boardSL);
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in and an owner of the board.</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An string response, unletss an error occurs </returns>
        public string DeleteBoard(string email, string boardName)
        {
            try
            {
                BoardFacade.DeleteBoard(email, boardName);
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }

        }
        /// <summary>
        /// the function state the limit of tasks in a specific column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnNum">The coulumn number we want to limit</param>
        /// <param name="limit">The amount we want to limit</param>
        /// <returns>An string response, unless an error occurs </returns>      
        /// /// <exception cref="NotImplementedException"></exception>
        public string LimitColumn(string email, string boardName, int columnNum, int limit)
        {
            try 
            {

                BoardFacade.LimitColumn(email, boardName, columnNum, limit);
                Response<string> response = new Response<string>();

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// the function gets the specific board column limits
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="coulmnNum">The coulumn number we want to get his limit</param>
        ///<returns>A response with the column's limit, unless an error occurs </returns>
        public string GetColumnLimit(string email, string boardName, int coulmnNum)
        {
            try
            {

                
                int limit = BoardFacade.GetColumnLimit( email,  boardName,  coulmnNum);
                Response<int> response = new Response<int>(limit);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>An string response, unless an error occurs </returns>
        public string AdvanceTask(string email, string boardName,int columnOrdinal, long taskID)
        {
            try
            {
                BoardFacade.AdvanceTask(email, boardName, columnOrdinal, taskID);
                Response<string> response = new Response<string>();

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not or task ID exist");

                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }

       
        }
        /// <summary>
        /// This method returns all in-progress tasks of a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of the in-progress tasks of the user, unless an error occurs</returns>
        public string InProgressTasks(string email)
        {
            try
            {

                List<TaskBL> tasks = BoardFacade.InProgressTasksService(email);
                List<TaskSL> tasksSL = new List<TaskSL>();
                foreach (TaskBL task in tasks)
                {
                    TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                    tasksSL.Add(taskSL);
                }
                Response<List<TaskSL>> response = new Response<List<TaskSL>>(tasksSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// this function create a board for the given user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>An string response, unless an error occurs <return>
        /// <exception cref="NotImplementedException"></exception>
        public string getBoards(string email)
        {
            try
            {
                List<BoardBL> boards = BoardFacade.getBoardsService(email);
                List<BoardSL<TaskSL>> boardsSL = new List<BoardSL<TaskSL>>();
                foreach (BoardBL board in boards)
                {
                    List<TaskSL> taskSL = new List<TaskSL>();
                    int[] columnNames = { Backlog, InProgress, Done };
                    foreach (int column in columnNames)
                    {
                        foreach (TaskBL task in board.GetColumnTask(email, column))
                        {
                            TaskSL tasksSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                            taskSL.Add(tasksSL);
                        }
                    }
                    BoardSL<TaskSL> boardSL = new BoardSL<TaskSL>(board.BoardName, board.BoardId, taskSL, board.BoardOwner, BoardFacade.GetBoardMembers(email, board.BoardName));
                    boardsSL.Add(boardSL);
                }
                Response<List<BoardSL<TaskSL>>> response = new Response<List<BoardSL<TaskSL>>>(boardsSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Retrieves the IDs of all boards associated with a specific user based on their email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string getUserBoardsID(string email)
        {
            try
            {
                List<int> boardsIDs = BoardFacade.getUserBoardsID(email);
                Response<List<int>> response = new Response<List<int>>(boardsIDs);
                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Retrieves the name of a specific column from a board based on the user's email and the board's name.
        /// </summary>
        /// <param name="email">Identifies the user requesting the column name.</param>
        /// <param name="boardName">Specifies the board from which to retrieve the column name.</param>
        /// <param name="columnOrdinal">Indicates the position of the column within the board.</param>
        /// <returns>Returns a serialized response containing the column name or an error message.</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            try
            {

                string columnName = BoardFacade.GetColumnName( email,  boardName,  columnOrdinal);
                Response<string> response = new Response<string> (null, columnName);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }

        }
        /// <summary>
        /// Retrieves tasks from a specified column of a board for a given user.
        /// </summary>
        /// <param name="email">Identifies the user whose tasks are being retrieved.</param>
        /// <param name="boardName">Specifies the board from which tasks are to be fetched.</param>
        /// <param name="columnOrdinal">Indicates the specific column from which tasks should be retrieved.</param>
        /// <returns>A JSON serialized response containing the list of tasks or an error message.</returns>
        public string GetColumnTask(string email, string boardName, int columnOrdinal)
        {
            try
            {

             
                List<TaskBL> tasks = BoardFacade.GetColumnTask(email, boardName, columnOrdinal);
                List<TaskSL> tasksSL = new List<TaskSL>();
                foreach (TaskBL task in tasks)
                {
                    TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                    tasksSL.Add(taskSL);
                }
                Response<List<TaskSL>> response = new Response<List<TaskSL>>(tasksSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }

        }

        /// <summary>
        /// Removes a user from a specified board and returns a JSON response indicating success or failure.
        /// </summary>
        /// <param name="boardId">Identifies the specific board from which a user is to be removed.</param>
        /// <param name="email">Specifies the email address of the user to be removed from the board.</param>
        /// <returns>A JSON serialized response indicating the result of the operation.</returns>
         public String LeaveBoard(int boardId, string email)
         {
            try
            {
                BoardFacade.LeaveBoard(boardId, email);
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
        /// <summary>
        /// Transfers ownership of a board from the current owner to a new owner and returns a JSON response indicating success or failure.
        /// </summary>
        /// <param name="boardName"></param>
        /// <param name="currOwner"></param>
        /// <param name="newOwner"></param>
        /// <returns></returns>
        public string TransferOwnership(String boardName,String currOwner,String newOwner)
        {
            try
            {
                BoardBL board = BoardFacade.GetBoard(currOwner, boardName);
                try
                {
                    BoardFacade.TransferOwnership(boardName, currOwner, newOwner);
                }
                catch (KeyNotFoundException)
                {
                    Response<string> response1 = new Response<string>("there board with this currOwner");
                    return JsonSerializer.Serialize(response1);
                }
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Joins a user to a specified board and returns a JSON response indicating success or failure.
        /// </summary>
        /// <param name="boardId">Identifies the specific board to which the user is being added.</param>
        /// <param name="email">Specifies the email address of the user who is joining the board.</param>
        /// <returns>Returns a JSON serialized response indicating the result of the join operation.</returns>
        public string JoinBoard(int boardId, string email)
        {

            try
            {
                BoardFacade.JoinBoard(boardId,email);
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Retrieves the name of a board based on its ID and returns it in a JSON serialized response.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public string getBoardName(int boardId)
        {
            try
            {
                string boardName = BoardFacade.getBoardNameService(boardId);
                Response<string> response = new Response<string>(null, boardName);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// Retrieves the members of a specified board and returns them in a JSON serialized response.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="boardName"></param>
        /// <returns></returns>
        public string GetBoardMembers(string email, string boardName)
        {
            try
            {
                List<string> members = BoardFacade.GetBoardMembers(email, boardName);
                Response<List<string>> response = new Response<List<string>>(members);
                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }
    }
   

}


