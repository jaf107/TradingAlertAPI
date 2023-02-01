using TradingAlertAPI.Constants;
namespace TradingAlertAPI.Services
{
    public class AlertService
    {
        public void AlertAPIService(string message)
        {
            //string headerValue = req.Headers["validator"];

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("validator", "Noshin");

                var httpResponse = client.PostAsJsonAsync(AlertConstant.ClientAPIUrl, message).Result;

                var strResponse = httpResponse.Content.ReadAsStringAsync().Result;

                Console.WriteLine(strResponse);
            }

        }

    }
}
