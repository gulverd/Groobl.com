using GroubelNew.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class InterestsController : Controller
    {
        private InterestsService _interestService;

        public InterestsController(InterestsService interSer) {

            _interestService = interSer;

        }

        public JsonResult GetRandomInterests()
        {
            var data = _interestService.GetRandomInterests(15);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserInterests(int userId)
        {
            var data = _interestService.GetUserInterests(userId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilterInterest(int userId, int id)
        {
            var data = _interestService.GetFilterInterest(id, userId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public void AddToMyInterest(int userId,int id)
        {
            _interestService.AddToMyInterests(userId, id);
        }

        public JsonResult GetInterestsByName(string name)
        {
            var data = _interestService.GetInterestsByName(name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}