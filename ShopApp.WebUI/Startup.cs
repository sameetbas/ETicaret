using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopApp.BLL.Abstract;
using ShopApp.BLL.Concrete;
using ShopApp.DAL.Abstract;
using ShopApp.DAL.Concrete.EFCore;
using ShopApp.WebUI.EmailServices;
using ShopApp.WebUI.Identity;

namespace ShopApp.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        //proje çalışınca illk bu metot çalışıp admin oluşacak ve program admını bılecek
        public Startup(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(@"Server=.;Database=E-TicaretProjesi;uid=sa;pwd=1234;"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();



            services.Configure<IdentityOptions>(options =>
            {
                //PASSWORD 
                options.Password.RequireDigit = true; //Passworda rakam girme zorunlulugu
                options.Password.RequireLowercase = true;//Küçük harf zorunlulugu
                options.Password.RequiredLength = 6;//En az 6 basamak
                options.Password.RequireNonAlphanumeric = true; //Ozel karakter girmemeli
                options.Password.RequireUppercase = true; // Büyük harf zorunlulugu


                options.Lockout.MaxFailedAccessAttempts = 5;//Maksımum kaç kez denesin
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //Kaç dakika cezalı kalsın 
                options.Lockout.AllowedForNewUsers = true; //Yeni üye de de aynı ozellıkler olsun mu


                //options.User.AllowedUserNameCharacters = ""; //user namede ğ kullanma ç kullanma gibi
                options.User.RequireUniqueEmail = true; //aynı mail adresi databasede varsa giirme



                options.SignIn.RequireConfirmedEmail = true; // Maille aktivasyon kontrol
                options.SignIn.RequireConfirmedPhoneNumber = false;//Telefon numarasıyla akt. konyrol edilsin mi 
            });

            services.ConfigureApplicationCookie(options =>

            {
                options.LoginPath = "/account/login"; //giriş yaptıgında gideceği sayfa
                options.LogoutPath = "/account/logout";//çıkış yaptıgında gideceği sayfa
                options.AccessDeniedPath = "/account/accessdenied"; //Adamın yetkisi varmı
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);//giriş yaptıktan sonra kaç dakıka işlem yapılmaz ise logout olsun
                options.SlidingExpiration = true; //60 dakika içerisinde herhangı bır tıklama yapılırsa sayaç sıfırla
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,//tarayıcıdaki scriptleri kullansın mı 
                    Name = ".ShopApp.Security.Cookie", //Cookie ismi
                    SameSite = SameSiteMode.Strict //cookieyi tarayıcıda tutmus oluyor.
                };
            });
            services.AddScoped<IProductDal, EFCoreProductDal>();
            services.AddScoped<ICategoryDal, EFCoreCategoryDal>();
            services.AddScoped<ICartDal, EFCoreCartDal>();
            services.AddScoped<IOrderDal, EFCoreOrderDal>();

            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICartService, CartManager>();
            services.AddScoped<IOrderService, OrderManager>();


            //Email için bir email servicess klasörü ve içine Email Sender classı açtık.
            //Bir tane De Interface Class açtık EmailSender Classına IEmailSenderı kalıtım alıp İmplement ettik.
            services.AddTransient<IEmailSender, EmailSender>();


            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            //IProduct EFCoreProductDal
            //IProduct MySqlProductDal
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "adminProducts",
                    template: "admin/products",
                    defaults: new { controller = "Admin", action = "ProductList" }
                    );

                routes.MapRoute(
                    name: "adminProducts",
                    template: "admin/products/{id?}",
                    defaults: new { controller = "Admin", action = "EditProduct" }
                    );

                routes.MapRoute(
                    name: "products",
                    template: "products/{category?}",
                    defaults: new { controller = "Shop", action = "List" }
                    );


                routes.MapRoute(
             name: "cart",
             template: "cart",
             defaults: new { controller = "Cart", action = "Index" }
             );
                //KULLANICI AÇIK İSE ESKİ VERDİĞİ SİPARİŞLERİ GÖRMELİ
                routes.MapRoute(
         name: "orders",
         template: "orders",
         defaults: new { controller = "Cart", action = "GetOrders" }
         );

                routes.MapRoute(
             name: "checkout",
             template: "checkout",
             defaults: new { controller = "Cart", action = "Checkout" }
              );



                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });

            SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();
        }
    }
}
