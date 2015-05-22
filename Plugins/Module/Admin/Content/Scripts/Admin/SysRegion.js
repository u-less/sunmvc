
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
$(function () {
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").treegrid({
         url: "/admin/sysregion/RegionGridJson/",
        title: '地区管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon_grid',
        rownumbers: true,
        animate: true,
        fitColumns: true,
        idField: 'RegionId',
        treeField: 'Name',
        columns: [[
            { field: 'Name', width: 200, editor: 'text', align: 'left', title: '名称' },
            { field: 'RegionId', width: 100, align: 'left', title: '编号' },
            { field: 'PostCode', width: 110, editor: 'text', align: 'center', title: '邮编' },
            { field: 'Code', width: 110, editor: 'text', align: 'center', title: '区号' },
            { field: 'Spell', width: 110, editor: 'text', align: 'center', title: '拼音' },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '排序' },
            { field: 'IsHot', title: '热点城市', width: 50, editor: { type: 'checkbox', options: { on: 1, off: 0 } }, align: 'center', formatter: function (value, row) { if (value == 1) return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } }

        ]],
        onAfterEdit: CompleteEdit,
        onDblClickRow: rowDbClick,
        onLoadSuccess: dataLoadSuccess,
        onClickRow: rowClick,
        onBeforeExpand: beforeLoad,
        toolbar: listToolBar
    });
    //createTopToolbar();
    $(window).resize(function () {
        var h = window.parent.window.$("#tabs").height();
        $('#List').datagrid('resize', { height: h - 46 });
    });
})
//创建顶部工具栏
function createTopToolbar() {
    $(".datagrid-toolbar").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\"  iconCls=\"icon-reset\">重置</a>");

    $("#btnClean").linkbutton().click(function () {
        $("#SearchName").val("");
    });

    $("#btnSelect").linkbutton().click(function () {
        var RegionName = $("#SearchName").val();
        $("#List").treegrid('load', {
            regionname: RegionName
        });
    });
}

var config = {
    add: {
        text: '新增地区', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增地区', '/sysregion/Edit/', 'icon-add');
        }
    },
    edit: {
        text: '编辑地区', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑地区', '/sysregion/Edit/?regionId=' + selectData.RegionId, 'icon-edit');
        }
    },
    remove: {
        text: '删除地区', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/sysregion/DeleteRegion/",
                        type: "post",
                        dataType: "html",
                        data: { regionId: selectData.RegionId },
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
    $("#List").treegrid('options').url = '/sysregion/RegionGridJson/';
    $("#List").treegrid("reload");
    $("#List").treegrid("unselectAll");
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
function rowDbClick(row) {
    if (row) {
        selectData = cloneObj(row);//克隆原始数据
        editData = row;
        if (pMList.indexOf("2") != -1)//编辑权限控制
            $('#List').treegrid('beginEdit', row.RegionId);
    }
}
function CompleteEdit() {

}

//单击行事件，如果编辑某行，点击其它行后自动同步到数据
function rowClick(row) {
    if (editData != undefined && editData != null && row.RegionId != editData.RegionId) {
        $('#List').treegrid('endEdit', editData.RegionId);
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
         url: '/admin/sysregion/EditOrUpdateRegion/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}

//数据加载完成执行方法
function dataLoadSuccess(row, data) {
}

function beforeLoad(row, param) {
    // 异步加载
    if (row) {
        $(this).treegrid('options').url = '/sysregion/RegionGridJson/?parentId=' + row.RegionId;
    }
}