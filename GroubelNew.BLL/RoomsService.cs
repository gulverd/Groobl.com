using Groubel.Data;
using Groubel.Helpers;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;
using Groubel.Enums;
using System.Data.Entity;

namespace GroubelNew.BLL
{
    public class RoomsService
    {
        InterestsService _interestService;
        SecurityService _securityService;
        UserService _userService;
        NotificationService _notificationService;

        public RoomsService(InterestsService intser, SecurityService sc, UserService us, NotificationService nt)
        {
            _interestService = intser;
            _securityService = sc;
            _userService = us;
            _notificationService = nt;
        }

        public string UploadImage(HttpPostedFileBase file)
        {
            var upload = new IOHandler("~/Uploads/RoomFiles/");

            var name = upload.Save(file);

            return name;
        }

        public ChatEntity AddRoom(ChatEntity chat, HttpPostedFileBase file)
        {
            using (var db = new groubel_dbEntities1())
            {
              //  var upload = new IOHandler("~/Uploads/RoomFile/");

              //  var name = upload.Save(file);

                using (var transaction = db.Database.BeginTransaction())
                {

                    try
                    {
                        var newChat = new Chat
                        {
                            Image = chat.Image,
                            Anonimus = chat.Anonimus,
                            IsArchive = false,
                            IsRoom = chat.IsRoom,
                            Date = DateTime.Now,
                            MaxUsers = chat.MaxUsers,
                            Name = chat.Name,
                            Visibility = chat.Visibility,
                            UserId = chat.UserId
                        };

                        db.Chats.Add(newChat);

                        db.SaveChanges();

                        var roomInterests = _interestService.GetInterestObjectsByName(chat.Interests.Select(i => i.Name).ToList(), chat.UserId);

                        foreach (var intere in roomInterests)
                        {
                            db.ChatInterests.Add(new ChatInterest
                            {
                                ChatId = newChat.Id,
                                InterestId = intere.Id
                            });
                        }

                        db.SaveChanges();

                        db.ChatMembers.Add(new ChatMember
                        {
                            UserId = chat.UserId,
                            RoomId = newChat.Id,
                            Date = DateTime.Now,
                            Agree = true,
                            SenderId = chat.UserId,
                            Status = 1,
                            Avatar = "",
                        });

                        foreach (var us in chat.Members)
                        {
                            db.ChatMembers.Add(new ChatMember
                            {
                                UserId = us.Id,
                                RoomId = newChat.Id,
                                Date = DateTime.Now,
                                Agree = true,
                                SenderId = chat.UserId,
                                Status = 1,
                                Avatar="",
                            });

                        }

                        db.SaveChanges();

                        _notificationService.AddNotification(chat.UserId, 0, NotificationTypeEnum.AddedYouInRoom, newChat.Id, "", null);

                        transaction.Commit();

                        chat.Id = newChat.Id;
                        chat.Interests = roomInterests;
                        chat.Members = GetChatMembers(chat.Id, chat.UserId);
                        chat.Comments = new List<ChatCommentEntity>();

                        return chat;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return null;
                    }
                }

            }
        }

        public bool DeleteChat(int userId, int Id)
        {
            using (var db = new groubel_dbEntities1())
            {


                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var chat = db.Chats.FirstOrDefault(i => i.UserId == userId && i.Id == Id);

                        if (chat == null)
                            return false;

                        var members = db.ChatMembers.Where(i => i.RoomId == Id);
                        var interests = db.ChatInterests.Where(i => i.ChatId == Id);
                        var comments = db.ChatComments.Where(i => i.RoomId == Id);

                        db.ChatComments.RemoveRange(comments);
                        db.ChatInterests.RemoveRange(interests);
                        db.ChatMembers.RemoveRange(members);

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

        public List<ChatEntity> GetUserRooms(int userId, int mainUserId, bool isRoom)
        {
            using (var db = new groubel_dbEntities1())
            {

                var t = DateTime.Now.AddDays(-100);

                var rooms = db.Chats.Where(i =>
                  db.ChatMembers.Where(j => j.UserId == userId).Select(j => j.RoomId).Contains(i.Id)
                  && i.IsRoom == isRoom
                  && !i.IsArchive
                )
                .OrderByDescending(
                    i => db.ChatComments.Any(j => j.RoomId == i.Id) ? db.ChatComments.Where(j => j.RoomId == i.Id).OrderByDescending(j => j.Id).FirstOrDefault().Date : i.Date
                    )
                    .ToList()
                    .Select(i => new ChatEntity
                    {
                        Id = i.Id,
                        Name = i.Name,
                        LastCommentDate = db.ChatComments.Any(j => j.RoomId == i.Id) ? db.ChatComments.Where(j => j.RoomId == i.Id).OrderByDescending(j => j.Id).FirstOrDefault().Date.ToString("g") : i.Date.ToString("g"),
                        AreMember = db.ChatMembers.Any(j => j.UserId == mainUserId),
                        UnSeenCmments = _notificationService.GetChatCommentStatus(userId, i.Id),
                        Image = i.Image,
                        Visibility = i.Visibility,
                        UserId=i.UserId,
                        UserImage=getUserImage(userId, db.ChatMembers.Where(k=>k.RoomId==i.Id).ToList(),i.IsRoom, db),
                        AnotherUserId= i.IsRoom ?0 : db.ChatMembers.Where(j => j.RoomId == i.Id && j.UserId != userId).Select(b => b.UserId).FirstOrDefault(),
                        MemberName = db.ChatMembers.Where(j => j.RoomId == i.Id).Select(b => db.Users.FirstOrDefault(j => j.Id == b.UserId).FirstName + " " + db.Users.FirstOrDefault(j => j.Id == b.UserId).LastName).ToList(),
                        UserName=i.IsRoom?"":db.ChatMembers.Where(j => j.RoomId == i.Id && j.UserId!=userId).Select(b => db.Users.FirstOrDefault(j => j.Id == b.UserId).FirstName + " " + db.Users.FirstOrDefault(j => j.Id == b.UserId).LastName).FirstOrDefault()
                    }).ToList();

                return rooms;
            }
        }

        private string getUserImage(int userId, List<ChatMember> chatMembers, bool isRoom, groubel_dbEntities1 db)
        {
            if (isRoom)
                return "";

            var frId=chatMembers.Where(i => i.UserId != userId).Select(i => i.UserId).FirstOrDefault();

            var secImage = db.UserImages.FirstOrDefault(k => k.UserId == frId);

             var img = "";

            if (secImage != null)
                return secImage.Image;

            return img;
        }

        public List<ChatEntity> SearchRooms(string name, int mainUserId, bool isRoom)
        {
            using (var db = new groubel_dbEntities1())
            {
                var rooms = db.Chats.Where(i =>
                  db.ChatMembers.Where(j => i.Name.StartsWith(name)).Select(j => j.RoomId).Contains(i.Id)
                  && i.IsRoom == isRoom
                )
                    .ToList()
                    .Select(i => new ChatEntity
                    {
                        Name = i.Name,
                        LastCommentDate = "",
                        AreMember = db.ChatMembers.Any(j => j.UserId == mainUserId),
                        UnSeenCmments =0,
                        Image=i.Image,
                        Id=i.Id,
                        Visibility=i.Visibility
                    }).ToList();

                return rooms;
            }
        }

        public ChatEntity GetRoomById(int id, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var rm = db.Chats.Where(i => i.Id == id).Select(i => new ChatEntity
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    Date = i.Date,
                    IsArchive = i.IsArchive,
                    IsRoom = i.IsRoom,
                    MaxUsers = i.MaxUsers,
                    Image = i.Image,
                    Name = i.Name,
                    AreMember = db.ChatMembers.Any(j => j.UserId == userId && j.RoomId==id),
                    Visibility = i.Visibility,
                    RequestSent=db.Notifications.Any(t=>t.ApproovedStatus && t.AliasId==i.Id && t.SenderUserId==userId)
                }).FirstOrDefault();

                rm.Members= GetChatMembers(rm.Id, userId);
                rm.Comments = GetChatComment(rm.Id, userId, 0);
                rm.Interests = db.Interests.Where(
                    i => db.ChatInterests.Where(j => j.ChatId == rm.Id).Select(j=> j.InterestId).Contains(i.Id)
                    ).Select(i => new InterestEntity {
                        Id=i.Id,
                        Name=i.Name
                    }).ToList();


                return rm;

            }
        }

        public void RequestAddToRoom(int userId,int adminId,int roomId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var type = (int)NotificationTypeEnum.RequestedAddToRoom;
                db.Notifications.Add(new Notification
                {
                    SenderUserId=userId,
                    ReciverUserId=adminId,
                    Date=DateTime.Now,
                    AliasId=roomId,
                    ApproovedStatus=false,
                    SeenStatus=false,
                    Type=type
                });

                db.SaveChanges();
            }
        }

        #region Comments

        public ChatCommentEntity AddComment(ChatCommentEntity comment, HttpPostedFileBase file)
        {
            using (var db = new groubel_dbEntities1())
            {
                var upload = new IOHandler("~/Uploads/RoomFiles/");

                var name = upload.Save(file);

                var newComm = new ChatComment
                {
                    Attachement = name,
                    Date = DateTime.Now,
                    RoomId = comment.RoomId,
                    Text = comment.Text,
                    UserId = comment.UserId,
                    Image = name,
                };

                db.ChatComments.Add(newComm);

                var rm = db.Chats.FirstOrDefault(i => i.Id == comment.RoomId && i.IsRoom == false);
                if (rm != null)
                {
                    rm.IsArchive = false;
                }

                db.SaveChanges();

                comment.Id = newComm.Id;
                comment.Attachement = name;
                comment.Image = name;
                comment.Date = DateTime.Now;
                comment.User = _userService.GetUserById(comment.UserId, comment.UserId);

                if (db.Chats.FirstOrDefault(i => i.Id == comment.RoomId).IsRoom)
                    _notificationService.AddNotification(comment.UserId, 0, NotificationTypeEnum.CommentedInRoom, comment.RoomId, "", null);
                else
                    _notificationService.AddNotification(comment.UserId, 0, NotificationTypeEnum.SendMessageInChat, comment.RoomId, "", null);

                return comment;

            }
        }

        public bool DeleteComment(int userId, int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.ChatComments.FirstOrDefault(i => i.UserId == userId && i.Id == id);

                if (item == null)
                    return false;

                db.ChatComments.Remove(item);

                db.SaveChanges();

                return true;

            }
        }

        public List<ChatCommentEntity> GetLastComments(int chatId, int lastCommentId,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var d = db.ChatComments.Where(i => i.Id > lastCommentId && i.RoomId == chatId).ToList().Select(i => new ChatCommentEntity
                {
                    Attachement = i.Attachement,
                    Date = i.Date,
                    RoomId = i.RoomId,
                    Text = i.Text,
                    UserId = i.UserId,
                    Image = i.Image,
                    Id = i.Id,
                    User = _userService.GetUserById(i.UserId, i.UserId)
                }).ToList();

             //   var members = db.ChatMembers.Where(i => i.RoomId == chatId).Select(i => i.UserId);
                _notificationService.setChatCommentStatus(userId, chatId);

                return d;
            }
        }

        public Dictionary<string, int> GetLatestCommentsStatus(Dictionary<int, int> chats)
        {
            using (var db = new groubel_dbEntities1())
            {
                Dictionary<string, int> data = new Dictionary<string, int>();

                foreach (var chat in chats)
                {
                    var count = db.ChatComments.Count(i => i.Id > chat.Value && i.RoomId == chat.Key);

                    data.Add(chat.Key.ToString(), count);
                }

                return data;
            }
        }

        public List<ChatCommentEntity> GetChatComment(int chatId, int userId, int lastId)
        {
            using (var db = new groubel_dbEntities1())
            {
                _notificationService.setChatCommentStatus(userId, chatId);

                List<ChatCommentEntity> d;
                if (lastId == 0)
                {
                    d = db.ChatComments.Where(i => i.RoomId == chatId).OrderBy(i=>i.Id).ToList().Select(i => new ChatCommentEntity
                    {
                        Attachement = i.Attachement,
                        Date = i.Date,
                        RoomId = i.RoomId,
                        Text = i.Text,
                        UserId = i.UserId,
                        Image = i.Image,
                        Id = i.Id,
                        User= _userService.GetUserById(i.UserId, i.UserId)
                }).ToList();
                }
                else {
                    d = db.ChatComments.Where(i => i.RoomId == chatId && i.Id<lastId).Skip(0).Take(20).OrderBy(i=>i.Id).ToList().Select(i => new ChatCommentEntity
                    {
                        Attachement = i.Attachement,
                        Date = i.Date,
                        RoomId = i.RoomId,
                        Text = i.Text,
                        UserId = i.UserId,
                        Image = i.Image,
                        Id = i.Id,
                        User = _userService.GetUserById(i.UserId, i.UserId)
                    }).ToList();
                }

                return d;
            }
        }

        #endregion

        #region Members

        public bool AddUser(int chatId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userCount = db.ChatMembers.Count(i => i.RoomId == chatId);

                var room = db.Chats.FirstOrDefault(i => i.Id == chatId);

                if (room == null)
                    return false;

                var isMember = db.ChatMembers.Any(i => i.UserId == userId && i.RoomId == chatId);

                if (isMember)
                    return false;

                if (userCount >= room.MaxUsers)
                    return false;

                var notification = db.Notifications.FirstOrDefault(i => i.AliasId == chatId && i.SenderUserId == userId);

                if (notification != null)
                {
                    notification.ApproovedStatus = true;
                    db.SaveChanges();
                }

                db.ChatMembers.Add(new ChatMember {
                    RoomId = chatId,
                    SenderId = userId,
                    UserId = userId,
                    Date = DateTime.Now,
                    Agree = true,
                    Avatar = ""
                });

                db.SaveChanges();

                return true;
            }
        }

        public bool DeleteUser(int chatId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var u = db.ChatMembers.FirstOrDefault(i => i.RoomId == chatId && i.UserId == userId);

                if (u == null)
                    return false;

                db.ChatMembers.Remove(u);

                db.SaveChanges();

                return true;
            }
        }

        public bool ActivateUser(int adminUserId, int chatId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var isAdmin = db.Chats.Any(i => i.UserId == adminUserId && i.Id == chatId);

                if (!isAdmin)
                    return false;

                var u = db.ChatMembers.FirstOrDefault(i => i.RoomId == chatId && i.UserId == userId);

                if (u == null)
                    return false;

                u.Status = 1;

                db.SaveChanges();

                _notificationService.AddNotification(adminUserId, userId, NotificationTypeEnum.ChangedYouStatusInRoom, u.RoomId, "", null);

                return true;
            }
        }

        public bool HiseUser(int adminUserId, int chatId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var isAdmin = db.Chats.Any(i => i.UserId == adminUserId && i.Id == chatId);

                if (!isAdmin)
                    return false;

                var u = db.ChatMembers.FirstOrDefault(i => i.RoomId == chatId && i.UserId == userId);

                if (u == null)
                    return false;

                u.Status = 2;

                db.SaveChanges();

                _notificationService.AddNotification(adminUserId, userId, NotificationTypeEnum.ChangedYouStatusInRoom, u.RoomId, "", null);

                return true;
            }
        }

        public bool MuteUser(int adminUserId, int chatId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var isAdmin = db.Chats.Any(i => i.UserId == adminUserId && i.Id == chatId);

                if (!isAdmin)
                    return false;

                var u = db.ChatMembers.FirstOrDefault(i => i.RoomId == chatId && i.UserId == userId);

                if (u == null)
                    return false;

                u.Status = 3;

                db.SaveChanges();

                _notificationService.AddNotification(adminUserId, userId, NotificationTypeEnum.ChangedYouStatusInRoom, u.RoomId, "", null);

                return true;
            }
        }

        public List<ChatMemberEntity> GetChatMembers(int id, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var data = db.Users
                    .Where(i => db.ChatMembers
                                  .Where(j => j.RoomId == id)
                                  .Select(j => j.UserId)
                                  .Contains(i.Id)
                                  ).ToList()
                    .Select(i => new ChatMemberEntity
                    {
                        Id = i.Id,
                        FirstName = i.FirstName,
                        LastName = i.LastName,

                        LastLoginDate = i.LastLoginDate,

                        DateOfBirth = i.DateOfBirth,
                        Gender = i.Gender,
                        IsMe = i.Id == userId,

                        Image = _userService.GetUserImage(i.Id),
                        IsOnline = _securityService.IsOnline(i.Id),
                        IsFriend = _userService.IsFriend(userId, i.Id),
                        Status = db.ChatMembers.FirstOrDefault(j => j.UserId == i.Id && j.RoomId == id).Status
                    }).ToList();

                return data;
            }
        }

        #endregion

        #region Attachements

        public Dictionary<string, List<string>> GetAttachements(int roomId, bool isImage)
        {
            using (var db = new groubel_dbEntities1())
            {
                var extArray = new List<string> { "jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" };

                var data = db.ChatComments.Where(i => i.Attachement != "" && i.RoomId== roomId).Select(i => i.Attachement).ToList();

                List<string> filteredData;
                if (isImage)
                    filteredData = data.Where(i => extArray.Contains(i.Substring(i.Length - 3))).ToList();
                else
                    filteredData = data.Where(i => !extArray.Contains(i.Substring(i.Length - 3))).ToList();

                var b = filteredData.GroupBy(i => i.Split('_')[1].First().ToString().ToUpper()).ToDictionary(i => i.Key, i => i.ToList());

                return b;

            }
        }

        #endregion

        //Chats

        public void CloseChat(int id)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.Chats.FirstOrDefault(i => i.Id == id);

                if (item == null)
                    return;

                item.IsArchive = true;

                db.SaveChanges();

            }
        }

        public ChatEntity OpenChat(int userId, int secondUsrId)
        {
            using (var db = new groubel_dbEntities1())
            {

                using (var transaction = db.Database.BeginTransaction())
                {

                    try
                    {

                        var itmIds = db.ChatMembers
                                         .Where(i => i.UserId == userId || i.UserId == secondUsrId)
                                         .GroupBy(i => i.RoomId)
                                         .Where(i => i.Count() == 2)
                                         .Select(i => i.Key);

                        var chats = db.Chats.FirstOrDefault(i => itmIds.Contains(i.Id) && i.IsRoom == false);

                        if (chats != null)
                        {
                            chats.IsArchive = false;

                            db.SaveChanges();

                            var membersT = GetChatMembers(chats.Id, userId);

                            var secImage = db.UserImages.FirstOrDefault(k => k.UserId == secondUsrId);

                            var img = "";

                            if (secImage != null)
                                img = secImage.Image;

                            var returnChat = new ChatEntity
                            {
                                Id = chats.Id,
                                UserId = chats.UserId,
                                Date = chats.Date,
                                IsArchive = chats.IsArchive,
                                IsRoom = chats.IsRoom,
                                MaxUsers = chats.MaxUsers,
                                Image = chats.Image,
                                Name = chats.Name,
                                AreMember = true,
                                Visibility = chats.Visibility,
                                RequestSent = true,
                                Members= membersT,
                                MemberName = membersT.Select(i=>i.FirstName+" "+ i.LastName).ToList(),
                                Comments = GetChatComment(chats.Id, userId, 0),
                                UserImage=img
                            };

                            transaction.Commit();

                            return returnChat;
                        }

                        var newChat = new Chat
                        {
                            Image = "",
                            Anonimus = false,
                            IsArchive = false,
                            IsRoom = false,
                            Date = DateTime.Now,
                            MaxUsers = 2,
                            Name = "cat",
                            Visibility = true,
                            UserId = userId
                        };

                        db.Chats.Add(newChat);

                        db.SaveChanges();

                        db.ChatMembers.Add(new ChatMember
                        {
                            UserId = userId,
                            RoomId = newChat.Id,
                            Date = DateTime.Now,
                            Agree = true,
                            SenderId = userId,
                            Status = 1,
                            Avatar = "",
                        });

                        db.ChatMembers.Add(new ChatMember
                        {
                            UserId = secondUsrId,
                            RoomId = newChat.Id,
                            Date = DateTime.Now,
                            Agree = true,
                            SenderId = userId,
                            Status = 1,
                            Avatar = "",
                        });



                        db.SaveChanges();

                        transaction.Commit();

                        var members = GetChatMembers(newChat.Id, newChat.UserId);
                        var ct = new ChatEntity
                        {
                            Id = newChat.Id,
                            Members=members,
                            MemberName = members.Select(i => i.FirstName + " " + i.LastName).ToList(),
                            Comments = new List<ChatCommentEntity>()
                        };

                        return ct;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return null;
                    }
                }

            }
        }


        public List<UserEntity> GetUserSuggestions(int userId,int count)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userInterests = db.UserInterests.Where(i => i.UserId == userId).Select(i => i.InterestId);

                var friends = db.Friendships
                         .Where(i =>
                         i.UserId == userId &&
                         db.UserInterests
                             .Where(j => j.UserId == i.FriendId)
                             .Select(j => j.InterestId)
                             .Intersect(userInterests).Any()
                           )
                           .Select(i => i.FriendId);

                var users = db.Users.Where(i => friends.Contains(i.Id)).OrderBy(i=>i.Id).Skip(0).Take(count).Select(i => new UserEntity {
                    Id = i.Id,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    IsFriend = true
                }).ToList();

                return users;
            }
        }
    }
}
