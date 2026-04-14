using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities;
using MUSICNOW.Core.ViewModels;

namespace MUSICNOW.Core.Interfaces
{
    public interface IUserService
    {
        // Sự kiện phát ra khi người dùng thực hiện một hành động quan trọng
        event Action<int, string> OnUserActionOccurred;
        // Kiểm tra đăng nhập
        User ValidateUser(string email, string password);

        // Đăng ký tài khoản
        bool RegisterUser(RegisterViewModel model);

        // Lấy người dùng bằng Email
        User GetUserByEmail(string email);
        // Kiểm tra và cập nhật trạng thái Premium
        void CheckAndUpdatePremiumStatus(MUSICNOW.Core.Entities.User user);

        //apload ảnh đại diện
        string UpdateAvatar(int userId, System.Web.HttpPostedFileBase avatarFile);

        // Đổi mật khẩu
        bool ChangePassword(int userId, string oldPassword, string newPassword);

        // Xác nhận OTP 
        bool SendPasswordResetOtp(string email);
        bool ResetPasswordWithOtp(string email, string otp, string newPassword);

        // === THÊM HÀM MỚI NÀY VÀO ===
        bool UpgradeToCreator(int userId);

        // Lấy người dùng bằng ID
        User GetUserById(int userId);
        List<ProfileViewModel> GetPopularArtists(int count);
        // Nâng cấp tài khoản lên Premium
        bool UpgradeToPremium(int userId, long amount);
    }
}