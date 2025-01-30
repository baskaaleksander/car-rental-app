# Wypożyczalnia Samochodów

Aplikacja konsolowa do zarządzania wypożyczalnią samochodów, napisana w .NET. System umożliwia zarządzanie samochodami, użytkownikami oraz wypożyczeniami. Aplikacja wykorzystuje pliki tekstowe do przechowywania danych oraz system logów do śledzenia zdarzeń.

## Funkcjonalności

### Dla administratora:
#### Zarządzanie samochodami:
- Dodawanie, edycja, usuwanie samochodów.
- Zmiana statusu samochodu (dostępny/wynajęty).

#### Zarządzanie użytkownikami:
- Dodawanie, edycja, usuwanie użytkowników.

#### Zarządzanie wypożyczeniami:
- Przeglądanie wszystkich wypożyczeń.
- Zakończenie wypożyczenia.

### Dla użytkownika:
#### Moje wypożyczenia:
- Przeglądanie aktywnych i zakończonych wypożyczeń.

#### Nowe wypożyczenie:
- Wyszukiwanie dostępnych samochodów.
- Tworzenie nowego wypożyczenia.

## Wymagania systemowe

- **Środowisko:** .NET 9.0
- **System operacyjny:** Windows, macOS, Linux

## Instalacja

Pobierz lub sklonuj repozytorium:

```bash
git clone https://github.com/twoj-repozytorium/car-rental-app.git
```

Przejdź do katalogu projektu:

```bash
cd car-rental-app
```

Skompiluj projekt:

```bash
dotnet build
```

Uruchom aplikację:

```bash
dotnet run
```

## Dane logowania

### Administrator:
- **Login:** admin
- **Hasło:** admin

### Użytkownik:
- **Login:** olebas
- **Hasło:** test123

## Struktura projektu

```
CarRentalApp/
├── Data/                  # Folder z danymi (pliki .txt)
│   ├── users.txt          # Plik z danymi użytkowników
│   ├── cars.txt           # Plik z danymi samochodów
│   ├── rentals.txt        # Plik z danymi wypożyczeń
│   └── logs.txt           # Plik z logami aplikacji
├── Models/                # Modele danych
├── Services/              # Logika biznesowa
├── Utilities/             # Narzędzia pomocnicze
├── Program.cs             # Główny plik aplikacji
└── README.md              # Dokumentacja
```

## Logi aplikacji

Wszystkie zdarzenia są zapisywane w pliku `Data/logs.txt`. Przykładowy wpis:

```
[2023-10-25 14:30:45] Użytkownik admin zalogował się pomyślnie.
[2023-10-25 14:31:10] Dodano nowy samochód: Toyota Corolla (ID: 5)
[2023-10-25 14:32:00] Użytkownik admin wylogował się.
```

## Autor

- **Imię i nazwisko:** Aleksander Baska
- **Email:** baskaaleksander03@gmail.com
- **GitHub:** [https://github.com/baskaaleksander](https://github.com/baskaaleksander)


