using System;
using System.ComponentModel;

namespace Sun.Model.Common
{
    /// <summary>
    /// 配置项类型
    /// </summary>
    public enum ConfElementType
    {
        [Description("单选")]
        Radio=0,//单选
        [Description("多选")]
        checkbox=1,//多选
        [Description("单文本框")]
        Text=2,//输入框
        [Description("多文本框")]
        TextArea=3,//多行输入框
        [Description("编辑器")]
        HtmlText=4,//含编辑器的输入框
        [Description("时间")]
        Time=5,//日期
        [Description("数字")]
        Number= 6,//日期时间
        [Description("数字微调器")]
        NumberSpinner=7,
        [Description("图片")]
        Pic=8//图片
    }
}
