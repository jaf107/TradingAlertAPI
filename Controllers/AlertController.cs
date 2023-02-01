using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TradingAlertAPI.Services;

namespace TradingAlertAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private const string SuccessCode = "200";

        [HttpPost("/receivealert", Name = "ReceiveAlert")]
        public async Task<string> ReceiveAlert()
        {
            StreamWriter? sw = null;
            var req = Request;



            StreamReader streamReader = new StreamReader(req.Body, Encoding.UTF8);
            string reqBody = await streamReader.ReadToEndAsync();
            try
            {
                sw = new StreamWriter("AlertDetails.txt", true);
                
                await sw.WriteLineAsync(DateTime.Now + " , " + reqBody);

                new AlertService().AlertAPIService(reqBody);

            }
            catch (IOException ex)
            {
                sw = new StreamWriter("AlertErrors.txt", true);
                await sw.WriteLineAsync(DateTime.Now + " , " + ex.Message);
            }
            catch (Exception ex)
            {
                sw = new StreamWriter("AlertErrors.txt", true);
                await sw.WriteLineAsync(DateTime.Now + " , " + ex.Message);
            }
            finally
            {
                await sw!.DisposeAsync();
            }

            return SuccessCode;
        }

        [HttpGet("/getversion", Name = "GetVersion")]
        public string GetVersion()
        {
            return "1.0";
        }
    }
}
