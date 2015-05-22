
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#confList").datagrid({
         url: "/admin/WebConfig/GetConfPageJson/",
        title: '网站配置管理',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        idField: 'ConfigId',
        columns: [[
            { field: 'ConfigId', width: 50, align: 'center', title: '编号' },
            { field: 'CKey', width: 50, align: 'center', title: '主键' },
            { field: 'KeyName', width: 70, editor: 'text', align: 'center', title: '名称' },
            { field: 'GroupName', width: 50,align: 'center', title: '分组' },
            { field: 'CType', width: 50, align: 'center', title: '操作类型', formatter: function (value, row) { switch (value) { case 0: return "单选"; break; case 1: return "多选"; break; case 2: return "单文本框"; break; case 3: return "多文本框"; break; case 4: return "编辑器"; break; case 5: return "时间"; break; case 6: return "数字"; break; case 7: return '数字微调器'; break;case 8: return "图片"; break; default: return "未知"; break; } if (value == true || value == 'true') return '<span style="color:#0a6314;">可用</span>'; } },
            { field: 'ValidType', editor: 'text', width: 60, align: 'center', title: '验证规则' },
            { field: 'SortIndex', editor: 'text', width: 30, align: 'center', title: '顺序' },
            { field: 'Lock', title: '状态', width: 30, editor: { type: 'checkbox', options: { on: true, off:false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">锁定</span>'; else return '<span style="color:red;">未锁定</span>'; } }
        ]],
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    createTopToolbar();
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#confList').datagrid('resize', { height: h - 46 });
    });
})
//创建顶部工具栏
function createTopToolbar() {
    $(".datagrid-toolbar").append("分组：<input style=\"width:auto;\" id=\"group\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("操作类型：<input style=\"width:auto;\" id=\"opType\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:250px\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\"  iconCls=\"icon-reset\">重置</a>");

    $("#group").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/WebConfig/GetGroupCombox/',
        function (data, textStatus) {
            data.unshift({ "key": "", 'value': '请选择配置组' });
            $("#group").combobox('loadData', data);
        },
        "json"
    );

    $("#opType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/WebConfig/GetOpTypeCombox/',
        function (data, textStatus) {
            data.unshift({ 'key': "", 'value': '请选择配置类别' });
            $("#opType").combobox('loadData', data);
        },
        "json"
    );

    $("#btnClean").linkbutton().click(function () {
        $("#group").combobox("select","");
        $("#opType").combobox("select","");
        $("#SearchName").val("");
    });

    $("#btnSelect").linkbutton().click(function () {
        var groupId = $("#group").combobox("getValue");
        var opType = $("#opType").combobox("getValue");
        var Name = $("#SearchName").val();
        $("#confList").datagrid("load", { 'groupId': groupId, 'opType': opType, 'confName': Name });
    });
}

var config = {
    add: {
        text: '新增配置项', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增配置项', '/admin/WebConfig/Edit/', 'icon-add');
        }
    },
    edit: {
        text: '编辑配置', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑配置项', '/admin/WebConfig/Edit/?configId=' + selectData.ConfigId, 'icon-edit');
        }
    },
    remove: {
        text: '删除配置', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/WebConfig/DeleteConf/",
                        type: "post",
                        dataType: "html",
                        data: { configId: selectData.ConfigId },
                        success: function (msg) {
                            if (msg == "True") {
                                $.messager.alert("操作提醒", "删除成功");
                                reloadList();
                            }
                            else {
                                $.messager.alert("操作提醒", msg);
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

//新增或编辑
function addTab(title, url, icon) {
    if (!window.parent.window.$('#tabs').tabs('exists', title)) {
        add(title, url, icon);
    }
    else {
        window.parent.window.$('#tabs').tabs('close', title);
        add(title, url, icon);
    }
    function add(title, url, icon) {
        window.parent.window.$('#tabs').tabs('add', {
            title: title,
            iconCls: icon,
            content: '<iframe scrolling="no" frameborder="0" src="' + url + '" style="overflow:hidden;width:100%;height:99%;"></iframe> ',
            closable: true
        });
    }
}

//刷新列表数据
function reloadList() {
    $("#confList").datagrid("load");
    $("#confList").datagrid("unselectAll");
    $("#deleteBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
}
var listToolBar = [];
if (pMList.indexOf("3") != -1)//新增权限
    listToolBar.push(config.add);
if (pMList.indexOf("2") != -1)//编辑权限
    listToolBar.push(config.edit);
if (pMList.indexOf("4") != -1)//删除权限
    listToolBar.push(config.remove);
listToolBar.push(config.reload);

//双击事件
function rowDbClick(rowIndex, row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        editDataIndex = rowIndex;
        if (pMList.indexOf("2") != -1)//编辑权限控制
            $('#confList').datagrid('beginEdit', rowIndex);
    }
}
function CompleteEdit() {

}

//单击行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.ConfigId != editData.ConfigId) {
        $('#confList').datagrid('endEdit', editDataIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            submitEdit(editData, function (msg) {
                if (msg == "true" && msg != null && msg != "") {
                    editData = null;
                    selectData = row;
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
         url: '/admin/WebConfig/EditOrUpdateConf/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}