using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer
{
    internal class UserController
    {
        private const string MessageTableName = "Users";
        private const string dbName = "kanban.db";
        private readonly string _connectionString;
        private readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the UserController. Sets the connection string for the database and the table
        /// name.
        /// </summary>
        public UserController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),dbName));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = MessageTableName;
        }
        /// <summary>
        /// Retrieves all user records from the database and returns them as a list of UserDTO objects.
        /// </summary>
        /// <returns>A list of UserDTO objects representing all users in the database.</returns>
        public List<UserDTO> SelectAll()
        {
            List<UserDTO> results = new List<UserDTO>();
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
                        results.Add(ConvertReaderToUser(dataReader));
                    }
                }
                catch (SQLiteException e)
                {
                    log.Error("Error deleting from database: " + e.Message);
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
            log.Info($"Selected {results.Count} user from the database.");
            return results;
        }
        /// <summary>
        /// Deletes all user records from the database. This operation will remove all users and their associated data.
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
            log.Info($"Deleted all users from the database.");
            return res > 0;
        }
        /// <summary>
        /// Inserts a new user record into the database.
        /// </summary>
        /// <param name="UserDal">Contains the email and password of the user to be inserted.</param>
        /// <returns>Indicates whether the insertion was successful.</returns>
        public bool Insert(UserDTO UserDal)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {MessageTableName} ({UserDTO.EmailColumnName} ,{UserDTO.PasswordColumnName}) " +
                        $"VALUES (@emailVal,@passVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", UserDal.Email);
                    SQLiteParameter passParam = new SQLiteParameter(@"passVal", UserDal.Password);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error inserting user into database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Inserted user {UserDal.Email} into the database.");
            return res > 0;
        }

        /// <summary>
        /// Converts data from a database reader into a UserDTO object.
        /// </summary>
        /// <param name="reader">Extracts user information from a database reader.</param>
        /// <returns>Returns a UserDTO instance populated with email and password.</returns>
        private UserDTO ConvertReaderToUser(SQLiteDataReader reader)
        {
            string email = reader.GetString(reader.GetOrdinal("email"));
            string password = reader.GetString(reader.GetOrdinal("password"));
            log.Info($"Converted reader to UserDTO with email: {email}");
            return new UserDTO(email, password);
        }
    }
}
