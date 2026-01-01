using System.Text.Json;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayers;


namespace BackendTests
{
    class TestTaskService
    {
        private ServiceFactory factory;
        private TaskService taskService;
        private BoardService boardService;
        private UserService userService;

        public TestTaskService()
        {
            this.factory = new ServiceFactory();
            this.taskService = factory.taskS;
            this.boardService = factory.boardS;
            this.userService = factory.userS;
            userService.Register("testuser@example.com", "Aa123456");
            boardService.CreateBoard("testuser@example.com", "TestBoard");
        }
        /// <summary>
        /// this function tests adding a task (requirement 13)
        /// </summary>
        public void AddTask_valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "New Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);

            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: Received the expected respone.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }


        /// <summary>
        /// this function tests adding a task with invalid email (requirement 13, 5)
        /// </summary>
        public void AddTask_InvalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            String title = "New Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The email or board name not exist";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
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

        /// <summary>
        /// this function tests adding a task with an invalid board name (requirement 13, 5)
        /// </summary>
        public void AddTask_InvalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            String title = "New Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The email or board name not exist";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Inavalid board name detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests adding a task with an empty title (requirement 13, 5)
        /// </summary>
        public void AddTask_WithEmptyTitle()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "the title can not be empty or only spaces";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Empty title detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests adding a task with an over the max title (requirement 13, 5)
        /// </summary>
        public void AddTask_WithOverMaxTitle()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "the title can't be with more than 50 characters";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Over the max charecters for title detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests adding a task with an over the max description (requirement 13, 5)
        /// </summary>
        public void AddTask_WithOverMaxDescription()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "test";
            String description = new string('a', 301); // Description exceeding 300 characters
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "the description can not be with more than 300 characters";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Over the max charecters for description detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests adding a task with no due date (requirement 13, 5)
        /// </summary>
        public void AddTask_NoDueDate()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "Test Title";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(-1); // Invalid due date (past date)
            var expectedResponse = "Due date cannot be in the past";
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid due date detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

       
        

        /// <summary>
        /// this function tests that the creationTime of a task is correct (requirement 13, 5)
        /// </summary>
        public void AddTask_CheckCreationTime()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "New Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            DateTime beforeCreation = DateTime.Now; // Capture the time before task creation
            var result = taskService.AddTask(email, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ReturnValue").TryGetProperty("CreationTime", out JsonElement creationTimeElement))
            {
                DateTime creationTime = DateTime.Parse(creationTimeElement.GetString());
                if (creationTime >= beforeCreation && creationTime <= DateTime.Now)
                {
                    Console.WriteLine(" Passed: Task creationTime is correct.");
                }
                else
                {
                    Console.WriteLine(" Failed: Task creationTime is incorrect.");
                }
            }
            else
            {
                Console.WriteLine(" Failed: Task creationTime property is missing.");
            }
        }

        /// <summary>
        /// this function tests adding two tasks and ensures they have unique task IDs (requirement 13, 5)
        /// </summary>
        public void AddTask_UniqueTaskID()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title1 = "Task 1";
            String description1 = "Description for Task 1";
            DateTime dueDate1 = DateTime.Now.AddDays(7);
            String title2 = "Task 2";
            String description2 = "Description for Task 2";
            DateTime dueDate2 = DateTime.Now.AddDays(10); // Setting due date 10 days from now
            var result1 = taskService.AddTask(email, boardName, title1, description1, dueDate1);
            var resultJson1 = JsonSerializer.Deserialize<JsonElement>(result1);

            var result2 = taskService.AddTask(email, boardName, title2, description2, dueDate2);
            var resultJson2 = JsonSerializer.Deserialize<JsonElement>(result2);
            if (resultJson1.GetProperty("ReturnValue").TryGetProperty("Id", out JsonElement taskID1Element) &&
                resultJson2.GetProperty("ReturnValue").TryGetProperty("Id", out JsonElement taskID2Element))
            {
                long taskID1 = taskID1Element.GetInt64();
                long taskID2 = taskID2Element.GetInt64();

                if (taskID1 != taskID2)
                {
                    Console.WriteLine(" Passed: Task IDs are unique.");
                }
                else
                {
                    Console.WriteLine(" Failed: Task IDs are not unique.");
                }
            }
            else
            {
                Console.WriteLine(" Failed: Task ID property is missing in one or both tasks.");
            }
        }

        /// <summary>
        /// this method test adding task bt someone who is not board member (requirement 18)
        /// </summary>
        public void AddTask_NotBoardMember()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "Test Title";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "User does not have any boards";
            string user2 = "user2@example.com";
            userService.Register(user2, "Aa123456");
            var result = taskService.AddTask(user2, boardName, title, description, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: User not member try add task detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }


        //----------------------------------------------------------------------------------------------\\

        /// <summary>
        /// this function tests updating the due date of a task (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_Valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            taskService.AssignTask(taskID, boardName, 0, email, email); // Assign the task to the user
            DateTime dueDate = DateTime.Now.AddDays(4);
            var result = taskService.UpdateTaskDueDate(email, boardName,0, taskID, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: Received the expected message.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the due date of a task with an invalid email (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_InvalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            long taskID = 1;
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDueDate(email, boardName,0, taskID, dueDate);
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

        /// <summary>
        /// this function tests updating the due date of a task with an invalid board name (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_InvalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            long taskID = 1;
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDueDate(email, boardName,0, taskID, dueDate);
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

        /// <summary>
        /// this function tests updating the due date of a task with an invalid task ID (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_InvalidTaskID()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = -1; // Invalid task ID (negative value)
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDueDate(email, boardName,0, taskID, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid task ID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the due date of a task with an invalid due date (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_InvalidDueDate()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            DateTime dueDate = DateTime.Now.AddDays(-1); // Invalid due date (past date)
            var expectedResponse = "The due date is not valid";
            var result = taskService.UpdateTaskDueDate(email, boardName, 0, taskID, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid due date detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the due date of a task when the task is in columnNum = 2 (done) (requirement 15)
        /// </summary>
        public void UpdateTaskDueDate_TaskInDoneColumn()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "Test Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7); // Initial due date
            DateTime newDueDate = DateTime.Now.AddDays(10); // New due date
            var addTaskResult = taskService.AddTask(email, boardName, title, description, dueDate);
            var addTaskJson = JsonSerializer.Deserialize<JsonElement>(addTaskResult);

            if (!addTaskJson.GetProperty("ReturnValue").TryGetProperty("Id", out JsonElement taskIDElement))
            {
                Console.WriteLine(" Failed: Could not retrieve taskID from AddTask response.");
                return;
            }

            long taskID = taskIDElement.GetInt64();
            taskService.AssignTask(taskID, boardName, 0, email, email); // Assign the task to the user
            // Move the task to columnNum = 2 (done) using AdvanceTask twice
            boardService.AdvanceTask(email, boardName, 0, taskID); // Move to columnNum = 1
            boardService.AdvanceTask(email, boardName,1, taskID); // Move to columnNum = 2
            var result = taskService.UpdateTaskDueDate(email, boardName,2, taskID, newDueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var expectedResponse = "The task is already completed, you can not change it"; // Expected error response
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: try updating a task in the done column detected.");
            }
            else
            {
                Console.WriteLine($" Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this method test updating the due date of a task when the user is not the assignee (requirement 20)
        /// </summary>
        public void UpdateTaskDueDate_NotAssignee()
        {
            String email = "testuser@example.com";
            boardService.CreateBoard(email, "TestBoard11"); // Create a new board for this test
            String boardName = "TestBoard11";
            String title = "Test Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            var addTaskResult = taskService.AddTask(email, boardName, title, description, dueDate);
            long taskID = 1;
           
            var expectedResponse = "The task can not be updated by someone else than the assignee";
            var result = taskService.UpdateTaskDueDate(email, boardName,0, taskID, dueDate);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: not assignee detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }


        //----------------------------------------------------------------------------------------------\\

        /// <summary>
        /// this function tests updating the title of a task (requirement 15)
        /// </summary>
        public void UpdateTaskTitle_Valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            String newTitle = "Updated Task Title";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the title of a task with an empty title (requirement 15, 5)
        /// </summary>
        public void UpdateTaskTitle_EmptyTitle()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            String newTitle = ""; // Invalid empty title
            var expectedResponse = "the title can not be empty or only spaces";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Empty title detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the title of a task with a title exceeding the max length (requirement 15, 5)
        /// </summary>
        public void UpdateTaskTitle_OverMaxLength()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            String newTitle = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // Title exceeding 50 characters
            var expectedResponse = "the title can not be with more than 50 characters";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Over the max charecters for title detected..");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the title of a task with an invalid email (requirement 15)
        /// </summary>
        public void UpdateTaskTitle_InvalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            long taskID = 1;
            String newTitle = "Updated Task Title";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
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

        /// <summary>
        /// this function tests updating the title of a task with an invalid board name (requirement 15)
        /// </summary>
        public void UpdateTaskTitle_InvalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            long taskID = 1;
            String newTitle = "Updated Task Title";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
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

        /// <summary>
        /// this function tests updating the title of a task with an invalid task ID (requirement 15)
        /// </summary>
        public void UpdateTaskTitle_InvalidTaskID()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = -1; // Invalid task ID (negative value)
            String newTitle = "Updated Task Title";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, newTitle);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid task ID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the title of a task when the task is in columnNum = 2 (done) (requirement 15)
        /// </summary>
        public void UpdateTaskTitle_TaskInDoneColumn()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "Test Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            String newTitle = "Updated Task Title";
            var addTaskResult = taskService.AddTask(email, boardName, title, description, dueDate);
            var addTaskJson = JsonSerializer.Deserialize<JsonElement>(addTaskResult);

            if (!addTaskJson.GetProperty("ReturnValue").TryGetProperty("Id", out JsonElement taskIDElement))
            {
                Console.WriteLine(" Failed: Could not retrieve taskID from AddTask response.");
                return;
            }
            long taskID = taskIDElement.GetInt64();
            taskService.AssignTask(taskID, boardName, 0, email, email); // Assign the task to the user
            // Move the task to columnNum = 2 (done) using AdvanceTask twice
            boardService.AdvanceTask(email, boardName,0, taskID); // Move to columnNum = 1
            boardService.AdvanceTask(email, boardName,1, taskID); // Move to columnNum = 2
            var result = taskService.UpdateTaskTitle(email, boardName,2, taskID, newTitle);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var expectedResponse = "The task is already completed, you can not change it"; // Expected error response
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: try updating a task in the done column detected.");
            }
            else
            {
                Console.WriteLine($" Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this method test updating the title of a task when the user is not the assignee (requirement 20)
        /// </summary>
        public void UpdateTaskTitle_NotAssignee()
        {
            String email = "testuser@example.com";
            boardService.CreateBoard(email, "TestBoard100"); // Create a new board for this test
            String boardName = "TestBoard100";
            DateTime dueDate = DateTime.Now.AddDays(7);
            taskService.AddTask(email, boardName, "Test Task", "Task description", dueDate);
            long taskID = 1;
            String title = "Test Task";
            var expectedResponse = "The task can not be updated by someone else than the assignee";
            var result = taskService.UpdateTaskTitle(email, boardName,0, taskID, title);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: not assignee detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }


        //----------------------------------------------------------------------------------------------\\

        /// <summary>
        /// this function tests updating the description of a task (requirement 15)
        /// </summary>
        public void UpdateTaskDescription_Valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            String newDescription = "Updated Task Description";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, newDescription);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the description of a task with a description exceeding the max length (requirement 15, 5)
        /// </summary>
        public void UpdateTaskDescription_OverMaxLength()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            String newDescription = new string('a', 301); // Description exceeding 300 characters
            var expectedResponse = "the description can not be with more than 300 characters or only spaces";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, newDescription);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Over the max charecters for description detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the description of a task with an invalid email (requirement 15)
        /// </summary>
        public void UpdateTaskDescription_InvalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            long taskID = 1;
            String newDescription = "Updated Task Description";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, newDescription);
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

        /// <summary>
        /// this function tests updating the description of a task with an invalid board name (requirement 15)
        /// </summary>
        public void UpdateTaskDescription_InvalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            long taskID = 1;
            String newDescription = "Updated Task Description";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, newDescription);
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

        /// <summary>
        /// this function tests updating the description of a task with an invalid task ID (requirement 15)
        /// </summary>
        public void UpdateTaskDescription_InvalidTaskID()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = -1; // Invalid task ID (negative value)
            String newDescription = "Updated Task Description";
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, newDescription);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid task ID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests updating the description of a task when the task is in columnNum = 2 (done) (requirement 15)
        /// </summary>
        public void UpdateTaskDescription_TaskInDoneColumn()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "Test Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            String newDescription = "Updated Task Description"; // New description
            var addTaskResult = taskService.AddTask(email, boardName, title, description, dueDate);
            var addTaskJson = JsonSerializer.Deserialize<JsonElement>(addTaskResult);

            if (!addTaskJson.GetProperty("ReturnValue").TryGetProperty("Id", out JsonElement taskIDElement))
            {
                Console.WriteLine(" Failed: Could not retrieve taskID from AddTask response.");
                return;
            }
            long taskID = taskIDElement.GetInt64();
            taskService.AssignTask(taskID, boardName, 0, email, email); // Assign the task to the user
            // Move the task to columnNum = 2 (done) using AdvanceTask twice
            boardService.AdvanceTask(email, boardName,0, taskID); // Move to columnNum = 1
            boardService.AdvanceTask(email, boardName,1, taskID); // Move to columnNum = 2
            var result = taskService.UpdateTaskDescription(email, boardName, 2, taskID, newDescription);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var expectedResponse = "The task is already completed, you can not change it"; // Expected error response
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Received the expected error message when updating a task in the done column.");
            }
            else
            {
                Console.WriteLine($" Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this method test updating the description of a task when the user is not the assignee (requirement 20)
        /// </summary>
        public void UpdateTaskDescription_NotAssignee()
        {
            String email = "testuser@example.com";
            boardService.CreateBoard(email, "TestBoard101"); // Create a new board for this test
            String boardName = "TestBoard101";
            DateTime dueDate = DateTime.Now.AddDays(7);
            taskService.AddTask(email, boardName, "Test Task", "Task description", dueDate);
            long taskID = 1;
            String description = "Task description";
            
            var expectedResponse = "The task can not be updated by someone else than the assignee";
            var result = taskService.UpdateTaskDescription(email, boardName,0, taskID, description);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: not assignee detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        //----------------------------------------------------------------------------------------------\\

        /// <summary>
        /// this function tests deleting a task (requirement 15)
        /// </summary>
        public void DeleteTask_Valid()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            String title = "New Task";
            String description = "Task description";
            DateTime dueDate = DateTime.Now.AddDays(7);
            taskService.AddTask(email, boardName, title, description, dueDate);
            long taskID = 1; // Valid task ID
            taskService.AssignTask(taskID, boardName, 0, email, email); // Assign the task to the user
            var result = taskService.DeleteTask(email, boardName,0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: Received the expected response.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests deleting a task with an invalid email (requirement 15)
        /// </summary>
        public void DeleteTask_InvalidEmail()
        {
            String email = "invalidemail"; // Invalid email format
            String boardName = "TestBoard";
            long taskID = 1;
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.DeleteTask(email, boardName,0, taskID);
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

        /// <summary>
        /// this function tests deleting a task with an invalid board name (requirement 15)
        /// </summary>
        public void DeleteTask_InvalidBoardName()
        {
            String email = "testuser@example.com";
            String boardName = ""; // Invalid board name
            long taskID = 1;
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.DeleteTask(email, boardName,0, taskID);
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

        /// <summary>
        /// this function tests deleting a task with an invalid task ID (requirement 15)
        /// </summary>
        public void DeleteTask_InvalidTaskID()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = -1; // Invalid task ID (negative value)
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.DeleteTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid task ID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests deleting a task that does not exist (requirement 15)
        /// </summary>
        public void DeleteTask_NonExistentTask()
        {
            String email = "testuser@example.com";
            String boardName = "TestBoard";
            long taskID = 9999; // Non-existent task ID (for exmple)
            var expectedResponse = "The email or board name or task ID not exist";
            var result = taskService.DeleteTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: Invalid task ID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this method test deleting task when the user is not the assignee (requirement 20)
        /// </summary>
        public void DeleteTask_NotAssignee()
        {
            String email = "testuser1@example.com";
            String boardName = "TestBoard";
            long taskID = 1;
            DateTime dueDate = DateTime.Now.AddDays(7);
            var expectedResponse = "The task can not be removed by someone not the assignee";
            userService.Register(email, "Aa123456");
            boardService.CreateBoard(email, boardName);
            taskService.AddTask(email, boardName, "Task 1", "Description 1", dueDate);
            var result = taskService.DeleteTask(email, boardName, 0, taskID);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: not assignee detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }


        //-----------------------------------------------------------------------------------------------\\

        /// <summary>
        /// this function tests assigning a task to a user (requirement 23)
        /// </summary>
        public void AssignTask_valid()
        {
            string user1 = "user1@example.com";
            string boardName = "TestBoard";
            long taskID = 1;
            userService.Register(user1, "Aa123456");
            boardService.CreateBoard(user1, boardName);
            taskService.AddTask(user1, boardName, "Task 1", "Description 1", DateTime.Now.AddDays(7));
            var result = taskService.AssignTask(taskID,boardName,0, user1, user1);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == null)
            {
                Console.WriteLine(" Passed: the task is assign to user1.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected success, but got '{resultJson}'.");
            }
        }


        /// <summary>
        /// this function tests assigning a task with an invalid taskId (requirement 23)
        /// </summary>
        public void AssignTask_invalidTaskID()
        {
            string user1 = "user1@example.com";
            string boardName = "TestBoard";
            long taskID = -1;
            string expectedResponse = "The task ID is not valid";
            userService.Register(user1, "Aa123456");
            boardService.CreateBoard(user1, boardName);
            taskService.AddTask(user1, boardName, "Task 1", "Description 1", DateTime.Now.AddDays(7));
            var result = taskService.AssignTask(taskID, boardName, 0, user1, user1);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: invalid taskID detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests assigning a task when someone is not the assigned (requirement 23)
        /// </summary>
        public void AssignTask_NotAssigneeTryAssign()
        {
            string user1 = "user1@example.com";
            string user2 = "user2@example.com";
            string user3 = "user3@example.com";
            string boardName = "TestBoard";
            long taskID = 1;
            string expectedResponse = "User who is not the assignee try to assign the task to someone else";
            userService.Register(user1, "Aa123456");
            userService.Register(user2, "Aa123456");
            userService.Register(user3, "Aa123456");
            boardService.CreateBoard(user1, boardName);
            boardService.JoinBoard(6, user2);
            boardService.JoinBoard(6, user3);
            taskService.AddTask(user1, boardName, "Task 1", "Description 1", DateTime.Now.AddDays(7));
            taskService.AssignTask(taskID, boardName, 0, user1, user2);
            var result = taskService.AssignTask(taskID, boardName, 0, user1, user3);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: someone who is not the assigne try to assign the task.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }


        /// <summary>
        /// this function tests assigning a task with invalid new assignee (requirement 23)
        /// </summary>
        public void AssignTask_InvalidNewAssignee()
        {
            string user1 = "user1@example.com";
            string user210 = "user210@example.com";
            string boardName = "TestBoard";
            long taskID = 4;
            string expectedResponse = "the new assignee is not a member of the board";
            userService.Register(user1, "Aa123456");
            userService.Register(user210, "Aa123456");
            boardService.CreateBoard(user1, boardName);
            taskService.AddTask(user1, boardName, "Task 1", "Description 1", DateTime.Now.AddDays(7));
            taskService.AssignTask(taskID, boardName, 0, user1, user1); // Assign task to user1 first
            var result = taskService.AssignTask(taskID, boardName, 0, user1, user210);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: invalid new assignee detected.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

        /// <summary>
        /// this function tests assigning a task to a user that is not in the board (requirement 23)
        /// </summary>
        public void AssignTask_NewAssigneeNotInBoard()
        {
            string user5 = "user5@example.com";
            string user6 = "user6@example.com";
            string boardName = "TestBoard";
            long taskID = 1;
            string expectedResponse = "the new assignee is not a member of the board";
            userService.Register(user5, "Aa123456");
            userService.Register(user6, "Aa123456");
            boardService.CreateBoard(user5, boardName);
            taskService.AddTask(user5, boardName, "Task 1", "Description 1", DateTime.Now.AddDays(7));
            taskService.AssignTask(taskID, boardName, 0, user5, user5); // Assign task to user5 first
            var result = taskService.AssignTask(taskID, boardName, 0,  user5, user6);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            if (resultJson.GetProperty("ErrorMessage").GetString() == expectedResponse)
            {
                Console.WriteLine(" Passed: try to assign to someone is not the board");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expectedResponse}', but got '{resultJson}'.");
            }
        }

    }
}
