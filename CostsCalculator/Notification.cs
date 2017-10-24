using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Mail;

namespace CostsCalculator
{
    class Notification
    {
        public Notification()
        {

        }

        public void Notify(string mailAddress, string subject, string message)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(mailAddress));
            mail.From = new System.Net.Mail.MailAddress("costscalc2017@gmail.com");
            mail.Subject = subject;
            mail.Body = message;

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("costscalc2017@gmail.com", "Haslo123.");
            client.Host = "smtp.gmail.com";

            mail.IsBodyHtml = true;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                            ex.ToString());
            }
        }

    
    }
}