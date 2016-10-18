
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.Domain
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegiterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LastLoginIp { get; set; }
        public bool IsMe { get; set; }
        public List<InterestEntity> Interests { get; set; }
        public string Image { get; set; }
        public bool IsOnline { get; set; }
        public bool IsFriend { get; set; }
        public bool RequestSent { get; set; }
        public bool RequestRecived { get; set; }
        public string DateOfBirthString
        {
            set
            {
                DateOfBirth = DateOfBirth;
            }
            get
            {
                return DateOfBirth != null ? ((DateTime)DateOfBirth).ToString("MM/dd/yyyy") : "";
            }
        }
        public string LastLoginString
        {
            set
            {
                LastLoginDate = LastLoginDate;
            }
            get
            {
                var currentDate = DateTime.Now - LastLoginDate;
                if (currentDate != null)
                {
                    if (currentDate.Value.Days > 0)
                    {
                        if (currentDate.Value.Days < 7)
                        {
                            return currentDate.Value.Days + " Day Ago";
                        }
                        else if (currentDate.Value.Days == 7)
                        {
                            return "1 Week Ago";
                        }
                        else if (currentDate.Value.Days > 7 && currentDate.Value.Days < 31)
                        {
                            return currentDate.Value.Days + " Day Ago";
                        }
                        else if (currentDate.Value.Days > 30 && currentDate.Value.Days < 366)
                        {
                            return Convert.ToInt32(currentDate.Value.Days / 30) + " Month Ago";
                        }
                        else
                        {
                            return Convert.ToInt32(currentDate.Value.Days / 365) + " Years Ago";
                        }
                    }
                    else
                    {
                        if (currentDate.Value.Hours > 0)
                        {
                            return currentDate.Value.Hours + " Hours Ago";
                        }
                        else
                        {
                            if (currentDate.Value.Minutes > 0)
                            {
                                return currentDate.Value.Minutes + " Minutes Ago";
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
        public int Age
        {
            set
            {
                DateOfBirth = DateOfBirth;
            }
            get
            {
                return DateTime.Now.Year - DateOfBirth.Year;
            }
        }

        public List<string> ShareFriends { get; set; }
        public int RateType { get; set; }
        public int FriendsCount { get; set; }
    }
}
