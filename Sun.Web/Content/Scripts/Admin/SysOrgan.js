
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
var parentId = 0;
$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#OrganList").datagrid({
         url: "/admin/sysorgan/OrganGridJson/",
        title: '客栈管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50,80],
        fitColumns: true,
        singleSelect: true,
        idField: 'organid',
        columns: [[
            { field: 'OrganId', width: 100, align: 'center', title: '编号' },
            { field: 'OrganName', width: 100, editor: 'text', align: 'center', title: '名称' },
            { field: 'TypeName', width: 110, align: 'center', title: '类别' },
            { field: 'LevelName', width: 110, align: 'center', title: '等级' },
            { field: 'ParentName', width: 110, align: 'center', title: '上级客栈', formatter: function (value, row) {if (value ==null) value = '顶级机构'; return value; } },
            { field: 'State', title: '状态', width: 50, editor: { type: 'checkbox', options: { on: 1, off: 0 } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">可用</span>'; else return '<span style="color:red;">不可用</span>'; } },
            {
                field: 'ParentId', width: 80, align: 'center', title: '管理', formatter: function (value, row) {
                    return '<a href="javascript:void(0)" data-id="' + row.OrganId + '" iconCls="icon-book_next" class="list-link btnnext" style="margin-left:10px;" >下级客栈</a>';
                }
            }
        ]], onLoadSuccess: function () {
            $('.list-link').linkbutton();
            $('.btnnext').click(function () { parentId = $(this).attr('data-id'); $("#OrganList").datagrid("load", { 'parentid': parentId }); $("#btnClean").click;});
        },
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onClickRow: rowClick,
        toolbar: listToolBar
    });
    createTopToolbar();
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#OrganList').datagrid('resize', { height: h - 46 });
    });
})
//创建顶部工具栏
function createTopToolbar() {
    $(".datagrid-toolbar").append("类别：<input style=\"width:auto;\" id=\"SearchType\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("等级：<input style=\"width:auto;\" id=\"SearchLevel\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\"  iconCls=\"icon-reset\">重置</a>");

    $("#SearchType").combobox({
        valueField: 'key', textField: 'value',editable: false
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
        valueField: 'key', textField: 'value',editable: false
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

    $("#btnClean").linkbutton().click(function () {
        $("#SearchType").combobox("select", "");
        $("#SearchLevel").combobox("select", "");
        $("#SearchName").val("");
    });

    $("#btnSelect").linkbutton().click(function () { GridLoad();});
}
function GridLoad()
{
    var OrganType = $("#SearchType").combobox("getValue");
    var OrganLevel = $("#SearchLevel").combobox("getValue");
    var OrganName = $("#SearchName").val();
    $("#OrganList").datagrid("load", { 'typeid': OrganType, 'level': OrganLevel, 'organname': OrganName,'parentid':parentId });
}
var config = {
    add: {
        text: '新增机构', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增机构', '/sysorgan/Edit/', 'icon-add');
        }
    },
    edit: {
        text: '编辑机构', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑机构', '/sysorgan/Edit/?organId=' + selectData.OrganId, 'icon-edit');
        }
    },
    remove: {
        text: '删除机构', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/sysorgan/DeleteOrgan/",
                        type: "post",
                        dataType: "html",
                        data: { organId: selectData.OrganId },
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
            load();
        }
    },reset:{
        text: '重置刷新', id: 'btnReset', iconCls: 'icon-reset', handler: function () {
            resetLoad();
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
function resetLoad()
{
    parentId = 0;
    resetOp();
    GridLoad();
}
function load()
{
    $("#OrganList").datagrid("load");
    resetOp();
}
//刷新列表数据
function resetOp() {
    $("#AddorUpdate").dialog("close");
    $("#OrganList").datagrid("unselectAll");
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
listToolBar.push(config.reset);
//双击事件
function rowDbClick(rowIndex, row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        editDataIndex = rowIndex;
        if (pMList.indexOf("2") != -1)//编辑权限控制
            $('#OrganList').datagrid('beginEdit', rowIndex);
    }
}
function CompleteEdit() {

}

//单击行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(index, row) {
    if (editData != undefined && editData != null && row.OrganId != editData.OrganId) {
        $('#OrganList').datagrid('endEdit', editDataIndex);
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
         url: '/admin/sysorgan/EditOrUpdateOrgan/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    });
}