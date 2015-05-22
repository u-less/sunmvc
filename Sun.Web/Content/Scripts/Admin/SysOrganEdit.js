
var saveCount = 0;//防止重复提交表单
function submitForm()
{
    $("#editform").form("submit", {
         url: "/admin/SysOrgan/EditOrUpdateOrgan/",
        onSubmit: function () {
            var v = $('#ParentId').combogrid('getValue');
            $('#ParentId').val(v);
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
                    var id = $("#OrganId").val();
                    if (id == '' || parseInt(id) == 0) {
                        $('#editform').form('clear');
                        saveCount = 0;
                    } else {
                        if (window.parent.window.$('#tabs').tabs('exists', '编辑机构')) {
                            window.parent.window.$('#tabs').tabs("close", '编辑机构');
                        }
                    }
                });
            }
            else
                $.messager.alert("操作失败", data);
        }
    });
}

$(function () {
    $("#TypeId").combobox({  url: '/admin/SysOrgan/OrganTypeComboJson/', valueField: 'key', textField: 'value', editable: false })
    $("#Level").combobox({  url: '/admin/SysOrgan/OrganLevelComboJson/', valueField: 'key', textField: 'value', editable: false })
    $('#ParentId').combogrid({
        panelWidth: 600,
        panelHeight: 400,
         url: '/admin/AdminCommon/OrganComboGridJson/',
        idField: 'OrganId',
        textField: 'OrganName',
        editable:false,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        columns: [[
        { field: 'OrganId', title: '编号', width: 60 },
        { field: 'OrganName', title: '名称', width: 100 },
        { field: 'TypeName', title: '类别', width: 90 },
        { field: 'LevelName', title: '等级', width: 90 },
        { field: 'ParentName', title: '父节点', width: 110 },
        ]]
    });
    createTopToolbar();

    $('#ParentId').combogrid('grid').datagrid({
        toolbar: '#tb'
    });

    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
})

//创建顶部工具栏
function createTopToolbar() {
    $("#tb").append("类别：<input style=\"width:85px;\" id=\"SearchType\" class=\"easyui-combobox\">&nbsp;");
    $("#tb").append("等级：<input style=\"width:85px;\" id=\"SearchLevel\" class=\"easyui-combobox\">&nbsp;");
    $("#tb").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:100px\">&nbsp;");
    $("#tb").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#tb").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\" iconCls=\"icon-reset\">重置</a>");
    $("#SearchType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.post
    (
        '/admin/sysorgan/OrganTypeComboJson/',
        function (data, textStatus) {
            data.unshift({ "key": "", 'value': '请选择类别' });
            $("#SearchType").combobox('loadData', data);
        },
        "json"
    );

    $("#SearchLevel").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.post
    (
        '/admin/sysorgan/OrganLevelComboJson/',
        function (data, textStatus) {
            data.unshift({ 'key': "", 'value': '请选择等级' });
            $("#SearchLevel").combobox('loadData', data);
        },
        "json"
    );
    $("#btnSelect").linkbutton().click(function () {
        var OrganType = $("#SearchType").combobox("getValue");
        var OrganLevel = $("#SearchLevel").combobox("getValue");
        var OrganName = $("#SearchName").val();
        $('#ParentId').combogrid('grid').datagrid("load", { 'TypeId': OrganType, 'Level': OrganLevel, 'OrganName': OrganName });
    });
    $("#btnClean").linkbutton().click(function () {
        $("#SearchType").combobox("select", "");
        $("#SearchLevel").combobox("select", "");
        $("#SearchName").val("");
    });
}
