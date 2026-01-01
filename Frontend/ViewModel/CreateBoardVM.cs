using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;
using Frontend.Model.Controllers;
using IntroSE.Forum.Frontend;

namespace Frontend.ViewModel
{
    internal class CreateBoardVM : Notifiable
    {
        private string _boardName;
        private string _errorMessage;
        public UserModel UserModel { get; set; }
        BoardController boardController = ControllerFactory.Instance.BoardController;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        public string BoardName
        {
            get { return _boardName; }
            set
            {
                _boardName = value;
                RaisePropertyChanged("BoardName");
            }
        }
        public CreateBoardVM(UserModel userModel)
        {
            BoardName = string.Empty;
            ErrorMessage = "";
            UserModel = userModel;
        }
        public BoardModel? CreateBoard()
        {
            if (string.IsNullOrWhiteSpace(BoardName))
            {
                ErrorMessage = "BoardName cannot be empty";
                return null;
            }
            try
            {
                return boardController.createBoard(UserModel.Email,BoardName);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }
    }
}
