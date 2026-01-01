 ﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Data_accsses_layer;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer

{
    internal class UserFacade
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, UserBL> users;

        private AuthenticationFacade authenticationFacade;
        public UserFacade(AuthenticationFacade authenticationFacade)
        {
            users = new Dictionary<string, UserBL>(StringComparer.OrdinalIgnoreCase);
            this.authenticationFacade = authenticationFacade;
            
        }
        /// <summary>
        /// Loads user data by retrieving all user DTOs from the UserController. It then creates UserBL instances and
        /// adds them to a collection.
        /// </summary>
        public void loadDataUser()
        {
            log.Info("Loading user data from the database.");
            UserController userController = new UserController();
            List<UserDTO> userDTOs = userController.SelectAll();
            foreach (UserDTO userDTO in userDTOs)
            {
                UserBL userBL = new UserBL(userDTO.Email, userDTO.Password);
                users.Add(userBL.Email, userBL);
                
            }
        }

        /// <summary>
        /// Deletes all user records from the database. This operation will remove all users and their associated data.
        /// </summary>
        public void deleteDataUser()
        {
            log.Info("Deleting all user data from the database.");
            UserController userController = new UserController();
            users.Clear();
            userController.DeleteAll();
        }

        /// <summary>
        /// Registers a new user by creating an account with the provided credentials. If the email is already in use,
        /// an exception is thrown.
        /// </summary>
        /// <param name="email">The unique identifier for the user, used to check for existing accounts.</param>
        /// <param name="password">The secret key associated with the user account for authentication purposes.</param>
        /// <returns>Returns a UserBL object representing the newly registered user.</returns>
        /// <exception cref="Exception">Thrown when the provided email is already associated with an existing user account.</exception>
        public UserBL Register(string email, string password)
        {
            log.Info($"User {email} is trying to register.");
            if (users.ContainsKey(email))
            {
                log.Error($"User {email} already exists.");
                throw new Exception("Email already registered");
            }
            UserBL u = new UserBL(email, password);
            users.Add(email, u);
            authenticationFacade.login(email);
            log.Info($"User {email} registered successfully.");
            return u;
        }
        /// <summary>
        /// Authenticates a user by verifying their credentials and managing their login state.
        /// </summary>
        /// <param name="email">Used to identify the user attempting to log in.</param>
        /// <param name="password">Used to verify the user's identity during the login process.</param>
        /// <returns>Returns the user object if authentication is successful.</returns>
        /// <exception cref="Exception">Thrown if the user does not exist, the password is incorrect, or the user is already logged in.</exception>
        public UserBL Login(string email, string password)
        {
            log.Info($"User {email} is trying to log in."); 
            if (!users.ContainsKey(email))
            {
                log.Error($"User {email} does not exist.");
                throw new Exception($"User does not exist");
            }
            UserBL u = users[email];
            if (!u.login(password))
            {
                log.Error($"User {email} failed to log in. Incorrect password.");
                throw new Exception("Incorrect password");
            }
            if (authenticationFacade.IsLoginIn(email))
            {
                log.Error($"User {email} is already logged in.");
                throw new Exception($"User already concted.");
            }
            authenticationFacade.login(email);
            log.Info($"User {email} logged in successfully.");
            return u;
        }
        /// <summary>
        /// Logs out a user based on their email address. Throws an exception if the user is not currently logged in.
        /// </summary>
        /// <param name="email">The email address of the user attempting to log out.</param>
        /// <exception cref="Exception">Thrown when the specified user is not found in the logged-in users.</exception>
        public void Logout(string email)
        {
            log.Info($"User {email} is trying to log out.");
            if (!users.ContainsKey(email))
            {
                log.Error($"User {email} does not exist.");
                throw new Exception($"User is not logged in");
            }
            UserBL u = users[email];
            authenticationFacade.logout(email);
            log.Info($"User {email} logged out successfully.");


        }
    }
}


