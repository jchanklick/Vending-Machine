using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService.Model
{
    public partial class CardScanResult
    {
        // returns true if there is a vending request associated with this request that hasn't failed
        public bool HasVended
        {
            get
            {
                using (klick_vending_machineEntities context = new klick_vending_machineEntities())
                {
                    int count = (
                        from r in context.VendingRequests
                        where r.Status != "failed"
                        select r).Count();
                    return count > 0;
                }
            }
        }
    }
}
