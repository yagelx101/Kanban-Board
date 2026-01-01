using IntroSE.Kanban.Backend.BusinessLayer; 
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendTests
{
    public class testBoardService
    {

        private ServiceFactory factory;
        private TaskService taskService;
        private UserService userService;
        private BoardService service;

        public testBoardService()
        {
            this.factory = new ServiceFactory();
            this.taskService = factory.taskS;
            this.service = factory.boardS;
            this.userService = factory.userS;
            userService.Register("testuser@example.com", "Aa123456");
        }

        /// <summary>
        //This function tests Requirement 8 part 1 (add board)
        /// <summary>
        public void CreateBoard_ValidInput_ShouldReturnEmptyResponse()
        {

            var email = "testuser5@example.com";
            var boardName = "MyBoard5";
            userService.Register(email, "Aa123456");
            var result = service.CreateBoard(email, boardName);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Valid board creation.");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }
        public void CreateBoard_EmptyBoardName_ReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "";
            var expected = "invalid board name";
            var result = service.CreateBoard(email, boardName);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Empty board name rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void CreateBoard_BoardAlreadyExists_ShouldReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "ExistingBoard";
            var expected = "Board with this name already exists";
            service.CreateBoard(email, boardName);

            var result = service.CreateBoard(email, boardName);


            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: Duplicate board name rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }

        public void CreateBoard_invaildEmail_ShouldReturnErrorMessage()
        {
            var email = "nonexisting@example.com";
            var boardName = "MyBoard";
            var expected = "The email not exist";
            var result = service.CreateBoard(email, boardName);

            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: Cannot create board with unregistered email.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }



        /// <summary>
        //This function tests Requirement 8 part 1 (add board)
        /// <summary>
        public void DeleteBoard_ValidInput_ShouldReturnEmptyResponse()
        {

            var email = "testuseyr@example.com";
            userService.Register(email, "ASD765282d");
            var boardName = "MyBooardd";
            service.CreateBoard(email, boardName);

            var result = service.DeleteBoard(email, boardName);

            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);

            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Valid board deletion.");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }

        public void DeleteBoard_InvaildEmail_ShouldReturnErrorMessage()
        {
            var email = "inValidEmail";
            var boardName = "MyBoard";
            var expected = "The email not exist";
            var result = service.DeleteBoard(email, boardName);

            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: Cannot Delete board with unregistered email.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }





        public void DeleteBoard_BoardNotFound_ShouldReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "NonExistingBoard";
            var expected = "Board does not exist";
            var result = service.DeleteBoard(email, boardName);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: Deletion of non-existing board rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }

        public void DeleteBoard_EmptyBoardName_ShouldReturnErrorMessage()
        {
            var email = "testuser@example.com";
            var boardName = "";
            var expected = "User does not have this board";
            var result = service.DeleteBoard(email, boardName);

            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: Empty board name rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        /// <summary>
        // This function tests Requirement 11
        /// <summary>
        public void LimitColumn_ValidInput_ShouldReturnEmptyResponse()
        {
            var email = "testuser@example.com";
            var boardName = "ProjectX";
            var title = "Task 1";
            var description = "Description of Task 1";
            var dueDate = DateTime.Now.AddDays(1);
            var columnOrdinal = 1;
            var limit = 5;

            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, title, description, dueDate);
            var result = service.LimitColumn(email, boardName, columnOrdinal, limit);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Column limit set successfully.");
            else
                Console.WriteLine($"Failed: Expected success, got ' {resultJson}'");
        }
        public void LimitColumn_InvalidColumnOrdinal_ShouldReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "ProjectX";
            var columnOrdinal = -1;
            var limit = 5;
            var expected = "the column number is not valid";

            service.CreateBoard(email, boardName);

            var result = service.LimitColumn(email, boardName, columnOrdinal, limit);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Invalid column ordinal rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void LimitColumn_NegativeLimit_ShouldReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "ProjectX";
            var columnOrdinal = 1;
            var limit = -3;
            var expected = "The limit can not be negative or zero";

            service.CreateBoard(email, boardName);

            var result = service.LimitColumn(email, boardName, columnOrdinal, limit);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Negative limit rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void LimitColumn_LimitSmallerThanExistingTasks_ShouldReturnErrorMessage()
        {

            var email = "testuser@example.com";
            var boardName = "ProjectX";
            var columnOrdinal = 0;
            var expected = "The limit can not be less than the number of tasks in the column";

            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task 1", "", DateTime.Now.AddDays(1));
            taskService.AddTask(email, boardName, "Task 2", "", DateTime.Now.AddDays(2));
            taskService.AddTask(email, boardName, "Task 3", "", DateTime.Now.AddDays(3));

            var result = service.LimitColumn(email, boardName, columnOrdinal, 2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Limit too small rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void LimitColumn_InvalidEmailformat_ShouldReturnErrorMessage()
        {
            var email = "invalid-email";
            var boardName = "ProjectX";
            var columnOrdinal = 0;
            var limit = 5;
            var expected = "The email or board name not exist";

            var result = service.LimitColumn(email, boardName, columnOrdinal, limit);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Invalid email rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }


        /// <summary>
        // // This function tests Requirement 11
        /// <summary>
        public void GetColumnLimit_ValidInput_ShouldReturnCorrectLimit()
        {

            var email = "user@example.com";
            var boardName = "MyBoard";
            var columnNum = 1;
            var expected = 10;

            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);
            service.LimitColumn(email, boardName, columnNum, expected);

            var result = service.GetColumnLimit(email, boardName, columnNum);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);

            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Correct limit returned.");
            else
                Console.WriteLine($"Failed: Expected success, got ' {resultJson}'");
        }
        public void GetColumnLimit_EmailNotRegistered_ShouldReturnError()
        {

            var email = "notfound@example.com";
            var boardName = "AnyBoard";
            var columnNum = 1;
            var expected = "The email or board name not exist";

            var result = service.GetColumnLimit(email, boardName, columnNum);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Unregistered email correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void GetColumnLimit_BoardNotFound_ShouldReturnError()
        {

            var email = "user@example.com";
            var boardName = "NotExistBoard";
            var columnNum = 1;
            var expected = "The email or board name not exist";

            userService.Register(email, "Abc12345");

            var result = service.GetColumnLimit(email, boardName, columnNum);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Non-existent board correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void GetColumnLimit_InvalidColumnOrdinal_ShouldReturnError()
        {

            var email = "user@example.com";
            var boardName = "MyBoard";
            var columnNum = 99;
            var expected = "the column number is not valid";

            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);

            var result = service.GetColumnLimit(email, boardName, columnNum);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Invalid column index correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        /// <summary>
        //// This function tests Requirement 14
        ////// <summary>
        public void AdvanceTask_ValidInput_ShouldReturnEmptyResponse()
        {

            var email = "user@example.com";
            var boardName = "MyBoard13";


            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "desc", DateTime.Now.AddDays(1));
            var taskID = 1;
            taskService.AssignTask(1, boardName, 0, email, email);
            var result = service.AdvanceTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);

            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Task advanced successfully.");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }
        public void AdvanceTask_EmailNotRegistered_ShouldReturnError()
        {

            var email = "inVaild";
            var boardName = "MyBoard";
            var taskID = 4;
            var expected = "The email or board name not or task ID exist";

            var result = service.AdvanceTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Unregistered email correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void AdvanceTask_BoardNotFound_ShouldReturnError()
        {

            var email = "user@example.com";
            var boardName = "NotExistBoard";
            var taskID = 3;
            var expected = "The email or board name not or task ID exist";

            userService.Register(email, "Abc12345");

            var result = service.AdvanceTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Non-existent board correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void AdvanceTask_TaskNotFound_ShouldReturnError()
        {

            var email = "user@example.com";
            var boardName = "MyBoard";
            var taskID = 999;
            var expected = "The email or board name not or task ID exist";

            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);

            var result = service.AdvanceTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Non-existent task correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void AdvanceTask_AlreadyInLastColumn_ShouldReturnError()
        {

            var email = "user@example.com";
            var boardName = "MyBoard";
            var taskID = 1;
            var expected = "the task is already in the last column";

            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "desc", DateTime.Now.AddDays(1));


            service.AdvanceTask(email, boardName, 0, taskID);
            service.AdvanceTask(email, boardName, 1, taskID);

            var result = service.AdvanceTask(email, boardName, 2, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Prevented advancing task from last column.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void AdvanceTask_TaskIDNotEXIST_ShouldReturnError()
        {

            var user1 = "user1@example.com";
            var user2 = "user2@example.com";
            var boardName = "MyBoard";
            var taskID = 0;
            var expected = "The email or board name not or task ID exist";

            userService.Register(user1, "Abc12345");
            service.CreateBoard(user1, boardName);
            taskService.AddTask(user1, boardName, "Task1", "desc", DateTime.Now.AddDays(1));

            var result = service.AdvanceTask(user1, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Cannot advance task that not exist");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }

        /// <summary>
        /// this method test advance task by someone that is not the assignee (requirement 19)
        /// </summary>
        public void AdvanceTask_NotAssignee_ShouldReturnError()
        {

            var user1 = "user1@example.com";
            var boardName = "MyBoard";
            var taskID = 1;
            var expected = "user not assignee can not advanced task";

            userService.Register(user1, "Abc12345");
            service.CreateBoard(user1, boardName);
            taskService.AddTask(user1, boardName, "Task1", "desc", DateTime.Now.AddDays(1));

            var result = service.AdvanceTask(user1, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Cannot advance task by user who is not the assignee");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }

        /// <summary>
        /// This function tests Requirement 17
        /// <summary>
        public void InProgressTasks_ValidInput_ShouldReturnOnlyInProgressTasks()
        {

            var email = "user9@example.com";
            var boardName = "MyBoard";
            userService.Register(email, "Abc12345");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "desc1", DateTime.Now.AddDays(3));
            taskService.AddTask(email, boardName, "Task2", "desc2", DateTime.Now.AddDays(2));

            taskService.AssignTask(1,boardName,0, email, email);
            taskService.AssignTask(2, boardName, 0, email, email);
            service.AdvanceTask(email, boardName,0, 1);


            var result = service.InProgressTasks(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var taskArray = resultJson.GetProperty("ReturnValue").EnumerateArray().ToList();

            var expectedCount = 1;
            if (taskArray.Count == expectedCount && taskArray[0].GetProperty("Title").GetString() == "Task1")
                Console.WriteLine("Passed: Correct in-progress task returned.");
            else
                Console.WriteLine($"Failed: Expected {expectedCount} in-progress task(s), got {taskArray.Count}");
        }
        public void InProgressTasks_EmailInVaild_ShouldReturnError()
        {

            var email = "InVaild";
            var expected = "The email not exist";
            var result = service.InProgressTasks(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: InVaild email correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void InProgressTasks_NoInProgressTasks_ShouldReturnEmptyList()
        {


            var email = "user2@example.com";
            var boardName = "EmptyBoard";
            userService.Register(email, "Xyz12345");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "desc", DateTime.Now.AddDays(1));
            taskService.AddTask(email, boardName, "Task2", "desc", DateTime.Now.AddDays(2));

            var result = service.InProgressTasks(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var taskArray = resultJson.GetProperty("ReturnValue").EnumerateArray().ToList();

            if (taskArray.Count == 0)
                Console.WriteLine("Passed: No in-progress tasks as expected.");
            else
                Console.WriteLine($"Failed: Expected 0 in-progress tasks, got {taskArray.Count}");
        }
        //// this function tests Requirement 13
        public void GetBoards_Valid_ShouldReturnUserBoards()
        {

            var email = "user@example.com";

            userService.Register(email, "Abc12345");
            service.CreateBoard(email, "Board 1");
            service.CreateBoard(email, "Board 2");

            var result = service.getBoards(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var boards = resultJson.GetProperty("ReturnValue").EnumerateArray().ToList();


            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Correct number of boards returned.");
            else
                Console.WriteLine($"Failed: Expected succese got {resultJson}");
        }
        public void GetBoards_EmailInVaild_ShouldReturnError()
        {

            var email = "notfound@example.com";
            var expected = "The email not exist";

            var result = service.getBoards(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: InVaild email correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void GetBoards_RegisteredUserWithoutBoards_ShouldReturnEmptyList()
        {
            ;
            var email = "user23@example.com";
            var expected = "User does not have any boards";
            userService.Register(email, "Xyz12345");
            var result = service.getBoards(email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            if (errorMessage == expected)
                Console.WriteLine("Passed: No boards returned as expected.");
            else
                Console.WriteLine($"Failed: Expected 0 boards, got {resultJson}");
        }

        public void GetColumnName_valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            int columnOrdinal = 0; // Valid column ordinal
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.GetColumnName(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ReturnValue").GetString() == "Backlog")
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }

        }

        public void GetColumnName_invalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            int columnOrdinal = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "The email or board name not exist";
            var result = service.GetColumnName(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid email detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void GetColumnName_invalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            int columnOrdinal = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "The email or board name not exist";
            var result = service.GetColumnName(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid board name detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void GetColumnName_invalidColumnOrdinal()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            int columnOrdinal = -1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "the column number is not valid";
            var result = service.GetColumnName(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid column ordinal detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void GetColumnTask_valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            int columnOrdinal = 0;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "Description1", DateTime.Now.AddDays(1));
            taskService.AddTask(email, boardName, "Task2", "Description2", DateTime.Now.AddDays(2));
            taskService.AddTask(email, boardName, "Task3", "Description3", DateTime.Now.AddDays(3));
            var result = service.GetColumnTask(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var tasks = resultJson.GetProperty("ReturnValue").EnumerateArray().ToList();

            if (resultJson.GetProperty("ErrorMessage").GetString() == null && tasks.Count == 0)
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{tasks.Count}' tasks.");
            }
        }
        public void GetColumnTask_valid2()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            int columnOrdinal = 0;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task1", "Description1", DateTime.Now.AddDays(1));
            taskService.AddTask(email, boardName, "Task2", "Description2", DateTime.Now.AddDays(2));
            taskService.AddTask(email, boardName, "Task3", "Description3", DateTime.Now.AddDays(3));
            taskService.AssignTask(1, boardName, 0, email, email);
            var result = service.GetColumnTask(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var tasks = resultJson.GetProperty("ReturnValue").EnumerateArray().ToList();

            if (resultJson.GetProperty("ErrorMessage").GetString() == null && tasks.Count == 1)
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{tasks.Count}' tasks.");
            }
        }

        public void GetColumnTask_invalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            int columnOrdinal = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "The email or board name not exist";
            var result = service.GetColumnTask(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid email detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void GetColumnTask_invalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; //invalid board name
            int columnOrdinal = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "The email or board name not exist";
            var result = service.GetColumnTask(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid board name detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void GetColumnTask_invalidColumnOrdinal()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            int columnOrdinal = -1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var expectedResponse = "the column number is not valid";
            var result = service.GetColumnName(email, boardName, columnOrdinal);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid column ordinal detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        // <summary>
        // This function tests Requirement 12 part 2 (leave board)
        // <summary>
        public void LeaveBoard_vaild_ShouldReturnEmptyResposne()
        {
            int boardID = 1;
            String email = "testuser@example.com";
            userService.Register(email, "ABCv126734");
            service.CreateBoard(email, "boardNamee");
            userService.Register("tessstuser@example.com", "ABCvv61234");
            service.JoinBoard(boardID, "tessstuser@example.com");
            var result = service.LeaveBoard(boardID, "tessstuser@example.com");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: User Leaved The Board Succsesfuly");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }

        public void LeaveBoard_invalidEmail_ShouldReturnErrorMessage()
        {
            String email = "invalidemail"; // Invalid email format
            int boardID = 1;
            var expectedResponse = "the user is not exist";
            var result = service.LeaveBoard(boardID, email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid email detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }
        public void LeaveBoard_UserIsNotInTheBoard()
        {
            String email = "testuser1@example.com";
            String email2 = "testuser@example.com";
            int boardID = 1;
            var expected = "Owner cannot leave his board";
            userService.Register(email, "Abc12345");
            service.CreateBoard(email, "MyBoard");
            userService.Register(email2, "Abcd12345");
            // service.CreateBoard(email2, "MyBoard2");

            var result = service.LeaveBoard(boardID, email2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: user not in the Board correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }

        public void LeaveBoard_InVaild_BoardID()
        {
            String email = "testuser@example.com";
            int boardID = -2;
            var expected = "InVaild boardID";
            var result = service.LeaveBoard(boardID, email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: InVaild BoardID correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");

        }

        public void LeaveBoard_OwnerWantLeaveBoard_return_error()
        {
            String email = "testuser@example.com";
            int boardID = 1;
            userService.Register(email, "Abc12345");
            service.CreateBoard(email, "MyBoard");
            var expected = "Owner cannot leave his board";
            var result = service.LeaveBoard(boardID, email);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: owner try leave his board correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");

        }


        // This function tests Requirement 13
        public void TransferOwnership_vaild()
        {
            String currOwner = "testcuerr@example.com";
            String newOwner = "teustNew@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            service.CreateBoard(currOwner, "MyBooard");
           int boardId = 18;
            service.JoinBoard(boardId, newOwner);
            string boardName = "MyBooard";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: User transfer ownership successfully");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }

        public void TransferOwnership_invalidEmailOwner_ShouldReturnErrorMessage()
        {
            String currOwner = "invalidemail";
            String newOwner = "testNew@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            int boardID = 1;
            string boardName = "Abc12345";
            service.CreateBoard(currOwner, "MyBoard");
            service.JoinBoard(boardID, newOwner);
            var expectedResponse = "The email or board name not exist";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid owner email detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        public void TransferOwnership_invalidEmailNewOwner_ShouldReturnErrorMessage()
        {
            String currOwner = "testcurr@example.com";
            String newOwner = "invalidemail";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            int boardID = 1;
            string boardName = "Abc12345";
            service.CreateBoard(currOwner, "MyBoard");
            service.JoinBoard(boardID, newOwner);
            var expectedResponse = "The email or board name not exist";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid newOwner email detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }
        public void TransferOwnership_newOwnerIsNotInTheBoard()
        {
            String currOwner = "testcurr111@example.com";
            String newOwner = "testnew112@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            int boardID = 1;
            string boardName = "MyBoard";
            service.CreateBoard(currOwner, "MyBoard");

            var expectedResponse = "cant transfer owner user if newOwner not in the board";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: the new owner is not in the board rejected");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }
        public void TransferOwnership_InVaild_Boardname()
        {
            String currOwner = "testcurr@example.com";
            String newOwner = "testnew@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            service.CreateBoard(currOwner, "MyBoard");
            string boardName = null;
            service.JoinBoard(1, newOwner);
            var expected = "Value cannot be null. (Parameter 'key')";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: InVaild Board name correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");

        }
        public void TransferOwnership_InVaild_BoardnameWhithWhaitSpace()
        {
            String currOwner = "testcurr@example.com";
            String newOwner = "testnew@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            service.CreateBoard(currOwner, "MyBoard");
            string boardName = " ";
            service.JoinBoard(1, newOwner);
            var expected = "The email or board name not exist";
            var result = service.TransferOwnership(boardName, currOwner, newOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: InVaild Board name correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");

        }
        public void TransferOwnership_notOwnerTryTransferError()
        {
            String currOwner = "testcurr1@example.com";
            String newOwner = "testnew1@example.com";
            userService.Register(currOwner, "Abc12345");
            userService.Register(newOwner, "Abc123456");
            string boardName = "MyBoard103";
            service.CreateBoard(currOwner, "MyBoard103");
            service.JoinBoard(17, newOwner);
            var expected = "only the owner can transfer ownership";
            var result = service.TransferOwnership(boardName, newOwner, currOwner);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: not owner try transfer ownership  rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");

        }


        public void JoinBoard_valid()
        {
            String email = "estuser@example.com";
            String email2 = "tstuser@example.com";
            userService.Register(email2, "Aa123456");
            String boardName = "TesttBoard";
            int boardId = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.JoinBoard(boardId, email2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
                Console.WriteLine("Passed: Board joined successfully.");
            else
                Console.WriteLine($"Failed: Expected success, got '{resultJson}'");
        }
        public void JoinBoard_invalid_email()
        {
            String email = "testuser@example.com";
            String email2 = "testImvalid.com";
            userService.Register(email2, "Aa123456");
            String boardName = "TestBoard";
            int boardId = 1;
            string expectedResponse = "the user is not exist";
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.JoinBoard(boardId, email2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
                Console.WriteLine("Passed: invalid email detected.");
            else
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
        }


        public void JoinBoard_invalid_boardId()
        {
            String email = "testuser@example.com";
            String email2 = "testuser@example.com";
            userService.Register(email2, "Aa123456");
            String boardName = "TestBoard";
            int boardId = -1;
            string expectedResponse = "InVaild boardID";
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.JoinBoard(boardId, email2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
                Console.WriteLine("Passed: invalid board Id detected.");
            else
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
        }

        public void JoinBoard_invalid_user_not_exist()
        {
            String email = "testuser@example.com";
            String email2 = "testImvalid.com";
            String boardName = "TestBoard";
            int boardId = 1;
            string expectedResponse = "the user is not exist";
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.JoinBoard(boardId, email2);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
                Console.WriteLine("Passed: user not exist detected.");
            else
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
        }

        public void JoinBoard_user_alredy_in_board()
        {
            String email = "testuser@example.com";

            String boardName = "TestBoard";
            int boardId = 1;
            userService.Register(email, "Aa123456");
            service.CreateBoard(email, boardName);
            var result = service.JoinBoard(boardId, email);
            var expectedResponse = "User is alrady in the board";
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
                Console.WriteLine("Passed: User is alrady in the board");
            else
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
        }
    }


        
}

    