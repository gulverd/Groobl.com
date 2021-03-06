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
    
    public partial class Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Post()
        {
            this.PostComments = new HashSet<PostComment>();
            this.PostInterests = new HashSet<PostInterest>();
            this.PostHides = new HashSet<PostHide>();
        }
    
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string Attachement { get; set; }
        public int UserId { get; set; }
        public System.DateTime Date { get; set; }
        public bool Anonoimus { get; set; }
        public string Ip { get; set; }
        public Nullable<int> OldUserId { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostComment> PostComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostInterest> PostInterests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostHide> PostHides { get; set; }
    }
}
