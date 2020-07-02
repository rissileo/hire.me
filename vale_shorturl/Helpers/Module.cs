using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vale_shorturl.DAL;

namespace vale_shorturl.Helpers
{
    public class Module
    {
        private readonly IConfiguration _configuration;

        public Module(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GeneratingRandomicNumber()
        {
            dalShortURL dal = new dalShortURL(_configuration);
            Random _random = new Random();
            string number = _random.Next(0, 999999999).ToString();

            if (!dal.AliasExists(number))
            {
                return number;
            }
            else
            {
                return GeneratingRandomicNumber();
            }
        }


        public string Crypt(string value)
        {
            string ret = string.Empty;

            foreach (char s in value)
            {
                switch (int.Parse(s.ToString()))
                {
                    case 0:
                        ret += "f";
                        break;
                    case 1:
                        ret += "W";
                        break;
                    case 2:
                        ret += "x";
                        break;
                    case 3:
                        ret += "P";
                        break;
                    case 4:
                        ret += "h";
                        break;
                    case 5:
                        ret += "F";
                        break;
                    case 6:
                        ret += "k";
                        break;
                    case 7:
                        ret += "j";
                        break;
                    case 8:
                        ret += "G";
                        break;
                    case 9:
                        ret += "l";
                        break;
                }
            }
            return ret;
        }
    }
}
