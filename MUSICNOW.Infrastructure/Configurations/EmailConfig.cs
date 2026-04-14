using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Infrastructure.Configurations
{
    public sealed class EmailConfig
    {// Thực thể duy nhất (Singleton)
        private static readonly Lazy<EmailConfig> _instance =
            new Lazy<EmailConfig>(() => new EmailConfig());

        public static EmailConfig Instance => _instance.Value;

        // Các thuộc tính lấy từ Web.config
        public string FromEmail { get; private set; }
        public string AppPassword { get; private set; }
        public string SmtpServer { get; private set; }
        public int SmtpPort { get; private set; }

        // Constructor private để ngăn chặn khởi tạo từ bên ngoài
        private EmailConfig()
        {
            // Đọc dữ liệu từ Web.config
            FromEmail = ConfigurationManager.AppSettings["EmailUser"];
            AppPassword = ConfigurationManager.AppSettings["EmailPass"];

            // Các thông số cố định có thể để ở đây hoặc đưa vào Web.config
            SmtpServer = "smtp.gmail.com";
            SmtpPort = 587;

            // Kiểm tra tính hợp lệ của dữ liệu
            if (string.IsNullOrEmpty(FromEmail) || string.IsNullOrEmpty(AppPassword))
            {
                throw new Exception("Lỗi: Chưa cấu hình EmailUser hoặc EmailPass trong Web.config.");
            }
        }
    }
}
