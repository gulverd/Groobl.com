﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class groubel_dbEntities1 : DbContext
    {
        public groubel_dbEntities1()
            : base("name=groubel_dbEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ChatComment> ChatComments { get; set; }
        public virtual DbSet<ChatInterest> ChatInterests { get; set; }
        public virtual DbSet<ChatMember> ChatMembers { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<CommentFavorite> CommentFavorites { get; set; }
        public virtual DbSet<CommentLike> CommentLikes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Interest> Interests { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<UserImage> UserImages { get; set; }
        public virtual DbSet<UserInterest> UserInterests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PostComment> PostComments { get; set; }
        public virtual DbSet<PostInterest> PostInterests { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<PostHide> PostHides { get; set; }
        public virtual DbSet<PostSave> PostSaves { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Reporting> Reportings { get; set; }
    }
}