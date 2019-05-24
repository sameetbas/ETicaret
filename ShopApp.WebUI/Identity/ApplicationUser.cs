using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string Fullname { get; set; } 
        //IdentityUserın ıcınden tekrar kalıtım aldıgı user classında zaten kendı ıcınde dıger propertylerı getırıyor

    }
}
