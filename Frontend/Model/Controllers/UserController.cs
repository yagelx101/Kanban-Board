using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Frontend.Model.Controllers
{
    internal class UserController
    {
        private UserService us;

        public UserController(UserService us)
        {
            this.us = us;
        }

        public UserModel Register(string username, string password)
        {
            var jsonResponse = us.Register(username, password);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            UserSL user =response.ReturnValue;
            return new UserModel(user);
        }
        public UserModel Login(string username, string password)
        {
            var jsonResponse = us.Login(username, password);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse);

            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            UserSL user = response.ReturnValue;
            return new UserModel(user);
        }
    }
}
