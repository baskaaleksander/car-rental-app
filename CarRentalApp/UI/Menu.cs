using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalApp.Models;
using CarRentalApp.Services;
using CarRentalApp.Utilities;

namespace CarRentalApp.UI
{
    public class Menu
    {
        private readonly AuthService _authService;
        private readonly CarManagement _carService;
        private readonly UserManagement _userService;
        private readonly RentalManagement _rentalService;
        private readonly FileManager _fileManager;
        public Menu()
        {
            _userService = new UserManagement();
            _authService = new AuthService(_userService);
            _carService = new CarManagement(new List<Car>());
            _rentalService = new RentalManagement(_carService, _userService);
            _fileManager = new FileManager();
            Logger.LogEvent += OnLogEvent;
        }
        private void OnLogEvent(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[LOG] {message}");
            Console.ResetColor();
        }
        public void Run()
        {
            while (true)
            {
                if (!ShowLoginUI()) return;
                ShowMainMenu();
            }
        }
        private bool ShowLoginUI()
        {
            Console.Clear();
            DisplayHeader("Logowanie");

            var username = ReadString("Nazwa użytkownika: ");
            var password = ReadString("Hasło: ");

            if (_authService.Login(username, password))
            {
                ShowSuccess("Zalogowano pomyślnie!");
                return true;
            }

            ShowError("Błędne dane logowania!");
            return false;
        }

        private void ShowMainMenu()
        {
            while (_authService.CurrentUser != null)
            {
                Console.Clear();
                DisplayHeader($"Witaj, {_authService.CurrentUser.Username}!");

                if (_authService.IsAdmin())
                {
                    ShowAdminMenu();
                }
                else
                {
                    ShowUserMenu();
                }
            }
        }

        #region Admin Menu
        private void ShowAdminMenu()
        {
            Console.WriteLine("1. Zarządzaj samochodami");
            Console.WriteLine("2. Zarządzaj użytkownikami");
            Console.WriteLine("3. Zarządzaj wypożyczeniami");
            Console.WriteLine("4. Zarządzaj logami");
            Console.WriteLine("5. Wyloguj");
            Console.WriteLine("0. Wyjście");

            switch (ReadInt("Wybierz opcję: "))
            {
                case 1: ManageCarsUI(); break;
                case 2: ManageUsersUI(); break;
                case 3: ManageRentalsUI(); break;
                case 4: ShowLogUI(); break;
                case 5: Logout(); break;
                case 0: Environment.Exit(0); break;
                default: ShowError("Nieprawidłowy wybór!"); break;
            }
        }
        #endregion

        #region User Menu
        private void ShowUserMenu()
        {
            Console.WriteLine("1. Moje wypożyczenia");
            Console.WriteLine("2. Nowe wypożyczenie");
            Console.WriteLine("3. Wyloguj");
            Console.WriteLine("0. Wyjście");

            switch (ReadInt("Wybierz opcję: "))
            {
                case 1: ShowUserRentalsUI(); break;
                case 2: CreateRentalUI(); break;
                case 3: Logout(); break;
                case 0: Environment.Exit(0); break;
                default: ShowError("Nieprawidłowy wybór!"); break;
            }
            WaitForContinue();


        }

        private void ShowUserRentalsUI()
        {
            DisplayHeader("Moje wypożyczenia");
            var rentals = _rentalService.GetUserRentals(_authService.CurrentUser.Id);

            if (rentals.Count == 0)
            {
                ShowInfo("Brak aktywnych wypożyczeń");
                return;
            }

            foreach (var rental in rentals)
            {
                DisplayRental(rental);
            }
        }
        private void Logout()
        {
            _authService.Logout();
            ShowSuccess("Wylogowano pomyślnie!");
            WaitForContinue();
        }
        #endregion

        #region Car Management
        private void UpdateCarStatusUI()
        {
            DisplayHeader("Zmiana statusu samochodu");
            ShowAllCarsUI();

            var carId = ReadInt("Podaj ID samochodu: ");
            var newStatus = ReadString("Nowy status (Dostępny/Wynajęty): ");

            try
            {
                _carService.UpdateCarStatus(carId, newStatus);
                ShowSuccess("Status zaktualizowany!");
            }
            catch (Exception ex)
            {
                ShowError($"Błąd: {ex.Message}");
            }
        }
        private void ManageCarsUI()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader("Zarządzanie samochodami");

                Console.WriteLine("1. Dodaj samochód");
                Console.WriteLine("2. Wyświetl wszystkie");
                Console.WriteLine("3. Edytuj samochód");
                Console.WriteLine("4. Zmień status");
                Console.WriteLine("5. Usuń samochód");
                Console.WriteLine("6. Wyszukaj samochody");
                Console.WriteLine("0. Powrót");

                switch (ReadInt("Wybierz opcję: "))
                {
                    case 1: AddCarUI(); break;
                    case 2: ShowAllCarsUI(); break;
                    case 3: UpdateCarUI(); break;
                    case 4: UpdateCarStatusUI(); break;
                    case 5: DeleteCarUI(); break;
                    case 6: SearchCarsUI(); break;
                    case 0: return;
                    default: ShowError("Nieprawidłowy wybór!"); break;
                }
                WaitForContinue();
            }
        }

        private void AddCarUI()
        {
            DisplayHeader("Dodaj nowy samochód");
            try
            {
                var newCar = new Car(
                    id: 0,
                    brand: ReadString("Marka: "),
                    model: ReadString("Model: "),
                    year: ReadInt("Rok produkcji: "),
                    color: ReadString("Kolor: "),
                    pricePerDay: ReadDouble("Cena/dzień: "),
                    status: "Dostępny"
                );
                _carService.AddCar(newCar);
                ShowSuccess("Samochód dodany pomyślnie!");
            }
            catch (Exception ex)
            {
                ShowError($"Błąd: {ex.Message}");
            }
        }

        private void ShowAllCarsUI()
        {
            DisplayHeader("Lista samochodów");
            var cars = _carService.GetAllCars();

            if (cars.Count == 0)
            {
                ShowInfo("Brak samochodów w systemie");
                return;
            }

            foreach (var car in cars)
            {
                DisplayCar(car);
            }
        }

        private void UpdateCarUI()
        {
            DisplayHeader("Edycja samochodu");
            ShowAllCarsUI();

            var carId = ReadInt("Podaj ID samochodu do edycji: ");
            var existingCar = _carService.GetCarById(carId);

            if (existingCar == null)
            {
                ShowError("Nie znaleziono samochodu!");
                return;
            }

            var updatedCar = new Car(
                id: carId,
                brand: ReadString($"Marka ({existingCar.Brand}): ", existingCar.Brand),
                model: ReadString($"Model ({existingCar.Model}): ", existingCar.Model),
                year: ReadInt($"Rok ({existingCar.Year}): ", existingCar.Year),
                color: ReadString($"Kolor ({existingCar.Color}): ", existingCar.Color),
                pricePerDay: ReadDouble($"Cena ({existingCar.PricePerDay}): ", existingCar.PricePerDay),
                status: existingCar.Status
            );

            _carService.UpdateCar(carId, updatedCar);
            ShowSuccess("Samochód zaktualizowany!");
        }

        private void DeleteCarUI()
        {
            DisplayHeader("Usuwanie samochodu");
            ShowAllCarsUI();
            var carId = ReadInt("Podaj ID samochodu do usunięcia: ");
            _carService.DeleteCar(carId);
            ShowSuccess("Samochód usunięty!");
        }

        private void SearchCarsUI()
        {
            DisplayHeader("Wyszukiwanie samochodów");

            Console.WriteLine("1. Po marce");
            Console.WriteLine("2. Po modelu");
            Console.WriteLine("3. Po roku");
            Console.WriteLine("4. Po cenie");
            var choice = ReadInt("Wybierz kryterium: ");

            List<Car> results = new List<Car>();

            switch (choice)
            {
                case 1:
                    results = _carService.GetCarsByBrand(ReadString("Podaj markę: "));
                    break;
                case 2:
                    results = _carService.GetCarsByModel(ReadString("Podaj model: "));
                    break;
                case 3:
                    results = _carService.GetCarsByYear(ReadInt("Podaj rok: "));
                    break;
                case 4:
                    results = _carService.GetCarsByPrice(ReadDouble("Podaj maksymalną cenę: "));
                    break;
                default:
                    ShowError("Nieprawidłowy wybór!");
                    return;
            }

            DisplayHeader("Wyniki wyszukiwania");
            foreach (var car in results)
            {
                DisplayCar(car);
            }
        }
        #endregion

        #region User Management
        private void SearchUserUI()
        {
            DisplayHeader("Wyszukiwanie użytkownika");

            Console.WriteLine("1. Po ID");
            Console.WriteLine("2. Po nazwie użytkownika");
            var choice = ReadInt("Wybierz opcję: ");

            User user = null;

            switch (choice)
            {
                case 1:
                    var userId = ReadInt("Podaj ID użytkownika: ");
                    user = _userService.GetUserById(userId);
                    break;
                case 2:
                    var username = ReadString("Podaj nazwę użytkownika: ");
                    user = _userService.GetUserByUsername(username);
                    break;
                default:
                    ShowError("Nieprawidłowy wybór!");
                    return;
            }

            if (user != null)
            {
                DisplayHeader("Znaleziony użytkownik");
                DisplayUser(user);
            }
            else
            {
                ShowInfo("Nie znaleziono użytkownika");
            }
        }
        private void ManageUsersUI()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader("Zarządzanie użytkownikami");

                Console.WriteLine("1. Dodaj użytkownika");
                Console.WriteLine("2. Wyświetl wszystkich");
                Console.WriteLine("3. Edytuj użytkownika");
                Console.WriteLine("4. Usuń użytkownika");
                Console.WriteLine("5. Wyszukaj użytkownika");
                Console.WriteLine("0. Powrót");

                switch (ReadInt("Wybierz opcję: "))
                {
                    case 1: AddUserUI(); break;
                    case 2: ShowAllUsersUI(); break;
                    case 3: UpdateUserUI(); break;
                    case 4: DeleteUserUI(); break;
                    case 5: SearchUserUI(); break;
                    case 0: return;
                    default: ShowError("Nieprawidłowy wybór!"); break;
                }
                WaitForContinue();
            }
        }

        private void AddUserUI()
        {
            DisplayHeader("Dodaj nowego użytkownika");
            try
            {
                var newUser = new User(
                    id: 0,
                    firstName: ReadString("Imię: "),
                    lastName: ReadString("Nazwisko: "),
                    username: ReadString("Nazwa użytkownika: "),
                    password: ReadString("Hasło: "),
                    phoneNumber: ReadString("Numer telefonu: "),
                    role: "user"
                );
                _userService.AddUser(newUser);
                ShowSuccess("Użytkownik dodany pomyślnie!");
            }
            catch (Exception ex)
            {
                ShowError($"Błąd: {ex.Message}");
            }
        }

        private void ShowAllUsersUI()
        {
            DisplayHeader("Lista użytkowników");
            var users = _userService.GetAllUsers();

            if (users.Count == 0)
            {
                ShowInfo("Brak użytkowników w systemie");
                return;
            }

            foreach (var user in users)
            {
                DisplayUser(user);
            }
        }

        private void UpdateUserUI()
        {
            DisplayHeader("Edycja użytkownika");
            ShowAllUsersUI();

            var userId = ReadInt("Podaj ID użytkownika do edycji: ");
            var existingUser = _userService.GetUserById(userId);

            if (existingUser == null)
            {
                ShowError("Nie znaleziono użytkownika!");
                return;
            }

            var updatedUser = new User(
                id: userId,
                firstName: ReadString($"Imię ({existingUser.FirstName}): ", existingUser.FirstName),
                lastName: ReadString($"Nazwisko ({existingUser.LastName}): ", existingUser.LastName),
                username: ReadString($"Nazwa użytkownika ({existingUser.Username}): ", existingUser.Username),
                password: ReadString("Nowe hasło (pozostaw puste aby nie zmieniać): ", ""),
                phoneNumber: ReadString($"Numer telefonu ({existingUser.PhoneNumber}): ", existingUser.PhoneNumber),
                role: existingUser.Role
            );

            _userService.UpdateUser(userId, updatedUser);
            ShowSuccess("Użytkownik zaktualizowany!");
        }

        private void DeleteUserUI()
        {
            DisplayHeader("Usuwanie użytkownika");
            ShowAllUsersUI();
            var userId = ReadInt("Podaj ID użytkownika do usunięcia: ");
            _userService.DeleteUser(userId);
            ShowSuccess("Użytkownik usunięty!");
        }
        #endregion

        #region Rental Management
        private void ManageRentalsUI()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader("Zarządzanie wypożyczeniami");

                Console.WriteLine("1. Nowe wypożyczenie");
                Console.WriteLine("2. Aktywne wypożyczenia");
                Console.WriteLine("3. Zakończ wypożyczenie");
                Console.WriteLine("4. Historia wypożyczeń");
                Console.WriteLine("0. Powrót");

                switch (ReadInt("Wybierz opcję: "))
                {
                    case 1: CreateRentalUI(); break;
                    case 2: ShowActiveRentalsUI(); break;
                    case 3: CompleteRentalUI(); break;
                    case 4: ShowAllRentalsUI(); break;
                    case 0: return;
                    default: ShowError("Nieprawidłowy wybór!"); break;
                }
                WaitForContinue();
            }
        }
        private void ShowAllRentalsUI()
        {
            DisplayHeader("Historia wszystkich wypożyczeń");
            var rentals = _rentalService.GetAllRentals();

            if (rentals.Count == 0)
            {
                ShowInfo("Brak historii wypożyczeń");
                return;
            }

            foreach (var rental in rentals)
            {
                DisplayRental(rental);
            }
        }
        private void CreateRentalUI()
        {
            var availableCars = _carService.GetAllCars()
                .Where(c => c.Status == "Dostępny")
                .ToList();

            if (availableCars.Count == 0)
            {
                ShowError("Brak dostępnych samochodów!");
                return;
            }


            Console.WriteLine("Dostępne samochody:");
            foreach (var car in availableCars)
            {
                Console.WriteLine($"ID: {car.Id} | {car.Brand} {car.Model} | {car.PricePerDay}zł/dzień");
            }
            var rental = new Rental(
                id: 0,
                userId: _authService.CurrentUser.Id, 
                carId: ReadInt("Podaj ID samochodu: "),
                startDate: ReadDate("Data rozpoczęcia (RRRR-MM-DD): "),
                endDate: ReadDate("Data zakończenia (RRRR-MM-DD): "),
                totalPrice: 0,
                status: "Aktywne"
            );

            try
            {
                _rentalService.AddRental(rental);
                ShowSuccess("Wypożyczenie utworzone!");
            }
            catch (Exception ex)
            {
                ShowError($"Błąd: {ex.Message}");
            }
        }
        private void ShowActiveRentalsUI()
        {
            DisplayHeader("Aktywne wypożyczenia");
            var rentals = _rentalService.GetActiveRentals();

            if (rentals.Count == 0)
            {
                ShowInfo("Brak aktywnych wypożyczeń");
                return;
            }

            foreach (var rental in rentals)
            {
                DisplayRental(rental);
            }
        }

        private void CompleteRentalUI()
        {
            DisplayHeader("Zakończ wypożyczenie");
            ShowActiveRentalsUI();
            var rentalId = ReadInt("Podaj ID wypożyczenia do zakończenia: ");
            _rentalService.CompleteRental(rentalId);
            ShowSuccess("Wypożyczenie zakończone!");
        }
        #endregion

        #region Log Management
        private void ShowLogUI()
        {
            DisplayHeader("Logi aplikacji");
            Console.WriteLine("1. Wyświetl logi");
            Console.WriteLine("2. Wyczyść logi");
            Console.WriteLine("0. Powrót");
            switch(ReadInt("Wybierz opcje: "))
            {
                case 1: ShowAllLogs(); break;
                case 2: ClearLogs(); break;
                case 0: return;
                default:  ShowError("Nieprawidłowy wybór!"); break;
            }
            WaitForContinue();


        }
        private void ShowAllLogs()
        {
            string fullPath = GetFullPath("logs.txt");

            DisplayHeader("Logi aplikacji");
            var logs = _fileManager.ReadFromFile(fullPath);
            Console.WriteLine(logs);
        }
        private void ClearLogs()
        {
            string fullPath = GetFullPath("logs.txt");

            _fileManager.ClearFile(fullPath);
            ShowSuccess("Logi wyczyszczone!");
        }
        #endregion

        #region Helpers
        private void DisplayHeader(string title)
        {
            Console.Clear();
            Console.WriteLine($"\n=== {title} ===");
            Console.WriteLine(new string('=', title.Length + 6));
            Console.WriteLine();
        }

        private string ReadString(string prompt, string defaultValue = "")
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
        }

        private int ReadInt(string prompt, int? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                    return defaultValue.Value;

                if (int.TryParse(input, out int result))
                    return result;

                ShowError("Nieprawidłowa liczba!");
            }
        }

        private double ReadDouble(string prompt, double? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                    return defaultValue.Value;

                if (double.TryParse(input, out double result))
                    return result;

                ShowError("Nieprawidłowa liczba!");
            }
        }

        private DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
                    return result;

                ShowError("Nieprawidłowy format daty! Użyj RRRR-MM-DD");
            }
        }

        private void DisplayCar(Car car)
        {
            Console.WriteLine($"ID: {car.Id}");
            Console.WriteLine($"Marka: {car.Brand}");
            Console.WriteLine($"Model: {car.Model}");
            Console.WriteLine($"Rok: {car.Year} | Kolor: {car.Color}");
            Console.WriteLine($"Cena/dzień: {car.PricePerDay}zł | Status: {car.Status}");
            Console.WriteLine(new string('-', 40));
        }

        private void DisplayUser(User user)
        {
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Imię: {user.FirstName}");
            Console.WriteLine($"Nazwisko: {user.LastName}");
            Console.WriteLine($"Login: {user.Username}");
            Console.WriteLine($"Telefon: {user.PhoneNumber} | Rola: {user.Role}");
            Console.WriteLine(new string('-', 40));
        }

        private void DisplayRental(Rental rental)
        {
            var car = _carService.GetCarById(rental.CarId);
            var user = _userService.GetUserById(rental.UserId);

            Console.WriteLine($"ID: {rental.Id}");
            Console.WriteLine($"Samochód: {car.Brand} {car.Model}");
            Console.WriteLine($"Użytkownik: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Okres: {rental.StartDate:yyyy-MM-dd} - {rental.EndDate:yyyy-MM-dd}");
            Console.WriteLine($"Koszt: {rental.TotalPrice}zł | Status: {rental.Status}");
            Console.WriteLine(new string('-', 40));
        }

        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nBłąd: {message}");
            Console.ResetColor();
        }

        private void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }

        private void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }
        private string GetFullPath(string fileName)
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            return Path.Combine(projectDirectory, "Data", fileName);
        }

        private void WaitForContinue()
        {
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }
        #endregion
    }
}