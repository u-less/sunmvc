
var saveCount = 0;//防止重复提交表单
function submitForm()
{
    $("#editform").form("submit", {
         url: "/admin/SysUserInfo/EditOrUpdateUser/",
        onSubmit: function () {
            var vr = $("#editform").form("validate");
            if (vr == false) return false;
            if (saveCount == 0) saveCount = 1;
            else {
                if (!window.confirm("已经点击过保存按钮,确定再次保存?"))
                    return false;
            }
        },
        success: function (data) {
            if (data == 'True') {
                $.messager.confirm("操作提醒", "操作成功", function (r) {
                    var id = $("#UserId").val();
                    if (id == '' || parseInt(id) == 0) {
                        $('#editform').form('clear');
                        saveCount = 0;
                    } else {
                        if (window.parent.window.$('#tabs').tabs('exists', '编辑用户')) {
                            window.parent.window.$('#tabs').tabs("close", '编辑用户');
                        }
                    }
                });
            }
            else
                $.messager.alert("操作失败", data);
        }
    });
}

function clearForm() {
    $('#editform').form('clear');
    $('#LoginId').val(loginid);
}

$(function () {
    $("#RoleId").combobox({  url: '/admin/SysUserInfo/GetRoleCombox/', valueField: 'key', textField: 'value', editable: false })
    $('#OrganId').combogrid({
        panelWidth: 600,
        panelHeight: 400,
         url: '/admin/AdminCommon/OrganComboGridJson/',
        editable: false,
        multiple: true,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        idField: 'OrganId',
        textField: 'OrganName',
        columns: [[
        { field: 'OrganId', title: '编号', width: 60 },
        { field: 'OrganName', title: '名称', width: 100 },
        { field: 'TypeName', title: '类别', width: 90 },
        { field: 'LevelName', title: '等级', width: 90 }
        ]],
        toolbar: '#organbar'
    });
    createTopToolbar();

    $("#UserType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetUserTypeCombox/',
        async: false,
        success: function (data) {
            $("#UserType").combobox('loadData', data);
        }
    });

    $("#States").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetUserStatusCombox/',
        async: false,
        success: function (data) {
            $("#States").combobox('loadData', data);
        }
    });

    $("#RoleId").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetRoleCombox/',
        async: false,
        success: function (data) {
            $("#RoleId").combobox('loadData', data);
        }
    });

    $("#RegionId").combotree({
         url: '/admin/common/RegionComboTreeJson/?idLength=4'
    })

    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
    if ($("#LoginId").attr("readonly") != "readonly") {
        $('#LoginId').validatebox({
            required: true,
            validType: "remote['/AdminValid/AccountNotExits/','LoginId']",
            invalidMessage: "该用户名已经存在！",
            delay: 1500
        });
    }
});
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

