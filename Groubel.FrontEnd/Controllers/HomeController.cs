using GroubelNew.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Groubel.FrontEnd.Controllers
{
    
    public class HomeController : Controller
    {
        SecurityService _securityService;

        public HomeController(SecurityService sc)
        {
            _securityService = sc;
        }

        [Authorize]
        public ActionResult Index()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            string name = ticket.Name;

            ViewBag.UserId = _securityService.UserIdByEmail(name);

            return View();
        }

    }
}