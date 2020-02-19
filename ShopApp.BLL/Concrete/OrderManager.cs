using ShopApp.BLL.Abstract;
using ShopApp.DAL.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.BLL.Concrete
{
    public class OrderManager:IOrderService
    { 
        //içerisini doldurduktan sonra IOrderServiceden kalıtım alıp implement ettik

        private IOrderDal _orderDal;

        public OrderManager(IOrderDal orderDal)
        {
            _orderDal = orderDal;
        }

        public void Create(Order entity)
        {
            _orderDal.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _orderDal.GetOrders(userId);
        }

        //sonrasında cart controllerda private IOrderService _orderService; ile bir nesne oluşturup
        //ctor methodda parametreden geleni IOrderServiceden gelene eşitledik.
        //En son StartUp services.AddScoped<IOrderDal, EFCoreOrderDal>(); ve  services.AddScoped<IOrderService, OrderManager>(); ekledik.
    }
}
