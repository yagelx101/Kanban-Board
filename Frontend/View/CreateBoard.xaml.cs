using Frontend.Model;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for CreateBoard.xaml
    /// </summary>
    public partial class CreateBoard : Window
    {
       
        private UserModel userModel;
        private CreateBoardVM boardVM ;
        


        public CreateBoard(UserModel user)
        {
            InitializeComponent();
            this.userModel = user;
            this.boardVM = new CreateBoardVM(userModel);
            this.DataContext = boardVM;

        }


        private void CreateBoard_Click(object sender, RoutedEventArgs e)
        {
            BoardModel? ret = boardVM.CreateBoard();
            if (ret != null)
            {
                boardView BoardView = new boardView(ret, userModel);
                BoardView.Show();
                this.Close();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UserBoards userBoards = new UserBoards(userModel);
            userBoards.Show();
            this.Close();
        }
    }
}
