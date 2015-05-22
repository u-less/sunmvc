
var editData;//用于编辑的数据
var editIndex = 0;
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
$(function () {
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
         url: '/admin/sysdict/GetDictPageJson/',
        title: '字典数据列表',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [30, 40, 50, 60, 70],
        fitColumns: true,
        singleSelect: true,
        idField: 'DictId',
        columns: [[{ field: 'DictId', width: 50, align: 'center', title: '编号' },
            { field: 'DictName', width: 150, editor: 'text', align: 'center', editor: 'text', title: '名称' },
            { field: 'DataValue', width: 150, editor: 'text', align: 'center', editor: 'text', title: '数值' },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '顺序', sortable: true },
            { field: 'IsUsable', title: '可用', width: 50, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else { return '<span style="color:red;">否</span>'; } } }
        ]],
        onLoadSuccess: dataLoadSuccess,
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    createTopToolbar();
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#List').datagrid('resize', { height: h - 46 });
    });
});
//创建顶部工具栏
function createTopToolbar() {
    $(".datagrid-toolbar").append("字典数据类别：<input type=\"text\"  class=\"easyui-combox\" id=\"toolModule\" style=\"width:250px\">");
    $("#toolModule").combobox({
         url: "/admin/sysdict/GetDictTypeCombox/",
        valueField:'key',textField:'value',
        onChange: function (newValue, oldValue) {
            $("#List").datagrid("load", { typeId: newValue });
        }
    })
}
//数据新增跟编辑触发按钮
var dataEdit = function (icon, title) {
    saveCount = 0;
    $('#AddorUpdate').dialog({
        resizable: 'true', shadow: 'false', iconCls: icon, maximizable: 'true', title: title,
        buttons: [{
            text: '保存', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                $("#edit").form("submit", {
                     url: "/admin/sysdict/EditOrUpdateDict/",
                    onSubmit: function () {
                        var vr = $("#edit").form("validate");
                        if (vr == false) return false;
                        if (saveCount == 0) saveCount = 1;
                        else {
                            $.messager.alert("操作重复提醒", "请不要两次点击保存按钮");
                            return false;
                        }
                    },
                    success: function (data) {
                        if (data == 'True')
                            $.messager.alert("操作提醒", "操作成功");
                        else
                            $.messager.alert("操作失败", data);
                        reloadList();
                    }
                })
            }
        }, {
            text: '取消', id: 'cancelBtn', iconCls: 'icon-cancel', handler: function () { $("#AddorUpdate").dialog("close"); }
        }]
    }).dialog("move", { top: 50 });
}
var config = {
    add: {
        text: '新增字典数据', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            $('#edit').form('clear');
            var value = $("#toolModule").combobox("getValue");
            $("#ModuleId").combobox({  url: '/admin/sysdict/GetDictTypeCombox/',valueField:'key',textField:'value'});
            $("#ModuleId").combobox("setValue", value);
            dataEdit('icon-add', '新增字典数据');
        }
    },
    edit: {
        text: '编辑字典数据', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            $("#ModuleId").combobox({  url: '/admin/sysdict/GetDictTypeCombox/', valueField: 'key', textField: 'value'})
            $("#edit").form("load", selectData);
            dataEdit('icon-edit', '编辑字典数据');
        }
    },
    remove: {
        text: '删除数据', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/sysdict/DeleteDict/",
                        type: "post",
                        dataType: "html",
                        data: { dictId: selectData.DictId },
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
                    })
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
    $("#List").datagrid("load");
    $("#List").datagrid("clearSelections");
    $("#deleteBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
}
var listToolBar = [];
if (Limits.indexOf("3") != -1)//添加权限
    listToolBar.push(config.add);
if (Limits.indexOf("2") != -1)//编辑权限
    listToolBar.push(config.edit);
if (Limits.indexOf("4") != -1)//删除权限
    listToolBar.push(config.remove);
listToolBar.push(config.reload);
//双击事件
function rowDbClick(index, row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        editIndex = index;
        if (Limits.indexOf("2") != -1)//编辑权限控制
            $('#List').datagrid('beginEdit', index);
    }
}
//数据加载成功执行方法
function dataLoadSuccess(row, data) {
    $("#deleteBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
}
//完成编辑执行方法
function CompleteEdit(rowIndex, rowData, changes) {
}
//单机行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.DictId != editData.DictId) {
        $('#List').datagrid('endEdit', editIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            submitEdit(editData, function (msg) {
                if (msg == "True" && msg != null && msg != "") {
                    editData = null;
                    selectData = row;
                }
                else {
                    $.messager.alert("失败提醒", msg);
                }
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
         url: '/admin/sysdict/EditOrUpdateDict/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}