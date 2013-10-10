using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMyJourney.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string AuthCode { get; set; }

        public string AuthToken { get; set; }

        public virtual ICollection<Journey> Journeys { get; set; }

        public User()
        {
            this.Journeys = new HashSet<Journey>();
        }
    }
}
