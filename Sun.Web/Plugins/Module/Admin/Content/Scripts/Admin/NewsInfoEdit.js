
var saveCount = 0;//防止重复提交表单
function submitForm() {
    if (editor.hasContents()) { //此处以非空为例
        editor.sync();
        $("#editform").form("submit", {
            url: "/NewsInfo/EditOrUpdateNews/?newskey=" + newsKey,
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
                        var id = $("#NewsId").val();
                        if (id == '' || parseInt(id) == 0) {
                            clearForm();
                            saveCount = 0;
                        } else {
                            if (window.parent.window.$('#tabs').tabs('exists', '编辑新闻资讯')) {
                                window.parent.window.$('#tabs').tabs("close", '编辑新闻资讯');
                            }
                        }
                    });
                }
                else
                    $.messager.alert("操作失败", data);
            }
        })
    }
}

function clearForm() {
    $("#Title").val("");
    $("#Source").val("");
    $("#Author").val("");
    $("#Abstract").val("");
    $("#KeyWord").val("");
    $("#SortIndex").val("");
    $("#Content").attr("value",'');
}

$(function () {
    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
})
