using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;



namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardSL<T>
    {

        public string BoardName { get; private set; }
        public int BoardId { get; private set; }
        public string BoardOwner { get; private set; }
        public List<T> TaskList { get; private set; }
        public List<string> Members { get; private set; }

        public BoardSL(string boardName,int boardId, List<T> taskList,string boardOwner, List<string> members)
        {
            this.BoardName = boardName;
            this.TaskList = taskList;
            this.BoardId = boardId;
            this.BoardOwner = boardOwner;
            this.Members = members;
        }
    }
}
