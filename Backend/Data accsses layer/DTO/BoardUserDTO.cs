using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class BoardUserDTO
    {
        public const string BoardIDColumnName = "board_id";
        public const string EmailColumnName = "email";
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BoardUserController BoardUserController { get; set; }
        public bool isPersisted { get; set; } = false;
        public int boardId;
        public int BoardId
        {
            get => boardId;
            set { boardId = value; }
        }
        private string email;
        public string Email
        {
            get => email;
            set { email = value; }
        }
        public List<BoardUserDTO> BoardMembers
        { get; private set; }
        public BoardUserDTO(int boardId, string email)
        {
            this.BoardUserController = new BoardUserController();
            this.BoardId = boardId;
            this.Email = email;

        }
        /// <summary>
        /// Saves the current object if it hasn't been persisted yet. Throws an exception if the object is already
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void save()
        {
            if (isPersisted)
            {
                throw new ArgumentException("cannot save persisted object");
            }
          
            BoardUserController.Insert(this);
            isPersisted = true;
            log.Info($"BoardUser {Email} saved successfully to board {BoardId}.");

        }
    }
}
