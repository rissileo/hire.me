using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vale_shorturl.Model
{
    public class RequestShortUrlViewModel
    {
        [Required]
        public string URL { get; set; }
        public string CUSTOM_ALIAS { get; set; }
    }
}
