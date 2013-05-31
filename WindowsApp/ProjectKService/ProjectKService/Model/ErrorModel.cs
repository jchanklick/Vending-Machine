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

            Created = DateTime.Now;
        }

        public static Error CreateError(Exception e)
        {
            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                Error error = new Error(e);
                context.Errors.Add(error);
                context.SaveChanges();
                return error;
            }
        }
    }
}
