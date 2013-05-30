using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ProjectKService
{
    class SqlUtils
    {
        public static string DefaultConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
            }
        }

    }
}
