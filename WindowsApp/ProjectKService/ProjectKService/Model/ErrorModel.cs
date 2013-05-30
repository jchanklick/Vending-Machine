using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService.Model
{
    public partial class Error
    {
        public Error(Exception e)
        {
            ErrorMessage = e.Message;
            ErrorStackTrace = e.StackTrace;

            if (e.InnerException != null)
            {
                ChildErrorMessage = e.InnerException.Message;
                ChildErrorStackTrace = e.InnerException.StackTrace;
            }
        }

        public static Error CreateError(Exception e)
        {
            return new Error(e);
        }
    }
}
