using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class ChatMemberEntity:UserEntity
    {
        public new int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public System.DateTime Date { get; set; }
        public bool Agree { get; set; }
        public string Avatar { get; set; }
        public int Status { get; set; }
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
