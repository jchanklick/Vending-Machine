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
        public static void Start()
        {
            using (SqlConnection conn = new SqlConnection(SqlUtils.DefaultConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand("select * from CardScan", conn);
                SqlDependency d = new SqlDependency(command);
                d.OnChange += new OnChangeEventHandler(OnCardScanChange);
                command.ExecuteReader();
            }
        }

        public static void Stop()
        {
            SqlDependency.Stop(SqlUtils.DefaultConnectionString);
        }

        private static void OnCardScanChange(object sender, SqlNotificationEventArgs e)
        {
            Logger.WriteLine("OnCardScanChange!!!");
        }

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
                    result.Status = "failed";
                    Error.CreateError(e);
                }

                context.SaveChanges();

                return result;
            }
        }
    }
}
