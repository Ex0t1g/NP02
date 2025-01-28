using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server
{
    public class Program
    {
        private static Dictionary<string, List<string>> streets = new();

        static void Main(string[] args)
        {
            LoadStreets("streets.txt");
            StartServer();
        }

        private static void LoadStreets(string filename)
        {
            Console.WriteLine("Улицы были успешно загружены...");
            string[] lines = File.ReadAllLines(filename);
            string currentPostcode = null;

            foreach (var line in lines)
            {
                if (line.Length == 6 && int.TryParse(line, out _))
                {
                    currentPostcode = line;
                    streets[currentPostcode] = new List<string>();
                }
                else if (currentPostcode != null)
                {
                    streets[currentPostcode].Add(line);
                }
            }
        }

        private static void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 65432);
            listener.Start();
            Console.WriteLine("Сервер запущен на порту 65432...");

            while (true)
            {
                using TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Подключен клиент.");

                byte[] buffer = new byte[256];
                int bytesRead = client.Client.Receive(buffer);

                string postcode = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                string response = HandleRequest(postcode);
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                client.Client.Send(responseData);
            }
        }

        private static string HandleRequest(string postcode)
        {
            if (streets.ContainsKey(postcode))
            {
                return string.Join(Environment.NewLine, streets[postcode]);
            }
            return "Почтовый индекс не найден.";
        }
    }
}
