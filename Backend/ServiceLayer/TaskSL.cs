using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class TaskSL
    {
        public long Id { get; private set; }
        public DateTime CreationTime { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public string Assignee { get; private set; }

        [JsonConstructor]
        public TaskSL(long Id, string Title, string Description, DateTime DueDate, DateTime CreationTime, string Assignee)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.DueDate = DueDate;
            this.CreationTime = CreationTime;
            this.Assignee = Assignee;
        }

    }
}
