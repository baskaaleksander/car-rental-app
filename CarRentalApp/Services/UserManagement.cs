using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalApp.Interfaces;
using CarRentalApp.Models;
using CarRentalApp.Utilities;

namespace CarRentalApp.Services
{
    public class UserManagement : IUserManagement
    {
        private List<User> users;
        private readonly FileManager fileManager = new FileManager();
        private const string UsersFilePath = "users.txt";


        public UserManagement()
        {
            users = new List<User>();
            LoadUsers(); 
        }

        private string GetFullPath(string fileName)
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            return Path.Combine(projectDirectory, "Data", fileName);
        }
        private void LoadUsers()
        {
            string fullPath = GetFullPath(UsersFilePath);
            Console.WriteLine($"Ładowanie użytkowników z: {Path.GetFullPath(fullPath)}"); 

            string content = fileManager.ReadFile(fullPath);
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Plik jest pusty lub nie istnieje!");
                return;
            }



            foreach (string line in content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] userData = line.Split(',');
                if (userData.Length != 7) continue;

                users.Add(new User(
                    id: int.Parse(userData[0]),
                    firstName: userData[1].Trim(),
                    lastName: userData[2].Trim(),
                    username: userData[3].Trim(),
                    password: userData[4].Trim(),
                    phoneNumber: userData[5].Trim(),
                    role: userData[6].Trim()
                ));
            }
        }


        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            user.Password = PasswordHasher.HashPassword(user.Password); 
            users.Add(user);
            Logger.Log($"Dodano nowego użytkownika: {user.Username} (ID: {user.Id})");
            AppendUserToFile(user);
        }

        public void UpdateUser(int userId, User updatedUser)
        {
            var user = GetUserById(userId);
            if (user == null) return;

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Username = updatedUser.Username;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Role = updatedUser.Role;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = PasswordHasher.HashPassword(updatedUser.Password);
            }
            Logger.Log($"Zaktualizowano użytkownika: {user.Username} (ID: {user.Id})");
            WriteAllUsers();
        }

        public void DeleteUser(int userId)
        {
            var user = GetUserById(userId);
            if (user == null) return;

            users.Remove(user);
            Logger.Log($"Usunięto użytkownika: {user.Username} (ID: {user.Id})");
            WriteAllUsers();
        }

        public List<User> GetAllUsers()
        {
            return users;
        }

        public User GetUserById(int userId)
        {
            return users.FirstOrDefault(u => u.Id == userId);
        }


        public User GetUserByUsername(string username)
        {
            return users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
        private void WriteAllUsers()
        {
            string fullPath = GetFullPath(UsersFilePath);
            fileManager.ClearFile(fullPath);
            foreach (var user in users) AppendUserToFile(user);
        }

        private void AppendUserToFile(User user)
        {
            string fullPath = GetFullPath(UsersFilePath);
            string line = $"{user.Id},{user.FirstName},{user.LastName}," +
                          $"{user.Username},{user.Password},{user.PhoneNumber},{user.Role}";
            fileManager.WriteToFile(fullPath, line + Environment.NewLine);
        }
    }
}