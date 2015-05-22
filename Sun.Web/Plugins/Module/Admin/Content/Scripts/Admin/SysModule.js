
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
$(function () {
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#ModuleList").treegrid({
         url: '/admin/module/ModuleGridJson/',
        title: '系统模块列表(双击行可进行简单编辑)',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        animate: true,
        fitColumns: true,
        idField: 'ModuleId',
        treeField: 'Name',
        columns: [[{ field: 'Name', width: 130, editor: 'text', align: 'left', title: '模块名称' },
            { field: 'ModuleId', width: 30, align: 'center', title: '模块编号' },
            { field: 'ModuleKey', width:120, align: 'left', title: '模块标识(唯一)' },
            { field: 'LinkUrl', width: 120, editor: 'text', align: 'center', title: '地址' },
            { field: 'ModuleValue', width: 40, editor: 'text', align: 'center', title: '关联值' },
            { field: 'TypeName', width: 50, align: 'center', title: '类别' },
            { field: 'Icon', width: 50, align: 'right', editor: 'text', align: 'center', title: '图标样式名' },
            { field: 'SortIndex', width: 30, editor: 'text', align: 'center', title: '顺序' },
            { field: 'IsUsable', title: '可用', width: 20, editor: { type: 'checkbox', options: { on:'True', off:'False' } }, align: 'center', formatter: function (value, row) { if (value == 'True') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } }
        ]],
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onLoadSuccess: dataLoadSuccess,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#ModuleList').treegrid('resize', { height: h - 46 });
    });
    $("#ParentId").combotree({  url: '/admin/module/ModuleComboTreeJson/' });
});
//数据新增跟编辑触发按钮
var dataEdit = function (icon, title) {
    saveCount = 0;
    $('#AddorUpdate').dialog({
        resizable: 'true', shadow: 'false', iconCls: icon, maximizable: 'true', title: title,
        buttons: [{
            text: '保存', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                $("#edit").form("submit", {
                     url: "/admin/module/EditOrUpdateModule/",
                    onSubmit: function () {
                        var goon = $("#edit").form("validate");
                        if (goon == false) return false;
                        if (saveCount == 0) saveCount = 1;
                        else {
                            var r = confirm("您已提交过保存,是否需要再次保存?");
                            if (r == false)
                                return r;
                        }
                    },
                    success: function (data) {
                        if (data ==true||data=='true') {
                            $.messager.alert("操作提醒", "操作成功");
                            reloadList();
                        }
                        else
                            $.messager.alert("操作失败", data);
                    }
                });
            }
        }, {
            text: '取消', id: 'cancelBtn', iconCls: 'icon-cancel', handler: function () { $("#AddorUpdate").dialog("close"); }
        }]
    }).dialog("move", { top: 50 });
}
var config = {
    add: {
        text: '新增模块', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            $('#edit').form('clear');
            dataEdit('icon-add', "新增模块");
        }
    },
    edit: {
        text: '编辑模块', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            $('#edit').form('clear');
            $("#edit").form("load", selectData);
            dataEdit('icon-edit', '编辑模块信息');
        }
    },
    remove: {
        text: '删除模块', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "删除模块可能会删除与模块有关的所有数据,你确定删除吗?", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/module/DeleteModule/",
                        type: "post",
                        dataType: "html",
                        data: { ModuleId: selectData.ModuleId },
                        success: function (msg) {
                            if (msg == "True") {
                                $.messager.alert("操作提醒", "删除成功");
                                reloadList();
                            }
                            else {
                                $.messager.alert("操作失败", msg);
                            }
                        },
                        error: function () { $.messager.alert("操作提醒", "连接服务器失败"); }
                    }
            );
                }
            });
        }
    },
    reload: {
        text: '刷新数据', id: 'reloadBtn', iconCls: 'icon-reload', handler: function () {
            reloadList();
        }
    }
};
//刷新列表数据
function reloadList() {
    $("#AddorUpdate").dialog("close");
    $("#ModuleList").treegrid("reload");
    $("#deleteBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
}
var listToolBar = [];
if (pMList.indexOf("3") != -1)//添加权限
    listToolBar.push(config.add);
if (pMList.indexOf("2") != -1)//编辑权限
    listToolBar.push(config.edit);
if (pMList.indexOf("4") != -1)//删除权限
    listToolBar.push(config.remove);
listToolBar.push(config.reload);
//双击事件
function rowDbClick(row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        if (pMList.indexOf("2") != -1)//编辑权限控制
            $('#ModuleList').treegrid('beginEdit', row.ModuleId);
    }
}
function CompleteEdit() {

}

//单机行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(row) {
    if (editData != undefined && editData != null && row.ModuleId != editData.ModuleId) {
        $('#ModuleList').treegrid('endEdit', editData.ModuleId);
        if (!compareObj(editData, selectData) && editData != null) {
            submitEdit(editData, function (msg) {
                if (msg == "True" && msg != null && msg != "") {
                    editData = null;
                    selectData = row;
                } else { $.messager.alert("更改保存失败", msg); }
            });
            editData = null;
        }
    }
    else {
        selectData = row;
        $("#deleteBtn").linkbutton({ disabled: false });
        $("#editBtn").linkbutton({ disabled: false });
    }
}
//提交新增或修改的数据
function submitEdit(data, sucFunc) {
    $.ajax({
        async: false,
         url: '/admin/module/EditOrUpdateModule/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    });
}
//数据加载完成执行方法
function dataLoadSuccess(row, data) {
    //$("#ModuleList").treegrid("collapseAll");
}