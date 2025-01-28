using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StreetClient;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GetStreets_Click(object sender, RoutedEventArgs e)
    {
        string postcode = PostcodeTextBox.Text.Trim();
        if (postcode.Length != 6 || !int.TryParse(postcode, out _))
        {
            MessageBox.Show("Ошибка: Почтовый индекс должен состоять из 6 цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            using TcpClient client = new TcpClient("127.0.0.1", 65432);
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(postcode);
            stream.Write(data, 0, data.Length);

            data = new byte[4048];
            int bytesRead = stream.Read(data);
            string response = Encoding.UTF8.GetString(data, 0, bytesRead);
            StreetsListBox.Items.Clear();
            string[] streets = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var street in streets)
            {
                StreetsListBox.Items.Add(street.Trim());
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}