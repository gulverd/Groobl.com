using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class UserFilesEntity
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string DateString
        {
            set
            {
                Date = Date;
            }
            get
            {
                return Date.ToString("g");
            }
        }
    }
}
