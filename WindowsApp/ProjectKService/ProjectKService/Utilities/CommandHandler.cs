using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectKService.Model;
using System.Diagnostics;
using WinFormCharpWebCam;
using System.IO;


namespace ProjectKService
{
    class CommandHandler
    {
        // Item Status
        public const string STATUS_PROCESSING = "processing";
        public const string STATUS_VENDING_ITEM = "vending";
        public const string STATUS_PLAYING_SOUND = "playing_sound";
        public const string STATUS_TAKING_PHOTO = "taking_photo";
        public const string STATUS_COMPLETE = "complete";

        public const string MSG_ERROR_INVALID_COMMAND = "INVALID COMMAND";
        public const string MSG_ERROR_INVALID_COORDS = "INVALID COORDS";
        public const string MSG_ERROR_INVALID_ITEM = "INVALID ITEM";
        public const string MSG_ERROR_NO_CARD = "SCAN CARD FIRST";
        public const string MSG_ERROR_INVALID_CARD = "BAD CARD";
        public const string MSG_ERROR_CARD_TIMEOUT = "TIMEOUT - SCAN AGAIN";
        public const string MSG_ERROR_ALREADY_VENDED = "ONE ITEM PER SCAN";
        public const string MSG_ERROR_VENDING_IN_PROGRESS = "PROCESSING";
        public const string MSG_ERROR_SYSTEM_FAIL = "SORRY I FAILED";
        public const string MSG_SYSTEM_READY_RESPONSE = "STATUS";
        public const string MSG_VEND_INPUT_READY = "9999";
        public const string MSG_VEND_ITEM = "1234";
        public const string MSG_TAKING_PHOTO = "TAKING PHOTO!";
        public const string MSG_PLAYING_SOUND = "LISTEN!";

        private static System.Windows.Forms.PictureBox imgVideo;
        private static WebCam webcam;

        // Processes the command sent from the Arduino
        // Returns the message to send back to the Arduino (or null if there is nothing to send back)
        public static string ProcessCommand(string command)
        {
            string[] data = command.Split(':');

            if (data.Length == 0)
            {
                return null;
            }

            try
            {
                if (data[0] == "System Ready")
                {
                    return SystemReady();
                }
                else if (data[0] == "KEYPAD")
                {
                    // format: KEYPAD:Y,X
                    return RequestItem(data.Length > 1 ? data[1] : "");
                }
                else if (data[0] == "VEND" || data[0] == MSG_TAKING_PHOTO || data[0] == MSG_PLAYING_SOUND)
                {
                    // format: Vend:Y,X
                    return VendComplete(data.Length > 1 ? data[1] : "");
                }
            }
            catch (Exception e)
            {
                Error.CreateError(e);
                return MSG_ERROR_SYSTEM_FAIL;
            }
            
            // unknown command
            // return MSG_ERROR_INVALID_COMMAND;
            // TODO: make this do something, but not an infinite loop please
            return null;
        }

        private static string SystemReady() 
        {
            return MSG_SYSTEM_READY_RESPONSE;
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
            string successMessage = MSG_VEND_ITEM;
            int x = -1;
            int y = -1;
            string requestStatus = null;

            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                CardScanResult scanResult = scan.CardScanResult;
                
                // create a new vending request
                VendingRequest request = new VendingRequest()
                {
                    CardScanResultID = scanResult.CardScanResultID,
                    Status = STATUS_PROCESSING,
                    RequestDate = DateTime.Now,
                    Coordinates = coordinateString
                };

                context.VendingRequests.Add(request);
                context.SaveChanges();

                try
                {
                    // Error checking block
                    if (lastRequest != null)
                    {
                        // We found a vending request with that is either processing or vending within a timeout interval
                        // Cannot vend again at this point
                        error = MSG_ERROR_VENDING_IN_PROGRESS;
                    }
                    else if (scan.HasTimedOut)
                    {
                        // We've timed out
                        if (DateTime.Now.Subtract(scan.ScanDate).TotalMinutes > 5)
                        {
                            // Last scan was a while ago - just tell the user to scan first, so they don't get confused
                            error = MSG_ERROR_NO_CARD;
                        }
                        else
                        {
                            // Display timeout error message
                            error = MSG_ERROR_CARD_TIMEOUT;
                        }
                    }
                    else if (scanResult.Status != "valid")
                    {
                        // Invalid card!
                        error = MSG_ERROR_INVALID_CARD;
                    }
                    else if (scan.HasVended(request.VendingRequestID))
                    {
                        // We've already vended an item for this card scan. Sneaky!
                        error = MSG_ERROR_ALREADY_VENDED;
                    }
                    else if (!GetCoordinates(coordinateString, out x, out y))
                    {
                        // Unable to parse coordinates
                        error = MSG_ERROR_INVALID_COORDS;
                    }
                    else
                    {
                        requestStatus = GetItemStatusType(x, y);
                        if (requestStatus == null)
                        {
                            error = MSG_ERROR_INVALID_ITEM;
                        }
                    }

                    request.X = x;
                    request.Y = y;
                }
                catch (Exception e)
                {
                    error = MSG_ERROR_SYSTEM_FAIL;
                    Error.CreateError(e);
                }

                if (error == null)
                {
                    // TODO: consider what happens if the message to the Arduino fails?
                    request.Status = requestStatus;
                    request.VendStartDate = DateTime.Now;

                    // Adjust the success message if necessary
                    if (requestStatus == STATUS_TAKING_PHOTO)
                    {
                        successMessage = MSG_TAKING_PHOTO;
                    }
                    else if (requestStatus == STATUS_PLAYING_SOUND)
                    {
                        successMessage = MSG_PLAYING_SOUND;
                    }
                }
                else
                {
                    request.Status = "failed";
                    request.ErrorMessage = error;
                }

                context.SaveChanges();
            }

            Logger.WriteLine("Returning! " + (error == null ? MSG_VEND_ITEM : error));

            // Finally, success! Vend it. Or display the error instead if there is one.
            return (error == null ? successMessage : error);
        }

        // Called after the machine has successfully vended the item
        private static string VendComplete(string coordinateString)
        {
            // look up previous request with status='vending' in database
            using (klick_vending_machineEntities context = new klick_vending_machineEntities()) {
                VendingRequest request = (
                     from r in context.VendingRequests
                     where (
                        r.Status == STATUS_VENDING_ITEM || 
                        r.Status == STATUS_TAKING_PHOTO || 
                        r.Status == STATUS_PLAYING_SOUND
                     )
                     orderby r.RequestDate descending
                     select r).FirstOrDefault();

                // TODO: consider timeout, confirm coordinates, etc
                if (request != null)
                {
                    if (request.Status == STATUS_TAKING_PHOTO)
                    {
                        TakePhoto();
                    }
                    else if (request.Status == STATUS_PLAYING_SOUND)
                    {
                        PlaySound();
                    }
                    request.VendEndDate = DateTime.Now;
                    request.Status = STATUS_COMPLETE;
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

        // Stolen from http://stackoverflow.com/questions/5519328/executing-batch-file-in-c-sharp
        private static void ExecuteCommand(string command)
        {
            Logger.WriteLine("ExecuteCommand: " + command);

            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Logger.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Logger.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Logger.WriteLine("ExitCode: " + exitCode.ToString());
            
            process.Close();
        }

        // Changed this to instead return the config value in the db
        private static string GetConfigPath(string configName, string defaultValue) 
        {
            string value = null;

            using (klick_vending_machineEntities context = new klick_vending_machineEntities())
            {
                Config config = (from c in context.Configs where c.Name == configName select c).FirstOrDefault();

                if (config != null) 
                {
                    value = config.Value;
                }
            }

            if (value == null || value == "") 
            {
                value = defaultValue;
            }

            return (value);
        }

        private static string GetItemStatusType(int x, int y)
        {
            if (x >= 1 && x <= 8 && y >= 1 && y <= 6)
            {
                return STATUS_VENDING_ITEM;
            }
            if (x == 0 && y == 0)
            {
                return STATUS_TAKING_PHOTO;
            }
            if (x == 9 && y == 6)
            {
                return STATUS_PLAYING_SOUND;
            }
            return null;
        }

        public static void TakePhoto()
        {
            //Processing sketch is having issues interacting with the desktop.  Might be easier to just code the logic right into the service
            //string defaultPath = "Photobooth\vending_machine_photobooth\application.windows64\vending_machine_photobooth.bat";
            //ExecuteCommandAtPath("photo", defaultPath);
            // Save Image
            string uniqueFilename = String.Format("photo_{0}.jpg", Guid.NewGuid().ToString());
            string defaultPath = "c:/temp/";
            string filename = GetConfigPath("photo", defaultPath) + uniqueFilename;
            FileStream fstream = new FileStream(filename, FileMode.Create);
            imgVideo.Image.Save(fstream, System.Drawing.Imaging.ImageFormat.Jpeg);
            fstream.Close();
        }

        public static void initWebCam()
        {
            imgVideo = new System.Windows.Forms.PictureBox();
            imgVideo.Size = new System.Drawing.Size(163, 160);

            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            webcam.Start();
        }

        public static void PlaySound()
        {
            //Looks like a permission issue playing the file.  Again, might be easier to code a sound player instead.
            // From http://www.crowsprogramming.com/archives/58

            WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
            string defaultPath = "song.mp3";
            wplayer.URL = GetConfigPath("sound", defaultPath);
            wplayer.controls.play(); 
                     
        }
    }
}
