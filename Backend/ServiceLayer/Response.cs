using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Frontend")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Response<T>
    {
        public string? ErrorMessage { get; set; }
        public T? ReturnValue { get; set; }
        public bool ErrorOccured { get => !string.IsNullOrEmpty(ErrorMessage); }

        public Response() { }

        public Response(string errorMessage, T value)
        {
            ErrorMessage = null;
            ReturnValue = value;
        }
        public Response(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public Response(T value)
        {
            ReturnValue = value;
        }

    }


}