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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private async void SendMessage()
        {
            string message = txtMessage.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                txtChat.AppendText($"You: {message}\n");

                string botReply = await GetGeminiResponse(message); // gọi AI
                txtChat.AppendText($"Chat Bot: {botReply}\n\n");

                txtMessage.Clear();
                txtMessage.Focus();
                txtChat.ScrollToEnd();
            }
        }
        private async Task<string> GetGeminiResponse(string message)
        {
            string apiKey = "AIzaSyAmPoHKbk5MVMpHR4ae2Wziv482zDnjeG4";
            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = message }
                }
            }
        }
            };

            using (HttpClient client = new HttpClient())
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
               

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                var json = await response.Content.ReadAsStringAsync();
  


                // debug
                //Console.WriteLine(json);

                dynamic result = JsonConvert.DeserializeObject(json);

                if (result?.candidates == null || result.candidates.Count == 0)
                {
                    return "⚠️ Không nhận được phản hồi từ Gemini.";
                }

                return result.candidates[0].content.parts[0].text;  // lấy text đầu tiên
            }
        }
    }


}
