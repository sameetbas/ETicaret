using System;
using System.Collections.Generic;
using System.Text;

namespace ShopApp.Entities
{
    public class Order
    {
        //2 tane kullanılacak api var 
        //stripe ve
        //IYZICO

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
        public int Id { get; set; }

        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }

        public EnumOrderState OrderState { get; set; }
        public EnumPaymentTypes PaymentTypes { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string OrderNote { get; set; }


        //apiler için alanlar Başlangıç
        public string PaymentId { get; set; }
        public string PaymentToken { get; set; }
        public string ConversationId { get; set; }
        //bitiş

            //sonrasında ShopContexte Order olusturduk ve cmd den migrations yaptık
            // dal yolu ile  dotnet ef database update --> dotnet ef migrations add AddingOrderEntities --> dotnet ef database update
            //buradan sonra cart controllerda Checkout Iactionresultu yazıp view oluşturuk


        public List<OrderItem> OrderItems { get; set; }

    }

    public enum EnumOrderState
    {
        waiting = 0,
        Unpaid =1,
        Completed=2
    }

    public enum EnumPaymentTypes
    {
        CreditCart = 0,
        EFT = 1
    }

}
