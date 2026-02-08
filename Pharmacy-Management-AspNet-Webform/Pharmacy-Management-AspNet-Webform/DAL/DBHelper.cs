using System.Configuration;
using System.Data.SqlClient;

namespace Pharmacy_Management_AspNet_Webform.DAL
{
   internal sealed class DBHelper
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}
