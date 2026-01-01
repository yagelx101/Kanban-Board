using Frontend.Model;
using Frontend.Model.Controllers;
using IntroSE.Kanban.Backend.Data_accsses_layer;
using System.Collections.ObjectModel;
using System.Windows;
using BoardController = Frontend.Model.Controllers.BoardController;


namespace Frontend.ViewModel
{
    internal class UserBoardVM : UserVM
    {

        private readonly BoardController boardController = ControllerFactory.Instance.BoardController;
        private readonly UserModel user;

        public ObservableCollection<BoardModel> Boards { get; }

        public string WelcomeMessage => $"Welcome {user.Email}";

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        public UserBoardVM(UserModel user)
        {
            this.user = user;
            Boards = boardController.GetBoards(user.Email);
        }
        private BoardModel selectedBoard;
        public BoardModel SelectedBoard
        {
            get => selectedBoard;
            set
            {
                selectedBoard = value;
                RaisePropertyChanged(nameof(SelectedBoard));
            }
        }

       

        public void DeleteSelectedBoard(BoardModel boardToDelete)
        {
            if (SelectedBoard == null)
                return;

            try
            {
                boardController.DeleteBoard(user.Email, boardToDelete.BoardName);

                Boards.Remove(SelectedBoard);

                SelectedBoard = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete board: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}