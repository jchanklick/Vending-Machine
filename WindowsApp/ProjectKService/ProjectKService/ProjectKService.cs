﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService
{
    using System;


    // from: http://www.codeproject.com/Articles/106742/Creating-a-simple-Windows-Service
    class ProjectKService : System.ServiceProcess.ServiceBase
    {
        // The main entry point for the process
        static void Main()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun =
              new System.ServiceProcess.ServiceBase[] { new ProjectKService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceName = "ProjectKService";
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            Logger.WriteLine("ProjectKService Started");
            SerialPortHandler.Current.Start();
        }
        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            Logger.WriteLine("ProjectKService Stopped");
            SerialPortHandler.Current.Stop();
        }
    }
}