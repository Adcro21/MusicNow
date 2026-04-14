using MUSICNOW.Core.Entities;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using MUSICNOW.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Configuration; // Dùng để đọc Web.config
using System.Data.Entity; // Cần cho EntityState.Modified
using System.IO; // Thư viện để xử lý tệp tin
using System.Linq;
using System.Net;           // Dùng cho NetworkCredential
using System.Net.Mail; // Thư viện để gửi email
using System.Text;
using System.Threading.Tasks;
using System.Web; // Thư viện để xử lý HttpPostedFileBase
using MUSICNOW.Infrastructure.Configurations; // sử dụng lớp cấu hình EmailConfig (Singleton)

namespace MUSICNOW.Infrastructure.Services
{
    // Bước 1: Kế thừa từ Interface
    public class UserService : IUserService
    {
        // Bước 2: Tạo biến DbContext
        private readonly MusicNowDbContext _context;

        public event Action<int, string> OnUserActionOccurred;

        // Sử dụng Constructor Injection: Nhận context từ bên ngoài
        public UserService(MusicNowDbContext context)
        {
            _context = context;
        }

        // Hàm 1: Lấy User bằng Email
        public User GetUserByEmail(string email)
        {
            // Tìm trong bảng Users, người dùng đầu tiên có Email khớp
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        // Hàm 2: Đăng ký
        public bool RegisterUser(RegisterViewModel model)
        {
            // 1. Kiểm tra Email đã tồn tại chưa?
            if (GetUserByEmail(model.Email) != null)
            {
                return false; // Đã tồn tại, không đăng ký được
            }

            // 2. Tạo đối tượng User mới
            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password, // LƯU MẬT KHẨU GỐC (Theo yêu cầu của em)
                Role = "GeneralUser", // Vai trò mặc định
                Balance = 0,
                IsPremium = false,
                CreatedAt = DateTime.Now
            };

            // 3. Lưu vào DB
            _context.Users.Add(newUser);
            _context.SaveChanges();
            var newUserId = newUser.UserID;
            OnUserActionOccurred?.Invoke(newUserId, $"Đăng ký tài khoản thành công: {model.Email}");

            return true;
        }

        // Hàm 3: Xác thực đăng nhập
        public User ValidateUser(string email, string password)
        {
            // 1. Tìm user bằng email
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return null; // Không tìm thấy email
            }

            // 2. Nếu tìm thấy, so sánh mật khẩu gốc
            if (user.PasswordHash == password)
            {
                OnUserActionOccurred?.Invoke(user.UserID, $"Đăng nhập thành công: {email}");
                return user; // Mật khẩu đúng!
            }

            return null; // Sai mật khẩu
        }
        //// Hàm 4: Ghi log sau khi hành động thành công
        //private void LogUserAction(int userId, string action)
        //{
        //    var newLog = new MUSICNOW.Core.Entities.Logs
        //    {
        //        UserID = userId,
        //        Action = action,
        //        Timestamp = DateTime.Now
        //    };
        //    // Log vào database
        //    _context.Logs.Add(newLog);
        //    _context.SaveChanges();
        //}

        // Hàm 5: kiểm tra và cập nhật trạng thái Premium
        public void CheckAndUpdatePremiumStatus(MUSICNOW.Core.Entities.User user)
        {
            if (user.IsPremium && user.PremiumExpiryDate.HasValue && user.PremiumExpiryDate.Value < DateTime.Now)
            {
                user.IsPremium = false;
                user.PremiumExpiryDate = null;
                _context.SaveChanges();
            }
        }
        // Hàm 6: Cập nhật ảnh đại diện
        public string UpdateAvatar(int userId, System.Web.HttpPostedFileBase avatarFile)
        {
            var user = _context.Users.Find(userId);
            if (user == null || avatarFile == null || avatarFile.ContentLength == 0)
            {
                return null; // Không tìm thấy user hoặc file không hợp lệ
            }

            // --- Logic Lưu File Ảnh ---
            try
            {
                // 1. Tạo tên file duy nhất
                string fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();
                string fileName = $"user_{userId}_{DateTime.Now.Ticks}{fileExtension}";

                // 2. Xác định đường dẫn lưu file trên server
                string serverFolderPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Avatars/");

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }

                string fullPath = Path.Combine(serverFolderPath, fileName);

                // 3. Lưu file ảnh
                avatarFile.SaveAs(fullPath);

                // 4. Cập nhật đường dẫn vào Database
                string relativePath = $"/Uploads/Avatars/{fileName}";
                user.AvatarUrl = relativePath;
                _context.SaveChanges();

                return relativePath; // Trả về đường dẫn mới
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi upload avatar: {ex.Message}"); // Ghi log lỗi
                return null; // Upload thất bại
            }
        }

        // Hàm 7: Đổi mật khẩu
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return false; // Không tìm thấy user
            }

            // 1. Xác thực mật khẩu cũ (So sánh plaintext)
            if (user.PasswordHash != oldPassword)
            {
                return false; // Mật khẩu cũ không đúng
            }

            // 2. Nếu mật khẩu cũ đúng, lưu mật khẩu mới (plaintext)
            user.PasswordHash = newPassword;

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            return true;
        }

        // ====================================================================
        // === CÁC HÀM GỬI EMAIL ĐÃ ĐƯỢC CẬP NHẬT HOÀN CHỈNH ===
        // ====================================================================

        // Hàm 8: Gửi OTP (Triển khai Interface)
        public bool SendPasswordResetOtp(string email)
        {
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return false; // Không tìm thấy email
            }

            // 1. Tạo mã OTP
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            // 2. Lưu OTP vào Session
            HttpContext.Current.Session["ResetOtp"] = otp;
            HttpContext.Current.Session["ResetOtpExpiry"] = DateTime.Now.AddMinutes(10); // OTP có hạn 10 phút
            HttpContext.Current.Session["ResetEmail"] = email;

            // 3. Gửi email thật
            try
            {
                string subject = "MusicNow - Mã OTP Đặt Lại Mật Khẩu";
                string body = $"Mã OTP của bạn là: {otp}\n\nMã này sẽ hết hạn sau 10 phút.";

                SendEmail(email, subject, body); // <-- GỌI HÀM GỬI THẬT

                return true; // Gửi thành công
            }
            catch (Exception ex)
            {
                // Ghi log lỗi gửi mail
                System.Diagnostics.Debug.WriteLine($"LỖI TẠI SendPasswordResetOtp: {ex.Message}");
                return false; // Gửi thất bại
            }
        }

        // Hàm 9: Reset bằng OTP (Triển khai Interface)
        public bool ResetPasswordWithOtp(string email, string otp, string newPassword)
        {
            // 1. Lấy thông tin từ Session
            var sessionEmail = HttpContext.Current.Session["ResetEmail"] as string;
            var sessionOtp = HttpContext.Current.Session["ResetOtp"] as string;
            var sessionExpiry = HttpContext.Current.Session["ResetOtpExpiry"] as DateTime?;

            // 2. Kiểm tra
            if (string.IsNullOrEmpty(sessionOtp) || sessionEmail != email || sessionOtp != otp)
            {
                return false; // Sai OTP hoặc sai email
            }

            if (sessionExpiry == null || sessionExpiry.Value < DateTime.Now)
            {
                return false; // Hết hạn OTP
            }

            // 3. Mọi thứ đều đúng, reset mật khẩu
            var user = GetUserByEmail(email);
            if (user == null) return false;

            // 4. LƯU NGUYÊN MẪU (THEO YÊU CẦU CỦA EM)
            user.PasswordHash = newPassword;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            // 5. Xóa OTP khỏi Session
            HttpContext.Current.Session.Remove("ResetOtp");
            HttpContext.Current.Session.Remove("ResetOtpExpiry");
            HttpContext.Current.Session.Remove("ResetEmail");

            return true;
        }

        // Hàm 10: HÀM GỬI MAIL (Hàm private nội bộ)
        private void SendEmail(string toEmail, string subject, string body)
        {
            // Lấy thông tin từ Singleton thay vì đọc file Web.config trực tiếp
            var config = EmailConfig.Instance; 

    try
            {
                using (SmtpClient smtpClient = new SmtpClient(config.SmtpServer))
                {
                    smtpClient.Port = config.SmtpPort;
                    smtpClient.Credentials = new NetworkCredential(config.FromEmail, config.AppPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(config.FromEmail, "MusicNow"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(toEmail);

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LỖI GỬI EMAIL: {ex.Message}");
                throw;
            }
        }

        // Hàm 11: Nâng cấp lên Creator

        public bool UpgradeToCreator(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return false; // Không tìm thấy user
            }

            // Nếu đã là Creator hoặc Admin, không cần làm gì
            if (user.Role == "MusicCreator" || user.Role == "Admin")
            {
                return true;
            }

            // Nâng cấp từ GeneralUser
            if (user.Role == "GeneralUser")
            {
                user.Role = "MusicCreator";
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

                OnUserActionOccurred?.Invoke(userId, "Nâng cấp tài khoản lên MusicCreator");
                return true;
            }

            return false; // Lỗi logic (ví dụ: user không có role)
        }


        // Hàm 12: Lấy User bằng ID
        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        // Hàm 13: Lấy nghệ sĩ nổi bật
        public List<ProfileViewModel> GetPopularArtists(int count)
        {
            // Tạm thời, chúng ta lấy các user có vai trò "MusicCreator"
            // (Sau này có thể nâng cấp để sắp xếp theo lượt view)
            var artists = _context.Users
                .Where(u => u.Role == "MusicCreator")
                .OrderByDescending(u => u.CreatedAt) // Lấy nghệ sĩ mới nhất
                .Take(count)
                .Select(u => new ProfileViewModel
                {
                    UserID = u.UserID, // Chúng ta đã thêm trường này ở Bước 1
                    Username = u.Username,
                    AvatarUrl = u.AvatarUrl,
                    Role = u.Role
                })
                .ToList();

            return artists;
        }

        // Hàm 14: Nâng cấp lên Premium
        public bool UpgradeToPremium(int userId, long amount)
        {
            // Tìm User
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            // 1. Cập nhật User
            user.IsPremium = true;
            user.PremiumExpiryDate = DateTime.Now.AddMonths(1); // Cho 1 tháng
            _context.Entry(user).State = EntityState.Modified;

            // 2. Ghi lại lịch sử giao dịch
            var newPurchase = new PremiumPurchases
            {
                UserID = userId,
                PurchaseDate = DateTime.Now,
                Amount = amount,
                ExpiryDate = user.PremiumExpiryDate.Value
            };
            _context.PremiumPurchases.Add(newPurchase);

            try
            {
                _context.SaveChanges();
                OnUserActionOccurred?.Invoke(userId, $"Nâng cấp Premium thành công. Số tiền: {amount}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi nâng cấp Premium: {ex.Message}");
                return false;
            }
        }


    } // Đóng class UserService

} // Đóng namespace