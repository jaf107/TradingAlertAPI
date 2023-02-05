﻿using Microsoft.AspNetCore.Mvc;
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
            StreamWriter? sw1 = null;
            StreamWriter? sw2 = null;
            var req = Request;

            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);


            // Read req body and write to file
            StreamReader streamReader = new StreamReader(req.Body, Encoding.UTF8);
            string reqBody = await streamReader.ReadToEndAsync();
            try
            {
                sw1 = new StreamWriter("AlertDetails.txt", true);
                await sw1.WriteLineAsync(easternTime + " , " + reqBody);
                new AlertService().CallExternalAPI(reqBody);

            }
            catch (IOException ex)
            {
                sw2 = new StreamWriter("AlertErrors.txt", true);
                await sw2.WriteLineAsync(easternTime+ " , " + ex.Message);
            }
            catch (Exception ex)
            {
                sw2 = new StreamWriter("AlertErrors.txt", true);
                await sw2.WriteLineAsync(easternTime + " , " + ex.Message);
            }
            finally
            {
                await sw1!.DisposeAsync();
                await sw2!.DisposeAsync();
            }

            return SuccessCode;
        }

        [HttpGet("/getversion", Name = "GetVersion")]
        public string GetVersion()
        {
            return "1.2";
        }
    }
}
