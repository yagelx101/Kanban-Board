using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("BackendUnitTests")]

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class TaskBL
    {
        private const int MinTitleLength = 0;
        private const int MaxTitleLength = 50;
        private const int MaxDescriptionLength = 300;
        private const int MaxColumnNum = 2;
        public long TaskID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime CreationTime { get; private set; }
        public int ColumnNum { get; set; }
        public string Assignee { get; private set; }
        public TaskDTO TaskDTO { get; private set; }
        private AuthenticationFacade authFacade;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public TaskBL(long taskID, int columnOrdinal, string title, string description, DateTime dueDate, DateTime creationTime, AuthenticationFacade authFacade)
        {
            this.TaskID = taskID;
            this.Title = title;
            this.Description = description;
            this.DueDate = dueDate;
            this.CreationTime = creationTime;
            this.ColumnNum = columnOrdinal;
            this.authFacade = authFacade;
            this.Assignee = null;
            this.TaskDTO = new TaskDTO(TaskID, title, description, dueDate, columnOrdinal, 0, creationTime, Assignee);
            TaskDTO.save();
        }

        public TaskBL(TaskDTO taskDTO, AuthenticationFacade authFacade)
        {
            this.TaskID = taskDTO.Id;
            this.Title = taskDTO.Title;
            this.Description = taskDTO.Description;
            this.DueDate = taskDTO.DueDate;
            this.CreationTime = taskDTO.CreationTime;
            this.ColumnNum = taskDTO.ColumnNumber;
            this.authFacade = authFacade;
            this.Assignee = taskDTO.Assignee;
        }


        /// <summary>
        /// This method updates the task. The user can update the title, description and due date of the task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="title">New title for the task</param>
        /// <param name="description">New description for the task</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void updateTask(string email, string title, string description, DateTime dueDate)
        {
            log.Info($"Task {TaskID} is being updated by {email}");
            if (!authFacade.IsLoginIn(email))
            {
                log.Error($"The task can not ce change by someone else");
                throw new Exception("The task can not ce change by someone else");
            }
            if (title == null && description == null && dueDate == default(DateTime))
            {
                log.Error($"The task can not be updated with empty values");
                throw new Exception("The task can not be updated with empty values");
            }
            if (Assignee == null || !email.Equals(Assignee))
            {
                log.Error($"The task can not be updated by someone else than the assignee");
                throw new Exception("The task can not be updated by someone else than the assignee");
            }
            if (title != null && (title.Length <= MinTitleLength || title.Trim().Length == 0))
            {
                log.Error($"The title can not be empty or only spaces");
                throw new Exception("the title can not be empty or only spaces");
            }
            if (title != null && title.Length > MaxTitleLength)
            {
                log.Error($"The title can not be with more than 50 characters");
                throw new Exception("the title can not be with more than 50 characters");
            }
            if (description != null && (description.Length > MaxDescriptionLength || (string.IsNullOrWhiteSpace(description) && description.Length != 0)))
            {
                log.Error($"The description can't be with more than 300 characters or only spaces");
                throw new Exception("the description can not be with more than 300 characters or only spaces");
            }

            if (dueDate != default(DateTime) && dueDate < DateTime.Now)
            {
                log.Error($"Due date cannot be in the past");
                throw new Exception("The due date is not valid");
            }
            if (ColumnNum == MaxColumnNum)
            {
                log.Error($"The task is already completed, you can not change it");
                throw new Exception("The task is already completed, you can not change it");
            }

            if (title != null)
            {
                TaskDTO.Title = title;
                this.Title = title;
                log.Info("the title of the task updated");
            }
            if (description != null)
            {
                TaskDTO.Description = description;
                this.Description = description;
                log.Info("the description of the task updated");
            }
            if (dueDate != default(DateTime))
            {
                TaskDTO.DueDate = dueDate;
                this.DueDate = dueDate;
                log.Info("the due date of the task updated");
            }

        }

        /// <summary>
        /// This method assigns the task to a new user. The user who is trying to assign the task must be the current assignee.
        /// </summary>
        /// <param name="currAssignee">the user that want to pass the assignee</param>
        /// <param name="newAssignee">the new assignee of the task</param>
        /// <exception cref="Exception"></exception>
        public void AssignTask(string currAssignee, string newAssignee)
        {
            if (Assignee == null)
            {
                TaskDTO.Assignee = newAssignee;
                Assignee = newAssignee;
                log.Info($"Task {TaskID} is being assigned from {currAssignee} to {newAssignee}");
            }
            else if (currAssignee.Equals(Assignee))
            {
                TaskDTO.Assignee = newAssignee;
                Assignee = newAssignee;
                log.Info($"Task {TaskID} is being assigned from {currAssignee} to {newAssignee}");
            }
            else
            {
                log.Error($"User who is not the assignee try to assign the task to someone else");
                throw new Exception("User who is not the assignee try to assign the task to someone else");
            }


        }
    }
}
