using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        //Send Grid sitesine kayıt olduk kullanıcı girişi yaptıktan sonra soldaki menuden Şu adımları izledik
        // Setting --> Apı Key --> Create Apı key oradan aldıgımız keyi buraya yapıştırdık.
        private const string SendGridKey = "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";
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Execute hata verdi generic method olustur dedik Execute adında Generic bir metot  oluşturdu.
            return Execute(SendGridKey, subject, htmlMessage, email);
        }

        private Task Execute(string sendGridKey, string subject, string htmlMessage, string email)
        {
            //SendGridClient görmuyor WebUI Gelerek Nugetten SendGrid Ekledik
            //Sonrasında SendGridClient in kutuphanesini ampulden ekledik
            var client = new SendGridClient(sendGridKey);

            //SendGridMessageyi Ampulden ekledık içerisinde email aktivasyon için olan hersey tanımlı f12 ile bakabılırsınız
            //Bize uygun olanları kullanacağız.
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@shopappp.com", "ShopApp"), //Mailin kimden gönderileceği (Maili gönderek kişi)
                Subject = subject,
                PlainTextContent = htmlMessage, //Gidecek olan mesaj
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email)); //Benım yollayacağım Email Adresi

            return client.SendEmailAsync(msg);//Benım verdiğim mesajı al götür.
        }
    }
}
