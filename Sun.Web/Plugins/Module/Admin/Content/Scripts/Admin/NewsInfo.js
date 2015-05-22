
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
        url: "/newsinfo/NewsInfoGridJson/?newskey=" + newsKey,
        title: '新闻管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        idField: 'NewsId',
        columns: [[
            {
                field: 'NewsId', width: 80, align: 'center', title: '编号', formatter: function (value, row) {
                    switch (newsKey) {
                        case "newsxw":
                        case "newsgg":
                        case "newshd":
                            return value + '<span style="color:blue" onclick="javascript:window.open(\'/news/detail/' + value + '\',\'_blank\')">(浏览)</span>';
                        default:
                            return value + '<span style="color:blue" onclick="javascript:window.open(\'/news/about/' + value + '\',\'_blank\')">(浏览)</span>';
                            break;
                    }
                }
            },
            { field: 'Title', width: 210, editor: 'text', align: 'center', title: '标题' },
            { field: 'Source', width: 110, editor: 'text', align: 'center', title: '来源' },
            { field: 'Author', width: 110, editor: 'text', align: 'center', title: '作者' },
            { field: 'AddTime', width: 110, align: 'center', title: '添加时间', formatter: function (value, row) { return Common.DateTimeFormatter(value); } },
            { field: 'ReleaseTime', width: 110, align: 'center', title: '发布时间', formatter: function (value, row) { return Common.DateTimeFormatter(value); } },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '排序' },
            { field: 'AdminName', width: 110, align: 'center', title: '管理员' }
        ]],
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
})
//创建顶部工具栏
function createTopToolbar() {
    $(".datagrid-toolbar").append("标题：<input type=\"text\" id=\"SearchTitle\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("作者：<input type=\"text\" id=\"SearchAuthor\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("内容：<input type=\"text\" id=\"SearchContent\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\" iconCls=\"icon-reset\">重置</a>");

    $("#SearchType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $("#btnClean").linkbutton().click(function () {
        $("#SearchTitle").val("");
        $("#SearchAuthor").val("");
        $("#SearchContent").val("");
    });

    $("#btnSelect").linkbutton().click(function () {
        var NewsTitle = $("#SearchTitle").val();
        var NewsAuthor = $("#SearchAuthor").val();
        var NewsContent = $("#SearchContent").val();
        $("#List").datagrid("load", { 'newskey':newsKey,'title': NewsTitle, 'author': NewsAuthor, 'content': NewsContent });
    });
}

var config = {
    add: {
        text: '新增新闻资讯', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增新闻资讯', '/newsinfo/Edit/?newskey='+newsKey, 'icon-add');
        }
    },
    edit: {
        text: '编辑新闻资讯', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑新闻资讯', '/newsinfo/Edit/?newskey=' + newsKey + '&newsId=' + selectData.NewsId, 'icon-edit');
        }
    },
    remove: {
        text: '删除新闻资讯', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                        url: "/newsinfo/DeleteNews/?newskey="+newsKey,
                        type: "post",
                        dataType: "html",
                        data: { newsId: selectData.NewsId },
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
    $("#List").datagrid("load");
    $("#List").datagrid("unselectAll");
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
            $('#List').datagrid('beginEdit', rowIndex);
    }
}
function CompleteEdit() {

}

//单击行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.NewsId != editData.NewsId) {
        $('#List').datagrid('endEdit', editDataIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            editData.AddTime = Common.DateTimeFormatter(editData.AddTime);
            editData.ReleaseTime = Common.DateTimeFormatter(editData.ReleaseTime);
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
        url: '/newsinfo/EditOrUpdateNews/?newskey='+newsKey,
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}