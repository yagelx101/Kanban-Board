using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;
using log4net.Config;


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class UserBL
    {
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 20;
        private string password;
        private string email;
        private UserDTO userDTO;
        private readonly Regex PasswordCharRegex = new Regex("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>/?~`]+$");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UserBL(string email, string password)
        {
            if (!IsValidEmail(email))
            {
                log.Error("Invalid email format");
                throw new ArgumentException("Invalid email format");
            }   
            if (!IsValidPassword(password))
            {
                log.Error("Password must be 6-20 characters long, contain at least one uppercase letter, one lowercase letter, and one number");
                throw new ArgumentException("Password must be 6-20 characters long, contain at least one uppercase letter, one lowercase letter, and one number");
            }
               
            this.email = email;
            this.password = password;
            userDTO = new UserDTO(email, password);
            userDTO.save();
        }


        public UserBL(UserDTO userDTO)
        {
            this.email = userDTO.Email;
            this.password = userDTO.Password;
        }
        /// <summary>
        /// Validates the provided password and returns a success status if it matches the stored password.
        /// </summary>
        /// <param name="password">The input string used to verify access to the system.</param>
        /// <returns>Returns true if the password matches the stored password.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided password does not match the stored password.</exception>
        public bool login(string password)
        {
            log.Info($"User {email} is trying to log in.");
            if (password != this.password)
            {
                log.Error($"User {email} failed to log in. Incorrect password.");
                throw new ArgumentException("Incorrect password");
              
            }
            log.Info($"User {email} logged in successfully.");
            return true;
        }
        /// <summary>
        /// Gets the email address associated with the object. Returns a string representation of the email.
        /// </summary>
        public string Email { get { return email; } }
        /// <summary>
        /// Represents a password with validation rules. Ensures the password is 6-20 characters long and includes an
        /// uppercase letter, a lowercase letter, and a number.
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                if (!IsValidPassword(value))
                {
                    log.Error("Password must be 6-20 characters long, contain at least one uppercase letter, one lowercase letter, and one number");
                    throw new ArgumentException("Password must be 6-20 characters long, contain at least one uppercase letter, one lowercase letter, and one number");
                }
                  
                password = value;
            }
        }

        public Regex PasswordCharRegex1 => PasswordCharRegex;

        /// <summary>
        /// Validates whether a given string is a properly formatted email address.
        /// </summary>
        /// <param name="email">The input string is checked for a valid email format.</param>
        /// <returns>Returns true if the email format is valid, otherwise false.</returns>
        private bool IsValidEmail(string email)
        {
            
            if (email == null || string.IsNullOrWhiteSpace(email) )
                return false;

            log.Info($"Validating email: {email}");
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z]{2,})+$";
            if (email.StartsWith('.'))
            {
                log.Error("Email cannot start with '.' or end with '@'");
                return false;
            }
            int atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@'))
                return false;

            string localPart = email.Substring(0, atIndex);

            char beforeAt = localPart[localPart.Length - 1];
            if (!char.IsLetterOrDigit(beforeAt))
                return false;
            return Regex.IsMatch(email, pattern);
        }
        /// <summary>
        /// Validates the strength of a password based on specific criteria.
        /// </summary>
        /// <param name="password">The input string is checked for length, character variety, and whitespace.</param>
        /// <returns>Returns true if the password meets all strength requirements; otherwise, false.</returns>
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
                return false;

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, @"[a-z]"))
                return false;

            if (!Regex.IsMatch(password, @"[0-9]"))
                return false;
            if (!PasswordCharRegex.IsMatch(password))
                return false;
            return true;
        }
    }
}

