using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class UserDTO
    {
        public UserController userController { get; set; }
        public const string EmailColumnName = "email";
        public const string PasswordColumnName = "password";

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool isPersisted { get; set; } = false;

        private string email;
        public string Email
        {
            get => email;
            set { email = value;  }
        }
        private string password;
        public string Password
        {
            get => password;
            set { password = value;  }
        }
        /// <summary>
        /// Saves the current object if it hasn't been persisted yet. Throws an exception if the object is already
        /// persisted.
        /// </summary>
        public void save()
        {
            if (isPersisted)
                throw new ArgumentException("cannot save persisted object");
            userController.Insert(this);
            isPersisted = true;
            log.Info($"User {Email} saved successfully.");
        }
        /// <summary>
        /// Initializes a new instance of the UserDTO class with specified credentials.
        /// </summary>
        /// <param name="email">Represents the user's email address for authentication purposes.</param>
        /// <param name="password">Represents the user's password for authentication purposes.</param>
        public UserDTO(string email, string password)
        {
            userController = new UserController();
            this.email = email;
            this.password = password;
           
        }
    }
}
