using System;
using System.Collections.Generic;

namespace GroubelNew.Domain
{
    public class PostEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string Attachement { get; set; }
        public int UserId { get; set; }
        public System.DateTime Date { get; set; }
        public string DateString
        {
            set {
                Date = Date;
            }
            get
            {
                var currentDate =DateTime.Now - Date;
                if (currentDate != null)
                {
                    if (currentDate.Days > 0)
                    {
                        if (currentDate.Days < 7)
                        {
                            return currentDate.Days + " Day Ago";
                        }
                        else if (currentDate.Days == 7)
                        {
                            return "1 Week Ago";
                        }
                        else if (currentDate.Days > 7 && currentDate.Days < 31)
                        {
                            return currentDate.Days + " Day Ago";
                        }
                        else if (currentDate.Days > 30 && currentDate.Days < 366)
                        {
                            return Convert.ToInt32(currentDate.Days / 30) + " Month Ago";
                        }
                        else
                        {
                            return Convert.ToInt32(currentDate.Days / 365) + " Years Ago";
                        }
                    }
                    else
                    {
                        if (currentDate.Hours > 0)
                        {
                            return currentDate.Hours + " Hours Ago";
                        }
                        else
                        {
                            if (currentDate.Minutes > 0)
                            {
                                return currentDate.Minutes + " Minutes Ago";
                            }
                            else
                            {
                                return "Just Now";
                            }
                        }
                    }
                }

                return "";
            }
        }
        public bool Anonoimus { get; set; }
        public string Ip { get; set; }
        public int Saved { get; set; }
        public Tuple<Dictionary<string, int>,int> Rate { get; set; }
        public UserEntity User { get; set; }
        public UserEntity OldUser { get; set; }
        public List<PostCommentEntity> PostComments { get; set; }
        public List<InterestEntity> PostInterests { get; set; }
        public List<string> InterestNames { get; set; }
        public bool?Hide { get; set; }
        public string ImageString {
            set
            {
                Image = Image;
            }
            get
            {

                if (Attachement.Length > 0)
                {
                    var arr = Attachement.Split('.');
                    var extention = arr[arr.Length-1];

                    var extArray = new List<string> { "jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" };

                    if (extArray.Contains(extention))
                        return Attachement;
                }

                return "";
            }
        }
        public string AttachementString
        {
            set
            {
                Attachement = Attachement;
            }
            get
            {

                if (Attachement.Length > 0)
                {
                    var att = Attachement.Split('.');
                    var extention = att[att.Length-1];

                    var extArray = new List<string> { "jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" };

                    if (!extArray.Contains(extention))
                        return Attachement;
                }

                return "";
            }
        }

        public int CommentsCount { get; set; }
        public bool Reported { get; set; }
        public Tuple<int, List<string>> PostLikers { get; set; }
    }
}
