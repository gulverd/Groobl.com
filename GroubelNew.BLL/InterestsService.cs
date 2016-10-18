using Groubel.Data;
using Groubel.Enums;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.BLL
{
    public class InterestsService
    {
        #region private methods

        private bool IsMain(int userId, int interestId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var count = db.UserInterests.Count(i => i.UserId == userId && i.InterestId == interestId);

                return count > 0;
            }
        }

        #endregion

        #region Geters

        public List<InterestEntity> GetRandomInterests(int count)
        {
            using (var db = new groubel_dbEntities1())
            {
                var interestType = (int)InterestTypeEnum.Special;
                var interests = db.Interests
                    .Where(i=>i.Type!= interestType)
                    .OrderBy(i => Guid.NewGuid())
                    .Skip(0)
                    .Take(count)
                    .Select(i => new InterestEntity
                    {
                        Id=i.Id,
                        Name=i.Name
                    }).ToList();

                return interests;
            }
        }

        public List<InterestEntity> GetUserInterests(int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var usInterestIds = db.UserInterests.Where(i=>i.UserId==userId).Select(i => i.InterestId).ToList();

                var tp = (int)NotificationTypeEnum.PostAddedOnInterest;

                var d = new List<InterestEntity>();

                d.Add(new InterestEntity {
                    Id = -3,
                    Name = "friends",
                    IsMain = true,
                    Posts = db.Notifications.Count(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == -3)

                });
                d.Add(new InterestEntity
                {
                    Id = -2,
                    Name = "nearby",
                    IsMain = true,
                    Posts = db.Notifications.Count(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == -2)

                });
                d.Add(new InterestEntity
                {
                    Id = -1,
                    Name = "anonimus",
                    IsMain = true,
                    Posts = db.Notifications.Count(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == -1)

                });

                var data = db.Interests.Where(i => usInterestIds.Contains(i.Id)).Select(b=>new InterestEntity {

                    Id = b.Id,
                    Name =b.Name,
                    IsMain=true,
                    Posts= db.Notifications.Count(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId ==b.Id),
                    
            }).ToList();

                d.AddRange(data);

                return d;
            }
        }

        public List<InterestEntity> GetInterestByPostId(int postId,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var data=db.Interests.Where(i=>db.PostInterests.Where(j=>j.PostId==postId).Select(j=>j.InterestId).Contains(i.Id)).Select(i => new InterestEntity
                {

                    Id = i.Id,
                    Name = i.Name,
                    IsMain = db.UserInterests.Count(t => t.UserId == userId && t.InterestId == i.Id)>0

                }).ToList();

                return data;
            }
        }

        public FilterInterestEntity GetFilterInterest(int id,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.Interests.FirstOrDefault(i => i.Id == id);

                if (item == null)
                {
                    if (id < 0)
                        return GetSpecialInterestFilter(id,userId);

                    return null;
                }
                    

                var posts = db.PostInterests.Where(i => i.InterestId == id).Select(i => i.PostId).Distinct().ToList();

                var date = DateTime.Now.AddDays(-1);

                var tp = (int)NotificationTypeEnum.PostAddedOnInterest;
                var newPosts = db.Notifications.Count(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId==id);

                SetSeen(db.Notifications.Where(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == id).ToList());

                var rt = new FilterInterestEntity
                {
                    Id = item.Id,
                    Name = item.Name,
                    AddDate = item.AddDate,
                    IsMain = db.UserInterests.Count(t => t.UserId == userId && t.InterestId == item.Id) > 0,
                    NewPostsNumber = newPosts,
                    PostNumber=posts.Count(),
                };

                return rt;

            }
        }

        private void SetSeen(List<Notification> items)
        {
            using (var db = new groubel_dbEntities1())
            {
                foreach (var it in items)
                {
                    var notItem = db.Notifications.FirstOrDefault(i => i.Id == it.Id);

                    if (notItem != null)
                    {
                        notItem.SeenStatus = true;
                        notItem.ApproovedStatus = true;
                    }
                }

                db.SaveChanges();
            }
        }

        public List<string> GetInterestsByName(string name)
        {
            using (var db = new groubel_dbEntities1())
            {
                var dt = db.Interests.Where(i => i.Name.StartsWith(name)).Select(i => i.Name).ToList();

                return dt;
            }
        }

        #endregion

        #region Add

        public void AddNewRegisteredUserInterests(int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var interestType = (int)InterestTypeEnum.Special;

                var inter = db.Interests
                        .Where(i => i.Type == interestType)
                        .ToList().Select(i => new UserInterest
                        {
                            InterestId = i.Id,
                            UserId = userId,
                            AddDate = DateTime.Now
                        }).ToList();

                db.UserInterests.AddRange(inter);

                db.SaveChanges();
            }
        }

        public void AddInterestsToUser(int userId, List<int> interestIds)
        {

            AddNewRegisteredUserInterests(userId);

            using (var db = new groubel_dbEntities1())
            {
                var list = new List<UserInterest>();

                foreach(var i in interestIds)
                {
                    list.Add(new UserInterest
                    {
                        InterestId=i,
                        UserId=userId,
                        AddDate=DateTime.Now
                    });
                }

                db.UserInterests.AddRange(list);

                db.SaveChanges();
            }
        }

        public List<InterestEntity> GetInterestObjectsByName(List<string> interests, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var interestObjects = new List<InterestEntity>();

                foreach(var inter in interests)
                {
                    var item = db.Interests.FirstOrDefault(i => i.Name == inter);

                    if (item != null)
                    {
                        interestObjects.Add(new InterestEntity {
                            Id = item.Id,
                            Name = item.Name,
                            UserId = item.UserId
                        });
                    }
                    else
                    {
                        var newInt = new Interest
                        {
                            Name = inter,
                            UserId = userId,
                            AddDate=DateTime.Now
                        };

                        db.Interests.Add(newInt);

                        db.SaveChanges();

                        db.UserInterests.Add(new UserInterest {
                            UserId=userId,
                            InterestId=newInt.Id,
                            AddDate=DateTime.Now
                        });

                        db.SaveChanges();

                        interestObjects.Add(new InterestEntity
                        {
                            Id = newInt.Id,
                            Name = newInt.Name,
                            UserId = newInt.UserId
                        });
                    }
                }

                return interestObjects;
            }
        }

        public bool AddToMyInterests(int userId, int interestId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.UserInterests.FirstOrDefault(i => i.InterestId == interestId && i.UserId==userId);

                if (item != null)
                {
                    db.UserInterests.Remove(item);

                    db.SaveChanges();

                    return false;
                }
                    

                db.UserInterests.Add(new UserInterest
                {
                    UserId=userId,
                    InterestId=interestId,
                    AddDate=DateTime.Now
                });

                db.SaveChanges();

                return true;
            }
        }

        public bool RemoveFromMyInterests(int userId, int interestId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.UserInterests.FirstOrDefault(i => i.InterestId == interestId && i.UserId == userId);

                if (item == null)
                    return false;

                db.UserInterests.Remove(item);

                db.SaveChanges();

                return true;
            }
        }

        #endregion

        #region SpeciaInterest Section
        private FilterInterestEntity GetSpecialInterestFilter(int id, int userId)
        {

            var name = (id == -3 ? "friends" : (id == -2 ? "nearby" : "anonymous "));

            var newPostsCount = 0;
            var allPostCount = 0;

            if (id == -3)
                NewPostsCountByFriends(id, userId, out newPostsCount, out allPostCount);
            else if(id==-2)
                NewPostsCountByNearby(id, userId, out newPostsCount, out allPostCount);
            else if(id==-1)
                NewPostsCountByAnonimus(id, userId, out newPostsCount, out allPostCount);

            var rt = new FilterInterestEntity
            {
                Id = id,
                Name = name,
                AddDate = DateTime.Now,
                IsMain = false,
                NewPostsNumber = newPostsCount,
                PostNumber = allPostCount,
            };

            return rt;
        }

        private void NewPostsCountByFriends(int id, int userId, out int newPostsCount, out int allPostCount)
        {
            using (var db = new groubel_dbEntities1())
            {

                allPostCount = db.Posts
                                 .Count(i => 
                                            db.Friendships
                                                         .Where(j => j.UserId == userId)
                                                         .Select(j => j.FriendId)
                                                         .Contains(i.UserId)
                                        );

                var tp = (int)NotificationTypeEnum.PostAddedOnInterest;

                var up = db.Notifications.Where(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == id).ToList();

                newPostsCount = up.Count();

                up.ForEach(a => { a.SeenStatus = true;a.ApproovedStatus = true; });

                db.SaveChanges();
            }
        }

        private void NewPostsCountByNearby(int id, int userId, out int newPostsCount, out int allPostCount)
        {
            using (var db = new groubel_dbEntities1())
            {

                var userIp = db.Users.FirstOrDefault(i => i.Id == userId).LastLoginIp;

                var ip = userIp;

                if (ip.Split('.').Count() > 2)
                {
                    var t = ip.Split('.');

                    ip = t[0] + "." + t[1] + "." + t[2];

                }

                allPostCount = db.Posts.Count(i => i.Ip.StartsWith(ip));

                var tp = (int)NotificationTypeEnum.PostAddedOnInterest;

                var up = db.Notifications.Where(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == id).ToList();

                newPostsCount = up.Count();

                up.ForEach(a => { a.SeenStatus = true; a.ApproovedStatus = true; });

                db.SaveChanges();

            }
        }

        private void NewPostsCountByAnonimus(int id, int userId, out int newPostsCount, out int allPostCount)
        {
            using (var db = new groubel_dbEntities1())
            {
                allPostCount = db.Posts.Count(i =>i.Anonoimus);

                var tp = (int)NotificationTypeEnum.PostAddedOnInterest;

                var up = db.Notifications.Where(i => i.Type == tp && i.ReciverUserId == userId && i.SeenStatus == false && i.AliasId == id).ToList();

                newPostsCount = up.Count();

                up.ForEach(a => { a.SeenStatus = true; a.ApproovedStatus = true; });

                db.SaveChanges();
            }
        }

        #endregion
    }
}
