using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService
{
    class CommandHandler
    {
        public const string MSG_ERROR_INVALID_COMMAND = "INVALID COMMAND";
        public const string MSG_ERROR_INVALID_COORDS = "INVALID COORDS";
        public const string MSG_ERROR_INVALID_ITEM = "INVALID ITEM";
        public const string MSG_ERROR_NO_CARD = "SCAN CARD FIRST";
        public const string MSG_ERROR_INVALID_CARD = "BAD CARD";
        public const string MSG_ERROR_CARD_TIMEOUT = "TIMEOUT - SCAN AGAIN";
        public const string MSG_VEND_ITEM = "VEND_OK";


        // Processes the command sent from the Arduino
        // Returns the message to send back to the Arduino (or null if there is nothing to send back)
        public static string ProcessCommand(string command)
        {
            string[] data = command.Split(':');

            if (data.Length == 0)
            {
                return null;
            }

            if (data[0] == "System Ready") 
            {
                return SystemReady();
            }
            else if (data[0] == "Req") 
            {
                return RequestItem(data.Length > 1 ? data[1] : ""); 
            }
            else if (data[0] == "Vend") 
            {
                return VendComplete(data.Length > 1 ? data[1] : ""); 
            }
            
            // unknown command
            return MSG_ERROR_INVALID_COMMAND;
        }

        private static string SystemReady() 
        {
            return null;
        }

        // Called when the user has pushed some coordinates
        private static string RequestItem(string coordinateString)
        {
            int x = -1;
            int y = -1;

            if (!GetCoordinates(coordinateString, out x, out y))
            {
                return MSG_ERROR_INVALID_COORDS;
            }

            if (!IsValidItem(x, y))
            {
                return MSG_ERROR_INVALID_ITEM;
            }

            // TODO: check database for last card scan

            // TODO: confirm that it's within the timeout window

            return MSG_VEND_ITEM;
        }

        // Called after the machine has successfully vended the item
        // TODO: not sure that we care about coordinates here
        private static string VendComplete(string coordinateString)
        {
            int x = -1;
            int y = -1;

            if (!GetCoordinates(coordinateString, out x, out y))
            {
                return MSG_ERROR_INVALID_COORDS;
            }

            if (!IsValidItem(x, y))
            {
                return MSG_ERROR_INVALID_ITEM;
            }

            // TODO: look up previous completed request in database

            // TODO: mark as complete

            return null;
        }

        private static bool GetCoordinates(string coordinateString, out int x, out int y) 
        {
            string[] data = coordinateString.Split(',');
            if (data.Length == 2)
            {
                // Note that coordinate string is in format y,x
                if (int.TryParse(data[0], out y) && int.TryParse(data[1], out x))
                {
                    return true;
                }
            }

            x = -1;
            y = -1;

            return false;
        }

        private static bool IsValidItem(int x, int y)
        {
            return x >= 1 && x <= 8 && y >= 1 && y <= 6;
        }
    }
}
