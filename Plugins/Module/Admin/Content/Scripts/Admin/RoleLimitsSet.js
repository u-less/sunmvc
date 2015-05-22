
$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#Limits").treegrid({
        data: GetGridData(),
        title: '角色权限分配',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        striped: true,
        rownumbers: true,
        animate: true,
        fitColumns: true,
        idField: 'ModuleId',
        treeField: 'Name',
        columns: [[
            { field: 'Name', width: 85, editor: 'text', align: 'left', title: '模块名称' },
            { field: 'ModuleId', width: 20, align: 'center', title: '模块编号', hidden: true },
            {
                field: 'Select', width: 15, align: 'center', title: '访问', formatter: function (value, row) {
                    if (parseInt(value) > 0) return '<input type="checkbox" id="MD' + row.ModuleId + '" value="' + row.ModuleId + '" parentId="' + row._parentId + '" class="MDSelect" checked="checked" />'; else return '<input type="checkbox" id="MD' + row.ModuleId + '" value="' + row.ModuleId + '" parentId="' + row._parentId + '" class="MDSelect" />';
                }
            },
            {
                field: 'LimitList', align: 'left', title: '权限信息', width: 600, formatter: function (value, row) {
                    if (value!=undefined&&value.length >= 1) {
                        var result = '';
                        $.each(value, function (i, n) {
                            if (parseInt(n.Select) > 0)
                                result += '<input type="checkbox" parentId="' + row.ModuleId + '" moduleId="' + n.ModuleId + '" value="' + n.LimitId + '" class="limitSelect" checked="checked" />' + n.Name + '';
                            else
                                result += '<input type="checkbox" parentId="' + row.ModuleId + '" moduleId="' + n.ModuleId + '" value="' + n.LimitId + '" class="limitSelect" />' + n.Name + '';
                        })
                        return result;
                    } else
                        return '<span style="color:red;">没有可供分配的权限</span>';
                }
            },
        ]],
        toolbar: listToolBar
    });
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#Limits').treegrid('resize', { height: h - 46 });
    });
    SelectChangeDelegate()
});
//获取Grid控件的数据方法
function GetGridData() {
    var Data;
    $.ajax({
        async: false,
         url: "/admin/SysRole/GetLimitAboutRoleJson/",
        type: "post",
        dataType: "html",
        data: { roleId: roleId },
        success: function (msg) {
            if (msg != "" && msg != null) {
                Data = eval("(" + msg + ")");
            }
        },
        error: function () { $.messager.alert("失败提醒", "连接服务器失败"); }
    });
    return Data;
}
var config = {
    save: {
        text: '保存权限', id: 'addBtn', iconCls: 'icon-disk', handler: function () {
            save();
        }
    },
    reload: {
        text: '刷新数据', id: 'deleteBtn', iconCls: 'icon-reload', handler: function () {
            reloadList();
        }
    }
};
var listToolBar = [];
if (Limits.indexOf("5") != -1)//分配权限
    listToolBar.push(config.save);
listToolBar.push(config.reload);
//刷新列表数据
function reloadList() {
    $("#Limits").treegrid("loadData", GetGridData()); SelectChangeDelegate();
}
var changeType = 0;
function SelectChangeDelegate()
{
    $(".MDSelect").change(function () {
        var obj = $(this);
        var value = $(this).attr("value");
        if (obj.attr("checked") == "checked") {
            var parentID = obj.attr("parentId");
            if (parentID != undefined && parentID != "undefined") {
                $("#MD" + parentID + "").attr("checked", "checked").change();
                if (changeType == 0) {
                    $("[parentId='" + value + "']").each(function (i, e) {
                        if ($(e).hasClass("limitSelect")) {
                            $(e).attr("checked", "checked");
                        }
                    })
                } else {
                    changeType = 0;
                }
            }
            else
                return;
        }
        else {
            var ID = obj.attr("value");
            $("[parentId='" + ID + "']").removeAttr("checked").change();
        }
    });
    $(".limitSelect").change(function () {
        var obj = $(this);
        if (obj.attr("checked") == "checked") {
            var parentID = obj.attr("parentId");
            if (parentID != undefined && parentID != "undefined") {
                changeType = 1;
                $("#MD" + parentID + "").attr("checked", "checked").change();
            }
            else
                return;
        }
    });
}
//保存权限更改
function save() {
    var moduleResult = '';
    var limitResult = '';
    var i = 0;
    $(".MDSelect").each(function () {
        if (i > 0)
            moduleResult += "|";
        if ($(this).attr("checked") == "checked") {
            moduleResult += $(this).attr("value") + '#1';
        }
        else
            moduleResult += $(this).attr("value");
        i++;
    })
    var x = 0;
    $(".limitSelect").each(function () {
        if (x > 0)
            limitResult += "|";
        if ($(this).attr("checked") == "checked") {
            var moduleId = $(this).attr("moduleId");
            var limitId = $(this).attr("value");
            limitResult += (limitId + '>' + moduleId) + '#1';
        }
        else
            limitResult += $(this).attr("value");
        x++;
    })
    $.ajax({//Ajax提交数据进行保存
        async: false,
         url: "/admin/SysRole/SaveRoleLimits/",
        type: "post",
        dataType: "html",
        data: { roleId: roleId, limitStr: limitResult, moduleStr: moduleResult },
        success: function (msg) {
            if (msg == "True")
                $.messager.alert("保存数据提醒", "权限保存成功");
            else
                $.messager.alert("保存数据提醒", msg);
        },
        error: function () { $.messager.alert("保存数据提醒", "权限保存失败"); }
    })
}
