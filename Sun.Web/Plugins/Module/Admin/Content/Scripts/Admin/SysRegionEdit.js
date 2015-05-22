
var saveCount = 0;//防止重复提交表单
function submitForm()
{
    $("#editform").form("submit", {
         url: "/admin/SysRegion/EditOrUpdateRegion/",
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
                    var id = $("#RegionId").val();
                    if (id==''||parseInt(id) == 0) {
                        $('#editform').form('clear');
                        saveCount = 0;
                    } else {
                        if (window.parent.window.$('#tabs').tabs('exists', '编辑地区')) {
                            window.parent.window.$('#tabs').tabs("close", '编辑地区');
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
    $('#ParentId').combotree({
         url: '/admin/common/RegionComboTreeJson/?idLength=4'
    })
    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
})

