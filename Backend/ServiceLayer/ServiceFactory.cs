using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayers;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("UserFacade")]
[assembly: InternalsVisibleTo("BoardFacade")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class ServiceFactory
    {

        private AuthenticationFacade af;
        private UserFacade userF;
        private BoardFacade boardF;
        public UserService userS;
        public BoardService boardS;
        public TaskService taskS;

        public ServiceFactory()
        {
            var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            af = new AuthenticationFacade();
            this.userF = new UserFacade(af);
            this.boardF = new BoardFacade(af);
            this.userS = new UserService(userF);
            this.boardS = new BoardService(boardF);
            this.taskS = new TaskService(boardF);
        }

        /// <summary>
        ///   Loads all data from the database into the business layer.
        /// </summary>
        /// <returns></returns>
        public string LoadData()
        {
            try
            {
                userF.loadDataUser();
                boardF.LoadDataBoards();
                Response<string> response = new Response<string>();
                return JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                Response<string> response = new Response<string>(ex.Message);
                return JsonSerializer.Serialize(response);
            }            
        }

        public string DeleteData()
        {
            try
            {
                userF.deleteDataUser();
                boardF.deleteDataBoards();
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
