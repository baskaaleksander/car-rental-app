using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRentalApp.Models;

namespace CarRentalApp.Interfaces
{
    public interface ICarManagement
    {
        void AddCar(Car car); 
        void UpdateCar(int carId, Car updatedCar); 
        void UpdateCarStatus(int carId, string newStatus); 
        void DeleteCar(int carId); 

        List<Car> GetAllCars(); 
        List<Car> GetCarsByBrand(string brand); 
        List<Car> GetCarsByModel(string model);
        List<Car> GetCarsByYear(int year);
        List<Car> GetCarsByPrice(double maxPrice); 

        Car GetCarById(int carId); 
    }
}