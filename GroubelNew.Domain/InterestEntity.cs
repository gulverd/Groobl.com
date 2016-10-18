using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class InterestEntity
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public DateTime AddDate { get; set; }
        public int? Rate { get; set; }
        public bool? IsMain { get; set; }
        public int Posts { get; set; }
    }
}
