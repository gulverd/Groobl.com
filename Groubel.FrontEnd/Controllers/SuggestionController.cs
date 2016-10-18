using GroubelNew.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Groubel.FrontEnd.Controllers
{
    public class SuggestionController : Controller
    {
        SuggestionsService _suggestionService;
        public SuggestionController(SuggestionsService sg)
        {
            _suggestionService = sg;
        }

        public JsonResult GetSuggestions(int userId)
        {
            var d = _suggestionService.GetSuggestions(userId);

            return Json(d,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestedUsers(int userId,int ind)
        {
            var d = _suggestionService.GetSuggestedUsers(Request.UserHostAddress, userId, ind);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestedRooms(int userId, int ind)
        {
            var d = _suggestionService.GetSuggestedRooms(userId, ind);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuggestedInterests(int userId, int ind)
        {
            var d = _suggestionService.GetSuggestedInterests(userId, ind, Request.UserHostAddress);

            return Json(d, JsonRequestBehavior.AllowGet);
        }
    }
}