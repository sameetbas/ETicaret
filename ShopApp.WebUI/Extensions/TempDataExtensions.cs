using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Extensions
{
    public static class TempDataExtensions
    {
        //WiewImportsa @using ShopApp.WebUI.Extensions Ekledik.
        //Hataların yanı sıra Email gönderildi , giriş yapıldı, çıkış yapıldı tarzı geri dönusler için.
        //sonrasında AccountControllerde Register postuna  TempData.Put() kısmını yazdık.
        public static void Put<T>(this ITempDataDictionary tempData,string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get <T>(this ITempDataDictionary tempData , string key ) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        
    }
}
