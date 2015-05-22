
var saveCount = 0;//防止重复提交表单
function submitForm() {
    $("#editform").form("submit", {
         url: "/admin/WebConfig/EditOrUpdateConf/",
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
                    var id = $("#configId").val();
                    if (id==''||parseInt(id) == 0) {
                        $('#editform').form('clear');
                        saveCount = 0;
                    } else {
                        if (window.parent.window.$('#tabs').tabs('exists', '编辑配置项')) {
                            window.parent.window.$('#tabs').tabs("close", '编辑配置项');
                        }
                    }
                });
            }
            else
                $.messager.alert("操作失败", data);
        }
    })
}

$(function () {
    $("#CGroup").combobox({  url: '/admin/WebConfig/GetGroupCombox/', valueField: 'key', textField: 'value', editable: false })
    $("#CType").combobox({  url: '/admin/WebConfig/GetOpTypeCombox/', valueField: 'key', textField: 'value', editable: false })
    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
    if ($("#CKey").attr("readonly") != "readonly") {
        $('#CKey').validatebox({
            required: true,
            validType: "remote['/admin/AdminValid/ConfigKeyExits/','CKey']",
            delay: 1000
        });
    }
})

