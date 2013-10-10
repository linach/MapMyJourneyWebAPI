using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapMyJourney.WebApi.Models
{
    public class VisitedPlaceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Picture { get; set; }

        public string Comment { get; set; }
    }
}