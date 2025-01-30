using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Utilities
{
    using System;
    using System.IO;

    public class FileManager
    {

        public void WriteToFile(string filePath, string content)
        {
            try
            {
                File.AppendAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy zapisywaniu pliku: {ex.Message}");
            }
        }

        public void ClearFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy czyszczeniu pliku: {ex.Message}");
            }
        }

        public string ReadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    Console.WriteLine("Plik nie istnieje.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w odczycie pliku: {ex.Message}");
                return string.Empty;
            }
        }

        public string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu pliku: {ex.Message}");
                return string.Empty;
            }
        }
        public static void WriteAllLines(string path, IEnumerable<string> lines)
        {
            File.WriteAllLines(path, lines);
        }
    }

}
