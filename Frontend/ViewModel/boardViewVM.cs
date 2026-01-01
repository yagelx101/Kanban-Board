using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;
using Frontend.Model.Controllers;
using IntroSE.Forum.Frontend;



namespace Frontend.ViewModel
{
    internal class boardViewVM : Notifiable
    {
        public BoardModel Board { get; }
        public string WelcomeMessage { get { return $"Board Owner: {Board.BoardOwner}  Board Name: {Board.BoardName}"; } }
        public ObservableCollection<TaskModel> AllInProgressTask { get; }
        public ObservableCollection<TaskModel> AllDoneTask { get; }
        public ObservableCollection<TaskModel> AllBacklogTask { get; }
        public ObservableCollection<string> BoardMembers { get { return boardController.GetBoardMembers(Board.BoardOwner, Board.BoardName); } }

        public string BoardMembersAsString => "Board members: " + string.Join(", ", BoardMembers);

        BoardController boardController = ControllerFactory.Instance.BoardController;

        

        public boardViewVM(BoardModel board)
        {
            Board = board;
            AllInProgressTask = boardController.GetInProgressObserver(Board.BoardOwner, Board.BoardName);
            AllDoneTask = boardController.GetDoneObserver(Board.BoardOwner, Board.BoardName);
            AllBacklogTask = boardController.GetBacklogObserver(Board.BoardOwner, Board.BoardName);
            
        }
    }
}
