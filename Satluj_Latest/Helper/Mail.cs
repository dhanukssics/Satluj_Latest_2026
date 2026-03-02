using System.Net.Mail;

namespace Satluj_Latest.Helper
{
    public class Mail
    {
        internal static bool Send(string subject, string mailbody, string receiverName, string smtpEmail, string smtpPassword, System.Collections.ArrayList list_emails)
        {
            //{
            //    SmtpClient client = new SmtpClient();
            //    string userName = "childacademy@srishtis.com";
            //    string password = "ca12345";
            //    string fromName = "Child Academy";
            //    MailAddress address = new MailAddress(userName, fromName);

            //    foreach (string mailList in list_emails)
            //    {
            //        MailMessage message = new MailMessage();
            //        message.To.Add(new MailAddress(mailList, receiverName));
            //        message.From = address;
            //        message.Subject = subject;
            //        message.IsBodyHtml = true;
            //        message.Body = mailbody;
            //        //client.Host = "smtp.gmail.com";//ConfigurationManager.AppSettings["smptpserver"];
            //        client.Host = "smtpout.secureserver.net";
            //        //client.Host = "dedrelay.secureserver.net";
            //       // client.Port = 587;//Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            //        //client.Port = 465;
            //       client.Port = 25;
            //       // client.Port = 25;
            //        client.EnableSsl = false;
            //        client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //        client.UseDefaultCredentials = false;
            //       // client.UseDefaultCredentials = true;
            //        client.Credentials = new NetworkCredential(userName, password);
            //        try
            //        {
            //            client.Send(message);
            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //    }
            //}

            MailMessage msg = new MailMessage();
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            foreach (string mailList in list_emails)
            {
                string[] emailDomain = mailList.Split('@');
                string[] domain = emailDomain[1].Split('.');
                string dom = domain[0].ToString();
                if (dom == "sics") { }
                else
                {
                    try
                    {
                        msg.Subject = subject;
                        msg.Body = mailbody;
                        msg.From = new MailAddress(smtpEmail);
                        msg.To.Add(new MailAddress(mailList, receiverName));
                        msg.IsBodyHtml = true;
                        client.Host = "k2smtp.gmail.com";
                        System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
                        client.Port = int.Parse("587");
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = basicauthenticationinfo;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Send(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return true;
        }
    }

}
