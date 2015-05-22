
var saveCount = 0;//防止重复提交表单
function submitForm() {
    $("#editform").form("submit", {
         url: "/admin/WebConfOption/EditOrUpdateOption/",
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
                $.messager.alert("操作提醒", "操作成功");
                var id = $("#OptionId").val();
                if (id == ''|| parseInt(id) == 0) {
                    $('#editform').form('clear');
                    saveCount = 0;
                } else {
                    if (window.parent.window.$('#tabs').tabs('exists', title)) {
                        window.parent.window.$('#tabs').tabs("close", title);
                    }
                }
            }
            else
                $.messager.alert("操作失败", data);
        }
    })
}

$(function () {
    $("#ConfigId").combobox({ valueField: 'key', textField: 'value', editable: false })
    $("#GroupId").combobox({
         url: '/admin/WebConfig/GetGroupCombox/', valueField: 'key', textField: 'value', editable: false,
        onChange: function (newValue, oldValue) {
            $("#ConfigId").combobox("clear").combobox({  url: '/admin/WebConfig/GetConfComboxByGroupId/?groupid=' + newValue });
        }
    })
    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
})

