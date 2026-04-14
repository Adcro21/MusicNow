using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Core.Entities
{
    [Table("Users")] // Khớp với tên bảng "Users"
    public class User
    {
        [Key] // Khóa chính
        public int UserID { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public long Balance { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PremiumExpiryDate { get; set; } // Dấu ? cho phép giá trị null

        public string AvatarUrl { get; set; }
    }
}
