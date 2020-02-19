using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.BLL.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);

        //Sonrasında concrete ye OrderManager Oluşturduk


        //KULLANICI GİRİŞ YAPTIYSA ESKI SIPARISLERINI GÖRMELI BU SEBEPTEN STARTUPDA GEREKLI KODU YAZDIK BURADAN ORDERMANAGER ORADAN DA IORDERDAL DAOLUSTURUYORUZ SONRASINDA EFCOREDAL
        List<Order> GetOrders(string userId);
    }
}
