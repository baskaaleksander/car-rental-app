using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Models
{
    public class CurrentUser : UserModel
    {
        public CurrentUser(int id, string role, string username) : base(id, role, username)
        {
        }
    }
}
