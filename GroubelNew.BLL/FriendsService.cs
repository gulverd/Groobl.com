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
    public class FriendsService
    {

        SecurityService _securityService;
        NotificationService _notificationService;

        public FriendsService(SecurityService sc, NotificationService nt)
        {
            _securityService = sc;
            _notificationService = nt;
        }

        public bool IsFriend(int firstId, int secondId)
        {
            if (firstId == secondId)
                return false;

            using (var db = new groubel_dbEntities1())
            {
                var us = db.Friendships.FirstOrDefault(i => i.UserId == firstId && i.FriendId == secondId);

                return us != null;
            }
        }

        public bool AddFriend(int firstuserId, int secondUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Friendships.FirstOrDefault(i => i.UserId == firstuserId && i.FriendId == secondUserId);

                        if (item != null)
                            return false;

                        db.Friendships.Add(new Friendship
                        {
                            UserId = firstuserId,
                            FriendId = secondUserId,
                            Date = DateTime.Now,
                            Status = true
                        });

                        db.Friendships.Add(new Friendship
                        {
                            UserId = secondUserId,
                            FriendId = firstuserId,
                            Date = DateTime.Now,
                            Status = true
                        });

                        db.SaveChanges();

                        _notificationService.SetApproved(firstuserId, secondUserId);

                        _notificationService.AddNotification(firstuserId, secondUserId, NotificationTypeEnum.AddEdYouAsFriend, 0,"",null);

                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return false;
                    }

                }
            }
        }

        public bool RemoveFriend(int firstuserId, int secondUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Friendships.FirstOrDefault(i => i.UserId == firstuserId && i.FriendId == secondUserId);
                        var item2 = db.Friendships.FirstOrDefault(i => i.UserId == secondUserId && i.FriendId == firstuserId);

                        if (item == null)
                            return false;

                        db.Friendships.Remove(item);

                        db.Friendships.Remove(item2);

                        var st = new List<int> { 9, 11 };

                        var not = db.Notifications
                        .Where(i => i.SenderUserId == firstuserId && i.ReciverUserId == secondUserId && st.Contains(i.Type));
                        var not2= db.Notifications
                        .Where(i => i.SenderUserId == secondUserId && i.ReciverUserId == firstuserId && st.Contains(i.Type));


                            db.Notifications.RemoveRange(not);

                            db.Notifications.RemoveRange(not2);

                        db.SaveChanges();

                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return false;
                    }

                }
            }
        }

        public void SubmitFriendship(int firstuserId, int secondUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.Friendships.FirstOrDefault(i => i.UserId == firstuserId && i.FriendId == secondUserId);
                var item2 = db.Friendships.FirstOrDefault(i => i.UserId == secondUserId && i.FriendId == firstuserId);

                item.Status = true;
                item2.Status = true;

                db.SaveChanges();
            }
        }

        public List<UserEntity> GetFriends(int userId)
        {
            using (var db = new groubel_dbEntities1())
            {

                var friends = db.Users
                    .Where(i => db.Friendships
                                  .Where(j => j.UserId == userId)
                                  .Select(j => j.FriendId)
                                  .Contains(i.Id)).ToList()
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
                        IsFriend = IsFriend(i.Id, userId)
                    }).ToList();

                return friends;

            }
        }

        public List<UserEntity> GetFriendsByName(int userId, string name)
        {
            using (var db = new groubel_dbEntities1())
            {

                var friends = db.Users
                    .Where(i => db.Friendships
                                  .Where(j => j.UserId == userId)
                                  .Select(j => j.FriendId)
                                  .Contains(i.Id)

                                  && (i.FirstName.StartsWith(name) || i.LastName.StartsWith(name))

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
                        IsFriend = IsFriend(i.Id, userId)
                    }).ToList();

                return friends;

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

    }
}
