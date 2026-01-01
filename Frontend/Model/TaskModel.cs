using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Frontend.Model
{
    internal class TaskModel
    {
        public string Title { get; private set; }

        public DateTime DueDate { get; private set; }
        public DateTime CreationTime { get; private set; }

        public string Assignee { get; private set; }
        internal TaskModel(TaskSL task) 
        { 
            Title = task.Title;
            DueDate = task.DueDate;
            Assignee = task.Assignee;
            CreationTime = task.CreationTime;
        }
        
    }
}
