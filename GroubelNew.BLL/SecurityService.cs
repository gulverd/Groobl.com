using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Groubel.Data;
using GroubelNew.Domain;

namespace GroubelNew.BLL
{
    public class SecurityService
    {
        private InterestsService _interestsService;

        public SecurityService (InterestsService intser)
        {
            _interestsService = intser;
        }

        public bool SignIn(UserEntity user)
        {

            using(var db=new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Email == user.Email && i.Password == user.Password);

                if (us == null)
                    return false;

                us.LastLoginDate = DateTime.Now;
                us.LastLoginIp = user.LastLoginIp;

                db.SaveChanges();

                return true;
            }

        }

        public bool CheckEmail(string email)
        {
            using (var db=new groubel_dbEntities1())
            {
                var item = db.Users.FirstOrDefault(i => i.Email == email);

                return item != null;
            }
        }

        public bool Register(UserEntity user)
        {

            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Email == user.Email);

                if (us != null)
                    return false;

                var item = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    LastLoginDate = user.LastLoginDate,
                    LastLoginIp = user.LastLoginIp,
                    DateOfBirth = user.DateOfBirth,
                    RegisterDate = user.RegiterDate,
                    Gender = user.Gender,
                    Password = user.Password,

                };

                db.Users.Add(item);

                db.SaveChanges();

                db.UserImages.Add(new UserImage {
                    UserId = item.Id,
                    Image = "http://groobl.com/uploads/profile.jpg",
                    IsMain = true,
                    AddDate=DateTime.Now,
                    Sort=1
                });

                db.SaveChanges();

                _interestsService.AddInterestsToUser(item.Id, user.Interests.Select(i => i.Id).ToList());

                return true;
            }
        }

        public void Ping(int id)
        {

            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Id == id);

                if (us == null)
                    return;

                us.LastLoginDate = DateTime.Now;

                db.SaveChanges();

            }

        }

        public bool IsOnline(int id)
        {

            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Id == id);

                if (us == null)
                    return false;

                var tm = (us.LastLoginDate>DateTime.Now.AddMinutes(-2));

                return tm;

            }

        }

        public string ForgotPassword(string email)
        {

            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Email == email);

                if (us == null)
                    return "";


                var em = new Groubel.Helpers.Email();

                var tmpId = em.SendEmail(email);

                us.TempGuId = tmpId;

                db.SaveChanges();

                return tmpId;
            }

        }

        public bool GetResetPasswordState(string code)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.TempGuId == code);
                if (us == null || code == null)
                    return false;

                return true;
            }
        }

        public bool UpdatePassword(string email, string pass)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Email == email);

                if (us == null)
                    return false;

                us.Password = pass;
                us.TempGuId = null;

                db.SaveChanges();

                return true;
            }
        }

        public int UserIdByEmail(string email)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Email == email);

                if (us == null)
                    return -1;

                return us.Id;
            }
        }

    }
}
