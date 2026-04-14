using MUSICNOW.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    // <-- SỬA: Kế thừa từ BaseController
    public class HomeController : BaseController
    {
        public HomeController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MUSICNOW.Infrastructure.Data.MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }
        // GET: Home
        public ActionResult Index()
        {
            // Chuyển hướng người dùng đến trang nhạc
            return RedirectToAction("Index", "Music");
        }
    }
}