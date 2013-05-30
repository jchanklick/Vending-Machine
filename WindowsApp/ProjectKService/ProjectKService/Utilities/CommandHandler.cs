using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectKService.Model;

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
        public const string MSG_ERROR_ALREADY_VENDED = "ONE ITEM PER SCAN";
        public const string MSG_ERROR_VENDING_IN_PROGRESS = "PROCESSING";
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
            // Get the last card scan
            CardScan scan = CardScan.LastCardScan();
            if (scan == null)
            {
                return MSG_ERROR_NO_CARD;
            }

            VendingRequest lastRequest = VendingRequest.LastOutstandingRequest();

            string error = null;
            int x = -1;
            int y = -1;

            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                CardScanResult scanResult = CardScanHandler.GetCardScanResult(scan);

                // create a new vending request
                VendingRequest request = new VendingRequest()
                {
                    CardScanResultID = scanResult.CardScanResultID,
                    Status = "processing",
                    RequestDate = DateTime.Now,
                    Coordinates = coordinateString
                };

                context.SaveChanges();

                // Error checking block
                if (scanResult.Status != "valid")
                {
                    // Invalid card!
                    error = MSG_ERROR_INVALID_CARD;
                }
                else if (lastRequest != null)
                {
                    // We found a vending request with that is either processing or vending within a timeout interval
                    // Cannot vend again at this point
                    error = MSG_ERROR_VENDING_IN_PROGRESS;
                }
                else if (scan.HasTimedOut)
                {
                    // We've timed out
                    error = MSG_ERROR_CARD_TIMEOUT;
                }
                else if (scan.HasVended)
                {
                    // We've already vended an item for this card scan. Sneaky!
                    error = MSG_ERROR_ALREADY_VENDED;
                }
                else if (!GetCoordinates(coordinateString, out x, out y))
                {
                    // Unable to parse coordinates
                    error = MSG_ERROR_INVALID_COORDS;
                }
                else if (!IsValidItem(x, y))
                {
                    // Bad X, Y values
                    error = MSG_ERROR_INVALID_ITEM;
                }

                request.X = x;
                request.Y = y;

                if (error != null)
                {
                    // TODO: consider what happens if the message to the Arduino fails?
                    request.Status = "vending";
                    request.VendStartDate = DateTime.Now;
                }
                else
                {
                    request.Status = "failed";
                    request.ErrorMessage = error;
                }

                context.SaveChanges();
            }

            // Finally, success! Vend it.
            return (error == null ? MSG_VEND_ITEM : error);
        }

        // Called after the machine has successfully vended the item
        private static string VendComplete(string coordinateString)
        {
            // look up previous request with status='vending' in database
            using (klick_vending_machineEntities context = new klick_vending_machineEntities()) {
                VendingRequest request = VendingRequest.LastRequestWithStatus("vending");
                // TODO: consider timeout, confirm coordinates, etc
                if (request != null)
                {
                    request.VendEndDate = DateTime.Now;
                    request.Status = "complete";
                }
                context.SaveChanges();
            }
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
