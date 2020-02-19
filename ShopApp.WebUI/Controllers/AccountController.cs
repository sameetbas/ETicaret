using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ShopApp.BLL.Abstract;
using ShopApp.WebUI.Extensions;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{

    [AutoValidateAntiforgeryToken] //Artık post için bir benzersiz anahtar olusturuyor. Kullanıcı adı ve şifre cookie de tutuldugu için servera kullanıcı adı şifreyi aktarmamak sadece kendı bılgısayarımızda tutma işini görüyor.
    public class AccountController : Controller
    {
        //BİRİ USERI KONTROL EDIYOR DİĞERİ GİRİŞİ KONTROL EDIYOR.

        private IEmailSender _emailSender; //Burada bir nesne olusturduk ve AccountController metodundan parametre gonderıp eşitledik. Bunları yaptıktan sonra Register methodunda send mail kısmını doldurduk. LocalHosttan gelen lınkle callbackurl ıle geri dönus adresinin verdik.

        private UserManager<ApplicationUser> _userManager;
        //UserManager kendı kutuphanesi var oluşturdugumuz ıdentıty kutuphanesınden geliyor
        //ApplicationUser de hazırda bulunan bir komuttur. 

        private SignInManager<ApplicationUser> _signInManager;
        //SıgnInManager kendı kutuphanesi var oluşturdugumuz ıdentıty kutuphanesınden geliyor

            private ICartService _cartService;

        //Her olusturdugumuzda çalışması için kurucu method kullanıyoruz.
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        }

        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Fullname = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            //create Asyncnin kendi kütüphanesinde olan bir metot
            //await classla beraber çalıştıdığından dolayı metoda async Task<> diyerek çalışır hale getiriyoruz.


            if (result.Succeeded) //giriş yapıldı mı yapılmadı mı
            {
                //generate token
                _cartService.InitializeCart(user.Id);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user); //Code değişkenıne benzersiz bir token atıyor

                var callbackurl = Url.Action("ConfirmEmail", "account", new
                {
                    userId = user.Id,
                    token = code
                });

                //send mail
                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı Onaylayınız.", $"Lütfen E-Mail Hesabınızı Onaylamak İçin Linke <a href='https://localhost:44305{callbackurl}'>  Tıklayınız</a>");

                TempData.Put("meesage",new ResultMessage()
                {
                    Title= "Hesap Onayı",
                    Message="EPosta adresinize gelen link ile hesabınızı aktifleştiriniz.",
                    Css="warning"
                });

                return RedirectToAction("login", "account");
            }

            ModelState.AddModelError("", "Bilinmeyen bir hata oluştur tekrar deneyiniz.");
            return View(model);
        }

        //KAYIT YAPARKEN PROPERTYLERİ TUTUP  KARŞILAŞTIRMAK ICIN MODELS KLASORUNDE REGISTER MANAGER CLASSI OLUSTURDUK.

        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);


            //if (user == null)
            //{
            //    ModelState.AddModelError("", "Bu mail ile eşleşen bir kayıt yok");
            //    return View(model);
            //}

            if (!await _userManager.IsEmailConfirmedAsync(user))//onaylandı mı onaylanmadı mı bak
            {
                ModelState.AddModelError("", "Lütfen hesabınızı E-mail adresiniz üzerinden onaylayınız.");
                return View(model);
            }

            //Giriş yapıldı mı yapılmadı mı ilk false taryıcı kapatınca oturum kapansın mı , ikincisi 5 kere yanlış girerse kilitle
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/"); // giriş yapıldıysa returnrul ile gönderdiğim bilgieri bana geri gönder
            }

            ModelState.AddModelError("", "Kullanıcı mail adresi veya şifre yanlış!");

            return View();
        }

        public async Task<IActionResult> Logout()
        {            await _signInManager.SignOutAsync(); // Oturum kapatma
            TempData.Put("message", new ResultMessage()
            {
                Title = "Oturum Kapatıldı.",
                Message = "Hesabınız güvenli bir şekilde sonlandırıldı",
                Css = "warning"
            });
            return Redirect("~/");//En üst kademe nerden geldıysen oraya geri çık demek
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //VİEW OLUSTURDUK HATALARI VE BİLGİLERİ BU VİEWDE TAŞIYACAĞIZ

            if (userId == null || token == null)
            {

                TempData.Put("message", new ResultMessage()
                {
                    Title = "Hesap onayı.",
                    Message = "Hesap onayı için bilgileriniz yanlış",
                    Css = "danger"
                });
                return Redirect("~/");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //create cart object
                    TempData.Put("message", new ResultMessage()
                    {
                        Title = "Hesap onayı.",
                        Message = "Hesapınız başarıyla onaylanmıştır.",
                        Css = "success"
                    });
                    return RedirectToAction("Login");
                }
            }

            TempData.Put("message", new ResultMessage()
            {
                Title = "Hesap onayı.",
                Message = "Hesabınız onaylanmadı",
                Css = "danger"
            });
            return View();
        }

        // ŞİFRENİZİ UNUTTUYSANIZ.
        public IActionResult ForGotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForGotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Forgot Password.",
                    Message = "Bilgileriniz Hatalı!",
                    Css = "danger"
                });
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);//Bu email adresi var mı ? 
            if (user == null)
            {
                 TempData.Put("message", new ResultMessage()
                {
                    Title = "Forgot Password.",
                    Message = "E-Posta adresi ile bir kullanıcı bulunamadı!",
                    Css = "danger"
                });
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user); //_userManagerden GeneratePasswordResetTokenAsync  çalıştır ve bunu code at.Yenı bır token olusturacak.

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token = code
            });



            //send email
            await _emailSender.SendEmailAsync(email, "ResetPassword", $"Parolanızı yenilemek için linke <a href='https://localhost:44305{callbackUrl}'> tıklayınız</a>");

            TempData.Put("message", new ResultMessage()
            {
                Title = "Forgot Password.",
                Message = "Parolayı yenilemek için hesabınıza mail gönderildi!",
                Css = "warning"
            });

            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordModel { token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {

            if (!ModelState.IsValid)//Uygun değilse
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

       public IActionResult Accessdenied()
        {
            return View();
        }
    }
}