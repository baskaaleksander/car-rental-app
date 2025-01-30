using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }

        public UserModel(int id, string role, string username)
        {
            Id = id;
            Role = role;
            Username = username;
        }
    }
}
