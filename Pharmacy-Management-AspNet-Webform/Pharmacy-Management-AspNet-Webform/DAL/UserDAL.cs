using System.Data;
using System.Data.SqlClient;
using Pharmacy_Management_AspNet_Webform.Common;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform.DAL
{
    internal sealed class UserDAL
    {
        public User ValidateUser(string username, string password)
        {
            User user = null;
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.ValidateUser, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = (int)reader["Id"],
                        Username = reader["Username"].ToString()
                    };
                }
            }
            return user;
        }
    }
}
