using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService
{
    using System;
    using System.IO;


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
        private string folderPath = @"c:\temp";
        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            FileStream fs = new FileStream(folderPath + "\\WindowsService.txt",
                                FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" WindowsService: Service Started at " +
               DateTime.Now.ToShortDateString() + " " +
               DateTime.Now.ToShortTimeString() + "\n");
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }
        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            FileStream fs = new FileStream(folderPath +
              "\\WindowsService.txt",
              FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" WindowsService: Service Stopped at " +
              DateTime.Now.ToShortDateString() + " " +
              DateTime.Now.ToShortTimeString() + "\n");
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }
    }
}
