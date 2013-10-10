using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMyJourney.Models
{
    public class VisitedPlace
    {
        public int Id { get; set; }

        public string  Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Picture { get; set; }

        public string Comment { get; set; }

        public virtual Journey Journey { get; set; }
    }
}
