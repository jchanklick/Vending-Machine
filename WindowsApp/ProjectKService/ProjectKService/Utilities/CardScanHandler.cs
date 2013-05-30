using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectKService.Model;

namespace ProjectKService
{
    class CardScanHandler
    {
        public static CardScanResult GetCardScanResult(CardScan cardScan)
        {
            // first check if we already have a card scan
            CardScanResult result = cardScan.CardScanResult;
            if (result != null)
            {
                return result;
            }

            // now we have to create a result, and validate against the keyscan database
            using(klick_vending_machineEntities context = new klick_vending_machineEntities()) {
                result = new CardScanResult()
                {
                    CardScanID = cardScan.CardScanID,
                    ResultDate = DateTime.Now,
                    Status = "invalid"
                };

                try {
                    string hexID = cardScan.CardID.Replace(" ", ""); // strip out extra spaces

                    // assert correct length
                    if (hexID.Length == 16)
                    {
                        // take the last 4 octets only (this is a 16-digit string)
                        hexID = hexID.Substring(8);

                        string binaryID = String.Join(String.Empty,
                          hexID.Select(
                            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                          )
                        );

                        // card batch = 8 digits, starting at digit 7
                        // card id = 15 digits, starting at digit 16
                        string binaryCardBatch = binaryID.Substring(7, 8);
                        string binaryCardNumber = binaryID.Substring(16, 15);

                        // convert to decimal, then padded string (3-digit batch, 5-digit number)
                        result.CardBatch = Convert.ToInt64(binaryCardBatch, 2).ToString().PadLeft(3, '0'); 
                        result.CardNumber = Convert.ToInt64(binaryCardNumber, 2).ToString().PadLeft(5, '0');

                        // TODO: lookup card in keyscan database and validate
                        // for now, just hard-code to valid
                        result.Status = "valid";
                    }
                }
                catch(Exception e) 
                {
                    result.Status = "failed";
                    Error.CreateError(e);
                }

                context.SaveChanges();

                return result;
            }
        }
    }
}
