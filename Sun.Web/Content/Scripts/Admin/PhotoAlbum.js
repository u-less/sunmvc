
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
var dataSeason1 = [{ 'key': "", 'value': '请选择时节' }, { 'key': 1, 'value': '一月' }, { 'key': 2, 'value': '二月' }, { 'key': 3, 'value': '三月' }, { 'key': 4, 'value': '四月' }, { 'key': 5, 'value': '五月' }, { 'key': 6, 'value': '六月' }, { 'key': 7, 'value': '七月' }, { 'key': 8, 'value': '八月' }, { 'key': 9, 'value': '九月' }, { 'key': 10, 'value': '十月' }, { 'key': 11, 'value': '十一月' }, { 'key': 12, 'value': '十二月' }];
$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
        url: "/AdminAlbum/GetGridPageJson/",
        title: '画册管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        nowrap:false,
        singleSelect: true,
        idField: 'AlbumId',
        columns: [[
            { field: 'AlbumId', width: 50, align: 'center', title: '编号', formatter: function (value, row) { return value + '<span style="color:blue" onclick="javascript:window.open(\'/album/detail/' + value + '\',\'_blank\')">(浏览)</span>' } },
            { field: 'Title', width: 120, align: 'center', title: '标题'},
            {
                field: 'LogoSrc', width: 100, align: 'center', title: '封面图', formatter: function (value, row) {
                    return '<img src="'+value+'" height="100"/>';
                }
            },
            { field: 'BornsName', width: 100, align: 'center', title: '目的地' },
            { field: 'AltradionsName', width: 110, align: 'center', title: '景点' },
            { field: 'DiscussCount', width: 50, align: 'center', editor: 'text', title: '评价数' },
            { field: 'Score', width: 50, align: 'center', editor: 'text', title: '评分' },
            { field: 'Good', width: 50, align: 'center', editor: 'text', title: '赞' },
            { field: 'UserName', width: 50, align: 'center', title: '作者' },
             {
                 field: 'Publish', title: '发布', width: 40, formatter: function (value, row) {
                     var v = Common.DateTimeFormatter(value);
                     if (v == dTime)
                         return '<span style="color:red;">未发布</span>';
                     else
                         return '<span style="color:#0a6314;">已发布</span>';
                 }
             }, {
                 field: 'Finish', title: '完结', width: 40, formatter: function (value, row) {
                     var v = Common.DateTimeFormatter(value);
                     if (v == dTime)
                         return '<span style="color:red;">未完结</span>';
                     else
                         return '<span style="color:#0a6314;">已完结</span>';
                 }
             },
            { field: 'HighQuality', title: '优质', width: 40, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            { field: 'IsClass', title: '经典', width: 40, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            {
                field: 'IsTui', title: '推荐', width: 40, editor: { type: 'checkbox', options: { on: true, off: false } }, formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; }
            },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '排序' },
            {
                field: 'AddTime', width: 70, align: 'center', title: '添加时间', formatter: function (value, row) {
                    return Common.DateTimeFormatter(value);
                }
            },
            { field: 'IsUsable', title: '可用', width: 50, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">可用</span>'; else return '<span style="color:red;">不可用</span>'; } }
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
    $(".datagrid-toolbar").append("目的地：<input style=\"width:130px;\" id=\"bourn\">&nbsp;");
    $(".datagrid-toolbar").append("推荐：<input style=\"width:50px;\" id=\"SearchIsTui\">&nbsp;");
    $(".datagrid-toolbar").append("来源：<input style=\"width:50px;\" id=\"Source\">&nbsp;");
    $(".datagrid-toolbar").append("可用：<input style=\"width:50px;\" id=\"IsUsable\">&nbsp;");
    $(".datagrid-toolbar").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" iconcls=\"icon-reset\" id=\"btnClean\">重置</a>");

    //目的地comboGrid
    $("#bourn").combogrid({
        panelWidth: 600,
        panelHeight: 400,
        url: "/AdminAlbum/BournGridJson/",
        iconCls: 'icon-table',
        loadMsg: '正在加载,请稍等。。。',
        rownumbers: true,
        pagination: true,
        pageList: [30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        idField: 'BournId',
        textField: 'BName',
        columns: [[
            { field: 'BournId', width: 100, align: 'center', title: '编号' },
            { field: 'BName', width: 110, align: 'center', title: '名称' },
            { field: 'RegionName', width: 100, align: 'center', title: '地区' },
            { field: 'DiscussCount', width: 110, align: 'center', title: '点评次数' },
            { field: 'Score', width: 110, align: 'center', title: '用户评分' },
            { field: 'IsTui', title: '推荐', width: 50, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            { field: 'SortIndex', width: 80, align: 'center', title: '排序' }
        ]],
        toolbar: '#bourntool'
    });
    createbournTopToolbar();//渲染目的地toolbar

    $("#SearchIsTui").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    var dataIsTui = new Array();
    dataIsTui.push({ 'key': "", 'value': '全部' }, { 'key': "true", 'value': '是' }, { 'key': "false", 'value': '否' });
    $("#SearchIsTui").combobox('loadData', dataIsTui);

    var source = [{ key: '', value: '全部' }, { key: 'true', value: '官方' }, { key: 'false', value: '个人' }];
    $("#Source").combobox({ valueField: 'key', textField: 'value', editable: false, data: source });
    var usable = [{ key: '', value: '全部' }, { key: 'true', value: '可用' }, { key: 'false', value: '不可用' }];
    $("#IsUsable").combobox({ valueField: 'key', textField: 'value', editable: false, data: usable });
    $("#btnClean").linkbutton().click(function () {
        $("#bourn").combogrid("clear");
        $("#SearchIsTui").combobox("select", "");
        $("#Source").combobox("select", "");
        $("#IsUsable").combobox("select", "");
        $("#SearchName").val("");
    });

    $("#btnSelect").linkbutton().click(function () {
        var name = $("#SearchName").val();
        var bournid = $("#bourn").combogrid("getValue");
        var tui = $("#SearchIsTui").combobox("getValue");
        var source = $("#Source").combobox("getValue");
        var usable = $("#IsUsable").combobox("getValue");
        $("#List").datagrid("load", {'title': name, 'bournid': bournid, 'altradionId': altradionId,  'istui': tui, 'source': source, 'regionid': region, 'isusable': usable });
    });
}

//创建目的地顶部工具栏
function createbournTopToolbar() {
    $("#bourntool").append("类别：<input style=\"width:130px;\" id=\"bournType\">&nbsp;");
    $("#bourntool").append("主题：<input style=\"width:130px;\" id=\"bournTheme\">&nbsp;");
    $("#bourntool").append("时节：<input style=\"width:130px;\" id=\"bournSeason\"><br />");
    $("#bourntool").append("名称：<input type=\"text\" id=\"bournName\" style=\"width:200px\">&nbsp;");
    $("#bourntool").append("推荐：<input style=\"width:70px;\" id=\"bournIsTui\">&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"bournbtnSelect\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"bournbtnClean\"  iconcls=\"icon-reset\">重置</a>");
    $("#bourntool>#bournType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/bourninfo/BournTypeComboJson/',
        async: false,
        success: function (data) {
            data.unshift({ 'key': "", 'value': '请选择类别' });
            $("#bourntool>#bournType").combobox('loadData', data);
        }
    });

    $("#bourntool>#bournTheme").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
        url: '/bourninfo/BournThemeComboJson/',
        async: false,
        success: function (data) {
            data.unshift({ 'key': "", 'value': '请选择主题' });
            $("#bourntool>#bournTheme").combobox('loadData', data);
        }
    });

    $("#bourntool>#bournSeason").combobox({
        valueField: 'key', textField: 'value', editable: false,data:dataSeason1
    });

    $("#bourntool>#bournIsTui").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    var dataIsTui = new Array();
    dataIsTui.push({ 'key': "", 'value': '全部' }, { 'key': "true", 'value': '是' }, { 'key': "false", 'value': '否' });
    $("#bourntool>#bournIsTui").combobox('loadData', dataIsTui);

    $("#bourntool>#bournbtnClean").linkbutton().click(function () {
        $("#bourntool>#bournType").combobox("select", "");
        $("#bourntool>#bournTheme").combobox("select", "");
        $("#bourntool>#bournSeason").combobox("select", "");
        $("#bourntool>#bournIsTui").combobox("select", "");
        $("#bourntool>#bournName").val("");
    });

    $("#bourntool>#bournbtnSelect").linkbutton().click(function () {
        var BournType = $("#bourntool>#bournType").combobox("getValue");
        var BournTheme = $("#bourntool>#bournTheme").combobox("getValue");
        var BournSeason = $("#bourntool>#bournSeason").combobox("getValue");
        var BournName = $("#bourntool>#bournName").val();
        var IsTui = $("#bourntool>#bournIsTui").combobox("getValue");

        $("#bourn").combogrid("grid").datagrid("load", { 'btype': BournType, 'btheme': BournTheme, 'bseason': BournSeason, 'bname': BournName, 'istui': IsTui });
    });
}

var config = {
    lookImgs: {
        text: '查看图片', id: 'lookImg', iconCls: 'icon-image', disabled: true, handler: function () {
            addTab('查看图片', '/adminAlbum/LookPic/' + selectData.AlbumId, 'icon-edit');
        }
    },
    nousable: {
        text: '删除', id: 'nousableBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                        url: "/adminalbum/Delete/?complete=false",
                        type: "post",
                        dataType: "html",
                        data: { id: selectData.AlbumId },
                        success: function (r) {
                            if (parseInt(r)>0) {
                                $.messager.alert("操作提醒", "删除成功");
                                reloadList();
                            }
                            else {
                                $.messager.alert("操作提醒", "操作失败");
                            }
                        },
                        error: function () { $.messager.alert("操作提醒", "连接服务器失败"); }
                    }
                  )
                }
            });
        }
    }, remove: {
        text: '完全删除', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                        url: "/adminalbum/Delete/?complete=true",
                        type: "post",
                        dataType: "html",
                        data: { id: selectData.AlbumId },
                        success: function (r) {
                            if (parseInt(r) > 0) {
                                $.messager.alert("操作提醒", "删除成功");
                                reloadList();
                            }
                            else {
                                $.messager.alert("操作提醒", "操作失败");
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
    $("#AddorUpdate").dialog("close");
    $("#List").datagrid("load");
    $("#List").datagrid("unselectAll");
    $("#deleteBtn").linkbutton({ disabled: true }); 
    $("#nousableBtn").linkbutton({ disabled: true });
    $("#editBtn").linkbutton({ disabled: true });
    $("#lookImg").linkbutton({ disabled: true });
}
var listToolBar = [];
if (pMList.indexOf("4") != -1)//删除画册权限
    listToolBar.push(config.nousable);
if (pMList.indexOf("5") != -1)
    listToolBar.push(config.lookImgs);//查看画册图片
if (pMList.indexOf("6") != -1)
    listToolBar.push(config.remove);//查看画册图片
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
    if (editData != undefined && editData != null && row.AlbumId != editData.AlbumId) {
        $('#List').datagrid('endEdit', editDataIndex);
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
        $("#lookImg").linkbutton({ disabled: false });
        $("#nousableBtn").linkbutton({ disabled: false });
    }
}
//修改的数据
function submitEdit(data, sucFunc) {
    $.ajax({
        async: false,
        url: '/adminAlbum/GridEdit/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}