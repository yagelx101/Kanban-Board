using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.Data_accsses_layer.DTO
{
    internal class BoardUserController
    {
        public const string MessageTableName = "BoardsMembers";
        public const string dbName = "kanban.db";
        private readonly string _connectionString;
        private readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BoardUserController()

        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), dbName));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = MessageTableName;
        }
        /// <summary>
        /// Retrieves a list of BoardUserDTO  id boards objects from the data source. This method is used to access board data.
        /// </summary>
        /// <returns>Returns a list of BoardUserDTO  id boards instances.</returns>
        public List<string> SelectAllEmail(int boardId)
        {
            List<string> results = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {MessageTableName} WHERE {BoardUserDTO.BoardIDColumnName} = @boardIdVal;";

                SQLiteParameter boardIdParam = new SQLiteParameter("@boardIdVal", boardId);
                command.Parameters.Add(boardIdParam);
                command.Prepare();

                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToBoardUser(dataReader).Email);
                    }
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating while select boardUser in database: " + e.Message);
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

            log.Info($"Selected {results.Count} all emails that have boards - {boardId} from the database.");
            return results;

        }

        /// <summary>
        /// Deletes all boardUser records from the database. This operation will remove all boardUser and their associated data.
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
            log.Info($"Deleted all boardUser from the database.");
            return res > 0;
        }
        /// <summary>
        /// Retrieves if the object insert successfully into the data source. This method is used to insert BoardUserDTO data.
        /// </summary>
        /// <param name="boardDal"></param>
        /// <returns>Returns if the BoardUserDTO was inserted successfully.</returns>
        public bool Insert(BoardUserDTO boardUserDal)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {MessageTableName} ({BoardUserDTO.BoardIDColumnName} ,{BoardUserDTO.EmailColumnName}) " +
                        $"VALUES (@idVal,@nameVal);";

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardUserDal.BoardId);
                    SQLiteParameter titleParam = new SQLiteParameter(@"nameVal", boardUserDal.Email);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(titleParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating while insert boardUser in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Inserted boardUser with boardId - {boardUserDal.BoardId} and email - {boardUserDal.Email} into the database.");
            return res > 0;

        }
        /// <summary>
        /// Retrieves if the object deleted successfully from the data source. This method is used to delete BoardUserDTO data.
        /// </summary>
        /// <param name="boardDal"></param>
        /// <returns>Returns if the BoardUserDTO was deleted successfully.</returns>
        public bool Delete(int boardId ,string email)
        {

            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {_tableName} WHERE {BoardUserDTO.BoardIDColumnName} =@idVal AND {BoardUserDTO.EmailColumnName} =@EmailVal"
                };
                SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardId);
                SQLiteParameter EmailParam = new SQLiteParameter(@"EmailVal", email);
                command.Parameters.Add(idParam);
                command.Parameters.Add(EmailParam);
                command.Prepare();

                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating while deleted boardUser in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Deleted boardUser with boardId - {boardId} and email - {email} from the database.");
            return res > 0;
        }

        /// <summary>
        /// convert data  from the data base to BoardUserDTO
        /// </summary>
        /// <param name="reader"></param>
        /// <returns> the converted BoardUserDTO </returns>
        private BoardUserDTO ConvertReaderToBoardUser(SQLiteDataReader reader)
        {
            int boardId = reader.GetInt32(reader.GetOrdinal(BoardUserDTO.BoardIDColumnName));
            string email = reader.GetString(reader.GetOrdinal(BoardUserDTO.EmailColumnName));
            var boardUser = new BoardUserDTO(boardId, email);
            log.Info($"Converted data from database to BoardUserDTO with boardId - {boardId} and email - {email}.");
            return boardUser;
        }
    }
}
