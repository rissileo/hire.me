using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nancy.ModelBinding.DefaultConverters;
using vale_shorturl.DAL;
using vale_shorturl.Helpers;
using vale_shorturl.Model;

namespace vale_shorturl.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShortUrlController : Controller
    {
        private readonly IConfiguration _configuration;

        public ShortUrlController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPut]
        public IActionResult create([FromQuery] string URL, [FromQuery] string CUSTOM_ALIAS)
        {
            Trace tc = new Trace();
            DateTime dtNowStart = DateTime.Now;
            int isAutomaticController = 0;
            ResponseShortUrlViewModel_Error respError = new ResponseShortUrlViewModel_Error();
            ResponseShortUrlViewModel_Success respSuccess = new ResponseShortUrlViewModel_Success();

            Module hlp = new Module(_configuration);
            dalShortURL dal = new dalShortURL(_configuration);

            try
            {
                tc.GeraLog("REQUEST PARAMETES: " + URL + " | " + CUSTOM_ALIAS);

                string sRandomNotRepeaterNumber = hlp.GeneratingRandomicNumber();
                string sCryptNumber = hlp.Crypt(sRandomNotRepeaterNumber);

                if (!ModelState.IsValid)
                {
                    respError.err_code = "400";
                    respError.description = HttpStatusCode.BadRequest.ToString();

                    tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                    return Ok(respError);
                }
                else
                {
                    DateTime dtNowEnd = DateTime.Now;

                    if (!string.IsNullOrEmpty(CUSTOM_ALIAS))
                    {
                        if (!dal.AliasExists(CUSTOM_ALIAS))
                        {
                            respSuccess.alias = CUSTOM_ALIAS;
                        }
                        else
                        {
                            respError.err_code = "001";
                            respError.description = "CUSTOM ALIAS ALREADY EXISTS";

                            tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                            return Ok(respError);
                        }
                    }
                    else
                    {
                        respSuccess.alias = sCryptNumber;
                        isAutomaticController = 1;
                    }

                    string shortURLRoot = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build()
                        .GetSection("AppSettings")["urlRootShort"];

                    string shortURL = shortURLRoot + sCryptNumber;

                    if (dal.CreateShortURLRegister(URL, shortURL, respSuccess.alias, isAutomaticController))
                    {
                        TimeSpan span = dtNowEnd - dtNowStart;
                        int ms = (int)span.TotalMilliseconds;

                        respSuccess.url = shortURL;
                        respSuccess.statistic = new statistic
                        {
                            time_taken = ms.ToString() + " ms"
                        };

                        tc.GeraLog("RESULT: " + respSuccess.alias + " | " + respSuccess.url + " | " + respSuccess.statistic.time_taken);

                        return Ok(respSuccess);
                    }
                    else
                    {
                        respError.err_code = "777";
                        respError.description = "WAS NOT POSSIBLE INSERT IN DATABASE. CHECK IF IT WAS CREATED.";

                        tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                        return Ok(respError);
                    }
                }
            }
            catch (Exception ex)
            {
                respError.err_code = "500";
                respError.description = ex.ToString().ToUpper();

                tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                return Ok(respError);
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string URL)
        {
            ResponseShortUrlViewModel_Error respError = new ResponseShortUrlViewModel_Error();
            ResponseShortUrlViewModel_Success respSuccess = new ResponseShortUrlViewModel_Success();
            dalShortURL dal = new dalShortURL(_configuration);

            Trace tc = new Trace();

            try
            {
                if (!ModelState.IsValid)
                {
                    respError.err_code = "400";
                    respError.description = HttpStatusCode.BadRequest.ToString();

                    tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                    return Ok(respError);
                }
                else
                {
                    string OriginalURL = dal.GetURLByShort(URL);
                    if (!string.IsNullOrEmpty(OriginalURL))
                    {
                        if (dal.RegistreShortURLAccess(URL))
                        {
                            tc.GeraLog("REDIRECT TO: " + OriginalURL);
                            tc.GeraLog("SUCCESS.");
                            return Redirect(OriginalURL);
                        }
                        else
                        {
                            respError.err_code = "777";
                            respError.description = "WAS NOT POSSIBLE INSERT IN DATABASE. CHECK IF IT WAS CREATED.";

                            tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                            return Ok(respError);
                        }
                    }
                    else
                    {
                        respError.err_code = "404";
                        respError.description = HttpStatusCode.NotFound.ToString();

                        tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                        return Ok(respError);
                    }
                }
            }
            catch (Exception ex)
            {
                respError.err_code = "500";
                respError.description = ex.ToString().ToUpper();

                tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                return Ok(respError);
            }
        }


        [HttpGet]
        public IActionResult GetTopTen()
        {
            ResponseShortUrlViewModel_Error respError = new ResponseShortUrlViewModel_Error();
            ResponseShortUrlViewModel_Success respSuccess = new ResponseShortUrlViewModel_Success();
            dalShortURL dal = new dalShortURL(_configuration);
            Trace tc = new Trace();

            try
            {
                if (!ModelState.IsValid)
                {
                    respError.err_code = "400";
                    respError.description = HttpStatusCode.BadRequest.ToString();

                    tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                    return Ok(respError);
                }
                else
                {
                    var result = dal.GetTopTen();
                    tc.GeraLog("SUCCESS.");

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                respError.err_code = "500";
                respError.description = ex.ToString().ToUpper();

                tc.GeraLog("RESULT: " + respError.err_code + " | " + respError.description);

                return Ok(respError);
            }
        }
    }
}