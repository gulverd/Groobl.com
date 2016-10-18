using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class ChatEntity
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public bool IsRoom { get; set; }
        public string Name { get; set; }
        public bool Visibility { get; set; }
        public bool Anonimus { get; set; }
        public string Image { get; set; }
        public bool IsArchive { get; set; }
        public int MaxUsers { get; set; }
        public int UserId { get; set; }

        public List<InterestEntity> Interests { get; set; }
        public List<ChatMemberEntity> Members { get; set; }
        public List<ChatCommentEntity> Comments { get; set; }
        public string LastCommentDate { get; set; }
        public bool AreMember { get; set; }
        public int UnSeenCmments { get; set; }
        public List<string> MemberName { get; set; }
        public bool RequestSent { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public int AnotherUserId { get; set; }
    }
}
