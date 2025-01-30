using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public double PricePerDay { get; set; }
        public string Status { get; set; }

        public Car(int id, string brand, string model, int year, string color, double pricePerDay, string status)
        {
            Id = id;
            Brand = brand;
            Model = model;
            Year = year;
            Color = color;
            PricePerDay = pricePerDay;
            Status = status;
        }
    }
}
