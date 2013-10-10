using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMyJourney.Models
{
    public class Journey
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<VisitedPlace> VisitedPlaces { get; set; }

        public Journey()
        {
            this.VisitedPlaces = new HashSet<VisitedPlace>();
        }
    }
}
