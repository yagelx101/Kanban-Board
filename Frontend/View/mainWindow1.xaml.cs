using Frontend.Model.Controllers;
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
    /// Interaction logic for mainWindow1.xaml
    /// </summary>
    public partial class mainWindow1 : Window
    {
        private UserVM UserVM;

        public mainWindow1()
        {
            InitializeComponent();
            ControllerFactory.Instance.serviceFactory.LoadData();
            this.UserVM = new UserVM();
            this.DataContext = UserVM;

        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            
                Register register = new Register();
                register.Show();
                this.Close();
            
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
                Login login = new Login();
                login.Show();
                this.Close();
            
        }
    }
}
