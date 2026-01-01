using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using BackendTests;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayers;
using static System.Net.Mime.MediaTypeNames;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]



public class BackendTest
{
   
    static void Main(string[] args)
    {

        BackendTest test = new BackendTest();
        TestTaskService testTaskService = new TestTaskService();
        testBoardService testBoardService = new testBoardService();
        testUserService testUserService = new testUserService();
        Console.WriteLine("Starting Task Service Tests...");
        test.AddTaskTest(testTaskService);
        test.UpdateTaskDueDate(testTaskService);
        test.UpdateTaskTitleTest(testTaskService);
        test.UpdateTaskDescriptionTest(testTaskService);
        test.DeleteTaskTest(testTaskService);
        test.AssignTaskTest(testTaskService);
        Console.WriteLine("Task Service Tests Completed.\n");
        Console.WriteLine("Starting User Service Tests...");
        test.RegisterTest(testUserService);
        test.LoginTest(testUserService);
        test.LogoutTest(testUserService);
        Console.WriteLine("User Service Tests Completed.\n");
        Console.WriteLine("Starting Board Service Tests...");
        test.CreateBoardTest(testBoardService);
        test.DeleteBoardTest(testBoardService);
        test.LimitColumnTest(testBoardService);
        test.GetColumnLimitTest(testBoardService);
        test.AdvancedTaskTest(testBoardService);
        test.InProgressTaskTest(testBoardService);
        test.GetBoardTest(testBoardService);
        test.GetColumnNameTest(testBoardService);
        test.GetColumnTaskTest(testBoardService);
        test.JoinBoardTest(testBoardService);
        test.LeaveBoard(testBoardService);
        test.TransferOwnership(testBoardService);
        Console.WriteLine("Board Service Tests Completed.\n");

    }


    private void AddTaskTest(TestTaskService testTaskService)
    {
        Console.WriteLine("test add task:");
        testTaskService.AddTask_valid();
        testTaskService.AddTask_InvalidEmail();
        testTaskService.AddTask_InvalidBoardName();
        testTaskService.AddTask_WithEmptyTitle();
        testTaskService.AddTask_WithOverMaxTitle();
        testTaskService.AddTask_WithOverMaxDescription();
        testTaskService.AddTask_NoDueDate();
        testTaskService.AddTask_CheckCreationTime();
        testTaskService.AddTask_UniqueTaskID();
        testTaskService.AddTask_NotBoardMember();
    }
    private void UpdateTaskDueDate(TestTaskService testTaskService)
    {
        Console.WriteLine("test update task dueDate:");
        testTaskService.UpdateTaskDueDate_Valid();
        testTaskService.UpdateTaskDueDate_InvalidEmail();
        testTaskService.UpdateTaskDueDate_InvalidBoardName();
        testTaskService.UpdateTaskDueDate_InvalidTaskID();
        testTaskService.UpdateTaskDueDate_InvalidDueDate();
        testTaskService.UpdateTaskDueDate_TaskInDoneColumn();
        testTaskService.UpdateTaskDueDate_NotAssignee();

    }
    private void UpdateTaskTitleTest(TestTaskService testTaskService)
    {
        Console.WriteLine("test update task title:");
        testTaskService.UpdateTaskTitle_Valid();
        testTaskService.UpdateTaskTitle_InvalidEmail();
        testTaskService.UpdateTaskTitle_InvalidBoardName();
        testTaskService.UpdateTaskTitle_InvalidTaskID();
        testTaskService.UpdateTaskTitle_EmptyTitle();
        testTaskService.UpdateTaskTitle_OverMaxLength();
        testTaskService.UpdateTaskTitle_TaskInDoneColumn();
        testTaskService.UpdateTaskTitle_NotAssignee();
    }

    private void UpdateTaskDescriptionTest(TestTaskService testTaskService)
    {
        Console.WriteLine("test update task description:");
        testTaskService.UpdateTaskDescription_Valid();
        testTaskService.UpdateTaskDescription_InvalidEmail();
        testTaskService.UpdateTaskDescription_InvalidBoardName();
        testTaskService.UpdateTaskDescription_InvalidTaskID();
        testTaskService.UpdateTaskDescription_OverMaxLength();
        testTaskService.UpdateTaskDescription_TaskInDoneColumn();
        testTaskService.UpdateTaskDescription_NotAssignee();
    }

    private void DeleteTaskTest(TestTaskService testTaskService)
    {
        Console.WriteLine("test delete task:");
        testTaskService.DeleteTask_Valid();
        testTaskService.DeleteTask_InvalidEmail();
        testTaskService.DeleteTask_InvalidBoardName();
        testTaskService.DeleteTask_InvalidTaskID();
        testTaskService.DeleteTask_NonExistentTask();
        testTaskService.DeleteTask_NotAssignee();
    }
    private void RegisterTest(testUserService testUserService)
    {
        Console.WriteLine("test register:");
        testUserService.Register_InvalidPassword_TooShort();
        testUserService.Register_InvalidEmail();
        testUserService.Register_DuplicateEmail();
        testUserService.Register_ValidUser_ShouldSucceed();
    }

    private void LoginTest(testUserService testUserService)
    {
        Console.WriteLine("test login:");
        testUserService.Login_UserDoesNotExist();
        testUserService.Login_InvalidPassword();
        testUserService.Login_NonExistingUser_ShouldFail();
        testUserService.Login_ValidCredentials_ShouldSucceed();
        testUserService.Login_WrongPassword_ShouldFail();
    }

    private void LogoutTest(testUserService testUserService)
    {
        Console.WriteLine("test logout:");
        testUserService.Logout_WithoutLogin_ShouldFail();
        testUserService.Logout_EmptyEmail_ShouldFail();
    }

    private void CreateBoardTest(testBoardService testBoardService)
    {
        Console.WriteLine("test create board:");
        testBoardService.CreateBoard_ValidInput_ShouldReturnEmptyResponse();
        testBoardService.CreateBoard_EmptyBoardName_ReturnErrorMessage();
        testBoardService.CreateBoard_BoardAlreadyExists_ShouldReturnErrorMessage();
        testBoardService.CreateBoard_invaildEmail_ShouldReturnErrorMessage();
    }

    private void DeleteBoardTest(testBoardService testBoardService)
    {
        Console.WriteLine("test delete board:");
        testBoardService.DeleteBoard_ValidInput_ShouldReturnEmptyResponse();
        testBoardService.DeleteBoard_InvaildEmail_ShouldReturnErrorMessage();
        testBoardService.DeleteBoard_BoardNotFound_ShouldReturnErrorMessage();
        testBoardService.DeleteBoard_EmptyBoardName_ShouldReturnErrorMessage();

    }

    private void LimitColumnTest(testBoardService testBoardService)
    {
        Console.WriteLine("test limit column:");
        testBoardService.LimitColumn_ValidInput_ShouldReturnEmptyResponse();
        testBoardService.LimitColumn_InvalidColumnOrdinal_ShouldReturnErrorMessage();
        testBoardService.LimitColumn_NegativeLimit_ShouldReturnErrorMessage();
        testBoardService.LimitColumn_LimitSmallerThanExistingTasks_ShouldReturnErrorMessage();
        testBoardService.LimitColumn_InvalidEmailformat_ShouldReturnErrorMessage();

    }

    private void GetColumnLimitTest(testBoardService testBoardService)
    {
        Console.WriteLine("test get column limit:");
        testBoardService.GetColumnLimit_ValidInput_ShouldReturnCorrectLimit();
        testBoardService.GetColumnLimit_EmailNotRegistered_ShouldReturnError();
        testBoardService.GetColumnLimit_BoardNotFound_ShouldReturnError();
        testBoardService.GetColumnLimit_InvalidColumnOrdinal_ShouldReturnError();
    }

    private void AdvancedTaskTest(testBoardService testBoardService)
    {
        Console.WriteLine("test advance task:");
        testBoardService.AdvanceTask_ValidInput_ShouldReturnEmptyResponse();
        testBoardService.AdvanceTask_EmailNotRegistered_ShouldReturnError();
        testBoardService.AdvanceTask_BoardNotFound_ShouldReturnError();
        testBoardService.AdvanceTask_TaskNotFound_ShouldReturnError();
        testBoardService.AdvanceTask_AlreadyInLastColumn_ShouldReturnError();
        testBoardService.AdvanceTask_TaskIDNotEXIST_ShouldReturnError();
        testBoardService.AdvanceTask_NotAssignee_ShouldReturnError();

    }

    private void InProgressTaskTest(testBoardService testBoardService)
    {
        Console.WriteLine("test in progress task:");
        testBoardService.InProgressTasks_ValidInput_ShouldReturnOnlyInProgressTasks();
        testBoardService.InProgressTasks_EmailInVaild_ShouldReturnError();
        testBoardService.InProgressTasks_NoInProgressTasks_ShouldReturnEmptyList();

    }

    private void GetBoardTest(testBoardService testBoardService)
    {
        Console.WriteLine("test get board:");
        testBoardService.GetBoards_Valid_ShouldReturnUserBoards();
        testBoardService.GetBoards_EmailInVaild_ShouldReturnError();


    }

    private void GetColumnNameTest(testBoardService testBoardService)
    {
        Console.WriteLine("test get column name:");
        testBoardService.GetColumnName_valid();
        testBoardService.GetColumnName_invalidEmail();
        testBoardService.GetColumnName_invalidBoardName();
        testBoardService.GetColumnName_invalidColumnOrdinal();
    }

    private void GetColumnTaskTest(testBoardService testBoardService)
    {
        Console.WriteLine("test get task list:");
        testBoardService.GetColumnTask_invalidEmail();
        testBoardService.GetColumnTask_invalidBoardName();
        testBoardService.GetColumnTask_invalidColumnOrdinal();
    }


    private void AssignTaskTest(TestTaskService testTaskService)
    {
        Console.WriteLine("test assign task:");
        testTaskService.AssignTask_valid();
        testTaskService.AssignTask_invalidTaskID();
        testTaskService.AssignTask_NotAssigneeTryAssign();
        testTaskService.AssignTask_InvalidNewAssignee();
        testTaskService.AssignTask_NewAssigneeNotInBoard();
    }

    
    private void JoinBoardTest(testBoardService testBoardService)
    {
        Console.WriteLine("test join board:");
        testBoardService.JoinBoard_valid();
        testBoardService.JoinBoard_invalid_email();
        testBoardService.JoinBoard_invalid_boardId();
        testBoardService.JoinBoard_invalid_user_not_exist();
    }

    private void TransferOwnership(testBoardService testBoardService)
    {
        Console.WriteLine("test tranfer ownership:");
        testBoardService.TransferOwnership_InVaild_Boardname();
        testBoardService.TransferOwnership_newOwnerIsNotInTheBoard();
        testBoardService.TransferOwnership_invalidEmailNewOwner_ShouldReturnErrorMessage();
        testBoardService.TransferOwnership_invalidEmailOwner_ShouldReturnErrorMessage();
        testBoardService.TransferOwnership_InVaild_BoardnameWhithWhaitSpace();
    }

    private void LeaveBoard(testBoardService testBoardService)
    {
        Console.WriteLine("test leave board:");
        testBoardService.LeaveBoard_InVaild_BoardID();
        testBoardService.LeaveBoard_invalidEmail_ShouldReturnErrorMessage();
        testBoardService.LeaveBoard_vaild_ShouldReturnEmptyResposne();
    }
}
