using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService.Model
{
    public partial class VendingRequest
    {
        // returns the most recent request with given status
        public static VendingRequest LastRequestWithStatus(string status)
        {
            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                VendingRequest request = (
                    from r in context.VendingRequests
                    where r.Status == status
                    orderby r.RequestDate descending
                    select r).FirstOrDefault();
                return request;
            }
        }
    }
}
