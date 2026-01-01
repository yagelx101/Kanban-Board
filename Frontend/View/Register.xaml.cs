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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        
        private UserVM UserVM;

        public Register()
        {
            InitializeComponent();
            this.UserVM = new UserVM();
            this.DataContext = UserVM;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            UserModel? ret = UserVM.Register();
            if (ret != null)
            {
                UserBoards userBoards = new UserBoards(ret);
                userBoards.Show();
                this.Close();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            mainWindow1 mainWindow = new mainWindow1();
            mainWindow.Show();
            this.Close();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

       
    }
}
