using Pharmacy_Management_AspNet_Webform.DAL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform.BLL
{
    public class UserBLL
    {
        private readonly UserDAL userDAL = new UserDAL();

        public User ValidateUser(string username, string password)
        {
            return userDAL.ValidateUser(username, password);
        }
    }
}
