using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    public class MiniGameController : Controller
    {
        // GET: MiniGame
        public ActionResult BowGame()
        {
            return View();
        }
    }
}