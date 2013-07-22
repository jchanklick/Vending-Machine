using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.SqlClient;

namespace ProjectKService.Model
{
    public partial class VendingRequest
    {
        // returns the last outstanding request; status in (processing, vending) and within a timeout interval
        public static VendingRequest LastOutstandingRequest()
        {
            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                int processingTimeoutSeconds = 30;
                int vendingTimeoutSeconds = 60;

                // request in status 'processing'
                VendingRequest request = (
                    from r in context.VendingRequests
                    where r.Status == "processing" && SqlFunctions.DateDiff("second", r.RequestDate, DateTime.Now) < processingTimeoutSeconds
                    orderby r.RequestDate descending
                    select r).FirstOrDefault();

                if (request != null)
                {
                    return request;
                }

                request = (
                    from r in context.VendingRequests
                    where r.Status == "vending" && SqlFunctions.DateDiff("second", r.VendStartDate, DateTime.Now) < vendingTimeoutSeconds
                    orderby r.VendStartDate descending
                    select r).FirstOrDefault();

                return request;
            }
        }
    }
}
