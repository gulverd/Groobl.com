using GroubelNew.BLL;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class UserController : Controller
    {
        private UserService _userService;
        private SecurityService _securityService;
        public UserController(UserService us, SecurityService sc)
        {
            _userService = us;
            _securityService = sc;

        }
        
        public JsonResult GetUserData(int id, int curId)
        {
            return Json(_userService.GetUserById(id, curId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByIdForPopup(int id, int curId)
        {
            return Json(_userService.GetUserByIdForPopup(id, curId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddImage(HttpPostedFileBase file,int id)
        {
            return Json(_userService.AddImage(file, id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void UpdateUser(UserEntity user)
        {
            _userService.UpdateUser(user);
           
        }

        public void Ping(int id)
        {
            _securityService.Ping(id);

        }

        public JsonResult IsOnline(int id)
        {
            return Json(_securityService.IsOnline(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByName(int userId, string name)
        {
            return Json(_userService.GetUserByName(userId,name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserFiles(int userId, bool isImage)
        {
            return Json(_userService.GetUserFiles(userId, isImage), JsonRequestBehavior.AllowGet);
        }
    }
}