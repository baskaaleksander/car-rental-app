using CarRentalApp.Interfaces;
using CarRentalApp.Models;
using CarRentalApp.Utilities;

namespace CarRentalApp.Services
{
    public class AuthService
    {
        private readonly IUserManagement _userService;
        public CurrentUser CurrentUser { get; private set; }


        public AuthService(IUserManagement userService)
        {
            _userService = userService;
        }

        public bool Login(string username, string password)
        {
            var user = _userService.GetUserByUsername(username);
            if (user != null && user.Password == PasswordHasher.HashPassword(password))
            {
                CurrentUser = new CurrentUser
                (
                    user.Id,
                    user.Role,
                    user.Username
                );
                Logger.Log($"Użytkownik {username} zalogował się pomyślnie.");
                return true;
            }
            Logger.Log($"Nieudana próba logowania dla użytkownika {username}.");
            return false;
        }

        public void Logout()
        {
            Logger.Log($"Użytkownik {CurrentUser.Username} wylogował się.");
            CurrentUser = null;
        }

        public bool IsAdmin() => CurrentUser?.Role == "admin";
    }
}