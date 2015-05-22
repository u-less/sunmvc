using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sun.Core.DBContext;

namespace Sun.Core.Logging
{
    public interface ILoger
    {
        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="info">日志信息对象</param>
        /// <returns></returns>
        void Log(LogInfo info);
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Info(Exception e);
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Info(string title, Exception e);
        /// <summary>
        /// 消息日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        void Info(string title, string message);
        /// <summary>
        /// Bug日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Debug(Exception e);
        /// <summary>
        /// Bug日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Debug(string title,Exception e);
        /// <summary>
        /// Bug日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        void Debug(string title, string message);
        /// <summary>
        /// 提醒日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Warning(Exception e);
        /// <summary>
        /// 提醒日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Warning(string title, Exception e);
        /// <summary>
        /// 提醒日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        void Warning(string title,string message);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Error(Exception e);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Error(string title,Exception e);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        void Error(string title, string message);
        /// <summary>
        /// 严重错误日志
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Fatal(Exception e);
        /// <summary>
        /// 严重错误日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="e">异常对象</param>
        /// <returns></returns>
        void Fatal(string title, Exception e);
        /// <summary>
        /// 严重错误日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        void Fatal(string title, string message);
        /// <summary>
        /// 通过日志编号获取日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LogInfo GetLogById(int id);
        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="id">日志编号</param>
        /// <returns></returns>
        void DeleteById(int id);
        /// <summary>
        /// 清理日志
        /// </summary>
        /// <returns></returns>
        void Clear(DateTime endTime,LogLevel? level=null,bool isHand=true);
        /// <summary>
        /// 标记日志已处理
        /// </summary>
        /// <param name="id">日志编号</param>
        /// <returns></returns>
        void Handle(int id);
        /// <summary>
        /// 获取日志分页数据
        /// </summary>
        /// <param name="rows">每页数据量</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="level">日志等级</param>
        /// <param name="title">日志标题</param>
        /// <returns></returns>
        Page<LogInfo> Page(int rows, int page, LogLevel? level = null, string title = null, bool? isHand = null);
    }
}
