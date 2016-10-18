using Groubel.Enums;
using GroubelNew.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class NotificationController : Controller
    {
        NotificationService _notificationService;

        public NotificationController(NotificationService nt)
        {
            _notificationService = nt;
        }

        public JsonResult AddNotification(int userId, int friendId)
        {
            _notificationService.AddNotification(userId, friendId, NotificationTypeEnum.RequestAddFriend, 0, "", null);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserNotificationsCount(int userId)
        {
            var data = _notificationService.GetUserNotificationsCount(userId);

            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserNotifications(int userId,int lastId, int nextId)
        {
            var data = _notificationService.GetUserNotifications(userId, lastId,nextId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserActivity(int userId, int lastId)
        {
            var data = _notificationService.GetUserActivity(userId, lastId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetApproved(int id)
        {
             _notificationService.SetApproved(id);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        
    }
}