using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalApp.Models;
using CarRentalApp.Services;
using CarRentalApp.Interfaces;
using Moq;
using Xunit;
using CarRentalApp.Utilities;

public class Tests
{
    private readonly List<Car> _cars;
    private readonly Mock<IFileManager> _fileManagerMock;
    private readonly CarManagement _carManagement;
    private readonly UserManagement _userManagement;
    private readonly RentalManagement _rentalManagement;

    public Tests()
    {
        _cars = new List<Car>
        {
            new Car(1, "Toyota", "Corolla", 2020, "Biały", 150, "Dostępny"),
            new Car(2, "Ford", "Focus", 2018, "Czarny", 120, "Wynajęty")
        };
        _fileManagerMock = new Mock<IFileManager>();
        _carManagement = new CarManagement(_cars);
        _userManagement = new UserManagement();
        _rentalManagement = new RentalManagement(_carManagement, _userManagement);
    }


    [Fact]
    public void GetCarById_ShouldReturnCorrectCar()
    {
        var car = _carManagement.GetCarById(1);

        Assert.NotNull(car);
        Assert.Equal("Toyota", car.Brand);
    }


    [Fact]
    public void GetCarsByBrand_ShouldReturnCorrectCars()
    {
        var result = _carManagement.GetCarsByBrand("Toyota");

        Assert.Single(result);
        Assert.Equal("Corolla", result[0].Model);
    }
    [Fact]
    public void GetUserById_ShouldReturnCorrectUser()
    {
   
        var user = _userManagement.GetUserById(1);


        Assert.Null(user); 
    }
    [Fact]
    public void GetRentalsByUserId_ShouldReturnCorrectRentals()
    { 
        var result = _rentalManagement.GetUserRentals(1);

        Assert.Empty(result);
    }
}
