using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Frontend.Model.Controllers
{
    internal class BoardController
    {
        private BoardService boardService;

        public BoardController(BoardService boardService)
        {
            this.boardService = boardService;
        }
        internal BoardModel createBoard(string email, string boardName)
        {
            var jsonResponse = boardService.CreateBoard(email, boardName);
            Response<BoardSL<TaskSL>> response = JsonSerializer.Deserialize<Response<BoardSL<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            return new BoardModel(response.ReturnValue);
        }
        internal void DeleteBoard(string email, string boardName)
        {
            var jsonResponse = boardService.DeleteBoard(email, boardName);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        internal ObservableCollection<string> GetBoardMembers(string email, string boardName)
        {
            var jsonResponse = boardService.GetBoardMembers(email, boardName);
            Response<List<string>> response = JsonSerializer.Deserialize<Response<List<string>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<string> membersList = response.ReturnValue;
            return new ObservableCollection<string>(membersList);
        }

        internal ObservableCollection<BoardModel> GetBoards(string email)
        {
            var jsonResponse = boardService.getBoards(email);
            Response<List<BoardSL<TaskSL>>> response = JsonSerializer.Deserialize<Response<List<BoardSL<TaskSL>>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }

            List<BoardSL<TaskSL>> boardList =response.ReturnValue;
            return new ObservableCollection<BoardModel>(
                boardList.Select(bsl => new BoardModel(bsl)));

        }
        internal List<BoardModel> GetAllBoards(string email)
        {

            var jsonResponse = boardService.getBoards(email);
            Response<List<BoardSL<TaskSL>>> response = JsonSerializer.Deserialize<Response<List<BoardSL<TaskSL>>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<BoardSL<TaskSL>> boardList = response.ReturnValue;
            return boardList.Select(bsl => new BoardModel(bsl)).ToList();
        }
        internal ObservableCollection<TaskModel> GetInProgressObserver(string email, string boardName)
        {

            var jsonResponse = boardService.GetColumnTask(email, boardName, 1);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<TaskSL> taskList =response.ReturnValue;
            return new ObservableCollection<TaskModel>(
                taskList.Select(bsl => new TaskModel(bsl)));
            
        }
        internal ObservableCollection<TaskModel> GetDoneObserver(string email, string boardName)
        {

            var jsonResponse = boardService.GetColumnTask(email, boardName, 2);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<TaskSL> taskList = response.ReturnValue;
            return new ObservableCollection<TaskModel>(taskList.Select(bsl => new TaskModel(bsl)));
           
        }
        internal ObservableCollection<TaskModel> GetBacklogObserver(string email, string boardName)
        {

            var jsonResponse = boardService.GetColumnTask(email, boardName, 0);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<TaskSL> taskList =response.ReturnValue;
            return new ObservableCollection<TaskModel>(
                taskList.Select(bsl => new TaskModel(bsl)));
        }


        internal List<TaskModel> GetAllInProgress(string email,string boardName)
        {

            var jsonResponse = boardService.GetColumnTask(email,boardName,1);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
          
            List<TaskSL> taskList = response.ReturnValue;
            return taskList.Select(bsl => new TaskModel(bsl)).ToList();
        }
        internal List<TaskModel> GetAllDone(string email, string boardName)
        {


            var jsonResponse = boardService.GetColumnTask(email, boardName, 2);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<TaskSL> taskList = response.ReturnValue;
            return taskList.Select(bsl => new TaskModel(bsl)).ToList();
        }
        internal List<TaskModel> GetBacklogTask(string email, string boardName)
        {


            var jsonResponse = boardService.GetColumnTask(email, boardName, 0);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse);
            if (response.ErrorOccured)
            {
                throw new Exception(response.ErrorMessage);
            }
            List<TaskSL> taskList = response.ReturnValue;
            return taskList.Select(bsl => new TaskModel(bsl)).ToList();
        }
    }
}

