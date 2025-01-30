using CarRentalApp.UI;

class Program
{
    static void Main()
    {
        string dataDirectory = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Data");
        Console.WriteLine($"Data directory: {dataDirectory}");
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        var menu = new Menu();
        menu.Run();
    }
}