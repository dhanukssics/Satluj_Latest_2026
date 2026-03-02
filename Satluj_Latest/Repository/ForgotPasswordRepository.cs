using Satluj_Latest.Models;
using Satluj_Latest.PostModel;
using Satluj_Latest.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Satluj_Latest.Repository
{
    public class ForgotPasswordRepository
    {
        private readonly SchoolDbContext _Entity;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        public DateTime currentTime = DateTime.UtcNow;
        public ForgotPasswordRepository(
    IWebHostEnvironment env,
    IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public Tuple<bool, string> SendMail(ForgotPasswordPostModel model)
        {
            string msg = "Failed send Mail";
            bool status = false;
            string newPassword = CreateNewPassword();
            if (Convert.ToInt32(model.fromType) == 0)
            {
                var schoolPassword = _Entity.TbLogins.Where(x => x.Username.Trim().ToLower() == model.emailId.Trim().ToLower() && x.IsActive).FirstOrDefault();
                schoolPassword.Password = newPassword;
                status = _Entity.SaveChanges() > 0;
                {
                    bool sendData = SendMailData(schoolPassword.Username, schoolPassword.Password);
                    if (sendData == true)
                        msg = "Successful";
                    else
                        msg = "Failed to send mail";
                    return new Tuple<bool, string>(status, msg);
                }
            }
            else if (Convert.ToInt32(model.fromType) == 1)
            {
                var parentPassword = _Entity.TbParents.Where(x => x.Email.Trim().ToLower() == model.emailId.Trim().ToLower() && x.IsActive).FirstOrDefault();
                parentPassword.Password = newPassword;
                status = _Entity.SaveChanges() > 0;
                {
                    bool sendData = SendMailData(parentPassword.Email, parentPassword.Password);
                    if (sendData == true)
                        msg = "Successful";
                    else
                        msg = "Failed to send mail";
                    return new Tuple<bool, string>(status, msg);
                }
            }
            else
            {
                msg = "Email Not Exists"; 
                return new Tuple<bool, string>(status, msg);
            }
        }
        private string CreateNewPassword()
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[9];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < 9; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        private bool SendMailData(string email, string password)
        {
            var filePath = Path.Combine(
    _env.WebRootPath,
    "Content",
    "template",
    "ResetPassword.html"
);

            var emailTemplate = System.IO.File.ReadAllText(filePath);

            var request = _httpContextAccessor.HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}/";
            //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/template/ResetPassword.html");
            //var emailTemplate = System.IO.File.ReadAllText(filePath);
            //string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
            var mBody = emailTemplate.Replace("{{resetLink}}", password);
            bool sendMail = Send("Reset Password", mBody, email);
            return sendMail;
        }
        private bool Send(string subject, string mailbody, string email)
        {
            //MailMessage msg = new MailMessage();
            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            //msg.Subject = subject;
            //msg.Body = mailbody;
            //msg.From = new MailAddress("info.schoolman@gmail.com");
            //msg.To.Add(new MailAddress(email));
            //msg.Bcc.Add(new MailAddress("archanakv.srishti@gmail.com"));
            //msg.IsBodyHtml = true;
            //client.Host = "k2smtp.gmail.com";
            //System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("info.schoolman@gmail.com", "Info@123");
            //client.Port = int.Parse("587");
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = basicauthenticationinfo;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object s,
            //        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            //        System.Security.Cryptography.X509Certificates.X509Chain chain,
            //        System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //{
            //    return true;
            //};
            //try
            //{
            //    client.Send(msg);
            //}
            //catch (Exception ex)
            //{
            //}
            try
            {
                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.Body = mailbody;
                msg.From = new MailAddress("schoolman@srishtis.com");
                msg.To.Add(new MailAddress(email, "Dear"));
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = "k2smtpout.secureserver.net";
                System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("schoolman@srishtis.com", "ca@12345");
                client.Port = int.Parse("25");
                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(msg);
            }
            catch(Exception ex)
            {

            }
            return true;
        }
    }
}

