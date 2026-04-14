using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    [Authorize]
    public class SearchController : BaseController
    {
        public SearchController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MUSICNOW.Infrastructure.Data.MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }
        // GET: /Search?query=son+tung
        [HttpGet]
        public ActionResult Index(string query)
        {
            // Tạo một ViewModel
            var model = new SearchViewModel
            {
                SearchTerm = query
            };

            // Chỉ tìm kiếm nếu 'query' không rỗng
            if (!string.IsNullOrEmpty(query))
            {
                // Gọi service (đã được kế thừa từ BaseController)
                model.Results = _musicService.SearchTracks(query);
            }

            // Đặt ViewBag để thanh tìm kiếm hiển thị lại từ khóa
            ViewBag.SearchTerm = query;

            // Trả về View "Index.cshtml" (trong Views/Search)
            return View(model);
        }
    }
}