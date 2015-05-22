using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Sun.Core.Message
{
    public interface IEmail
    {
        /// <summary>
        /// 初始化邮件对象
        /// </summary>
        /// <param name="to">收件人地址</param>  
        /// <param name="body">邮件正文</param>  
        /// <param name="title">邮件的主题</param> 
        void Init(string[] tos, string body, string title);
        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="Path">附件路径</param>
        void Attachments(string Path);
        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="CompletedMethod"></param>
        void SendAsync(SendCompletedEventHandler CompletedMethod);
        /// <summary>
        /// 发送邮件
        /// </summary>
        void Send();
    }
}
