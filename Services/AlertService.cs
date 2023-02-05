using TradingAlertAPI.Constants;
using System.Globalization;
using System.Text;
using TradingAlertAPI.Models;

namespace TradingAlertAPI.Services
{
    public class AlertService
    {
        public void CallExternalAPI(string easternTime, string message)
        {
            // Initiate HTTPClient
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();

            // Create Token (Perform XOR for Security)
            List<String> xorStrList = new List<String>(new[] { easternTime, message });
            String token = PerformXOR(xorStrList);

            client.DefaultRequestHeaders.Add("CallerToken", token);
            
            // Send to Executor API
            AlertModel alert= new AlertModel()
            {
                TimeAndMessage = easternTime + "||" + message
            };
            var httpResponse = client.PostAsJsonAsync(AlertConstant.ClientAPIUrl, alert).Result;
            
            var strResponse = httpResponse.Content.ReadAsStringAsync().Result;

            Console.WriteLine(strResponse);
        }

        private string PerformXOR(List<string> textsBeforeModification)
        {
            // Check if list contains any null value, if so then throw Security exception
            if (textsBeforeModification.Any(x => x == null))
            {
                throw new Exception();
            }

            const int targetTextLength = 16;
            const char paddingCharacter = '9';
            List<StringBuilder> listStrBuilders = new();

            // Get String Builder ready and make texts contain equal number of characters
            List<string> textsAfterModification = new List<string>();
            foreach (string text in textsBeforeModification)
            {
                // make every texts length of 16 characters; 1 character = 1 byte
                string modifiedText = text.Length < targetTextLength ? text.PadLeft(targetTextLength, paddingCharacter) : text[..targetTextLength];
                textsAfterModification.Add(modifiedText);

                // create string builders of similar length of modified texts
                listStrBuilders.Add(new StringBuilder(modifiedText));
            }

            StringBuilder outputStringBuilder = new StringBuilder(textsAfterModification[0].Length);

            // Do XOR char by char
            var xoredChar = 'A';
            for (int i = 0; i < textsAfterModification[0].Length; i++)
            {
                for (int j = 0; j < listStrBuilders.Count; j++)
                {
                    if (j == 0)
                    {
                        xoredChar = listStrBuilders[j][i];
                    }
                    else
                    {
                        char charToXorWith = listStrBuilders[j][i];
                        xoredChar = (char)(xoredChar ^ charToXorWith);
                    }
                }
                outputStringBuilder.Append(xoredChar);
            }

            // Return XORed Value
            var xorOutput = outputStringBuilder.ToString();
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(xorOutput));
        }
    }
}
