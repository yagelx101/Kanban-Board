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
using Frontend.Model;
using Frontend.ViewModel;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for boardView.xaml
    /// </summary>
    public partial class boardView : Window
    {
        private BoardModel board;
        private UserModel user;
        private boardViewVM vm;

        internal boardView(BoardModel board, UserModel user)
        {
            InitializeComponent();
            this.board = board;
            this.user = user;
            vm = new boardViewVM(board);
            this.DataContext = vm;

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UserBoards userBoards = new UserBoards(user);
            userBoards.Show();
            this.Close();
        }

    }
}
