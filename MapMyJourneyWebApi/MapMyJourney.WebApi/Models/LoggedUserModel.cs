using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapMyJourney.WebApi.Models
{
    public class LoggedUserModel
    {
        public string Displayname { get; set; }

        public string AuthToken { get; set; } 
    }
}