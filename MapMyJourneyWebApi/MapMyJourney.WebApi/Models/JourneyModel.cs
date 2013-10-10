using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapMyJourney.WebApi.Models
{
    public class JourneyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}