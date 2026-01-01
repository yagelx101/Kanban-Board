using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class TaskDTO
    {
        public TaskController taskController { get; set; }

        public const string TaskIDColumnName = "taskId";
        public const string TaskTitleColumnName = "Title";
        public const string TaskDescriptionColumnName = "Description";
        public const string TaskDueDateColumnName = "DueDate";
        public const string TaskColumnNumber = "ColumnNum";
        public const string BoardIdColumnName = "boardId";
        public const string TaskCreationTimeColumnName = "CreationTime";
        public const string AssigneeColumnName = "Assignee";
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool isPersisted { get; set; } = false;

        public long Id { get; private set; }
        public int BoardId { get; private set; }

        private string title;

        public string Title
        {
            get => title;
            set { if (isPersisted) { taskController.UpdateTitle(Id, BoardId, TaskTitleColumnName, value); title = value; } }
        }
        private string description;
        public string Description
        {
            get => description;
            set { if (isPersisted) taskController.UpdateDescription(Id, BoardId, TaskDescriptionColumnName, value); description = value; }
        }
        private DateTime dueDate;
        public DateTime DueDate
        {
            get => dueDate;
            set {if (isPersisted) taskController.UpdateDueDate(Id, BoardId, TaskDueDateColumnName, value); dueDate = value; }
        }
        private int columnNumber;
        public int ColumnNumber
        {
            get => columnNumber;
            set {if(isPersisted) taskController.AdvancedTask(Id, BoardId, TaskColumnNumber, value); columnNumber = value; }
        }

        public DateTime CreationTime { get; private set; }

        private string assignee;
        public string Assignee
        {
            get => assignee; set
            {
                if (isPersisted)
                {
                    taskController.AssignTask(Id, BoardId, AssigneeColumnName, value); assignee = value;
                }
            }
        }

        /// <summary>
        /// This method saves the task to the database.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void save()
        {
            if (isPersisted)
            {
                throw new Exception("Task is already persisted.");
            }
        }
        /// <summary>
        /// This method saves the task to the database with the given board ID and column number.
        /// </summary>
        /// <param name="boardID"></param>
        /// <param name="columnNumber"></param>
        /// <exception cref="Exception"></exception>
        public void save(int boardID, int columnNumber)
        {
            if (isPersisted)
            {
                throw new Exception("Task is already persisted.");
            }
            this.BoardId = boardID;
            this.columnNumber = columnNumber;
            taskController.Insert(this);
            isPersisted = true;
            log.Info($"Task {Id} saved successfully to board {BoardId} in column {ColumnNumber}.");
        }
        public TaskDTO(long id, string title, string description, DateTime dueDate, int columnNumber, int boardId, DateTime creationTime, string assignee)
        {
            this.Id = id;
            this.title = title;
            this.description = description;
            this.dueDate = dueDate;
            this.columnNumber = columnNumber;
            this.BoardId = boardId;
            this.CreationTime = creationTime;
            this.assignee = assignee;
            taskController = new TaskController();

        }
    }
}
