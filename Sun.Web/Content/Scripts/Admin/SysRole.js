
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
var superadmin = false;
$(function () {
    superadmin = $('body').attr('data-super') == 'True';
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#RoleList").datagrid({
         url: '/admin/SysRole/RolePageJson/',
        title: '系统角色列表(双击行可进行简单编辑)',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50],
        fitColumns: true,
        singleSelect: true,
        idField: 'RoleId',
        columns: [[{ field: 'RoleId', width: 40, editor: 'text', align: 'left', title: '角色编号' },
            { field: 'Name', width: 150, editor: 'text', align: 'center', title: '角色名称' },
            {
                field: 'OrganName', width: 150, align: 'center', title: '所属机构', formatter: function (value, row) {
                    if (row.OrganId == 0)
                        return '<span style="color:#0a6314;">通用角色</span>';
                }
            },
            { field: 'AdminName', width: 150, editor: 'text', align: 'center', title: '添加人' },
            {
                field: 'AddTime', width: 150, align: 'center', title: '添加时间', formatter: function (value, row) {
                    return Common.DateTimeFormatter(value);
                }
            },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '顺序', sortable: true },
            { field: 'IsUsable', title: '可用', width: 50, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else { return '<span style="color:red;">否</span>'; } } }
        ]],
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#RoleList').datagrid('resize', { height: h - 46 });
    });
    $('#OrganId').combogrid({
        panelWidth: 600,
        panelHeight: 400,
         url: '/admin/AdminCommon/OrganComboGridJson/',
        idField: 'OrganId',
        textField: 'OrganName',
        editable: false,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        columns: [[
        { field: 'OrganId', title: '编号', width: 60 },
        { field: 'OrganName', title: '名称', width: 100 },
        { field: 'TypeName', title: '类别', width: 90 },
        { field: 'LevelName', title: '等级', width: 90 }
        ]],
        toolbar: '#organbar'
    });
    createTopToolbar();
});
//数据新增跟编辑触发按钮
var dataEdit = function (icon, title) {
    saveCount = 0;
    $('#AddorUpdate').dialog({
        resizable: 'true', shadow: 'false', iconCls: icon, maximizable: 'true', title: title,
        buttons: [{
            text: '保存', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                $("#edit").form("submit", {
                     url: "/admin/SysRole/EditOrUpdateRole/",
                    onSubmit: function () {
                        var goon = $("#edit").form("validate");
                        if (goon == false) return false;
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
        text: '新增角色', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            $('#edit').form('clear');
            dataEdit('icon-add', "新增角色");
        }
    },
    edit: {
        text: '编辑角色', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            $("#edit").form("load", selectData);
            dataEdit('icon-edit', '编辑角色信息');
        }
    },
    remove: {
        text: '删除角色', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "删除角色可能会删除与角色有关的所有数据,你确定删除吗?", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/sysrole/DeleteRole/",
                        type: "post",
                        dataType: "html",
                        data: { roleId: selectData.RoleId },
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
    },
    setLimits: {
        text: '分配权限', id: 'setRolePM', disabled: true, iconCls: 'icon-setlimit', handler: function () {
            addTab("权限分配", "/SysRole/RoleLimtsSet/?roleId=" + selectData.RoleId);
        }
    }
};
//分配权限触发
function addTab(title, url) {
    if (!window.parent.window.$('#tabs').tabs('exists', title)) {
        add(title, url);
    }
    else {
        window.parent.window.$('#tabs').tabs('close', title);
        add(title, url);
    }
    function add(title, url) {
        window.parent.window.$('#tabs').tabs('add', {
            title: title,
            content: '<iframe scrolling="no" frameborder="0" src="' + url + '" style="overflow:hidden;width:100%;height:99%;"></iframe> ',
            closable: true
        });
    }
}
//刷新列表数据
function reloadList() {
    $("#AddorUpdate").dialog("close");
    $("#RoleList").datagrid("reload");
    $("#deleteBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
    $("#setRolePM").linkbutton({ disabled: true });
}
var listToolBar = [];
if (pMList.indexOf("3") != -1)//添加权限
    listToolBar.push(config.add);
if (pMList.indexOf("2") != -1)//编辑权限
    listToolBar.push(config.edit);
if (pMList.indexOf("4") != -1)//删除权限
    listToolBar.push(config.remove);
if (pMList.indexOf("5") != -1)
    listToolBar.push(config.setLimits);
listToolBar.push(config.reload);
//双击事件
function rowDbClick(rowIndex, row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        editDataIndex = rowIndex;
        if (pMList.indexOf("2") != -1&&(row.OrganId!=0||superadmin))//编辑权限控制
            $('#RoleList').datagrid('beginEdit', rowIndex);
    }
}
function CompleteEdit() {

}

//单机行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.RoleId != editData.RoleId) {
        $('#RoleList').datagrid('endEdit', editDataIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            editData.AddTime = Common.DateTimeFormatter(editData.AddTime);
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
        if (row.OrganId != 0 || superadmin) {
            $("#deleteBtn").linkbutton({ disabled: false });
            $("#editBtn").linkbutton({ disabled: false });
            $("#setRolePM").linkbutton({ disabled: false });
        } else {
            $("#deleteBtn").linkbutton({ disabled: true });
            $("#editBtn").linkbutton({ disabled: true });
            $("#setRolePM").linkbutton({ disabled: true });
        }
    }
}
//提交新增或修改的数据
function submitEdit(data, sucFunc) {
    $.ajax({
        async: false,
         url: '/admin/SysRole/EditOrUpdateRole/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    });
}
//创建顶部工具栏
function createTopToolbar() {
    $("#organbar").append("类别：<input style=\"width:85px;\" id=\"OrganType\" class=\"easyui-combobox\">&nbsp;");
    $("#organbar").append("等级：<input style=\"width:85px;\" id=\"OrganLevel\" class=\"easyui-combobox\">&nbsp;");
    $("#organbar").append("名称：<input type=\"text\" id=\"OrganName\" style=\"width:100px\">&nbsp;");
    $("#organbar").append("<a href=\"javascript:void(0)\" id=\"OrganSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#organbar").append("<a href=\"javascript:void(0)\" id=\"OrganClean\" class=\"easyui-linkbutton\" iconCls=\"icon-reset\">重置</a>");
    $("#OrganType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/sysorgan/OrganTypeComboJson/',
        function (data, textStatus) {
            data.unshift({ "key": "", 'value': '请选择类别' });
            $("#OrganType").combobox('loadData', data);
        },
        "json"
    );

    $("#OrganLevel").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/sysorgan/OrganLevelComboJson/',
        function (data, textStatus) {
            data.unshift({ 'key': "", 'value': '请选择等级' });
            $("#OrganLevel").combobox('loadData', data);
        },
        "json"
    );
    $("#OrganSelect").linkbutton().click(function () {
        var OrganType = $("#OrganType").combobox("getValue");
        var OrganLevel = $("#OrganLevel").combobox("getValue");
        var OrganName = $("#OrganName").val();
        $('#OrganId').combogrid('grid').datagrid("load", { 'TypeId': OrganType, 'Level': OrganLevel, 'OrganName': OrganName });
    });

    $("#OrganClean").linkbutton().click(function () {
        $("#OrganType").combobox("select", "");
        $("#OrganLevel").combobox("select", "");
        $("#OrganName").val("");
    });
}