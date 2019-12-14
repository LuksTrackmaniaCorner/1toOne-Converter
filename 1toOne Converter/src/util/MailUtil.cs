using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter
{
    class MailUtil
    {
        private static readonly string _DebugMailAddress = "Platzhalter@web.de";

        public static void OpenDebugMail(Exception e)
        {
            MailMessage Mail = new MailMessage {
                From = new MailAddress(_DebugMailAddress),
                Subject = "Error Report",
                //IsBodyHtml = true,
                Body = "Hallo"
            };
            Mail.To.Add(_DebugMailAddress);
            var AttachmentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\897c140f-3995-4556-8e42-b582275ef8dd.eml";
            Mail.Attachments.Add(new Attachment(AttachmentPath));
            Mail.Headers.Add("X-Unsent", "1");

            SmtpClient Client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            Client.Send(Mail);

            //Cleanup
            Mail.Dispose();
            Client.Dispose();
        }
    }
}
