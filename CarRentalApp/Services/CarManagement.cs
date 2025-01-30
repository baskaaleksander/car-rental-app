using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalApp.Interfaces;
using CarRentalApp.Models;
using CarRentalApp.Utilities;

namespace CarRentalApp.Services
{
    public class CarManagement : ICarManagement
    {
        private List<Car> cars;
        private readonly FileManager fileManager = new FileManager();
        private const string CarsFilePath = "cars.txt";

        private string GetFullPath(string fileName)
        {
            // Pobierz katalog projektu
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            // Połącz z folderem Data
            return Path.Combine(projectDirectory, "Data", fileName);
        }

        public CarManagement(List<Car> cars)
        {
            this.cars = cars;
            LoadCars(); // Automatyczne ładowanie przy inicjalizacji
        }

        private void LoadCars()
        {
            string fullPath = GetFullPath(CarsFilePath);
            string content = fileManager.ReadFile(fullPath);
            if (string.IsNullOrWhiteSpace(content)) return;

            foreach (string line in content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] carData = line.Split(',');
                if (carData.Length != 7) continue;

                if (int.TryParse(carData[0], out int id) &&
                    int.TryParse(carData[3], out int year) &&
                    double.TryParse(carData[5], out double price))
                {
                    cars.Add(new Car(
                        id,
                        carData[1].Trim(),
                        carData[2].Trim(),
                        year,
                        carData[4].Trim(),
                        price,
                        carData[6].Trim()
                    ));
                }
            }
        }

        public void AddCar(Car car)
        {
            car.Id = cars.Any() ? cars.Max(c => c.Id) + 1 : 1;
            cars.Add(car);
            Logger.Log($"Dodano nowy samochód: {car.Brand} {car.Model} (ID: {car.Id})");
            AppendCarToFile(car);
        }

        public void UpdateCar(int carId, Car updatedCar)
        {
            var car = GetCarById(carId);
            if (car == null) return;

            car.Brand = updatedCar.Brand;
            car.Model = updatedCar.Model;
            car.Year = updatedCar.Year;
            car.Color = updatedCar.Color;
            car.PricePerDay = updatedCar.PricePerDay;
            Logger.Log($"Zaktualizowano samochód: {car.Brand} {car.Model} (ID: {car.Id})");
            WriteAllCars();
        }

        public void UpdateCarStatus(int carId, string newStatus)
        {
            var car = GetCarById(carId);
            if (car == null || !(newStatus == "Dostępny" || newStatus == "Wynajęty")) return;

            car.Status = newStatus;
            Logger.Log($"Zaktualizowano status samochodu: {car.Brand} {car.Model} (ID: {car.Id})");
            WriteAllCars();
        }

        public void DeleteCar(int carId)
        {
            var car = GetCarById(carId);
            if (car == null) return;

            cars.Remove(car);
            Logger.Log($"Usunięto samochód: {car.Brand} {car.Model} (ID: {car.Id})");
            WriteAllCars();
        }

        public List<Car> GetAllCars() => cars;

        public List<Car> GetCarsByBrand(string brand)
            => cars.Where(c => c.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Car> GetCarsByModel(string model)
            => cars.Where(c => c.Model.Equals(model, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Car> GetCarsByYear(int year)
            => cars.Where(c => c.Year == year).ToList();

        public List<Car> GetCarsByPrice(double maxPrice)
            => cars.Where(c => c.PricePerDay <= maxPrice).ToList();

        public Car GetCarById(int carId)
            => cars.FirstOrDefault(c => c.Id == carId);

        private void WriteAllCars()
        {
            string fullPath = GetFullPath(CarsFilePath);
            fileManager.ClearFile(fullPath);
            foreach (var car in cars) AppendCarToFile(car);
        }

        private void AppendCarToFile(Car car)
        {
            string fullPath = GetFullPath(CarsFilePath);
            fileManager.WriteToFile(fullPath, $"{car.Id},{car.Brand},{car.Model}," +
                $"{car.Year},{car.Color},{car.PricePerDay},{car.Status}\n");
        }
    }
}