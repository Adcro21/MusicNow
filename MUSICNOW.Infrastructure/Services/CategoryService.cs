using MUSICNOW.Core.Entities;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace MUSICNOW.Infrastructure.Services
{
    // Kế thừa từ Interface
    public class CategoryService : ICategoryService
    {
        private readonly MusicNowDbContext _context;

        // Sử dụng Constructor Injection: Nhận context từ bên ngoài
        public CategoryService(MusicNowDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            // Lấy tất cả thể loại từ DB và chuyển thành danh sách
            return _context.Categories.ToList();
        }
    }
}