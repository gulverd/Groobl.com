using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class FilterInterestEntity : InterestEntity
    {
        public int PostNumber { get; set; }
        public int NewPostsNumber { get; set; }
    }
}
