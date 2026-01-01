using IntroSE.Forum.Frontend;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    internal class BoardModel : Notifiable
    {
        public string BoardName { get; set; }
        public string BoardOwner { get; private set; }
        public List<string> boardMembers { get; private set; }
        internal BoardModel(BoardSL<TaskSL> board)
        {
            BoardName = board.BoardName;
            BoardOwner = board.BoardOwner;
            boardMembers = board.Members;
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
