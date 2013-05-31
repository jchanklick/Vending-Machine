using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using ProjectKService.Model;

namespace ProjectKService
{
    class CardScanHandler
    {
        public static bool _isRunning = false;
        public static Object _lock = new Object();

        public static void Start()
        {
            _isRunning = true;

            while (_isRunning)
            {
                ///Logger.WriteLine("Checking for Card Scan");

                try
                {
                    CardScan lastScan = CardScan.LastCardScan();
                    string message = null;

                    lock (_lock)
                    {
                        if (lastScan != null)
                        {
                            Logger.WriteLine("Found Card Scan: " + lastScan.CardScanID + " (" + lastScan.CardID + ")");

                            if (lastScan.HasTimedOut)
                            {
                                Logger.WriteLine("Card Scan Timed Out");
                            }
                            else if (lastScan.CardScanResult != null)
                            {
                                Logger.WriteLine("Card Scan Already Processed");
                            }
                            else
                            {
                               message = ProcessCardScan(lastScan);
                            }
                        }
                    }

                    if (message != null)
                    {
                        Logger.WriteLine("Message: " + message);
                        SerialPortHandler.Write(message);

                        /*
                         * uncomment this for testing only
                        if (message == "9999")
                        {
                            string result = CommandHandler.ProcessCommand("Req:1,1");
                            Logger.WriteLine("Req1 Response: " + result);
                            result = CommandHandler.ProcessCommand("Req:1,1");
                            Logger.WriteLine("Req2 Response: " + result);
                            result = CommandHandler.ProcessCommand("Req:1,1");
                            Logger.WriteLine("Req3 Response: " + result);
                            result = CommandHandler.ProcessCommand("Vend:1,1");
                            Logger.WriteLine("Vend1 Response: " + result);
                            result = CommandHandler.ProcessCommand("Vend:1,1");
                            Logger.WriteLine("Vend2 Response: " + result);
                        }
                         * */
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteException(e);
                }

                System.Threading.Thread.Sleep(5*1000);
            }
        }

        public static void Stop()
        {
            _isRunning = false;
        }

        // returns the message for the Arduino
        private static string ProcessCardScan(CardScan cardScan)
        {
            CardScanResult result = GetCardScanResult(cardScan);

            if (result.Status == "valid")
            {
                return CommandHandler.MSG_VEND_INPUT_READY;
            }
            
            return CommandHandler.MSG_ERROR_INVALID_CARD;
        }

        private static CardScanResult GetCardScanResult(CardScan cardScan)
        {            
            // now we have to create a result, and validate against the keyscan database
            using(klick_vending_machineEntities context = new klick_vending_machineEntities()) {
                CardScanResult result = new CardScanResult()
                {
                    CardScanID = cardScan.CardScanID,
                    ResultDate = DateTime.Now,
                    Status = "invalid"
                };

                context.CardScanResults.Add(result);

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

                        // lookup card in keyscan database and validate, and grab first and lastname

                        SqlConnection o_sqlConnection = new SqlConnection();
                        o_sqlConnection.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["keyscandb"].ConnectionString;
                        o_sqlConnection.Open();
                        SqlCommand o_sqlCommand = new SqlCommand();
                        o_sqlCommand.CommandText = "Select * from CARD where CARD_BATCH = " + result.CardBatch + " and CARD_NUMBER = " + result.CardNumber + " and isnull([CARD_VALIDFROM], getdate()-1) < Getdate() and isnull([CARD_VALIDTO], getdate() + 1) > getdate() and isnull([CARD_ARCHIVED],0) = 0 and isnull([CARD_LIMITED],0) = 0 and [CARD_DELETED] = 0";
                        o_sqlCommand.Connection = o_sqlConnection;
                        SqlDataReader o_sqlDataReader;
                        o_sqlDataReader = o_sqlCommand.ExecuteReader();
                        if (o_sqlDataReader.Read())
                        {
                            result.CardFirstName = o_sqlDataReader["CARD_FIRSTNAME"].ToString();
                            result.CardLastName = o_sqlDataReader["CARD_LASTNAME"].ToString();
                            result.Status = "valid";
                        }
                        else {
                            result.Status = "invalid";
                        }
                        o_sqlDataReader.Close();
                        o_sqlConnection.Close();

                    }
                }
                catch(Exception e) 
                {
                    Logger.WriteException(e);
                    result.Status = "failed";
                   // Error.CreateError(e);
                }

                context.SaveChanges();

                return result;
            }
        }
    }
}
