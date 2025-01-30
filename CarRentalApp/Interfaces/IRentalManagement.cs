using System.Collections.Generic;
using CarRentalApp.Models;

namespace CarRentalApp.Interfaces
{
    public interface IRentalManagement
    {
        void AddRental(Rental rental);
        void CancelRental(int rentalId);
        List<Rental> GetAllRentals();
        Rental GetRentalById(int rentalId);
        List<Rental> GetUserRentals(int userId);
        List<Rental> GetActiveRentals();
        void CompleteRental(int rentalId);
    }
}