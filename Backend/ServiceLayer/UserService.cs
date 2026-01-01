using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserService
    {
        private UserFacade userFacade;
        internal UserService(UserFacade uf)
        {
            this.userFacade = uf;
        }
        /// <summary>
        ///     Registers a new user with the provided credentials.
        /// </summary>
        /// <param name="email">The user's email address used for account creation.</param>
        /// <param name="password">The user's chosen password for account security.</param>
        /// <returns> </returns>
        
        public string Register(string email, string password)
        {
            try
            {
                UserBL user1 = userFacade.Register(email, password);
                UserSL user = new UserSL(user1.Email);
                Response<UserSL> response = new Response<UserSL>(user);
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Login(string email, string password)
        {
            try
            {
                UserBL user1 = userFacade.Login(email, password);
                UserSL user = new UserSL(user1.Email);
                Response<UserSL> response = new Response<UserSL>(user);
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }
        }


        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Logout(string email)
        {
            try
            {
                userFacade.Logout(email);
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }

        }

    }
}
