using GroubelNew.BLL;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class RoomsController : Controller
    {
        RoomsService _roomsService;
        public RoomsController(RoomsService rm)
        {
            _roomsService = rm;
        }

        [HttpPost]
        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            var name=_roomsService.UploadImage(file);

            return Json(name, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddRoom(ChatEntity chat, HttpPostedFileBase file)
        {
            var d = _roomsService.AddRoom(chat, file);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteChat(int userId, int id)
        {
            var d = _roomsService.DeleteChat(userId, id);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserRooms(int userId,int mainUserId, bool isRoom)
        {
            var d = _roomsService.GetUserRooms(userId, mainUserId, isRoom);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchRooms(string name,int mainUserId, bool isRoom)
        {
            var d = _roomsService.SearchRooms(name, mainUserId, isRoom);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomById(int userId, int id)
        {
            var d = _roomsService.GetRoomById(id, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        #region Comments

        [HttpPost]
        public JsonResult AddComment(int roomId,int userId,string text, HttpPostedFileBase file)
        {
            var comment = new ChatCommentEntity
            {
                RoomId = roomId,
                UserId = userId,
                Text = text,
                Image = "",
                Date = DateTime.Now
            };

            var d = _roomsService.AddComment(comment, file);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteComment(int userId, int id)
        {
            var d = _roomsService.DeleteComment(userId, id);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastComments(int chatId, int lastCommentId,int userId)
        {
            var d = _roomsService.GetLastComments(chatId, lastCommentId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLatestCommentsStatus(Dictionary<int, int> chats)
        {
            var d = _roomsService.GetLatestCommentsStatus(chats);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Members

        public JsonResult AddUser(int chatId, int userId)
        {
            var d = _roomsService.AddUser(chatId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUser(int chatId, int userId)
        {
            var d = _roomsService.DeleteUser(chatId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateUser(int adminUserId, int chatId, int userId)
        {
            var d = _roomsService.ActivateUser(adminUserId, chatId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HideUser(int adminUserId, int chatId, int userId)
        {
            var d = _roomsService.HiseUser(adminUserId, chatId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MuteUser(int adminUserId, int chatId, int userId)
        {
            var d = _roomsService.MuteUser(adminUserId, chatId, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChatMembers(int id, int userId)
        {
            var d = _roomsService.GetChatMembers(id, userId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Attachements

        public JsonResult GetAttachements(int roomId, bool isImage)
        {
            var d = _roomsService.GetAttachements(roomId, isImage);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult RequestAddToRoom(int userId, int adminId, int roomId)
        {
            _roomsService.RequestAddToRoom(userId, adminId,roomId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CloseChat(int id)
        {
            _roomsService.CloseChat(id);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OpenChat(int userId, int secondUserId)
        {
            var d=_roomsService.OpenChat(userId, secondUserId);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserSuggestions(int userId,int count)
        {
            var d = _roomsService.GetUserSuggestions(userId,count);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        
    }
}