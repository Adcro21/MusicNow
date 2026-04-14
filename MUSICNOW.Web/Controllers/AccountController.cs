using MUSICNOW.Core.Entities;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using MUSICNOW.Infrastructure.Services;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace MUSICNOW.Web.Controllers
{
    // Kế thừa từ BaseController
    public class AccountController : BaseController
    {
        public AccountController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MUSICNOW.Infrastructure.Data.MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Music");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _userService.RegisterUser(model); // Vẫn dùng _userService từ Base
                if (isRegistered)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index", "Music");
                }
                else
                {
                    ModelState.AddModelError("", "Email này đã được sử dụng.");
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Music");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.ValidateUser(model.Email, model.Password); // Vẫn dùng _userService từ Base

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    
                    // Luôn luôn chuyển hướng về trang chủ Âm nhạc
                    return RedirectToAction("Index", "Music");
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Music");
        }

        // GET: /Account/Profile
        [Authorize]
        public new ActionResult Profile() // (Giữ 'new' ở đây là ĐÚNG)
        {
            var userEmail = User.Identity.Name;
            var user = _userService.GetUserByEmail(userEmail);

            if (user == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsPremium = user.IsPremium,
                PremiumExpiryDate = user.PremiumExpiryDate,
                AvatarUrl = user.AvatarUrl
            };

            ViewBag.ActivePage = "Profile";
            return View(model);
        }


        // POST: /Account/UploadAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        // <<< SỬA 2: XÓA TỪ KHÓA 'new' VÌ NÓ KHÔNG CẦN THIẾT >>>
        public ActionResult UploadAvatar(HttpPostedFileBase avatarFile)
        {
            var userEmail = User.Identity.Name;
            var user = _userService.GetUserByEmail(userEmail);

            if (user == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            if (avatarFile == null || avatarFile.ContentLength == 0)
            {
                ModelState.AddModelError("AvatarUploadError", "Vui lòng chọn một file ảnh.");
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(avatarFile.FileName)?.ToLower();

                if (fileExtension == null || !allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("AvatarUploadError", "Chỉ chấp nhận file ảnh JPG hoặc PNG.");
                }
                else
                {
                    string newAvatarUrl = _userService.UpdateAvatar(user.UserID, avatarFile);

                    if (!string.IsNullOrEmpty(newAvatarUrl))
                    {
                        TempData["SuccessMessage"] = "Cập nhật avatar thành công!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("AvatarUploadError", "Có lỗi xảy ra khi tải ảnh lên. Vui lòng thử lại.");
                    }
                }
            }

            // --- TRẢ VỀ VIEW VỚI LỖI (Giữ nguyên) ---
            var updatedUser = _userService.GetUserByEmail(userEmail);
            var model = new ProfileViewModel
            {
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                Role = updatedUser.Role,
                IsPremium = updatedUser.IsPremium,
                PremiumExpiryDate = updatedUser.PremiumExpiryDate,
                AvatarUrl = updatedUser.AvatarUrl
            };
            ViewBag.ActivePage = "Profile";
            return View("Profile", model);
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            if (user == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                TempData["PasswordError"] = "Mật khẩu mới và xác nhận phải giống nhau.";
                return RedirectToAction("Profile");
            }

            bool success = _userService.ChangePassword(user.UserID, model.OldPassword, model.NewPassword);

            if (success)
            {
                TempData["PasswordSuccess"] = "Đổi mật khẩu thành công!";
            }
            else
            {
                TempData["PasswordError"] = "Mật khẩu cũ không đúng.";
            }

            return RedirectToAction("Profile");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isSent = _userService.SendPasswordResetOtp(model.Email);

                if (isSent)
                {
                    return RedirectToAction("ResetPassword", new { email = model.Email });
                }
                else
                {
                    ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                }
            }
            return View(model);
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isReset = _userService.ResetPasswordWithOtp(model.Email, model.Otp, model.NewPassword);
                if (isReset)
                {
                    TempData["ResetSuccess"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "OTP không hợp lệ hoặc đã hết hạn.");
                }
            }
            return View(model);
        }

        // ACTION MỚI CHO VIỆC NÂNG CẤP TÀI KHOẢN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult UpgradeToCreator()
        {
            return RedirectToAction("Index", "Payment", new { type = "Creator" });
        }
    }
}