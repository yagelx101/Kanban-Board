using Frontend.Model;
using Frontend.Model.Controllers;
using Frontend.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.View
{
    public partial class UserBoards : Window
    {
        private readonly UserModel user;
        private readonly UserBoardVM userBoardVM;

        internal UserBoards(UserModel user)
        {
            InitializeComponent();
            this.user = user;
            userBoardVM = new UserBoardVM(user);
            this.DataContext = userBoardVM;
        }

        private void BoardCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is BoardModel clickedBoard)
            {
                if (e.ClickCount == 2)
                {
                    boardView boardView = new boardView(clickedBoard, user);
                    boardView.Show();
                    this.Close();
                }
                else if (e.ClickCount == 1)
                {
                    foreach (var board in userBoardVM.Boards)
                        board.IsSelected = false;

                    clickedBoard.IsSelected = true;
                    userBoardVM.SelectedBoard = clickedBoard;

                    DeleteButton.Visibility = Visibility.Visible;
                }
            }
        }


        private void CreateBoard_Click(object sender, RoutedEventArgs e)
        {
            CreateBoard createBoard = new CreateBoard(user);
            createBoard.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow1 mainWindow = new mainWindow1();
            mainWindow.Show();
            this.Close();
        }

        private void DeleteBoard_Click(object sender, RoutedEventArgs e)
        {
            if (userBoardVM.SelectedBoard != null)
            {
                userBoardVM.DeleteSelectedBoard(userBoardVM.SelectedBoard);

                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}

