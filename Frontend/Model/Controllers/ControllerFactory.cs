using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model.Controllers
{
    internal class ControllerFactory
    {
        public static ControllerFactory Instance { get; } = new ControllerFactory();
        public readonly ServiceFactory serviceFactory;
        public readonly UserController UserController;
        public readonly BoardController BoardController;
       
        private ControllerFactory()
        {
            this.serviceFactory = new ServiceFactory();
            this.UserController = new UserController(serviceFactory.userS);
            this.BoardController = new BoardController(serviceFactory.boardS);
            serviceFactory.LoadData();
        }
    }
}
