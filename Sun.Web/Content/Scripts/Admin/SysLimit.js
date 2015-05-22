
var editData;//用于编辑的数据
var editIndex;
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var moduleData = new Object();
var fist = 0;
$(function () {
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
         url: '/admin/syslimit/GetLimitPageJson/',
        title: '系统权限列表',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50],
        fitColumns: true,
        singleSelect: true,
        idField: 'LimitId',
        queryParams: { ModuleId: 0 },
        columns: [[{ field: 'Code', width: 150, align: 'center', title: '编码' },
            { field: 'Name', width: 100, editor: 'text', align: 'center', title: '名称' },
            { field: 'ModuleName', width: 110, align: 'right', align: 'center', title: '所属模块', formatter: function (value, row) { return '<span style="color:red;">'+value+'</span>'; } },
            { field: 'Icon', width: 50, editor: 'text', align: 'center', title: '图标' },
        ]],
        onLoadSuccess: dataLoadSuccess,
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#List').datagrid('resize', { height: h - 46 });
    });
    $.post('/admin/module/ModuleComboTreeJson/', {}, function (data) { createTopToolbar(data); $("#ModuleId").combotree({ data: data }); }, 'json');
});
//创建顶部工具栏
function createTopToolbar(data) {
    $(".datagrid-toolbar").append("模块：<input type=\"text\"  class=\"easyui-combotree\" id=\"toolModule\" style=\"width:250px\">");
    $("#toolModule").combotree({data: data, onChange: function (newValue, oldValue) {
            $("#List").datagrid("load", { ModuleId: newValue });
        }
    }).combotree('setValue',0);
}
//数据新增跟编辑触发按钮
var dataEdit = function (icon, title) {
    saveCount = 0;
    $('#AddorUpdate').dialog({
        resizable: 'true', shadow: 'false', iconCls: icon, maximizable: 'true', title: title,
        buttons: [{
            text: '保存', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                $("#edit").form("submit", {
                     url: "/admin/syslimit/EditOrUpdateLimit/",
                    onSubmit: function () {
                        var vr = $("#edit").form("validate");
                        var code = $("#txtCode").combobox('getValue');
                        var codeValidate = (/^\d+$/g.test(code) && code < 10000);
                        if (codeValidate == false)
                        {
                            $.messager.alert("验证失败","编码只能为数字", 'error')
                            return false;
                        }
                        if (vr == false) return false;
                        if (saveCount == 0) saveCount = 1;
                        else {
                            var r = confirm("您已提交过保存,是否需要再次保存?");
                            if (r == false)
                                return r;
                        }
                    },
                    success: function (data) {
                        if (data == 'true')
                        { $.messager.alert("操作提醒", "操作成功"); reloadList(); }
                        else
                            $.messager.alert("操作失败", data);
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
        text: '新增权限', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            $('#edit').form('clear');
            var value = $("#toolModule").combotree("getValue");
            $("#ModuleId").combotree("setValue", value);
            dataEdit('icon-add', '新增权限');
        }
    },
    edit: {
        text: '编辑数据', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            $("#edit").form("load", selectData);
            dataEdit('icon-edit', '编辑权限信息');
        }
    },
    remove: {
        text: '删除数据', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/syslimit/DeleteLimit/",
                        type: "post",
                        dataType: "html",
                        data: { limitId: selectData.LimitId },
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
)
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
    if (fist == 0) {
        var comData = $("#List").datagrid('getData');
        $("#txtCode").combobox({
            data: comData.rows, valueField: 'Code', textField: 'Name', onSelect: function (record) {
                $("#txtName").val(record.Name);
            }
        });
        fist = 1;
    }
}
//完成编辑执行方法
function CompleteEdit() {

}
//单机行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.LimitId != editData.LimitId) {
        $('#List').datagrid('endEdit', editIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            submitEdit(editData, function (msg) {
                if (msg =='true') {
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
         url: '/admin/syslimit/EditOrUpdateLimit/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}