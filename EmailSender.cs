// Email sender
// David Piao
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCKLIB
{
    class EmailSender
    {
        public static string send_mail(EmailSetting setting, string to_address, string body)
        {
            try
            {
                new Thread(() =>
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(setting.smtp_server);
                    SmtpServer.Credentials = new System.Net.NetworkCredential(setting.smtp_user, setting.smtp_password);
                    SmtpServer.EnableSsl = true;
                    try
                    {
                        SmtpServer.Port = Int32.Parse(setting.smtp_port);
                    }
                    catch (Exception ex)
                    {
                        SmtpServer.Port = 587;
                    }

                    try
                    {
                        mail.From = new MailAddress(setting.smtp_user);
                        mail.To.Add(to_address);
                        mail.Subject = "PCKLIB";
                        mail.Body = body;
                        mail.IsBodyHtml = true;
                        SmtpServer.Send(mail);
                    }
                    catch (Exception ex1)
                    {
                        MainApp.log_error(ex1.Message + "\n" + ex1.StackTrace);
                    }
                }).Start();
                return "Email sent successfully.";
            }
            catch (Exception ex)
            {
                return "Sending email failed." + ex.Message;
            }
        }
    }

    public class EmailSetting
    {
        public string smtp_server;
        public string smtp_port;
        public string smtp_user;
        public string smtp_password;
    }
}
