using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groubel.Helpers
{
    public static class Extentions
    {
        public static List<GroupedPostEntity> ToGrouped(this List<PostEntity> post)
        {
            var data = post.GroupBy(i => (DateTime.Now.Date - i.Date.Date).TotalDays.ToString()).Select(i => new GroupedPostEntity { Date = i.Key, Posts = i.ToList() }).ToList();

            return data;
        }
    }
}
