using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Models
{
    public struct ActionResult
    { 
        ActionResultStatus ActionStatus { get; }
        string ErrorMessage { get; }

        public ActionResult(ActionResultStatus actionStatus, string errorMessage)
        {
            this.ActionStatus = actionStatus;
            this.ErrorMessage = errorMessage;
        }

        public bool IsError()
        {
            return ActionStatus == ActionResultStatus.Error;
        }

        public string GetMessage()
        {
            return ErrorMessage;
        }
    }

    public enum ActionResultStatus
    {
        Undefinded = 0,
        Success = 1,
        Unknown = 2,
        Warning = 3,
        Error = 4
    }
}
