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
    
    public partial class CommentLike
    {
        public int Id { get; set; }
        public int CoomentId { get; set; }
        public int UserId { get; set; }
        public bool Like { get; set; }
    
        public virtual ChatComment ChatComment { get; set; }
        public virtual User User { get; set; }
    }
}