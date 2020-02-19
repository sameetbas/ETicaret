using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class ResultMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Css { get; set; }//success danger warnig gibi özellikler verilmesi için.
        //bunların arkasından shared klasoruna _resultmeesage adında bir view olusturduk layout aldırmadık ve partial view yaptık.
    }
}
