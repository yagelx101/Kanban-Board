using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.Data_accsses_layer.DTO;
using log4net;
using Microsoft.VisualBasic;
using static System.Data.Entity.Infrastructure.Design.Executor;



namespace IntroSE.Kanban.Backend.Data_accsses_layer
{
    internal class TaskController
    {
        private const string TaskTableName = "Tasks";

        private readonly string _connectionString;
        private readonly string _tableName;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public TaskController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = TaskTableName;
        }

        /// <summary>
        /// Retrieves all tasks associated with a specific board ID and column number from the database.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="columnNum"></param>
        /// <returns></returns>
        public List<TaskDTO> SelectBoardAndColumnTasks(int boardId, int columnNum)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    command.CommandText = $"SELECT * FROM {TaskTableName} WHERE {TaskDTO.BoardIdColumnName} = @boardIdVal AND {TaskDTO.TaskColumnNumber} = @columnNumVal;";

                    SQLiteParameter boardIdParam = new SQLiteParameter("@boardIdVal", boardId);
                    SQLiteParameter columnNumParam = new SQLiteParameter("@columnNumVal", columnNum);
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(columnNumParam);
                    command.Prepare();

                    SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToTask(dataReader));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }
                    command.Dispose();
                    if (connection!= null)
                    {
                        connection.Close();
                    }
                }
            }
            log.Info($"Selected {results.Count} tasks for board ID {boardId} and column number {columnNum} from the database.");
            return results;
        }
        /// <summary>
        /// Retrieves all tasks associated with a specific board ID from the database.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public List<TaskDTO> SelectBoard(int boardId)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {TaskTableName} WHERE {TaskDTO.BoardIdColumnName} = @boardIdVal";

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
                        results.Add(ConvertReaderToTask(dataReader));
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
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
            log.Info($"Selected {results.Count} tasks for board ID {boardId} from the database.");
            return results;
        }

        /// <summary>
        /// Deletes all task records from the database. This operation will remove all tasks and their associated data.
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
            log.Info($"Deleted all tasks from the database.");
            return res > 0;
        }
        /// <summary>
        /// Inserts a new task into the database.
        /// </summary>
        /// <param name="TaskDal"></param>
        /// <returns></returns>
        public bool Insert(TaskDTO TaskDal)
        {
            using (var connection= new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {TaskTableName} ({TaskDTO.TaskIDColumnName}, {TaskDTO.TaskTitleColumnName}, {TaskDTO.TaskDescriptionColumnName}, {TaskDTO.TaskDueDateColumnName}, {TaskDTO.TaskColumnNumber}, {TaskDTO.BoardIdColumnName}, {TaskDTO.TaskCreationTimeColumnName}, {TaskDTO.AssigneeColumnName}) " +
                                          $"VALUES (@taskID, @title, @description, @dueDate, @columnNumber, @boardId, @creationTime, @assignee);";

                    SQLiteParameter taskIdParam = new SQLiteParameter(@"taskID", TaskDal.Id);
                    SQLiteParameter titleParam = new SQLiteParameter(@"title", TaskDal.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"description", TaskDal.Description);
                    SQLiteParameter dueDateParam = new SQLiteParameter(@"dueDate", TaskDal.DueDate);
                    SQLiteParameter columnNumberParam = new SQLiteParameter(@"columnNumber", TaskDal.ColumnNumber);
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardId", TaskDal.BoardId);
                    SQLiteParameter creationTimeParam = new SQLiteParameter(@"creationTime", TaskDal.CreationTime);
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"assignee", TaskDal.Assignee);

                    command.Parameters.Add(taskIdParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(dueDateParam);
                    command.Parameters.Add(columnNumberParam);
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(creationTimeParam);
                    command.Parameters.Add(assigneeParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                   
                }
                catch (SQLiteException e)
                {
                    log.Error("Error inserting task into database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                log.Info($"Inserted task with ID {TaskDal.Id} into the database.");
                return res > 0;
            }
        }
        /// <summary>
        /// Deletes a task from the database based on the provided board ID and task ID.
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        public bool Delete( int boardId,long taskId)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TaskTableName} WHERE {TaskDTO.TaskIDColumnName} = @taskId AND {TaskDTO.BoardIdColumnName} = @BoardId ;"

                };
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskId);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"BoardId", boardId);

                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);   
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error deleting task from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Deleted task with ID {taskId} from board ID {boardId} in the database.");
            return res > 0;
        }

        /// <summary>
        /// Deletes all tasks associated with a specific board ID from the database.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public bool DeleteByBoard(int boardId)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {TaskTableName} WHERE {TaskDTO.BoardIdColumnName} = @BoardId ;"

                };
                SQLiteParameter boardIdParam = new SQLiteParameter(@"BoardId", boardId);

                command.Parameters.Add(boardIdParam);
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error deleting task from database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Deleted all tasks for board ID {boardId} from the database.");
            return res > 0;
        }
        /// <summary>
        /// Updates the title of a task in the database based on the provided task ID and title column name.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="taskTitleColumnName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool UpdateTitle(long taskID,int boardId, string taskTitleColumnName, string title)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,

                    CommandText = $"UPDATE {_tableName} SET {taskTitleColumnName}=@titleValue WHERE {TaskDTO.TaskIDColumnName} = @taskId AND {TaskDTO.BoardIdColumnName}=@boardIdVal;"

                };

                SQLiteParameter titleParam = new SQLiteParameter(@"titleValue", title);
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskID);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", boardId);

                command.Parameters.Add(titleParam);
                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);

                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating task in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Updated title of task with ID {taskID} in board ID {boardId} to '{title}' in the database.");
            return res > 0;
        }
        /// <summary>
        /// Updates the description of a task in the database based on the provided task ID and description column name.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="taskDescriptionColumnName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public bool UpdateDescription(long taskID,int boardId, string taskDescriptionColumnName, string description)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,

                    CommandText = $"UPDATE {_tableName} SET {taskDescriptionColumnName}=@descriptionValue WHERE {TaskDTO.TaskIDColumnName} = @taskId  AND {TaskDTO.BoardIdColumnName}=@boardIdVal;"

                };

                SQLiteParameter descriptionParam = new SQLiteParameter(@"descriptionValue", description);
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskID);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", boardId);


                command.Parameters.Add(descriptionParam);
                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);

                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating task in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Updated description of task with ID {taskID} in board ID {boardId} to '{description}' in the database.");
            return res > 0;
        }
        /// <summary>
        /// Updates the due date of a task in the database based on the provided task ID and due date column name.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="taskDueDateColumnName"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public bool UpdateDueDate(long taskID, int boardId, string taskDueDateColumnName, DateTime dueDate)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,

                    CommandText = $"UPDATE {_tableName} SET {taskDueDateColumnName}=@dueDateValue  WHERE {TaskDTO.TaskIDColumnName} = @taskId  AND {TaskDTO.BoardIdColumnName}=@boardIdVal;"

                };

                SQLiteParameter dueDateParam = new SQLiteParameter(@"dueDateValue", dueDate);
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskID);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", boardId);


                command.Parameters.Add(dueDateParam);
                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);

                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating task in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Updated due date of task with ID {taskID} in board ID {boardId} to '{dueDate}' in the database.");
            return res > 0;
        }
        /// <summary>
        /// Advances a task to the next column in the database based on the provided task ID and column number.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="taskColumnNumber"></param>
        /// <param name="columnNum"></param>
        /// <returns></returns>
        public bool AdvancedTask(long taskID,int boardId, string taskColumnNumber, int columnNum)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {_tableName} SET {taskColumnNumber}=@columnValue WHERE {TaskDTO.TaskIDColumnName} = @taskId  AND  {TaskDTO.BoardIdColumnName} =@boardIdVal;"
                };

                SQLiteParameter columnParam = new SQLiteParameter(@"columnValue", columnNum);
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskID);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", boardId);

                command.Parameters.Add(columnParam);
                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating task in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            log.Info($"Advanced task with ID {taskID} in board ID {boardId} to column number {columnNum} in the database.");
            return res > 0;
        }
        /// <summary>
        /// Assigns a task to a new assignee in the database based on the provided task ID and assignee column name.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="assigneeColumnName"></param>
        /// <param name="newAssignee"></param>
        /// <returns></returns>
        public bool AssignTask(long taskID, int boardId, string assigneeColumnName, string newAssignee)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"UPDATE {_tableName} SET {assigneeColumnName}=@assigneeValue WHERE {TaskDTO.TaskIDColumnName} = @taskId  AND  {TaskDTO.BoardIdColumnName} =@boardIdVal;"
                }; 
                SQLiteParameter assigneeParam = new SQLiteParameter(@"assigneeValue", newAssignee);
                SQLiteParameter taskIdParam = new SQLiteParameter(@"taskId", taskID);
                SQLiteParameter boardIdParam = new SQLiteParameter(@"boardIdVal", boardId);

                command.Parameters.Add(assigneeParam);
                command.Parameters.Add(taskIdParam);
                command.Parameters.Add(boardIdParam);
                command.Prepare();
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error("Error updating task in database: " + e.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
            }
            log.Info($"Assigned task with ID {taskID} in board ID {boardId} to new assignee '{newAssignee}' in the database.");
            return res > 0;
        }
        /// <summary>
        /// Converts a SQLiteDataReader object to a TaskDTO object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private TaskDTO ConvertReaderToTask(SQLiteDataReader reader)
        {
            long taskID = reader.GetInt64(reader.GetOrdinal(TaskDTO.TaskIDColumnName));
            string title = reader.GetString(reader.GetOrdinal(TaskDTO.TaskTitleColumnName));
            string description = reader.GetString(reader.GetOrdinal(TaskDTO.TaskDescriptionColumnName));
            DateTime dueDate = reader.GetDateTime(reader.GetOrdinal(TaskDTO.TaskDueDateColumnName));
            int columnNumber = reader.GetInt32(reader.GetOrdinal(TaskDTO.TaskColumnNumber));
            int boardId = reader.GetInt32(reader.GetOrdinal(TaskDTO.BoardIdColumnName));
            DateTime creationTime = reader.GetDateTime(reader.GetOrdinal(TaskDTO.TaskCreationTimeColumnName));
            string assignee = reader.IsDBNull(reader.GetOrdinal(TaskDTO.AssigneeColumnName))
                ? null
                : reader.GetString(reader.GetOrdinal(TaskDTO.AssigneeColumnName));
            TaskDTO task = new TaskDTO(taskID, title, description, dueDate, columnNumber, boardId, creationTime, assignee);
            return task;
        }
    }
}
