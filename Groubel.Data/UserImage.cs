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
    
    public partial class UserImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public bool IsMain { get; set; }
        public Nullable<int> Sort { get; set; }
        public System.DateTime AddDate { get; set; }
        public int UserId { get; set; }
    
        public virtual User User { get; set; }
    }
}
