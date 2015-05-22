using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Core.Logging;

namespace Plugin.SunLog.DB
{
    public interface ILogOperate
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        bool Init();
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="info">日志数据</param>
        /// <returns></returns>
        int Insert(LogInfo info);
        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="id">日志编号</param>
        /// <returns></returns>
        int Delete(int id);
        /// <summary>
        /// 清理日志
        /// </summary>
        /// <param name="endTime">结束日期</param>
        /// <param name="level">清理的日志等级</param>
        /// <param name="isHand">日志状态</param>
        /// <returns></returns>
        int Clear(DateTime endTime, LogLevel? level = null, bool status = true);
        /// <summary>
        /// 设置日志状态
        /// </summary>
        /// <param name="id">日志编号</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        int SetStatus(int id,bool status);
        /// <summary>
        /// 获取日志分页数据
        /// </summary>
        /// <param name="rows">每页数据量</param>
        /// <param name="page">当前页</param>
        /// <param name="level">日志等级</param>
        /// <param name="title">日志标题</param>
        /// <param name="status">日志状态</param>
        /// <returns></returns>
        Page<LogInfo> Page(int rows, int page, LogLevel? level = null, string title = null, bool? status = null);
    }
}
