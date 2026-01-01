using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Data_accsses_layer;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class ColumnBL
    {
        public string ColumnName { get; private set; }
        public int Limit { get; private set; }
        protected readonly Dictionary<long, TaskBL> tasks;
        private AuthenticationFacade authFacade;
        private const int Unlimited = -1;
        public ColumnDTO ColumnDTO { get;private set; }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ColumnBL(string columnName, int limit, AuthenticationFacade authFacade)
        {
            this.ColumnName = columnName;
            this.Limit = limit;
            this.tasks = new Dictionary<long, TaskBL>();
            this.authFacade = authFacade;
            switch (ColumnName)
            {
                case "Backlog":
                    this.ColumnDTO = new ColumnDTO(0, 0, limit);
                    break;

                case "In Progress":
                    this.ColumnDTO = new ColumnDTO(0, 1, limit);
                    break;

                case "Done":
                    this.ColumnDTO = new ColumnDTO(0, 2, limit);
                    break;
            }
            ColumnDTO.save();

        }

        public ColumnBL(ColumnDTO columnDTO, AuthenticationFacade authFacade)
        {
            switch (columnDTO.ColumnNum)
            {
                case 0:
                    this.ColumnName = "Backlog";
                    break;

                case 1:
                    this.ColumnName= "In Progress";
                    break;

                case 2:
                    this.ColumnName = "Done";
                    break;
            }
            this.Limit = columnDTO.ColumnLimit;
            this.authFacade = authFacade;
            TaskController taskController = new TaskController();
            this.tasks = new Dictionary<long, TaskBL>();
            List<TaskDTO> taskDTO = taskController.SelectBoardAndColumnTasks(columnDTO.BoardId, columnDTO.ColumnNum);
            foreach (TaskDTO task1 in taskDTO)
            {
                TaskBL taskBL = new TaskBL(task1,authFacade);            
                tasks.Add(task1.Id, taskBL);
            }
        }

        /// <summary>
        /// Sets the limit for the column. 
        /// </summary>
        /// <param name="limit">The limit must be a positive integer greater than zero and greater than or equal to the number of tasks in the column.</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetColumnLimit(int limit)
        {
            if (limit < Unlimited || limit == 0)
            {
                log.Error("The limit can not be negative or zero");
                throw new ArgumentException("The limit can not be negative or zero");
            }
            if (limit < tasks.Count && limit!=Unlimited)
            {
                log.Error("The limit can not be less than the number of tasks in the column");
                throw new ArgumentException("The limit can not be less than the number of tasks in the column");
            }
            ColumnDTO.ColumnLimit = limit;
            this.Limit = limit;
            log.Info($"column was limited successfully");

        }

        /// <summary>
        /// Returns a list of all tasks in the column.
        /// </summary>
        /// <returns></returns>
        public List<TaskBL> GetTasks(string email)
        {
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"User {email} is not logged in");
                throw new Exception($"User {email} is not logged in");
            }
            List<TaskBL> taskList = new List<TaskBL>();
            foreach (var task in tasks.Values)
            {
                taskList.Add(task);

            }
            log.Info($"Retrieved {taskList.Count} tasks from column {ColumnName}");
            return taskList;
        }

        /// <summary>
        /// Retrieves a task by its ID from the column.
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public TaskBL GetTask(long taskID)
        {
            if (!tasks.ContainsKey(taskID))
            {
                log.Error($"Task with ID {taskID} does not exist in column {ColumnName}");
                throw new KeyNotFoundException($"Task with ID {taskID} does not exist in column {ColumnName}");
            }
            log.Info($"Retrieved task with ID {taskID} from column {ColumnName}");
            return tasks[taskID];
        }

        /// <summary>
        /// Adds a new task to the column. The task must have a unique ID and the column must not exceed its limit.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="taskId"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TaskBL AddTask(string email, long taskId, int columnOrdinal, string title, string description, DateTime dueDate)
        {
            TaskBL task = new TaskBL(taskId, columnOrdinal, title, description, dueDate, DateTime.Now, authFacade);
            if (tasks.Count >= Limit && Limit != Unlimited)
            {
                log.Error($"Column limit reached");
                throw new ArgumentException($"Column limit reached");
            }
            if (tasks.ContainsKey(task.TaskID))
            {
                log.Error($"Task with ID {task.TaskID} already exists in column {ColumnName}");
                throw new ArgumentException($"Task with ID {task.TaskID} already exists in column {ColumnName}");
            }
            ColumnDTO.AddTask(task.TaskDTO);
            tasks[task.TaskID] = task;
            log.Info($"Task with ID {task.TaskID} added to column {ColumnName}");
            return task;
        }

        /// <summary>
        /// Removes a task from the column by its ID. The email must match the task's email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="taskID"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void RemoveTask(string email, long taskID)
        {
            if (!tasks.ContainsKey(taskID))
            {
                log.Error($"Task with ID {taskID} does not exist in column {ColumnName}");
                throw new KeyNotFoundException($"Task with ID {taskID} does not exist in column {ColumnName}");
            }
            if (GetTask(taskID).Assignee == null || !GetTask(taskID).Assignee.Equals(email))
            {
                log.Error($"The task can not be removed by someone not the assignee");
                throw new Exception("The task can not be removed by someone not the assignee");
            }
            ColumnDTO.RemoveTask(taskID);
            tasks.Remove(taskID);
            log.Info($"Task with ID {taskID} removed from column {ColumnName}");
        }

        /// <summary>
        /// Removes a task from the column by its ID for the advance task, without deleting task dto.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="taskID"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public void RemoveAdevancedTask(string email, long taskID)
        {
            if (!tasks.ContainsKey(taskID))
            {
                log.Error($"Task with ID {taskID} does not exist in column {ColumnName}");
                throw new KeyNotFoundException($"Task with ID {taskID} does not exist in column {ColumnName}");
            }
            if (GetTask(taskID).Assignee == null || !GetTask(taskID).Assignee.Equals(email))
            {
                log.Error($"The task can not be removed by someone not the assignee");
                throw new Exception("The task can not be removed by someone not the assignee");
            }
            tasks.Remove(taskID);
            log.Info($"Task with ID {taskID} removed from column {ColumnName}");
        }

        /// <summary>
        /// Assigns a task to a new assignee. The current assignee must match the task's current assignee.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currAssignee"></param>
        /// <param name="newAssignee"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void AssignTask(long taskId, string currAssignee, string newAssignee)
        {
            if (!tasks.ContainsKey(taskId))
            {
                log.Error($"The task ID is not valid");
                throw new Exception($"The task ID is not valid");
            }
            log.Info($"Assigning task with ID {taskId} from {currAssignee} to {newAssignee}");
            GetTask(taskId).AssignTask(currAssignee, newAssignee);
        }
        public void LeaveBoard(string email)
        {
            foreach(var task in tasks)
            {
                if(task.Value.Assignee == email)
                {
                    task.Value.AssignTask(email, null);
                }
            }
            log.Info($"User {email} has left the board, all tasks assigned to them have been unassigned.");
        }
    }
}
