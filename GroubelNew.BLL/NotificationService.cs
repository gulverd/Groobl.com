using Groubel.Data;
using Groubel.Enums;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.BLL
{
    public class NotificationService
    {

        public int GetChatCommentStatus(int userId, int roomId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item =  db.Notifications.Count(i => i.ReciverUserId == userId && i.AliasId == roomId && !i.SeenStatus);

                return item;

            }
        }

        public void setChatCommentStatus(int userId, int roomId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var items = db.Notifications.Where(i => i.ReciverUserId == userId && i.AliasId == roomId && !i.SeenStatus).ToList();

                SetSeen(items);


            }
        }

        public Tuple<int, NotifictionEntity> GetUserNotificationsCount(int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var count =  db.Notifications.Count(i => i.ReciverUserId == userId && !i.SeenStatus);

                var lastItem =  db.Notifications.Where(i => i.ReciverUserId == userId && i.SenderUserId!=userId).OrderByDescending(i => i.Id).Skip(0).Take(1).ToList().Select(i => new NotifictionEntity
                {
                    Id = i.Id,
                    Date = i.Date,
                    ReciverUserId = i.ReciverUserId,
                    SenderUserId = i.SenderUserId,
                    AliasId = i.AliasId,
                    Type = (NotificationTypeEnum)i.Type,
                    SeenStatus = i.SeenStatus,
                    ApproovedStatus = i.ApproovedStatus,
                    AliasName = GetAliasName(i.Type, i.AliasId),
                    SenderUser = GetUserById(i.SenderUserId, userId, (NotificationTypeEnum)i.Type, i.Anonimus),
                  //  ReciverUser = GetUserById(i.ReciverUserId, userId),
                }).GroupBy(i=>new { i.Type,i.AliasId,i.ReciverUserId}).Select(i=>i.FirstOrDefault()).ToList();

                NotifictionEntity item = null;

                if(lastItem.Count()>0)
                    item = lastItem.FirstOrDefault();

                //var lastThree = new List<NotifictionEntity>();

                //if (item != null)
                //{
                //    var type = item.Type;
                //    var aliasId = item.AliasId;

                //    var dt = db.Notifications.Where(i => i.AliasId == aliasId && i.Type == type).OrderByDescending(i => i.Id).Skip(0).Take(3).ToList();

                //    lastThree = dt.Select(i => new NotifictionEntity
                //    {
                //        Id = i.Id,
                //        Date = i.Date,
                //        ReciverUserId = i.ReciverUserId,
                //        SenderUserId = i.SenderUserId,
                //        AliasId = i.AliasId,
                //        Type = (NotificationTypeEnum)i.Type,
                //        SeenStatus = i.SeenStatus,
                //        ApproovedStatus = i.ApproovedStatus,
                //        AliasName = GetAliasName(i.Type, i.AliasId),
                //        SenderUser = GetUserById(i.SenderUserId, userId),
                //        ReciverUser = GetUserById(i.ReciverUserId, userId),
                //    }).ToList();

                //}

                var data = Tuple.Create(count, item);

                return data;

            }
        }

        public List<NotifictionEntity> GetUserNotifications(int userId, int lastId, int nextId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var items = new List<Notification>();

                if (lastId == 0 && nextId == 0)
                    items = db.Notifications.Where(i => i.ReciverUserId == userId && i.SenderUserId != userId).OrderByDescending(i => i.Id).Skip(0).Take(10).ToList();
                else if (lastId != 0)
                    items = db.Notifications.Where(i => i.ReciverUserId == userId && i.SenderUserId != userId && i.Id > lastId).OrderByDescending(i => i.Id).Skip(0).Take(10).ToList();
                else if (lastId != 0)
                    items = db.Notifications.Where(i => i.ReciverUserId == userId && i.SenderUserId != userId && i.Id < nextId).OrderByDescending(i => i.Id).Skip(0).Take(10).ToList();

                SetSeen(db.Notifications.Where(i => i.ReciverUserId == userId && i.SeenStatus==false).ToList());

                var data = items.Select(i => new NotifictionEntity
                {
                    Id = i.Id,
                    Date = i.Date,
                    ReciverUserId = i.ReciverUserId,
                    SenderUserId = i.SenderUserId,
                    AliasId = i.AliasId,
                    Type = (NotificationTypeEnum)i.Type,
                    SeenStatus = i.SeenStatus,
                    ApproovedStatus = i.ApproovedStatus,
                    AliasName = GetAliasName(i.Type, i.AliasId),
                    SenderUser = GetUserById(i.SenderUserId, userId, (NotificationTypeEnum)i.Type, i.Anonimus),
                   // ReciverUser = GetUserById(i.ReciverUserId, userId),
                }).GroupBy(i => new { i.Type, i.AliasId, i.ReciverUserId }).Select(i => i.FirstOrDefault()).ToList();

                return data;

            }
        }

        private UserEntity GetUserById(int id, int curId, NotificationTypeEnum type, bool? anonimus)
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
                    Image = GetUserImage(us.Id),

                };

                if (anonimus!=null && Convert.ToBoolean(anonimus))
                {

                    rt.FirstName = "someone";
                    rt.LastName = "";
                    rt.Image="";

                }

                return rt;
            }
        }

        private string GetUserImage(int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var us = db.UserImages.FirstOrDefault(i => i.UserId == id);

                if (us == null)
                    return "";

                return us.Image;
            }

        }

        private void SetSeen(List<Notification> items)
        {
            using (var db = new groubel_dbEntities1())
            {
                foreach(var it in items)
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

        private string GetAliasName(int type, int aliasId)
        {
            if(type>=5 && type<=10 && type !=8)
            {
                using (var db = new groubel_dbEntities1())
                {
                    var it = db.Chats.FirstOrDefault(i => i.Id == aliasId);

                    if (it == null)
                        return "";

                    return it.Name;
                }
            }
            if (type == 1)
            {
                using (var db = new groubel_dbEntities1())
                {
                    var it = db.Interests.FirstOrDefault(i => i.Id == aliasId);

                    if (it == null)
                        return "";

                    return it.Name;
                }
            }
                return "";
        }

        private void SaveNotification(int senderUserId, List<int> reciverUserIds, int type, int aliasId, bool? anonumus)
        {
            Task.Factory.StartNew(() =>
            {

                using (var db = new groubel_dbEntities1())
                {
                    foreach (var reciverUserId in reciverUserIds)
                    {

                        db.Notifications.Add(new Notification {
                            SenderUserId = senderUserId,
                            ReciverUserId = reciverUserId,
                            Date = DateTime.Now,
                            AliasId = aliasId,
                            Type = type,
                            SeenStatus = senderUserId == reciverUserId,
                            Anonimus=anonumus,
                            ApproovedStatus = senderUserId == reciverUserId
                        });

                    }

                    db.SaveChanges();
                }

            });
        }
        private async void NotifyPostAddedOnInterest(int senderUserId, NotificationTypeEnum type, int aliasId, bool? anonimus)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await  db.UserInterests
                                     .Where(i =>
                                               db.PostInterests
                                                     .Where(j=>j.PostId==aliasId)
                                                     .Select(j=>j.InterestId)
                                                     .Contains(i.InterestId)
                                               )
                                     .Select(i => i.UserId)
                                     .ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId, anonimus);
            }
        }
        private async void NotifyCmmentedOrLikedOnYourPost(int senderUserId, NotificationTypeEnum type, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await db.Posts.Where(i => i.Id == aliasId).Select(i => i.UserId).ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId, null);
            }
        }
        private async void NotifyAddedYouInRoom(int senderUserId, NotificationTypeEnum type, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await db.ChatMembers.Where(i => i.RoomId == aliasId).Select(i => i.UserId).ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId,null);
            }
        }
        private async void NotifyCommentedInRoom(int senderUserId, NotificationTypeEnum type, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await db.ChatMembers.Where(i => i.RoomId == aliasId).Select(i => i.UserId).ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId,null);
            }
        }
        private void NotifyChangedYouStatusInRoom(int senderUserId, int reciverUserId, NotificationTypeEnum type, int aliasId)
        {
            List<int> userIds = new List<int>();
            userIds.Add(reciverUserId);

            SaveNotification(senderUserId, userIds, (int)type, aliasId,null);

        }
        private async void NotifyRequestedAddToRoom(int senderUserId, NotificationTypeEnum type, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await db.Chats.Where(i => i.Id == aliasId).Select(i => i.UserId).ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId,null);
            }
        }
        private async void NotifySendMessageInChat(int senderUserId, NotificationTypeEnum type, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = await db.ChatMembers.Where(i=>i.RoomId==aliasId && i.UserId!=senderUserId).Select(i => i.UserId).ToListAsync();

                SaveNotification(senderUserId, userIds, (int)type, aliasId,null);
            }
        }
        private void NotifyAddEdYouAsFriend(int senderUserId, int reciverUserId, NotificationTypeEnum type, int aliasId)
        {
            List<int> userIds = new List<int>();
            userIds.Add(reciverUserId);

            SaveNotification(senderUserId, userIds, (int)type, aliasId,null);
        }

        private void NotifyRequestAddFriend(int senderUserId, int reciverUserId, NotificationTypeEnum type)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = new List<int>();

                userIds.Add(reciverUserId);

                SaveNotification(senderUserId, userIds, (int)type, 0,null);
            }
        }

        public void AddNotification(int senderUserId, int reciverUserId, NotificationTypeEnum type, int aliasId, string ip, bool? anonimus)
        {

            Task.Factory.StartNew(() =>
            {


                switch (type)
                {
                    case NotificationTypeEnum.PostAddedOnInterest:
                        NotifyPostAddedOnInterest(senderUserId, type, aliasId, anonimus);
                        AddNotificationOnSpecialInterests(senderUserId, ip, anonimus, aliasId);
                        break;
                    case NotificationTypeEnum.CmmentedOnYourPost:
                        NotifyCmmentedOrLikedOnYourPost(senderUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.LikedYourPost:
                        NotifyCmmentedOrLikedOnYourPost(senderUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.SharedYourPost:
                        NotifyCmmentedOrLikedOnYourPost(senderUserId, type, aliasId);
                        break;

                    case NotificationTypeEnum.AddedYouInRoom:
                        NotifyAddedYouInRoom(senderUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.CommentedInRoom:
                        NotifyCommentedInRoom(senderUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.ChangedYouStatusInRoom:
                        NotifyChangedYouStatusInRoom(senderUserId, reciverUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.RequestedAddToRoom:
                        NotifyRequestedAddToRoom(senderUserId, type, aliasId);
                        break;

                    case NotificationTypeEnum.SendMessageInChat:
                        NotifySendMessageInChat(senderUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.AddEdYouAsFriend:
                        NotifyAddEdYouAsFriend(senderUserId, reciverUserId, type, aliasId);
                        break;
                    case NotificationTypeEnum.RequestAddFriend:
                        NotifyRequestAddFriend(senderUserId, reciverUserId, type);
                        break;
                        
                }

            });
        }


        public Dictionary<string, List<NotifictionEntity>> GetUserActivity(int userId, int lastId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var data = new Dictionary<string, List<NotifictionEntity>>();

                IQueryable<Notification> dt;
                if(lastId==0)
                     dt = db.Notifications.Where(i => i.SenderUserId == userId);
                else
                    dt = db.Notifications.Where(i => i.SenderUserId == userId && i.Id<0);

                var dd=dt.OrderByDescending(i => i.Date).Skip(0).Take(100).ToList().Select(i => new NotifictionEntity
                {
                    Id = i.Id,
                    Date = i.Date,
                    ReciverUserId = i.ReciverUserId,
                    SenderUserId = i.SenderUserId,
                    AliasId = i.AliasId,
                    Type = (NotificationTypeEnum)i.Type,
                    SeenStatus = i.SeenStatus,
                    ApproovedStatus = i.ApproovedStatus,
                    AliasName = GetAliasName(i.Type, i.AliasId),
                    SenderUser = GetUserById(i.SenderUserId, userId, (NotificationTypeEnum)i.Type, i.Anonimus),
                    ReciverUser = GetUserById(i.ReciverUserId, userId, (NotificationTypeEnum)i.Type, i.Anonimus),
                }).GroupBy(i => new { i.Type, i.AliasId, i.SenderUserId }).Select(i => i.FirstOrDefault()).ToList();

                data = dd.GroupBy(i => i.GroupDateString).ToDictionary(i => i.Key, i=>i.ToList());

                return data;
            }
        }

        public void SetApproved(int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var it = db.Notifications.FirstOrDefault(i => i.Id == id);

                if (it != null)
                {
                    it.ApproovedStatus = true;
                    db.SaveChanges();
                }
            }
        }

        public void SetApproved(int firstuserId, int secondUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var it = db.Notifications.FirstOrDefault(i => i.SenderUserId == firstuserId && i.ReciverUserId==secondUserId && i.Type==11);

                if (it != null)
                {
                    it.ApproovedStatus = true;
                    db.SaveChanges();
                }

                it = db.Notifications.FirstOrDefault(i => i.SenderUserId == secondUserId && i.ReciverUserId == firstuserId && i.Type == 11);

                if (it != null)
                {
                    it.ApproovedStatus = true;
                    db.SaveChanges();
                }
            }
        }

        private void AddNotificationOnSpecialInterests(int userId, string ip, bool? anonimus, int aliasId)
        {
            using (var db = new groubel_dbEntities1())
            {

                var type = NotificationTypeEnum.PostAddedOnInterest;

                //1
                var friends = db.Friendships
                                            .Where(j => j.UserId == userId)
                                            .Select(j => j.FriendId)
                                            .ToList();

                SaveNotification(userId, friends, (int)type, -3,anonimus);

                //2
                if (ip.Split('.').Count() > 2)
                {
                    var t = ip.Split('.');

                    ip = t[0] + "." + t[1] + "." + t[2];

                }

                friends = db.Users
                            .Where(i => i.LastLoginIp.StartsWith(ip) && i.Id!=userId)
                            .Select(j => j.Id)
                            .ToList();

                SaveNotification(userId, friends, (int)type, -2,anonimus);

                //3
                if (anonimus==true)
                {
                    friends = db.Users
                            .Select(j => j.Id)
                            .ToList();

                    SaveNotification(userId, friends, (int)type, -1,anonimus);
                }

            }
        }
    }
}
