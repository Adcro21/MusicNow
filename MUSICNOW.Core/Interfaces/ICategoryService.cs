using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities;


namespace MUSICNOW.Core.Interfaces
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
    }

}
