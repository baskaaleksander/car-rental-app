using System;

namespace CarRentalApp.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // "Aktywne", "Zakończone", "Anulowane"

        public Rental(int id, int userId, int carId, DateTime startDate, DateTime endDate, decimal totalPrice, string status)
        {
            Id = id;
            UserId = userId;
            CarId = carId;
            StartDate = startDate;
            EndDate = endDate;
            TotalPrice = totalPrice;
            Status = status;
        }
    }
}