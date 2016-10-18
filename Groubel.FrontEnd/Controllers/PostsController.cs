using Groubel.Helpers;
using GroubelNew.BLL;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        PostsService _postService;
        public PostsController(PostsService ps)
        {
            _postService = ps;
        }

        #region Posts

        [HttpPost]
        public JsonResult AddPosts(string data, HttpPostedFileBase file)
        {

            PostEntity post = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PostEntity>(data);

            post.Ip = Request.UserHostAddress;
            var dt = _postService.AddPost(post, file);

            return Json(dt, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetPosts(int userId,int start, int end)
        {
            var dt = _postService.GetPostsByUserInterest(userId, start, end);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPostsByInterestId(int interestId, int userId, int start, int end)
        {
            var dt = _postService.GetPostsByInterestId(interestId, userId, start, end);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPostById(int postId, int userId)
        {
            var dt = _postService.GetPostById(userId, postId);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPostsByUserId(int postUserId, int userId, int start, int end)
        {
            var dt = _postService.GetPostsByUserId(postUserId, userId, start, end);

            return Json(dt.ToGrouped(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAnonimusPostsByUserId(int postUserId, int userId, int start, int end)
        {
            var dt = _postService.GetAnonimusPostsByUserId(postUserId, userId, start, end);

            return Json(dt.ToGrouped(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSavedPostsByUserId(int postUserId, int userId, int start, int end)
        {
            var dt = _postService.GetSavedPostsByUserId(postUserId, userId, start, end);

            return Json(dt.ToGrouped(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePost(int id, int userId)
        {
            var d=_postService.DeletePost(id,userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Comment

        public JsonResult AddComment(string data, HttpPostedFileBase file)
        {
            PostCommentEntity comment = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PostCommentEntity>(data);

            var dt = _postService.AddComment(comment,file);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteComment(int id,int userId)
        {
            _postService.DeleteComment(id,userId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPostComments(int postId, int start, int end, int userId)
        {
            var dt = _postService.GetPostComments(postId, start, end, userId);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewComment(int postId, int lastId, int userId)
        {
            var dt = _postService.GetNewComment(postId, lastId, userId);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Rate

        public JsonResult Rate(int postId, int rate,int userId)
        {
            var dt = _postService.RatePost(new PostRatesEntity {
                PostId=postId,
                Rate1=rate,
                UserId=userId
            });

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Save 

        public void Save(int postId, int userId)
        {
            _postService.SavePost(postId, userId);
        }

        #endregion

        #region Share 

        public JsonResult Share(int userId, int postId)
        {

            var pt= _postService.SharePost(postId, userId, Request.UserHostAddress);

            return Json(pt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hide

        public void Hide(int postId, int userId)
        {
            _postService.HidePost(postId, userId);
        }

        #endregion

        #region Report

        public void Report(int postId, int userId)
        {
            _postService.ReportPost(postId, userId);
        }

        #endregion

        public JsonResult UploadTemp(HttpPostedFileBase file)
        {
            var nm = _postService.UploadTemp(file);

            return Json(nm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPostRatesByPostId(int postId,int userId,int start)
        {
            var nm = _postService.GetPostRatesByPostId(postId,userId,start);

            return Json(nm, JsonRequestBehavior.AllowGet);
        }
    }
}