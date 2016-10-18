using Groubel.Data;
using Groubel.Helpers;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GroubelNew.BLL
{
    public class UserService
    {
        SecurityService _securityService;
        FriendsService _friendService;
        InterestsService _interestService;

        public UserService(SecurityService sc, FriendsService fr, InterestsService inter)
        {
            _securityService = sc;
            _friendService = fr;
            _interestService = inter;
        }

        public UserEntity GetUserById(int id, int curId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Id == id);

                if (us == null)
                    return null;

                var rt = new UserEntity
                {
                    Id = us.Id,
                    FirstName = us.FirstName,
                    LastName = us.LastName,
                    UserName = us.UserName,
                    Password = us.Password,
                    Email = us.Email,
                    RegiterDate = us.RegisterDate,
                    LastLoginDate = us.LastLoginDate,
                    LastLoginIp = us.LastLoginIp,
                    DateOfBirth = us.DateOfBirth,
                    Gender = us.Gender,
                    IsMe = us.Id == id,
                    RequestSent = db.Notifications.Any(k => k.SenderUserId == curId && !k.ApproovedStatus && k.Type==11),
                    RequestRecived= db.Notifications.Any(k => k.ReciverUserId == curId && !k.ApproovedStatus && k.Type == 11),
                    Interests = db.Interests.Where(i=>db.UserInterests.Where(j=>j.UserId==id).Select(j=>j.InterestId).Contains(i.Id)).Select(i => new InterestEntity
                    {
                        Id = i.Id,
                        Name = i.Name,
                        UserId = us.Id,
                        IsMain= db.UserInterests.Where(j => j.UserId == curId).Select(j => j.InterestId).Contains(i.Id)
                    }).ToList(),
                    Image = GetUserImage(us.Id),
                    IsOnline = _securityService.IsOnline(us.Id),
                    IsFriend= _friendService.IsFriend(us.Id, curId),
                    FriendsCount=db.Friendships.Count(t=>t.UserId==us.Id)
                };

                if (id == curId)
                    rt.Interests = _interestService.GetUserInterests(id);

                return rt;
            }
        }

        public UserEntity GetUserByIdForPopup(int id, int curId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Id == id);

                if (us == null)
                    return null;

                var rt = new UserEntity
                {
                    Id = us.Id,
                    FirstName = us.FirstName,
                    LastName = us.LastName,

                    IsMe = us.Id == id,
                    RequestSent = db.Notifications.Any(k => k.SenderUserId == curId && !k.ApproovedStatus && k.Type == 11),
                    RequestRecived = db.Notifications.Any(k => k.ReciverUserId == curId && !k.ApproovedStatus && k.Type == 11),
                    Image = GetUserImage(us.Id),
                    IsOnline = _securityService.IsOnline(us.Id),
                    IsFriend = _friendService.IsFriend(us.Id, curId),
                    ShareFriends= GetSameFriends(curId,us.Id),
                    LastLoginDate=us.LastLoginDate
                };

                return rt;
            }
        }

        public List<string> GetSameFriends(int firstUserId,int secondUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var frIds = db.Friendships
                                 .Where(i => 
                                         i.UserId == firstUserId 
                                         && db.Friendships
                                              .Where(j => j.UserId == secondUserId)
                                              .Select(j => j.FriendId)
                                              .Contains(i.FriendId)       
                                 ).Select(i=>i.FriendId);

                var data = db.Users
                                   .Where(i => frIds.Contains(i.Id))
                                   .Select(i => i.FirstName + " " + i.LastName)
                                   .ToList();

                return data;
            }
        }

        public string GetUserImage(int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.UserImages.FirstOrDefault(i => i.UserId == id);

                if (us == null)
                    return "";

                return us.Image;
            }

        }

        public void UpdateUser(UserEntity user)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.Users.FirstOrDefault(i => i.Id == user.Id);

                if (us == null)
                    return;

                us.FirstName = user.FirstName;
                us.LastName = user.LastName;
                us.UserName = "";
                
                us.Email = user.Email;

                us.DateOfBirth = user.DateOfBirth;
                us.Gender = user.Gender;

                if(us.Password !="")
                     us.Password = user.Password;

                db.SaveChanges();
            }
        }

        public string AddImage(HttpPostedFileBase file, int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.UserImages.FirstOrDefault(i => i.UserId == id);

                var upload = new IOHandler("~/Uploads/ProfileImages/");

                var name = upload.Save(file);

                if (us == null)
                    db.UserImages.Add(new UserImage { UserId = id, Image = name, Sort = 0, IsMain = false,AddDate=DateTime.Now });
                else
                    us.Image = name;

                db.SaveChanges();

                return name;

            }
        }

        public bool IsFriend(int userId, int friendId)
        {
            using (var db = new groubel_dbEntities1())
            {
                return db.Friendships.Count(i => i.UserId == userId && i.FriendId == friendId) > 0;
            }
        }

        public List<UserEntity> GetUserByName(int userId, string name) {
            using (var db = new groubel_dbEntities1())
            {

                var friends = db.Users
                    .Where(i =>  (i.FirstName.StartsWith(name) || i.LastName.StartsWith(name))

                                  ).ToList()
                    .Select(i => new UserEntity
                    {
                        Id = i.Id,
                        FirstName = i.FirstName,
                        LastName = i.LastName,

                        LastLoginDate = i.LastLoginDate,

                        DateOfBirth = i.DateOfBirth,
                        Gender = i.Gender,
                        IsMe = i.Id == userId,

                        Image = GetUserImage(i.Id),
                        IsOnline = _securityService.IsOnline(i.Id),
                        RequestSent = db.Notifications.Any(k => k.SenderUserId == userId && k.ApproovedStatus),
                        RequestRecived = db.Notifications.Any(k => k.SenderUserId == i.Id && k.ApproovedStatus),
                        IsFriend = _friendService.IsFriend(i.Id, userId)
                    }).ToList();

                return friends;

            }
        }

        public List<UserFilesEntity> GetUserFiles(int userId, bool type)
        {
            using (var db = new groubel_dbEntities1())
            {
                var extArray = new List<string> { "jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" };

                var dt = db.Posts.Where(i => i.Attachement != "" && i.UserId == userId).Select(i => new UserFilesEntity {
                    Date = i.Date,
                    Name = i.Attachement,
                    Type = 1
                }).ToList();
                var comm=db.PostComments.Where(i => i.Attachement != "" && i.UserId == userId).Select(i => new UserFilesEntity
                {
                    Date = i.Date,
                    Name = i.Attachement,
                    Type =2
                }).ToList();
                var ctcomm=db.ChatComments.Where(i => i.Attachement != "" && i.UserId == userId).Select(i => new UserFilesEntity
                {
                    Date = i.Date,
                    Name = i.Attachement,
                    Type = 3
                }).ToList();


                var data = new List<UserFilesEntity>();

                data.AddRange(dt);
                data.AddRange(comm);
                data.AddRange(ctcomm);

                if(type)
                  return data.Where(i => extArray.Contains((i.Name.Split('.')).Last())).OrderByDescending(i=>i.Date).ToList();

                return data.Where(i => !extArray.Contains((i.Name.Split('.')).Last())).OrderByDescending(i => i.Date).ToList();

            }
        }

        public int Count()
        {
            using (var db = new groubel_dbEntities1())
            {
                return db.Users.Count();
            }
        }
    }
}
