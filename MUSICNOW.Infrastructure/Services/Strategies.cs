using MUSICNOW.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Infrastructure.Services.Strategies
{
    public class PremiumUpgradeStrategy : IUpgradeStrategy
    {
        public bool Execute(int userId, IUserService userService)
        {
            // Chiến lược này biết rõ gói Premium cần truyền số tiền 40,000đ
            return userService.UpgradeToPremium(userId, 40000);
        }
    }

    public class CreatorUpgradeStrategy : IUpgradeStrategy
    {
        public bool Execute(int userId, IUserService userService)
        {
            // Chiến lược này tập trung vào việc thay đổi Role thành MusicCreator
            return userService.UpgradeToCreator(userId);
        }
    }
}
