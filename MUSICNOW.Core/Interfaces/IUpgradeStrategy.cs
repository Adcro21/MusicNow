using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Core.Interfaces
{
    public interface IUpgradeStrategy
    {
        bool Execute(int userId, IUserService userService);
    }
}
