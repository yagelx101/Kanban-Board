using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Frontend")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserSL
    {
        private string Email;

        public UserSL(string email)
        {
           this.Email = email;    
        }
        public string email
        {
            get { return Email; }
        }
    }
}
