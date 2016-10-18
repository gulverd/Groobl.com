using GroubelNew.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class FriendsController : Controller
    {
        FriendsService _friendsService;
        public FriendsController(FriendsService fr)
        {
            _friendsService = fr;
        }

        public JsonResult AddFriend(int firstuserId, int secondUserId)
        {
            var st = _friendsService.AddFriend(firstuserId, secondUserId);

            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFriend(int firstuserId, int secondUserId)
        {
            var st = _friendsService.RemoveFriend(firstuserId, secondUserId);

            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitFriendship(int firstuserId, int secondUserId)
        {
            _friendsService.SubmitFriendship(firstuserId, secondUserId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFriends(int userId)
        {
            var st = _friendsService.GetFriends(userId);

            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFriendsByName(int userId, string name)
        {
            var st = _friendsService.GetFriendsByName(userId, name);

            return Json(st, JsonRequestBehavior.AllowGet);
        }
    }
}