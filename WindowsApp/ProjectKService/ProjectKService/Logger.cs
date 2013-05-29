using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProjectKService
{
    class Logger
    {
        private static string FolderPath = @"c:\temp";

        public static void WriteLine(string message)
        {
            if (!System.IO.Directory.Exists(FolderPath))
                System.IO.Directory.CreateDirectory(FolderPath);

            FileStream fs = new FileStream(FolderPath + "\\ProjectKService_Log.txt",
                                FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(DateTime.Now.ToShortDateString() + " " +
                DateTime.Now.ToShortTimeString() + ": " + message);
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }
    }
}
