using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService
{
    class CardScanHandler
    {
        public const string MSG_ERROR_INVALID_CARD_ID = "INVALID CARD ID";
        public const string MSG_START_VEND_SEQUENCE = "9999";

        // Called when the card is scanned
        public static void CardScan(string cardID)
        {
            string hexID = cardID.Replace(" ", ""); // strip out extra spaces

            // assert correct length
            if (hexID.Length != 16)
            {
                SerialPortHandler.Write(MSG_ERROR_INVALID_CARD_ID);
                return;
            }

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

            // convert to decimal
            long cardBatch = Convert.ToInt64(binaryCardBatch, 2);
            long cardNumber = Convert.ToInt64(binaryCardNumber, 2);

            // TODO: lookup card in keyscan database and validate

            // TODO: save to database

            // TODO: confirm that we're not currently in a vend sequence

            SerialPortHandler.Write(MSG_START_VEND_SEQUENCE);
        }
    }
}
