using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class AuthenticationFacade
    {
        private readonly Dictionary<string, string> auth_;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthenticationFacade()
        {
            auth_ = new Dictionary<string, string>();
        }
        /// <summary>
        ///     Checks if a user is logged in based on their email address.
        /// </summary>
        /// <param name="email">The user's email address is used to verify their login status.</param>
        /// <returns>Returns true if the email is associated with a logged-in user, otherwise false.</returns>
        public bool IsLoginIn(string email)
        {
            if(email == null) {
             throw  new Exception("email cant be null");
                 }
            log.Info($"Checking if user {email} is logged in.");
            return auth_.ContainsKey(email);
            
        }
        /// <summary>
        /// Attempts to log in a user by their email address. If the email already exists, an exception is thrown.
        /// </summary>
        /// <param name="email">The user's email address used to check for existing accounts.</param>
        /// <returns>Returns true if the login is successful and the email is added.</returns>
        /// <exception cref="Exception">Thrown when the provided email address is already associated with an existing account.</exception>
        public bool login(string email)
        {
            log.Info($"User {email} is trying to log in.");
            if (auth_.ContainsKey(email))
            {
                log.Error($"User {email} already exists.");
                throw new Exception($"Email {email} already exists.");
            }
            auth_.Add(email, email);
            return true;

        }
        /// <summary>
        ///     Logs out a user by removing their authentication details from the system.
        /// </summary>
        /// <param name="email">The user's email address is used to identify which account to log out.</param>
        /// <exception cref="Exception">Thrown when the provided email address is not found in the authentication records.</exception>
        public void logout(string email)
        {
            log.Info($"User {email} is trying to log out.");
            if (!auth_.ContainsKey(email))
            {
                log.Error($"User {email} does not exist.");
                throw new Exception($"User is not logged in");
            }
            auth_.Remove(email);

        }
    }
}