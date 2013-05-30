using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace ProjectKService.Model
{
    public partial class CardScan
    {
        // returns the most recent card scan
        public static CardScan LastCardScan()
        {
            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                CardScan cs = (
                    from c in context.CardScans orderby c.ScanDate descending
                    select c).FirstOrDefault();
                return cs;
            }
        }

        // returns the most recent card scan result (there should only be one anyway!)
        public CardScanResult CardScanResult
        {
            get
            {
                using (klick_vending_machineEntities context = new klick_vending_machineEntities())
                {   
                    CardScanResult result = (
                        from r in this.CardScanResults orderby r.ResultDate descending
                        select r
                        ).FirstOrDefault();
                    return result;
                }
            }
        }

        // returns true if this scan has timed out
        public bool HasTimedOut
        {
            get
            {
                // TODO: confirm timeout
                return DateTime.Now.Subtract(ScanDate).TotalSeconds >= 60; 
            }
        }

        // returns true if a successful vend request is associated with this card scan
        public bool HasVended
        {
            get
            {
                return CardScanResults.Where(r => r.HasVended).Count() > 0;
            }
        }
    }
}
