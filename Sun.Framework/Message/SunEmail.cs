using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Sun.Core.Message;


namespace Sun.Framework.Message
{
    /// <summary>  
    /// 发送邮件的类  
    /// </summary>  
    public class SunEmail:IEmail
    {
        private EmailUser account;
        public SunEmail(EmailUser user)
        {
            account = user;
        }
        private MailMessage mailMessage;
        private SmtpClient smtpClient;
        /// <summary>  
        /// 初始化邮件服务  
        /// </summary>  
        /// <param name="to">收件人地址</param>  
        /// <param name="body">邮件正文</param>  
        /// <param name="title">邮件的主题</param>  
        public void Init(string[] tos, string body, string title)
        {
            mailMessage = new MailMessage();
            foreach (var t in tos)
            {
                mailMessage.To.Add(t);
            }
            mailMessage.From = new System.Net.Mail.MailAddress(account.Account);
            mailMessage.Subject = title;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码  
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Priority = System.Net.Mail.MailPriority.High;
        }
        /// <summary>  
        /// 添加附件  
        /// </summary>  
        public void Attachments(string Path)  
        {  
            string[] path = Path.Split(',');  
            Attachment data;  
            ContentDisposition disposition;  
            for (int i = 0; i < path.Length; i++)  
            {  
                data = new Attachment(path[i], MediaTypeNames.Application.Octet);//实例化附件  
                disposition = data.ContentDisposition;  
                disposition.CreationDate = System.IO.File.GetCreationTime(path[i]);//获取附件的创建日期  
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path[i]);// 获取附件的修改日期  
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path[i]);//获取附件的读取日期  
                mailMessage.Attachments.Add(data);//添加到附件中  
            }  
        }
        /// <summary>  
        /// 异步发送邮件  
        /// </summary>  
        /// <param name="CompletedMethod"></param>  
        public void SendAsync(SendCompletedEventHandler CompletedMethod)
        {
            if (mailMessage != null)
            {
                smtpClient = new SmtpClient();
                smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, account.PassWord);//设置发件人身份的票据  
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.Host = account.Host;
                smtpClient.EnableSsl = true;
                if (CompletedMethod != null)
                    smtpClient.SendCompleted += new SendCompletedEventHandler(CompletedMethod);//注册异步发送邮件完成时的事件  
                smtpClient.SendAsync(mailMessage, mailMessage.Body);
            }
        }
        /// <summary>  
        /// 发送邮件  
        /// </summary>  
        public void Send()
        {
            if (mailMessage != null)
            {
                try
                {
                    smtpClient = new SmtpClient();
                    smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, account.PassWord);//设置发件人身份的票据  
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.Host = account.Host;
                    smtpClient.Port = account.Port;
                    smtpClient.EnableSsl = false;
                    smtpClient.Send(mailMessage);
                }
                catch { }
            }
        }
    }
}
