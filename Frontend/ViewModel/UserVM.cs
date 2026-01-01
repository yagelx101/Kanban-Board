using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Model;
using Frontend.Model.Controllers;
using IntroSE.Forum.Frontend;

namespace Frontend.ViewModel
{
    internal class UserVM : Notifiable
    {
        UserController userController = ControllerFactory.Instance.UserController;


        public string Email { get; set; }
        public string Password { get; set; }
        private string _errorMessage;
        internal readonly UserModel user;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        public UserVM()
        {
            Email = string.Empty;
            Password = string.Empty;
            ErrorMessage = "";
        }
       

        internal UserModel? Login()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email cannot be empty";
                return null;
            }
            try
            {
                return userController.Login(Email, Password);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        internal UserModel? Register()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email cannot be empty";
                return null;
            }
            try
            {
                return userController.Register(Email, Password);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }
    }
}
