using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer
{
    internal class BoardController
    {
        public const string MessageTableName = "Boards";
        public const string dbName = "kanban.db";
        private readonly string _connectionString;
        private readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BoardController()

        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), dbName));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = MessageTableName;
        }
        /// <summary>
        /// Retrieves a list of BoardDAL objects from the data source. This method is used to access board data.
        /// </summary>
        /// <returns>Returns a list of BoardDAL instances.</returns>
        public List<BoardDTO> Select()
        {
            List<BoardDTO> results = new List<BoardDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToBoard(dataReader));
                    }
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating while select board in database: " + e.Message);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Retrieved {results.Count} boards from the database.");
            return results;
        }
        /// <summary>
        /// Deletes all board records from the database. This operation will remove all boards and their associated data.
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"DELETE FROM {_tableName};";
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();

                }
                catch (SQLiteException e)
                {
                    log.Error("Error deleting from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Deleted all boards from the database.");
            return res > 0;
        }
        /// <summary>
        /// Retrieves if the object insert successfully into the data source. This method is used to insert board data.
        /// </summary>
        /// <param name="boardDal"></param>
        /// <returns>Returns if the board was inserted successfully.</returns>
        public bool Insert(BoardDTO boardDal)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {MessageTableName} ({BoardDTO.BoardIDColumnName} ,{BoardDTO.BoardNameColumnName} ,{BoardDTO.BoardOwnerColumName}) " + 
                        $"VALUES (@idVal,@nameVal,@OwnerVal);";

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardDal.BoardId );
                    SQLiteParameter titleParam = new SQLiteParameter(@"nameVal", boardDal.BoardName);
                    SQLiteParameter OwnerParam = new SQLiteParameter(@"OwnerVal", boardDal.BoardOwner);


                    command.Parameters.Add(idParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(OwnerParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating board in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Inserted board with ID {boardDal.BoardId} into the database.");
            return res > 0;

        }
        /// <summary>
        /// Retrieves if the object deleted successfully from the data source. This method is used to delete board data.
        /// </summary>
        /// <param name="boardDal"></param>
        /// <returns>Returns if the board was deleted successfully.</returns>
        public bool Delete(int boardId)
        {
           
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where {BoardDTO.BoardIDColumnName}=@idVal"
                };
                SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardId);
                command.Parameters.Add(idParam);
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating board in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Deleted board with ID {boardId} from the database.");
            return res > 0;
        }

        /// <summary>
        /// Retrieves if the owner has changes successfully in the data source. This method is used to transfer owner in the board data.
        /// </summary>
        /// <param name="boardDal"></param>
        /// <returns>Returns if the board owner was changed successfully.</returns>
        public bool TransferOwnership(int boardId,string BoardOwnerColumName, string newOwner )
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{BoardOwnerColumName}]=@ownerValue where {BoardDTO.BoardIDColumnName}=@idVal"
                };
                command.Parameters.Add(new SQLiteParameter(@"ownerValue", newOwner));
                command.Parameters.Add(new SQLiteParameter(@"idVal", boardId));
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating board in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Transferred ownership of board with ID {boardId} to new owner {newOwner} in the database.");
            return res > 0;
        }

             /// <summary>
             /// convert data  from the data base to boardDTO
             /// </summary>
             /// <param name="reader"></param>
             /// <returns> the converted Board DTO</returns>
        private BoardDTO ConvertReaderToBoard(SQLiteDataReader reader)
        {
            int boardId = reader.GetInt32(reader.GetOrdinal(BoardDTO.BoardIDColumnName));
            string boardName = reader.GetString(reader.GetOrdinal(BoardDTO.BoardNameColumnName));
            string boardOwner = reader.GetString(reader.GetOrdinal(BoardDTO.BoardOwnerColumName));
            var board = new BoardDTO(boardId,boardName,boardOwner);
            log.Info($"Converted board with ID {boardId} from database to BoardDTO.");
            return board;
        }
    }
}
