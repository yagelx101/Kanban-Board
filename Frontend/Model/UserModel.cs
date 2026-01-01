using Frontend.Model.Controllers;
using Frontend.View;
using Frontend.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class UserModel
    {
        public string Email { get; }
        internal UserModel(UserSL user)
        {
            Email = user.email;

        }

        
    }
}
