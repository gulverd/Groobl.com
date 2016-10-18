using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class GroupedPostEntity
    {
        public string Date { get; set; }
        public List<PostEntity> Posts { get; set; }
    }
}
