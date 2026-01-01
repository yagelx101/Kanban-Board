using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class BoardDTO
    {
        public BoardController boardController { get; set; }
        public const string BoardIDColumnName = "boardId";
        public const string BoardNameColumnName = "boardName";
        public const string BoardOwnerColumName = "BoardOwner";
        public const string BoardMembersColumnName = "BoardMembers";
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public bool isPersisted { get; set; } = false;

        private int boardId;
        public int BoardId
        {
            get => boardId;
        }
        private string boardName;
        public string BoardName
        {
            get => boardName;
        }

        private string boardOwner;
        public string BoardOwner
        {
            get => boardOwner;
            set { boardOwner = value; boardController.TransferOwnership(boardId, BoardOwnerColumName, value); }
        }

        public BoardDTO(int boardId, string boardName, string boardOwner)
        {
            boardController = new BoardController();
            this.boardId = boardId;
            this.boardName = boardName;
            this.BoardOwner = boardOwner;
        }
        /// <summary>
        /// This method saves the board to the database if it is not already persisted.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Save()
        { 
            if (!isPersisted)
            {
                boardController.Insert(this);
                isPersisted = true;
                log.Info($"Board {BoardId} with name '{BoardName}' saved successfully.");
            }
            else
            {
                throw new ArgumentException("cannot save persisted object}");
            }
        }
        /// <summary>
        /// This method saves the board to the database if it is not already persisted.
        /// </summary>
        /// <param name="column"></param>
        public void addColumn(ColumnDTO column)
        {
            column.save(BoardId);
        }

    }
}
