//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Groubel.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChatComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChatComment()
        {
            this.CommentFavorites = new HashSet<CommentFavorite>();
            this.CommentLikes = new HashSet<CommentLike>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public System.DateTime Date { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Attachement { get; set; }
    
        public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentFavorite> CommentFavorites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentLike> CommentLikes { get; set; }
    }
}
