using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using Microsoft.VisualBasic;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class ColumnDTO
    {
        
        public const string ColumnNameColumnName = "ColumnName";
        public const string ColumnLimitColumnName = "ColoumnLimit";
        public const string BoardIdColumnName = "boardId";
        private int columnNum;
        public int columnLimit;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int BoardId { get; private set; }
        public bool isPersisted { get; set; } = false;
        private ColumnController ColumnController { get; set; }
        private TaskController TaskController { get; set; }

        public int ColumnNum
        {
            get => columnNum;
            set
            { 
                columnNum = value;
            }
        }
        public int ColumnLimit
        {
            get => columnLimit;
            set { if (isPersisted) ColumnController.LimitColumnUpdate(BoardId, ColumnNum, value); columnLimit = value; }
        }

        /// <summary>
        /// Saves the column to the database if it is not already persisted.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void save()
        {
            if (isPersisted)
            {
                throw new Exception("Column is already persisted.");
            }
        }
        /// <summary>
        /// Saves the column to the database with the specified board ID. Throws an exception if the column is already persisted.
        /// </summary>
        /// <param name="BoardId"></param>
        /// <exception cref="Exception"></exception>
        public void save(int BoardId)
        {
            if (isPersisted)
            {
                throw new Exception("Column is already persisted.");
            }
            this.BoardId = BoardId;
            ColumnController.Insert(this);
            isPersisted = true;
            log.Info($"Column {ColumnNum} saved successfully to board {BoardId} with limit {ColumnLimit}.");
        }
        /// <summary>
        /// Adds a task to the column by saving it to the database with the current board ID and column number.
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(TaskDTO task)
        {
            task.save(BoardId, ColumnNum);
        }
        /// <summary>
        /// Removes a task from the column by deleting it from the database using the specified task ID.
        /// </summary>
        /// <param name="taskId"></param>
        public void RemoveTask(long taskId)
        {
            TaskController.Delete(BoardId,taskId);
        }

        public ColumnDTO(int boardId, int columnNum, int columnLimit)
        {
            this.ColumnController = new ColumnController();
            this.TaskController = new TaskController();
            this.ColumnNum = columnNum;
            this.ColumnLimit = columnLimit;
            this.BoardId = boardId;
            this.isPersisted = false;

        }
    }
}
