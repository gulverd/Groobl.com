using Groubel.Data;
using Groubel.Enums;
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
    public class PostsService
    {
        InterestsService _interestService;
        UserService _userService;
        NotificationService _notificationService;

        public PostsService(InterestsService inter, UserService us, NotificationService nt)
        {
            _interestService = inter;
            _userService = us;
            _notificationService = nt;
        }

        public PostEntity AddPost(PostEntity post, HttpPostedFileBase file)
        {
            using (var db = new groubel_dbEntities1())
            {

                var upload = new IOHandler("~/Uploads/PostAttachements/");

                var name = upload.Save(file);

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var newPost = new Post
                        {
                            Attachement = name,
                            Text = post.Text,
                            Date = DateTime.Now,
                            Anonoimus = post.Anonoimus,
                            Ip = post.Ip,
                            UserId = post.UserId,
                            Image="",
                            Video="",
                        };

                        db.Posts.Add(newPost);

                        db.SaveChanges();

                        var postInterests = _interestService.GetInterestObjectsByName(post.InterestNames, post.UserId);

                        foreach(var intere in postInterests)
                        {
                            db.PostInterests.Add(new PostInterest
                            {
                                PostId=newPost.Id,
                                InterestId=intere.Id
                            });

                            _notificationService.AddNotification(post.UserId, 0, NotificationTypeEnum.PostAddedOnInterest, intere.Id, newPost.Ip,newPost.Anonoimus);
                        }

                        db.SaveChanges();

                        transaction.Commit();

                        return new PostEntity
                        {
                            Id = newPost.Id,
                            Attachement = newPost.Attachement,
                            Text = newPost.Text,
                            Anonoimus = newPost.Anonoimus,
                            Ip = newPost.Ip,
                            UserId = newPost.UserId,
                            PostInterests = postInterests,
                            PostComments = new List<PostCommentEntity>(),
                            OldUser = null,
                            Rate = null,
                            User = _userService.GetUserById(newPost.UserId, newPost.UserId),
                            Saved = 0,
                            Date = DateTime.Now,
                            CommentsCount=0,
                            Reported=false,
                            PostLikers = GetPostLikers(newPost.Id, newPost.UserId)
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return null;
                    }
                }

            }

        }

        public bool DeletePost(int id,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var post = db.Posts.FirstOrDefault(i => i.Id == id && i.UserId==userId);

                        if (post == null)
                            return false;

                        var comments = db.PostComments.Where(i => i.PostId == id);
                        var interests=db.PostInterests.Where(i => i.PostId == id);

                        db.PostInterests.RemoveRange(interests);
                        db.PostComments.RemoveRange(comments);
                        db.Posts.Remove(post);

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

        public List<PostEntity> GetPostsByUserId(int userId,int postUserId, int start, int end)
        {
            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(i => i.UserId == userId && db.PostHides.Count(j=>j.PostId==i.Id && j.UserId==postUserId)==0)
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date=i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId==null? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), userId),
                        Rate = GetRate(i.Id, postUserId),
                        User=_userService.GetUserById(i.UserId,userId),
                        Saved=GetSaveStatus(i.Id,postUserId),
                        CommentsCount=db.PostComments.Count(j=>j.PostId==i.Id),
                        Reported=db.Reportings.Any(t=>t.UserId==postUserId && t.PostId==i.Id),
                        PostLikers = GetPostLikers(i.Id, userId)
                    }).ToList();

                return post;
            }
        }

        public List<PostEntity> GetAnonimusPostsByUserId(int userId, int postUserId, int start, int end)
        {
            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(i => i.UserId == userId && db.PostHides.Count(j => j.PostId == i.Id && j.UserId == postUserId) == 0 && i.Anonoimus)
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date = i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), userId),
                        Rate = GetRate(i.Id,postUserId),
                        User = _userService.GetUserById(i.UserId, userId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        PostLikers = GetPostLikers(i.Id, userId)
                    }).ToList();

                return post;
            }
        }

        public List<PostEntity> GetSavedPostsByUserId(int userId, int postUserId, int start, int end)
        {
            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(i => db.PostSaves.Where(j=>j.UserId==userId).Select(j=>j.PostId).Contains(i.Id) && db.PostHides.Count(j => j.PostId == i.Id && j.UserId == postUserId) == 0 )
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date = i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), userId),
                        Rate = GetRate(i.Id,postUserId),
                        User = _userService.GetUserById(i.UserId, userId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        PostLikers = GetPostLikers(i.Id, userId)
                    }).ToList();

                return post;
            }
        }

        public List<PostEntity> GetPostsByInterestId(int interestId, int postUserId, int start, int end)
        {
            if(interestId<0)
                return GetPostBySpecialInterest( interestId,  postUserId,  start,  end);

            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(j => db.PostInterests.Where(i=>i.InterestId==interestId).Select(i=>i.PostId).Distinct().Contains(j.Id) && db.PostHides.Count(i =>i.PostId == j.Id && i.UserId == postUserId) == 0)
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date=i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), postUserId),
                        Rate = GetRate(i.Id, postUserId),
                        User = _userService.GetUserById(i.UserId, postUserId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id),
                        PostLikers = GetPostLikers(i.Id, postUserId)
                    }).ToList();

                return post;
            }
        }

        public List<PostEntity> GetPostsByUserInterest(int userId, int start, int end)
        {
            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(j => 
                               db.PostInterests
                               .Where(i => 
                                        db.UserInterests
                                        .Where(t=>t.UserId==userId)
                                        .Select(t=>t.InterestId)
                                        .Contains(i.InterestId))
                               .Select(i => i.PostId)
                               .Distinct()
                               .Contains(j.Id)
                                && db.PostHides.Count(i=> i.PostId == j.Id && i.UserId == userId) == 0
                               )
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        Date=i.Date,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, userId),
                        PostComments = GetPostComments(i.Id, 0, 3, userId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), userId),
                        Rate = GetRate(i.Id,userId),
                        User = _userService.GetUserById(i.UserId, userId),
                        Saved = GetSaveStatus(i.Id, userId),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id),
                        Reported = db.Reportings.Any(t => t.UserId == userId && t.PostId == i.Id),
                        PostLikers = GetPostLikers(i.Id, userId)
                    }).ToList();

                return post;
            }
        }

        public PostEntity GetPostById(int userId,int postId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var post = db.Posts
                    .Where(j => j.Id == postId)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date=i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, userId),
                        PostComments = GetPostComments(i.Id, 0, 10, userId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), userId),
                        Rate = GetRate(i.Id,userId),
                        Saved = GetSaveStatus(i.Id, userId),
                        Reported = db.Reportings.Any(t => t.UserId == userId && t.PostId == i.Id),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id),
                        PostLikers= GetPostLikers(postId,userId),
                        User = _userService.GetUserById(Convert.ToInt32(i.UserId), userId)
                    }).FirstOrDefault() ;

                return post;
            }
        }

        #region Post Comments
        public PostCommentEntity AddComment(PostCommentEntity comment, HttpPostedFileBase file)
        {

            var upload = new IOHandler("~/Uploads/PostAttachements/");

            var name = upload.Save(file);

            using (var db = new groubel_dbEntities1())
            {
                var newComment = new PostComment
                {
                    Text = comment.Text,
                    UserId = comment.UserId,
                    Date = DateTime.Now,
                    PostId = comment.PostId,
                    Attachement=name
                };

                db.PostComments.Add(newComment);
                db.SaveChanges();

                comment.Id = newComment.Id;
                comment.Attachement = name;
                comment.User = _userService.GetUserById(comment.UserId, comment.UserId);

                _notificationService.AddNotification(comment.UserId, 0, NotificationTypeEnum.CmmentedOnYourPost, comment.PostId,"",null);

                return comment;

            }
        }

        public void DeleteComment(int id, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.PostComments.FirstOrDefault(i => i.Id == id && i.UserId == userId);

                if (item == null)
                    return;

                db.PostComments.Remove(item);

                db.SaveChanges();

            }
        }

        public List<PostCommentEntity> GetPostComments(int postId, int start, int end,int  currUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var comments = db.PostComments
                    .Where(i => i.PostId == postId)
                    .OrderByDescending(i => i.Date)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostCommentEntity
                    {

                        Id=i.Id,
                        Text = i.Text,
                        UserId = i.UserId,
                        Date = i.Date,
                        PostId = i.PostId,
                        Attachement=i.Attachement,
                        User=_userService.GetUserById(i.UserId, currUserId)

                    }).OrderByDescending(i => i.Date).ToList();

                return comments;
            }
        }

        public List<PostCommentEntity> GetNewComment(int postId,int lastId, int currUserId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var comments = db.PostComments
                    .Where(i => i.PostId == postId && i.Id>lastId)
                    .OrderByDescending(i => i.Date)
                    .ToList()
                    .Select(i => new PostCommentEntity
                    {
                        Id=i.Id,
                        Text = i.Text,
                        UserId = i.UserId,
                        Date = i.Date,
                        PostId = i.PostId,
                        Attachement = i.Attachement,
                        User = _userService.GetUserById(i.UserId, currUserId)

                    }).OrderByDescending(i => i.Date).ToList();

                return comments;
            }
        }

        #endregion

        #region Post Rates

        public Tuple<Dictionary<string, int>,int> RatePost(PostRatesEntity rate)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.Rates.FirstOrDefault(i => i.PostId == rate.PostId && i.UserId == rate.UserId && i.Rate1 == rate.Rate1);

                if (item != null)
                    db.Rates.Remove(item);
                else
                {
                    item = db.Rates.FirstOrDefault(i => i.PostId == rate.PostId && i.UserId == rate.UserId);

                    if (item != null)
                        db.Rates.Remove(item);

                    db.Rates.Add(new Rate
                    {
                        UserId = rate.UserId,
                        PostId = rate.PostId,
                        Rate1 = rate.Rate1
                    });
                }

                db.SaveChanges();

                _notificationService.AddNotification(rate.UserId, 0, NotificationTypeEnum.LikedYourPost, rate.PostId, "", null);

                return GetRate(rate.PostId,rate.UserId);
            }
        }

        public Tuple<Dictionary<string, int>, int> GetRate(int postId, int userId)
        {

            using (var db = new groubel_dbEntities1())
            {
                var userSelect = 0;

                var usSelObj = db.Rates.FirstOrDefault(i => i.PostId == postId && i.UserId == userId);

                if (usSelObj != null)
                    userSelect = usSelObj.Rate1;

                var data = db.Rates.Where(i => i.PostId == postId).GroupBy(i => i.Rate1).Select(i => new { i.Key, Count = i.Count() }).ToDictionary(i=>i.Key.ToString(), i=>i.Count);

                return Tuple.Create(data,userSelect);
            }

        }
        #endregion

        public Tuple<int,List<string>> GetPostLikers(int postId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = db.Rates.Where(i => i.PostId == postId).OrderBy(i => i.Id).Select(i => i.UserId).ToList();

                var friends = db.Friendships.Where(i => i.UserId == userId && userIds.Contains(i.FriendId)).OrderByDescending(i=>i.Id).Skip(0).Take(2).Select(i => i.FriendId);

                var names = db.Users.Where(i => friends.Contains(i.Id)).Select(i => i.FirstName + " " + i.LastName).ToList();

                var d = Tuple.Create(userIds.Count, names);

                return d;
            }
        }

        public List<UserEntity> GetPostRatesByPostId(int postId, int userId, int start)
        {
            using (var db = new groubel_dbEntities1())
            {
                var userIds = db.Rates.Where(i => i.PostId == postId).OrderBy(i => i.Id).Skip(start).Take(40).Select(i => i.UserId).ToList();

                var users=db.Users.Where(i=>userIds.Contains(i.Id)).ToList().Select(i => new UserEntity
                {
                    Id = i.Id,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Image = GetUserImage(i.Id),
                    RateType= db.Rates.FirstOrDefault(t=>t.UserId==i.Id && t.PostId==postId).Rate1
                }).ToList();

                return users;
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

        #region Save Post 
        public void SavePost(int postId,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.PostSaves.FirstOrDefault(i => i.PostId == postId && i.UserId == userId);

                if (item == null)
                    db.PostSaves.Add(new PostSave
                    {
                        UserId = userId,
                        PostId = postId,
                        Date = DateTime.Now
                    });
                else
                    db.PostSaves.Remove(item);

                db.SaveChanges();
            }
        }

        public int GetSaveStatus(int postId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                return db.PostSaves.Count(i => i.PostId == postId && i.UserId == userId);
            }
        }

        #endregion

        #region Post Share 

        public PostEntity SharePost(int postId,int userId, string ip)
        {
            using (var db = new groubel_dbEntities1())
            {

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var oldPost = GetPostById(userId, postId);

                        var newPost = new Post
                        {
                            Attachement = oldPost.Attachement,
                            Text = oldPost.Text,
                            Date = DateTime.Now,
                            Anonoimus = oldPost.Anonoimus,
                            Ip =ip,
                            UserId = userId,
                            Image = "",
                            Video = "",
                            OldUserId=oldPost.UserId
                        };

                        db.Posts.Add(newPost);

                        db.SaveChanges();

                        foreach (var intere in oldPost.PostInterests)
                        {
                            db.PostInterests.Add(new PostInterest
                            {
                                PostId = newPost.Id,
                                InterestId = intere.Id
                            });
                        }

                        db.SaveChanges();

                        _notificationService.AddNotification(userId, 0, NotificationTypeEnum.SharedYourPost, newPost.Id, "", null);

                        transaction.Commit();

                        return new PostEntity
                        {
                            Id = newPost.Id,
                            Attachement = newPost.Attachement,
                            Text = newPost.Text,
                            Anonoimus = newPost.Anonoimus,
                            Ip = newPost.Ip,
                            UserId = newPost.UserId,
                            PostInterests = oldPost.PostInterests,
                            PostComments = new List<PostCommentEntity>(),
                            OldUser = _userService.GetUserById(oldPost.UserId, newPost.UserId),
                            Rate = null,
                            User = _userService.GetUserById(newPost.UserId, newPost.UserId)
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return null;
                    }
                }
            }
        }

        #endregion

        #region HidePost

        public void HidePost(int postId,int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                db.PostHides.Add(new PostHide
                {
                    PostId = postId,
                    UserId = userId
                });

                db.SaveChanges();
            }
        }

        public void ShowPost(int postId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.PostHides.FirstOrDefault(i => i.PostId == postId && i.UserId == userId);

                if (item == null)
                    return;

                db.PostHides.Remove(item);

                db.SaveChanges();
            }
        }

        public bool HideStatus(int postId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var item = db.PostHides.FirstOrDefault(i => i.PostId == postId && i.UserId == userId);

                if (item == null)
                    return false;

                return true;
            }
        }

        #endregion

        #region Report
        public void ReportPost(int postId, int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                db.Reportings.Add(new Reporting
                {
                    PostId = postId,
                    UserId = userId,
                });
                db.SaveChanges();
            }
        }

        #endregion

        public string UploadTemp(HttpPostedFileBase file)
        {
            var upload = new IOHandler("~/Uploads/PostAttachements/");

            var name = upload.Save(file);

            return name;
        }

        #region Special Interests Region

        public List<PostEntity> GetPostBySpecialInterest(int interestId, int postUserId, int start, int end)
        {
            using (var db = new groubel_dbEntities1())
            {
                if (interestId == -1)
                {

                    var post = db.Posts
                    .Where(j => j.Anonoimus)
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date = i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), postUserId),
                        Rate = GetRate(i.Id, postUserId),
                        User = _userService.GetUserById(i.UserId, postUserId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id)
                    }).ToList();

                    return post;

                }
                else if (interestId == -2)
                {

                    var ip = db.Users.FirstOrDefault(i => i.Id == postUserId).LastLoginIp;

                    if (ip.Split('.').Count() > 2)
                    {
                        var t = ip.Split('.');

                        ip = t[0] + "." + t[1] + "." + t[2];

                    }

                    var post = db.Posts
                    .Where(j => j.Ip.StartsWith(ip))
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date = i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), postUserId),
                        Rate = GetRate(i.Id, postUserId),
                        User = _userService.GetUserById(i.UserId, postUserId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id)
                    }).ToList();

                    return post;

                }
                else if (interestId == -3)
                {
                    var friends = db.Friendships
                                            .Where(j => j.UserId == postUserId)
                                            .Select(j => j.FriendId)
                                            .ToList();

                    var post = db.Posts
                    .Where(j => friends.Contains(j.UserId))
                    .OrderByDescending(i => i.Id)
                    .Skip(start)
                    .Take(end)
                    .ToList()
                    .Select(i => new PostEntity
                    {

                        Id = i.Id,
                        Attachement = i.Attachement,
                        Text = i.Text,
                        Date = i.Date,
                        Anonoimus = i.Anonoimus,
                        Ip = i.Ip,
                        UserId = i.UserId,
                        PostInterests = _interestService.GetInterestByPostId(i.Id, postUserId),
                        PostComments = GetPostComments(i.Id, 0, 3, postUserId),
                        OldUser = i.OldUserId == null ? null : _userService.GetUserById(Convert.ToInt32(i.OldUserId), postUserId),
                        Rate = GetRate(i.Id, postUserId),
                        User = _userService.GetUserById(i.UserId, postUserId),
                        Saved = GetSaveStatus(i.Id, postUserId),
                        Reported = db.Reportings.Any(t => t.UserId == postUserId && t.PostId == i.Id),
                        CommentsCount = db.PostComments.Count(j => j.PostId == i.Id)
                    }).ToList();

                    return post;

                }

                return null;
            }

        }

        #endregion
    }
}
