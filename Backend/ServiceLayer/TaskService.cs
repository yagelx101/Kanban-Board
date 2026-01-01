using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using Microsoft.VisualBasic;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskService
    {
        private BoardFacade BF;
        internal TaskService(BoardFacade bf)
        {
            this.BF = bf;
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>An string response, unless an error occurs </returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try 
            { 

                TaskBL task = BF.addTask(email, boardName, title, description, dueDate);
                TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                Response<TaskSL> response = new Response<TaskSL>(taskSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }

        }

        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the current board</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>An string response, unless an error occurs </returns>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, long taskID, DateTime dueDate)
        {
            try
            {
                TaskBL task = BF.UpdateTaskDueDate(email,  boardName, columnOrdinal,  taskID,  dueDate);
                TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                Response<TaskSL> response = new Response<TaskSL>(taskSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name or task ID not exist");

                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>An string response, unless an error occurs </returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, long taskID, string title)
        {
            try
            {

                TaskBL task = BF.UpdateTaskTitle( email,  boardName,  columnOrdinal,  taskID,  title);
                TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                Response<TaskSL> response = new Response<TaskSL>(taskSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name or task ID not exist");

                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskID">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>An string response, unless an error occurs </returns>
        public string UpdateTaskDescription(string email, string boardName,int columnOrdinal, long taskID, string description)
        {
            try
            {

                TaskBL task = BF.UpdateTaskDescription( email,  boardName,  columnOrdinal,  taskID,  description);
                TaskSL taskSL = new TaskSL(task.TaskID, task.Title, task.Description, task.DueDate, task.CreationTime, task.Assignee);
                Response<TaskSL> response = new Response<TaskSL>(taskSL);

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {

                Response<string> response = new Response<string>("The email or board name or task ID not exist");

                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// This method delete a task given it's ID
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskID">The task identified ID</param>
        /// <returns>An empty response, unless an error occurs</returns>
        public string DeleteTask(string email, string boardName, int columnOrdinal, long taskID)
        {
            
            try
            {

                BF.DeleteTask(email, boardName, columnOrdinal, taskID);
                Response<string> response = new Response<string>();

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name or task ID not exist");

                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }
        }

        /// <summary>
        /// This method assign a task to user.
        /// </summary>
        /// <param name="taskId">the task identefied ID</param>
        /// <param name="currAssignee">the current assigne to the task</param>
        /// <param name="newAssignee">the new assigne to the task</param>
        /// <returns>an empty response</returns>
        /// <exception cref="NotImplementedException"></exception>

        public string AssignTask(long taskId, string boardName, int columnNum, string currAssignee, string newAssignee)
        { 
            try
            {

                BF.AssignTask(taskId, boardName, columnNum, currAssignee, newAssignee);
                Response<string> response = new Response<string>();

                return JsonSerializer.Serialize(response);
            }
            catch (KeyNotFoundException e)
            {
                Response<string> response = new Response<string>("The email or board name not exist");
                return JsonSerializer.Serialize(response);
            }
            catch (Exception e)
            {
                Response<string> response = new Response<string>(e.Message);
                return JsonSerializer.Serialize(response);
            }

        }  
    }
}
