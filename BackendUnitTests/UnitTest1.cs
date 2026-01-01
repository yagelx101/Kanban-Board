using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.BusinessLayer
{
    [TestFixture]
    public class BoardBLTests
    {
        private AuthenticationFacade _authFacade;
        private BoardBL _board;

        private const string OwnerEmail = "owner@test.com";
        private const string MemberEmail = "member@test.com";
        private const string NonMemberEmail = "nonmember@test.com";
        private const string BoardName = "Test Board";
        private const int BoardId = 1;


        [SetUp]
        public void Setup()
        {
            _authFacade = new AuthenticationFacade();

            _authFacade.login(OwnerEmail);
            _authFacade.login(MemberEmail);

            _board = new BoardBL(OwnerEmail, BoardName, BoardId, _authFacade);

            _board.BoardMembers.Add(OwnerEmail, "owner");
            _board.BoardMembers.Add(MemberEmail, "member");
        }

        // ===================================
        //            AddTask Tests
        // ===================================

        [Test]
        public void AddTask_UserNotLoggedIn_ThrowsException()
        {
            string notLoggedInUser = "guest@test.com";

            var ex = Assert.Throws<Exception>(() => _board.AddTask(notLoggedInUser, BoardName, "Title", "Desc", DateTime.Now.AddDays(1)));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void AddTask_TitleIsNullOrEmptyOrWhitespace_ThrowsException(string invalidTitle)
        {
            var ex = Assert.Throws<Exception>(() => _board.AddTask(OwnerEmail, BoardName, invalidTitle, "Desc", DateTime.Now.AddDays(1)));
            Assert.That(ex.Message, Is.EqualTo("the title can not be empty or only spaces"));
        }

        [Test]
        public void AddTask_TitleTooLong_ThrowsException()
        {
            string longTitle = new string('a', 51); 

            var ex = Assert.Throws<Exception>(() => _board.AddTask(OwnerEmail, BoardName, longTitle, "Desc", DateTime.Now.AddDays(1)));
            Assert.That(ex.Message, Is.EqualTo("the title can't be with more than 50 characters"));
        }

        [Test]
        public void AddTask_DescriptionTooLong_ThrowsException()
        {
            string longDescription = new string('b', 301); 

            var ex = Assert.Throws<Exception>(() => _board.AddTask(OwnerEmail, BoardName, "Title", longDescription, DateTime.Now.AddDays(1)));
            Assert.That(ex.Message, Is.EqualTo("the description can not be with more than 300 characters"));
        }

        [Test]
        public void AddTask_DueDateInThePast_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.AddTask(OwnerEmail, BoardName, "Title", "Desc", DateTime.Now.AddDays(-1)));
            Assert.That(ex.Message, Is.EqualTo("Due date cannot be in the past"));
        }

        [Test]
        public void AddTask_ValidTask_AddsTaskToBacklogColumn()
        {
            Assert.DoesNotThrow(() =>
            {
                _board.AddTask(OwnerEmail, BoardName, "Valid Title", "Valid Desc", DateTime.Now.AddDays(5));
            });

            var tasks = _board.GetColumnTask(OwnerEmail, 0); 
            Assert.That(tasks.Count, Is.EqualTo(1));
            Assert.That(tasks[0].Title, Is.EqualTo("Valid Title"));
        }

        // ===================================
        //           LeaveBoard Tests
        // ===================================

        [Test]
        public void LeaveBoard_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.LeaveBoard(NonMemberEmail));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        public void LeaveBoard_OwnerTriesToLeave_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.LeaveBoard(OwnerEmail));
            Assert.That(ex.Message, Is.EqualTo("Owner cannot leave his board"));
        }

        [Test]
        public void LeaveBoard_NonMemberTriesToLeave_ThrowsException()
        {
            _authFacade.login("another.user@test.com");

            var ex = Assert.Throws<Exception>(() => _board.LeaveBoard("another.user@test.com"));
            Assert.That(ex.Message, Is.EqualTo("The email not belong to the board"));
        }

        [Test]
        public void LeaveBoard_ValidMemberLeaves_RemovesMember()
        {
            Assert.DoesNotThrow(() => _board.LeaveBoard(MemberEmail));

            
            Assert.IsFalse(_board.BoardMembers.ContainsKey(MemberEmail));
        }

        // ===================================
        //           deleteTask Tests
        // ===================================

        [Test]
        public void DeleteTask_UserNotLoggedIn_ThrowsException()
        {
            _board.AddTask(OwnerEmail, BoardName, "t", "d", DateTime.Now.AddDays(1)); 
            var ex = Assert.Throws<Exception>(() => _board.deleteTask(NonMemberEmail, 0, 1));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        public void DeleteTask_EmptyEmail_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.deleteTask("", 0, 1));
            Assert.That(ex.Message, Is.EqualTo("the email can't be empty"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)] 
        public void DeleteTask_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.deleteTask(OwnerEmail, invalidColumn, 1));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        // ===================================
        //           advanceTask Tests
        // ===================================

        [Test]
        public void AdvanceTask_FromDoneColumn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.advanceTask(OwnerEmail, 2, 1));
            Assert.That(ex.Message, Is.EqualTo("the task is already in the last column"));
        }

        [Test]
        public void AdvanceTask_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.advanceTask(NonMemberEmail, 0, 1));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(-1)]
       
        public void AdvanceTask_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.advanceTask(OwnerEmail, invalidColumn, 1));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }
        [TestCase(2)]
        public void AdvanceTask_InvalidLastColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.advanceTask(OwnerEmail, invalidColumn, 1));
            Assert.That(ex.Message, Is.EqualTo("the task is already in the last column"));
        }

        [Test]
        public void AdvanceTask_UserIsNotAssignee_ThrowsException_WithReflection()
        {
            TaskBL task = _board.AddTask(OwnerEmail, BoardName, "Task", "Desc", DateTime.Now.AddDays(1));

            typeof(TaskBL).GetProperty("Assignee").SetValue(task, MemberEmail, null);

            var ex = Assert.Throws<Exception>(() => _board.advanceTask(OwnerEmail, 0, task.TaskID)); 
            Assert.That(ex.Message, Is.EqualTo("user not assignee can not advanced task"));

            Assert.DoesNotThrow(() =>
            {
                _board.advanceTask(MemberEmail, 0, task.TaskID); 
            });
        }

        // ===================================
        //         TransferOwnership Tests
        // ===================================

        [Test]
        public void TransferOwnership_CurrentUserNotLoggedIn_ThrowsException()
        {
            _authFacade.logout(OwnerEmail);
            var ex = Assert.Throws<Exception>(() => _board.TransferOwnership(OwnerEmail, MemberEmail));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(null, MemberEmail)]
        public void TransferOwnership_NullOrEmptyEmail_ThrowsException(string curr, string newOwner)
        {
            var ex = Assert.Throws<Exception>(() => _board.TransferOwnership(curr, newOwner));
            Assert.That(ex.Message, Is.EqualTo("email cant be null"));
        }
        [TestCase(OwnerEmail, "")]

        public void TransferOwnership_EmptyEmail_ThrowsException(string curr, string newOwner)
        {
            var ex = Assert.Throws<Exception>(() => _board.TransferOwnership(curr, newOwner));
            Assert.That(ex.Message, Is.EqualTo(" curr owner or new owner cannot be null or Empty"));
        }

        [Test]
        public void TransferOwnership_UserIsNotOwner_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.TransferOwnership(MemberEmail, OwnerEmail));
            Assert.That(ex.Message, Is.EqualTo("only the owner can transfer ownership"));
        }

        [Test]
        public void TransferOwnership_NewOwnerIsNotMember_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.TransferOwnership(OwnerEmail, NonMemberEmail));
            Assert.That(ex.Message, Is.EqualTo("cant transfer owner user if newOwner not in the board"));
        }

        [Test]
        public void TransferOwnership_ValidTransfer_ChangesBoardOwner()
        {
            _board.TransferOwnership(OwnerEmail, MemberEmail);

            Assert.That(_board.BoardOwner, Is.EqualTo(MemberEmail));
        }

        [Test]
        public void LimitColumn_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.LimitColumn(NonMemberEmail, 0, 10));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void LimitColumn_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.LimitColumn(OwnerEmail, invalidColumn, 10));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void LimitColumn_ValidLimit_SetsColumnLimitSuccessfully(int columnNum)
        {
            int newLimit = 5;

            Assert.DoesNotThrow(() => _board.LimitColumn(OwnerEmail, columnNum, newLimit));

            int actualLimit = _board.GetColumnLimit(OwnerEmail, columnNum);
            Assert.That(actualLimit, Is.EqualTo(newLimit));
        }

        // ===================================
        //           GetColumnLimit Tests
        // ===================================

        [Test]
        public void GetColumnLimit_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.GetColumnLimit(NonMemberEmail, 0));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetColumnLimit_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.GetColumnLimit(OwnerEmail, invalidColumn));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        public void GetColumnLimit_ValidRequest_ReturnsCorrectLimit()
        {
            _board.LimitColumn(OwnerEmail, 1, 7); 

            int limit = _board.GetColumnLimit(OwnerEmail, 1);

            Assert.That(limit, Is.EqualTo(7));
        }

        // ===================================
        //           GetColumnName Tests
        // ===================================

        [Test]
        public void GetColumnName_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.GetColumnName(NonMemberEmail, 0));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetColumnName_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.GetColumnName(OwnerEmail, invalidColumn));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        [TestCase(0, "Backlog")]
        [TestCase(1, "In Progress")]
        [TestCase(2, "Done")]
        public void GetColumnName_ValidRequest_ReturnsCorrectName(int columnNum, string expectedName)
        {
            string name = _board.GetColumnName(OwnerEmail, columnNum);

            Assert.That(name, Is.EqualTo(expectedName));
        }

        // ===================================
        //           GetColumnTask Tests
        // ===================================

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetColumnTask_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.GetColumnTask(OwnerEmail, invalidColumn));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        public void GetColumnTask_ValidRequest_ReturnsTasksFromCorrectColumn()
        {
            _board.AddTask(OwnerEmail, BoardName, "Task 1", "Desc", DateTime.Now.AddDays(1));
            _board.AddTask(OwnerEmail, BoardName, "Task 2", "Desc", DateTime.Now.AddDays(1));

            
            List<TaskBL> tasks = _board.GetColumnTask(OwnerEmail, 0); 

            Assert.That(tasks.Count, Is.EqualTo(2));
            Assert.IsTrue(tasks.Any(t => t.Title == "Task 1"));
        }

        // ===================================
        //             GetTask Tests
        // ===================================

        [Test]
        public void GetTask_UserNotLoggedIn_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => _board.GetTask(NonMemberEmail, 0, 1));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetTask_InvalidColumnOrdinal_ThrowsException(int invalidColumn)
        {
            var ex = Assert.Throws<Exception>(() => _board.GetTask(OwnerEmail, invalidColumn, 1));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        public void GetTask_TaskExists_ReturnsCorrectTask()
        {
            TaskBL addedTask = _board.AddTask(OwnerEmail, BoardName, "My Task", "Desc", DateTime.Now.AddDays(1));

            TaskBL retrievedTask = _board.GetTask(OwnerEmail, 0, addedTask.TaskID);

            Assert.IsNotNull(retrievedTask);
            Assert.That(retrievedTask.TaskID, Is.EqualTo(addedTask.TaskID));
            Assert.That(retrievedTask.Title, Is.EqualTo("My Task"));
        }

        // ===================================
        //           AssignTask Tests
        // ===================================

        [Test]
        public void AssignTask_AssignerNotLoggedIn_ThrowsException()
        {
            _authFacade.logout(OwnerEmail);

            var ex = Assert.Throws<Exception>(() => _board.AssignTask(1, 0, OwnerEmail, MemberEmail));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }

        [Test]
        [TestCase(" ", MemberEmail)]
       
        public void AssignTask_AssignerIsNullOrWhitespace_ThrowsException(string assigner, string assignee)
        {
            var ex = Assert.Throws<Exception>(() => _board.AssignTask(1, 0, assigner, assignee));
            Assert.That(ex.Message, Is.EqualTo("User is not logged in"));
        }
        [TestCase(OwnerEmail, "  ")]
        public void AssignTask_AssigneeIsNullOrWhitespace_ThrowsException(string assigner, string assignee)
        {
            var ex = Assert.Throws<Exception>(() => _board.AssignTask(1, 0, assigner, assignee));
            Assert.That(ex.Message, Is.EqualTo("the new assignee can't be empty"));
        }

        [Test]
        public void AssignTask_NewAssigneeIsNotMember_ThrowsException_WithReflection()
        {
           
            TaskBL task = _board.AddTask(OwnerEmail, BoardName, "Task", "Desc", DateTime.Now.AddDays(1));

          
            typeof(TaskBL).GetProperty("Assignee").SetValue(task, OwnerEmail, null);

            
            var ex = Assert.Throws<Exception>(() => _board.AssignTask(task.TaskID, 0, OwnerEmail, NonMemberEmail));
            Assert.That(ex.Message, Is.EqualTo("the new assignee is not a member of the board"));
        }


        [TestCase(-1)]
        [TestCase(2)] 
        public void AssignTask_InvalidColumnOrdinal_ThrowsException_WithReflection(int invalidColumn)
        {
           
            TaskBL task = _board.AddTask(OwnerEmail, BoardName, "Task", "Desc", DateTime.Now.AddDays(1));

            typeof(TaskBL).GetProperty("Assignee").SetValue(task, OwnerEmail, null);

            var ex = Assert.Throws<Exception>(() => _board.AssignTask(task.TaskID, invalidColumn, OwnerEmail, MemberEmail));
            Assert.That(ex.Message, Is.EqualTo("the column number is not valid"));
        }

        [Test]
        public void AssignTask_ValidAssignment_UpdatesTaskAssignee_WithReflection()
        {
            
            TaskBL task = _board.AddTask(OwnerEmail, BoardName, "Task to assign", "Desc", DateTime.Now.AddDays(1));

          
            typeof(TaskBL).GetProperty("Assignee").SetValue(task, OwnerEmail, null);
            Assert.That(task.Assignee, Is.EqualTo(OwnerEmail), "Pre-condition failed: Initial assignee was not set correctly.");

            _board.AssignTask(task.TaskID, 0, OwnerEmail, MemberEmail);

            
            TaskBL updatedTask = _board.GetTask(OwnerEmail, 0, task.TaskID);
            Assert.IsNotNull(updatedTask, "The task could not be retrieved after assignment.");
            Assert.That(updatedTask.Assignee, Is.EqualTo(MemberEmail));
        }
    

    // ===================================
    //           JoinBoard Tests
    // ===================================
    [Test]
    
        public void JoinBoard_NewUser_JoinsSuccessfully()
        {

            _board.JoinBoard(NonMemberEmail);

            Assert.IsTrue(_board.BoardMembers.ContainsKey(NonMemberEmail), "The new user (NonMemberEmail) should be in the board's members list after joining.");
        }

        /// <summary>
        /// בדיקה למקרה שגיאה: מוודאת שהמערכת זורקת חריגה כאשר מנסים להוסיף משתמש שכבר חבר בלוח.
        /// </summary>
        [Test]
        public void JoinBoard_UserAlreadyExists_ThrowsException()
        {

            var ex = Assert.Throws<Exception>(() => _board.JoinBoard(MemberEmail));

            Assert.That(ex.Message, Is.EqualTo("User is alrady in the board"));
        }
    }
}