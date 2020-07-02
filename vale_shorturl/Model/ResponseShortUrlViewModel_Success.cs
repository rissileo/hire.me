using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vale_shorturl.Model
{
    public class ResponseShortUrlViewModel_Success
    {
        public string alias { get; set; }
        public string url { get; set; }
        public statistic statistic { get; set; }
    }

    public class statistic
    {
        public string time_taken { get; set; }
    }
}
