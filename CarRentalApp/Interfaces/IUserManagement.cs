using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRentalApp.Models;

namespace CarRentalApp.Interfaces
{
    public interface IUserManagement
    {
        void AddUser(User user);
        void UpdateUser(int userId, User updatedUser);
        void DeleteUser(int userId);
        List<User> GetAllUsers();
        User GetUserById(int userId);
        User GetUserByUsername(string username);
    }
}
