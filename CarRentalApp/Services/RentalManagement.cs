using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalApp.Interfaces;
using CarRentalApp.Models;
using CarRentalApp.Utilities;

namespace CarRentalApp.Services
{
    public class RentalManagement : IRentalManagement
    {
        private readonly List<Rental> Rentals;
        private readonly FileManager FileManager = new FileManager();
        private const string RentalsFilePath = "rentals.txt"; 
        private readonly CarManagement CarService;
        private readonly UserManagement UserService;

        public RentalManagement(CarManagement carService, UserManagement userService)
        {
            CarService = carService;
            UserService = userService;
            Rentals = new List<Rental>();
            LoadRentals();
        }
        private string GetFullPath(string fileName)
        {
            // Pobierz katalog projektu
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            // Połącz z folderem Data
            return Path.Combine(projectDirectory, "Data", fileName);
        }
        private void LoadRentals()
        {
            var fullPath = GetFullPath(RentalsFilePath);
            var content = FileManager.ReadFile(fullPath);
            if (string.IsNullOrWhiteSpace(content)) return;

            foreach (var line in content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var data = line.Split(',');
                if (data.Length != 7) continue;

                Rentals.Add(new Rental(
                    id: int.Parse(data[0]),
                    userId: int.Parse(data[1]),
                    carId: int.Parse(data[2]),
                    startDate: DateTime.Parse(data[3]),
                    endDate: DateTime.Parse(data[4]),
                    totalPrice: decimal.Parse(data[5]),
                    status: data[6]
                ));
            }
        }

        public void AddRental(Rental rental)
        {
            if (UserService.GetUserById(rental.UserId) == null)
                throw new ArgumentException("Użytkownik nie istnieje");
            
            var car = CarService.GetCarById(rental.CarId);
            if (car == null)
                throw new ArgumentException("Samochód nie istnieje");
            
            if (car.Status != "Dostępny")
                throw new InvalidOperationException("Samochód jest już wynajęty");

            rental.Id = Rentals.Any() ? Rentals.Max(r => r.Id) + 1 : 1;
            
            rental.TotalPrice = CalculatePrice(rental.StartDate, rental.EndDate, car.PricePerDay);
            
            CarService.UpdateCarStatus(rental.CarId, "Wynajęty");
            
            Rentals.Add(rental);
            Logger.Log($"Dodano nowe wypożyczenie (ID: {rental.Id})");
            SaveRentals();
        }

        private decimal CalculatePrice(DateTime start, DateTime end, double pricePerDay)
        {
            var days = (end - start).Days;
            return (decimal)pricePerDay * (days > 0 ? days : 1);
        }

        public void CompleteRental(int rentalId)
        {
            var rental = GetRentalById(rentalId);
            if (rental == null || rental.Status != "Aktywne") return;
            
            rental.Status = "Zakończone";
            CarService.UpdateCarStatus(rental.CarId, "Dostępny");
            Logger.Log($"Zakończono wypożyczenie (ID: {rental.Id})");
            SaveRentals();
        }

        public void CancelRental(int rentalId)
        {
            var rental = GetRentalById(rentalId);
            if (rental == null || rental.Status != "Aktywne") return;
            
            rental.Status = "Anulowane";
            CarService.UpdateCarStatus(rental.CarId, "Dostępny");
            Logger.Log($"Anulowano wypożyczenie (ID: {rental.Id})");
            SaveRentals();
        }

        public List<Rental> GetAllRentals() => Rentals;
        public Rental GetRentalById(int rentalId) => Rentals.FirstOrDefault(r => r.Id == rentalId);
        public List<Rental> GetUserRentals(int userId)
        {
            return Rentals.Where(r => r.UserId == userId).ToList();
        }
        public List<Rental> GetActiveRentals() => Rentals.Where(r => r.Status == "Aktywne").ToList();

        private void SaveRentals()
        {
            var fullPath = GetFullPath(RentalsFilePath);

            var lines = Rentals.Select(r =>
                $"{r.Id},{r.UserId},{r.CarId},{r.StartDate:yyyy-MM-dd},{r.EndDate:yyyy-MM-dd},{r.TotalPrice},{r.Status}"
            );
            FileManager.WriteAllLines(fullPath, lines);
        }
    }
}