using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MUSICNOW.Core.ViewModels
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Vai trò")]
        public string Role { get; set; }

        [Display(Name = "Trạng thái Premium")]
        public bool IsPremium { get; set; }

        [Display(Name = "Ngày hết hạn Premium")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", NullDisplayText = "N/A")] // Định dạng hiển thị ngày giờ
        public DateTime? PremiumExpiryDate { get; set; } // Dấu ? cho phép giá trị null

        public string AvatarUrl { get; set; }
    }
}
