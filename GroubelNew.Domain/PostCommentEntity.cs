using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class PostCommentEntity
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public System.DateTime Date { get; set; }
        public int Status { get; set; }
        public string DateString
        {
            set
            {
                Date = Date;
            }
            get
            {
                var currentDate = DateTime.Now - Date;
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
        public UserEntity User { get; set; }

        public string Attachement { get; set; }
        public string ImageString
        {
            set
            {
                Attachement = Attachement;
            }
            get
            {

                if (Attachement!=null && Attachement.Length > 0)
                {
                    var arr = Attachement.Split('.');
                    var extention = arr[arr.Length - 1];

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

                if (Attachement != null && Attachement.Length > 0)
                {
                    var att = Attachement.Split('.');
                    var extention = att[att.Length - 1];

                    var extArray = new List<string> { "jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" };

                    if (!extArray.Contains(extention))
                        return Attachement;
                }

                return "";
            }
        }
    }
}
