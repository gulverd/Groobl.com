using Groubel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class NotifictionEntity
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public int ReciverUserId { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public bool SeenStatus { get; set; }
        public bool ApproovedStatus { get; set; }

        public UserEntity SenderUser { get; set; }
        public UserEntity ReciverUser { get; set; }

        public int AliasId { get; set; }
        public string AliasName { get; set; }
        public DateTime Date { get; set; }
        public string DateString
        {
            set
            {
                Date = Date;
            }
            get
            {
                return Date.ToString("h:mm:ss tt");
            }
        }
        public string GroupDateString
        {
            set
            {
                Date = Date;
            }
            get
            {
                return Date.ToString("MMM ddd yyyy");
            }
        }

    }
}
