
var saveCount = 0;//防止重复提交表单
var files;//图片地址存储
$(function () {
    $("#uploadwindow").window("close");//关闭图片上传弹出窗
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#confset").propertygrid({
         url: "/admin/WebConfigSet/GetConfSetData/",
        title: '网站配置设置',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        fitColumns: true,
        singleSelect: true,
        groupField: 'GroupName',
        showGroup: true,
        scrollbarSize: 0,
        PropertySort: 'NoSort',
        idField: 'ConfigId',
        columns: [[
            { field: 'KeyName', width: 15, title: '配置名称', sortable: true,align:'left'},
            {
                field: 'CValue', width: 120, title: '值', align: 'left', formatter: function (value, row) {
                switch (row.CType)
                {
                    case 0: {
                        if (row.Options.length >= 1) {
                            var result = '';
                            $.each(row.Options, function (i, n) {
                                result += '<input type="radio" class="radios" name="conf_' + row.ConfigId + '" '+GetChecked(value,n.OptionId)+' value="' + n.OptionId + '" />' + n.OptionName;
                            })
                            return result;
                        }
                    }; break;
                    case 1: {
                        if (row.Options.length >= 1) {
                            var result = '';
                            $.each(row.Options, function (i, n) {
                                result += '<input type="checkbox" class="checkboxs" name="conf_' + row.ConfigId + '" ' + GetChecked(value, n.OptionId) + ' value="' + n.OptionId + '" />' + n.OptionName;
                            })
                            return result;
                        }
                    }; break;
                    case 2: { return '<input class="celltxt easyui-validatebox"  type="text" name="conf_' + row.ConfigId + '" ' + GetOption(row) + ' value="' + value + '" />'; }; break;
                    case 3: { return '<textarea class="celltxt easyui-validatebox" name="conf_' + row.ConfigId + '"  ' + GetOption(row) + '>' + value + '</textarea>'; }; break;
                    case 4: { return '<textarea class="easyui-validatebox celledit" id="conf_' + row.ConfigId + '" name="conf_' + row.ConfigId + '" ' + GetOption(row) + '>' + value + '</textarea>'; }; break;
                    case 5: { return '<input class="cellTime"  name="conf_' + row.ConfigId + '" type=\"text\" style=\"width:auto;\" value="' + GetTimeValue(value) + '" ' + GetOption(row) + ' />'; }; break;
                    case 6: { return '<input class="cellNumber"  name="conf_' + row.ConfigId + '" type=\"text\" style=\"width:auto;\" value="' + GetNum(value) + '" ' + GetOption(row) + '/>'; }; break;
                    case 7: { return '<input class="cellNumberSpinner"  name="conf_' + row.ConfigId + '" type=\"text\" style=\"width:auto;\" value="' + GetNum(value) + '" ' + GetOption(row) + ' />'; }; break;
                    case 8: { return '<input type=\"text\" class="cellimg" name="conf_' + row.ConfigId + '" id="conf_' + row.ConfigId + '" style=\"width:auto;\" value="' + value + '" /><a href="#" data-id="conf_' + row.ConfigId + '" iconCls="icon-image" class="cellbtn imglook">预览</a><a href="#" data-id="conf_' + row.ConfigId + '" iconCls="icon-cadd" class="cellbtn imgupload">上传</a>' }; break;
                    default: { return value;}; break;
                }
                }
            },
            { field: 'ConfigId', width: 17, align: 'left', title: '操作', formatter: function (value, row) { return GetOpBtn(value, row); } }
        ]],
        onLoadSuccess: function (data) {
            $(".celltxt").validatebox(); $(".celledit").validatebox();
            $(".cellbtn").linkbutton({ plain: true });
            $(".celltxt").width($(".celltxt").parent().width() - 10);
            $(".cellimg").width($(".celltxt").parent().width() - 150);
            $(".cellTime").width(200).timespinner({ showSeconds: true });
            $(".cellNumber").width(500).numberbox();
            $(".cellNumberSpinner").width(500).numberspinner();
            $(".celledit").each(function () {
                var id = $(this).attr("id");
                UE.getEditor(id, { serverUrl: fileUrl + 'common/' });
            });
            $(".ruledetail").click(function () {
                var content = $(this).attr("data-c");
                $("#ruleDetail").window({ title: "配置规则", iconCls: 'icon-word', content: content, collapsible: false, minimizable: false, width: 500, height: 200 })
            });
            $(".imglook").click(function () {
                var imgId = $(this).attr("data-id");
                var src = $(("#" + imgId)).val();
                var content = '<img id="thumbimg" src="' + src + '"/>';
                $("#ruleDetail").window({ title: "图片预览", iconCls: 'icon-image', content: content, collapsible: false, minimizable: false, width: 300, height: 300 })
            });
            $(".imgupload").click(function () {
                var imgId = $(this).attr("data-id");
                files = new Array();
                var count = 0;
                $("#uploader").pluploadQueue({
                    runtimes: 'html5,flash,silverlight,html4',
                    url: fileUrl + 'admin/common/uploadimage/',
                    max_file_count: 1,
                    filters: {
                        max_file_size: '1000mb',
                        mime_types: [
                            { title: "Images", extensions: fileExts }
                        ]
                    },
                    rename: true,
                    sortable: true,
                    dragdrop: true,
                    flash_swf_url: '../../upload/Moxie.swf',
                    silverlight_xap_url: '../../upload/Moxie.xap',
                    init: {
                        FileUploaded: function (up, file, info) {
                            files[count] = info.response;
                            count++;
                        }
                    }
                }).init();
                dataEdit('icon-add', "图片上传", imgId);
            });
            //保存数据
            $(".cellsave").linkbutton({
                onClick: function () {
                    var confid = $(this).attr("data-id");
                    var key = $(this).attr("data-key");
                    var val = '';
                    if ($(("*[name='conf_" + confid + "']")).hasClass("celledit")) {
                        val = UE.getEditor(("conf_" + confid + "")).getContent();
                    }
                    else if ($(("*[name='conf_" + confid + "']")).hasClass("radios")) {
                        val = $(("*[name='conf_" + confid + "']:checked")).val();
                    } else if ($(("*[name='conf_" + confid + "']")).hasClass("checkboxs")) {
                        $(("*[name='conf_" + confid + "']:checked")).each(function (i) { if (i > 0) val += ','; val += $(this).val(); })
                    } else
                        val = $(("*[name='conf_" + confid + "']")).val();
                    $.ajax({
                        async: false,
                         url: "/admin/WebConfigSet/SetValueById/",
                        type: "post",
                        dataType: "json",
                        data: { confId: confid, value: val, key: key },
                        success: function (msg) {
                            if (msg == true) {
                                $.messager.alert("保存提醒", "保存成功");
                            }
                            else {
                                $.messager.alert("保存失败", msg);
                            }
                        },
                        error: function () { $.messager.alert("操作提醒", "连接服务器失败"); }
                    });
                }
            });
            //重置数据
            $(".cellReset").click(function () {
                var id = $(this).attr("data-id");
                var pval = $("#confset").data("c" + id);
                if ($(("*[name='conf_" + id + "']")).hasClass("cellTime"))
                { $(("*[name='conf_" + id + "']")).val(GetTimeValue(pval)); }
                else if ($(("*[name='conf_" + id + "']")).hasClass("celledit"))
                {
                    UE.getEditor(("conf_" + id + "")).setContent(pval);
                }
                else if ($(("*[name='conf_" + id + "']")).hasClass("cellNumber") || $(("*[name='conf_" + id + "']")).hasClass("cellNumberSpinner"))
                { $(("*[name='conf_" + id + "']")).val(GetNum(pval)); }
                else
                    $(("*[name='conf_" + id + "']")).val(pval);
            })
        }
    });
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#confset').datagrid('resize', { height: h - 46 });
        $(".celledit").each(function () {
            var id = $(this).attr("id");
            UE.getEditor(id).destroy();
        });
        $('#confset').datagrid("reload");
    });
})
var dataEdit = function (icon, title,id) {
    $('#uploadwindow').dialog({
        resizable: true, shadow: false, iconCls: icon, collapsible: false, minimizable: false, maximizable: 'true', title: title, buttons: [{
            text: '确定', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                if (files.length != 0) {
                    for (var i = 0; i < files.length; i++) {
                        var data = eval("(" + files[i] + ")");
                        $(("#"+id)).val(data.url);
                    }
                    $("#uploadwindow").window("close");
                }
            }
        }, {
            text: '取消', id: 'cancelBtn', iconCls: 'icon-cancel', handler: function () { $("#uploadwindow").dialog("close"); }
        }]
    }).dialog("move", { top: 50 });
}
//获取数字控件的参数
function GetOption(row)
{
    if (row.ValidType != null && row.ValidType != '') return 'data-options="' + row.ValidType + '"'; else return ''
}
function GetChecked(v,i)
{
    if (v == '')
        return v;
    var values=v.split(',');
    if (values.indexOf(''+i+'') != -1)
        return 'checked="checked"';
    else
        return '';
}
function GetTimeValue(value)
{
    if (value != null && value != '') return value;
    else
        return nowtime;
}
function GetNum(value)
{
    if (isNaN(value))
        return 0;
    else
        return value;
}
function GetOpBtn(value, row)
{
    var option='';
    if (row.Lock || pMList.indexOf("2")==-1)
        option = ',disabled:true';
    var str = '<a href="#" class="cellbtn ruledetail" data-c="' + row.CRule + '" iconCls="icon-word" >规则</a><a href="#" class="cellbtn cellsave" data-key="'+row.CKey+'" data-id="' + row.ConfigId + '" data-options="iconCls:\'icon-save\''+option+'" >保存</a>';
    if (row.CType > 1)
    {
        str += '<a href="#"  data-id="' + row.ConfigId + '" class="cellbtn cellReset"  data-options="iconCls:\'icon-remove\'' + option + '">重置</a>';
        $("#confset").data("c" + row.ConfigId, row.CValue);
    }
    return str;
}
