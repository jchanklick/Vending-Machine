using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKCardValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: confirm args, usage message

            string hexID = args[0];

            // strip out extra spaces
            hexID = hexID.Replace(" ", "");

            // TODO: assert correct length

            // strip out spaces and take the last 4 octets only (this is a 16-digit string)
            hexID = hexID.Substring(8);

            string binaryID = String.Join(String.Empty,
              hexID.Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
              )
            );

            // card batch = 8 digits, starting at digit 7
            // card id = 15 digits, starting at digit 16
            string binaryCardBatch = binaryID.Substring(7, 8);
            string binaryCardID = binaryID.Substring(16, 15);
            
            // convert to decimal
            long cardBatch = Convert.ToInt64(binaryCardBatch, 2);
            long cardID = Convert.ToInt64(binaryCardID, 2);

            Console.WriteLine("Hex: " + hexID);
            Console.WriteLine("Binary: " + binaryID);
            Console.WriteLine("Binary Batch ID: " + binaryCardBatch);
            Console.WriteLine("Binary Card ID: " + binaryCardID);
            Console.WriteLine("Final ID: " + cardBatch + " - " + cardID);
            
            // TODO: look up in keyscan database

            // TODO: track event in local database

            // TODO: write message to serial port
        }
    }
}
