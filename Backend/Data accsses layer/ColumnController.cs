using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;

namespace IntroSE.Kanban.Backend.Data_accsses_layer
{
    internal class ColumnController
    {
        private const string MessageTableName = "Columns";
        private const string dbName = "kanban.db";
        private readonly string _connectionString;
        private readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ColumnController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), dbName));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = MessageTableName;
        }

        /// <summary>
        /// Retrieves all column records associated with a specific board ID from the database.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public List<ColumnDTO> SelectByBoard(int boardId)
        {
            List<ColumnDTO> results = new List<ColumnDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} WHERE {ColumnDTO.BoardIdColumnName} =@BoardIdVal ;";
                SQLiteParameter BoardIdVal = new SQLiteParameter(@"BoardIdVal", boardId);
                command.Parameters.Add(BoardIdVal);
                command.Prepare();
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToColumn(dataReader));
                    }
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
            log.Info($"Selected {results.Count} column from the database for board id {boardId}.");
            return results;
        }

        /// <summary>
        /// Deletes all column records from the database. This operation will remove all columns and their associated data.
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
            log.Info($"Deleted all column from the database.");
            return res > 0;
        }
        /// <summary>
        /// Inserts a new column record into the database.
        /// </summary>
        /// <param name="columnDal">Contains the board id and a column limit, column name to be inserted.</param>
        /// <returns>Indicates whether the insertion was successful.</returns>
        public bool Insert(ColumnDTO columnDal)
        {
           
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    command.CommandText = $"INSERT INTO {_tableName} ({ColumnDTO.BoardIdColumnName} ,{ColumnDTO.ColumnNameColumnName},{ColumnDTO.ColumnLimitColumnName}) " +
                        $"VALUES (@boardIdVal,@ColumnNumVal,@ColumnLimitVal);";

                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", columnDal.BoardId);
                    SQLiteParameter ColumnNameParam = new SQLiteParameter(@"ColumnNumVal", columnDal.ColumnNum);
                    SQLiteParameter ColumnLimit = new SQLiteParameter(@"ColumnLimitVal", columnDal.ColumnLimit);

                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(ColumnNameParam);
                    command.Parameters.Add(ColumnLimit);
                    command.Prepare();

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch(SQLiteException e)
                {
                    log.Error("Error inserting column into database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Inserted column {columnDal.ColumnNum} with limit {columnDal.ColumnLimit} for board {columnDal.BoardId} into the database.");
            return res > 0;

            
        }
        /// <summary>
        /// delete  column from the database.
        /// </summary>
        /// <param name="columnDTO"></param>
        /// <returns></returns>true if the deletion was successful
        public bool Delete(int boardId)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,

                    CommandText = $"delete from {_tableName} where {ColumnDTO.BoardIdColumnName}=@BoardIdVal"
                };
                SQLiteParameter idParam = new SQLiteParameter(@"BoardIdVal", boardId);
                command.Parameters.Add(idParam);
                command.Prepare();
                try
                {

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error deleting column from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Deleted column for board id {boardId} from the database.");
            return res > 0;
        }
        /// <summary>
        /// Updates the limit for a specified column in a board's database entry.
        /// </summary>
        /// <param name="coluumNumber">Specifies which column's limit is being updated.</param>
        /// <param name="limit">Indicates the new limit value to be set for the specified column.</param>
        /// <param name="boardId">Identifies the specific board whose column limit is being updated.</param>
        /// <returns>Returns true if the update was successful, otherwise false.</returns>
        public bool LimitColumnUpdate(int boardId ,int coluumNumber, int limit)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,

                    CommandText = $"UPDATE {_tableName} SET {ColumnDTO.ColumnLimitColumnName}=@limitValue WHERE {ColumnDTO.BoardIdColumnName} = @boardId AND {ColumnDTO.ColumnNameColumnName} = @columnName;"

                };

                SQLiteParameter limitParam = new SQLiteParameter(@"limitValue", limit);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardId", boardId);
                SQLiteParameter columnNameParam = new SQLiteParameter(@"columnName", coluumNumber);

                command.Parameters.Add(limitParam);
                command.Parameters.Add(boardIdParam);
                command.Parameters.Add(columnNameParam);
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating column limit in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Updated column {coluumNumber} limit to {limit} for board {boardId} in the database.");
            return res > 0;
        }
        /// <summary>
        /// Converts data from a SQLiteDataReader into a ColumnDTO object.
        /// </summary>
        /// <param name="reader">The data reader provides access to the database records for extraction.</param>
        /// <returns>Returns a ColumnDTO instance populated with values from the data reader.</returns>
        private ColumnDTO ConvertReaderToColumn(SQLiteDataReader reader)
        {
            int boardId = reader.GetInt32(reader.GetOrdinal(ColumnDTO.BoardIdColumnName));
            int columnLimit = reader.GetInt32(reader.GetOrdinal(ColumnDTO.ColumnLimitColumnName));
            int columnName = reader.GetInt32(reader.GetOrdinal(ColumnDTO.ColumnNameColumnName));
            log.Info($"Converted column data from database: BoardId={boardId}, ColumnName={columnName}, ColumnLimit={columnLimit}.");
            return new ColumnDTO(boardId,columnName,columnLimit);
        }

    }
    
}
