
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
var dataType;
var dataSeason1 = [{ 'key': "", 'value': '请选择时节' }, { 'key': 1, 'value': '一月' }, { 'key': 2, 'value': '二月' }, { 'key': 3, 'value': '三月' }, { 'key': 4, 'value': '四月' }, { 'key': 5, 'value': '五月' }, { 'key': 6, 'value': '六月' }, { 'key': 7, 'value': '七月' }, { 'key': 8, 'value': '八月' }, { 'key': 9, 'value': '九月' }, { 'key': 10, 'value': '十月' }, { 'key': 11, 'value': '十一月' }, { 'key': 12, 'value': '十二月' }];

$(function () {
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
        url: "/specialtyinfo/SpecialtyInfoGridJson/",
        title: '特产管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        idField: 'specialtyid',
        columns: [[
            { field: 'SpecialtyId', width: 50, align: 'center', title: '编号', formatter: function (value, row) { return value + '<span style="color:blue" onclick="javascript:window.open(\'/specialty/detail/' + value + '\',\'_blank\')">(浏览)</span>' } },
            { field: 'SpecialtyName', width: 110, editor: 'text', align: 'center', title: '名称' },
            {
                field: 'TypeId', width: 150, align: 'center', title: '类型', formatter: function (value, row) {
                    return indexKey(dataType, value);
                }
            },
            //{
            //    field: 'CLogo', width: 60, align: 'center', title: '封面图', formatter: function (value, row) {
            //        return '<image src="' + value + '" onload="javascript:DrawImage(this,100,32)" width="100" height="32">';
            //    }
            //},
            {
                field: 'Bourn', width: 80, align: 'center', title: '目的地', formatter: function (value, row) {
                    var result = '', cache = $(window).data("bids-" + value);
                    if (cache == null) {
                        $.ajax({
                            type: "post",
                            url: '/common/SimpleBournsByIds/?ids=' + value,
                            async: false,
                            success: function (data) {
                                $.each(data, function (i, v) {
                                    if (i > 0) result += ",";
                                    result += v.BName;
                                })
                                $(window).data("bids-" + value, result)
                            }
                        });
                    } else result = cache;
                    return result;
                }
            },
            {
                field: 'Altradions', width: 80, align: 'center', title: '景点', formatter: function (value, row) {
                    var result = '', cache = $(window).data("aids-" + value);
                    if (cache == null) {
                        $.ajax({
                            type: "post",
                            url: '/common/SimpleAltradionsByIds/?ids=' + value,
                            async: false,
                            success: function (data) {
                                $.each(data, function (i, v) {
                                    if (i > 0) result += ",";
                                    result += v.AltradionName;
                                });
                                $(window).data("aids-" + value, result)
                            }
                        });
                    } else result = cache; return result;
                }
            },
            { field: 'AddTime', width: 110, align: 'center', title: '添加时间', formatter: function (value, row) { return Common.DateTimeFormatter(value); } },
            { field: 'IsTui', title: '推荐', width: 50, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            { field: 'IsUsable', title: '可用', width: 50, editor: { type: 'checkbox', options: { on: true, off: false } }, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            { field: 'SortIndex', width: 50, editor: 'text', align: 'center', title: '排序' }
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
    $(".datagrid-toolbar").append("类型：<input style=\"width:auto;\" id=\"SearchType\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("目的地：<input style=\"width:auto;\" id=\"SearchBourn\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("景点：<input style=\"width:auto;\" id=\"SearchAltradions\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:150px\">&nbsp;");
    $(".datagrid-toolbar").append("推荐：<input style=\"width:70px;\" id=\"SearchIsTui\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("可用：<input style=\"width:70px;\" id=\"SearchIsUsable\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\" iconCls=\"icon-reset\">重置</a>");
    $("#SearchType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/specialtyinfo/SpecialtyInfoTypeComboJson/',
        async: false,
        success: function (data) {
            dataType = data;
            data.unshift({ 'key': "", 'value': '请选择类型' });
            $("#SearchType").combobox('loadData', data);
        }
    });

    $('#SearchBourn').combogrid({
        panelWidth: 600,
        panelHeight: 400,
        url: '/bourninfo/BournGridJson/',
        idField: 'BournId',
        textField: 'BName',
        editable: false,
        pagination: true,
        pageList: [5, 10],
        fitColumns: true,
        columns: [[
        { field: 'BournId', title: '编号', width: 60 },
        { field: 'BName', title: '名称', width: 100 },
        {
            field: 'BTheme', width: 110, align: 'center', title: '主题', formatter: function (value, row) {
                return splitKey(dataTheme1, value);
            }
        },
        {
            field: 'BType', width: 110, align: 'center', title: '类型', formatter: function (value, row) {
                return splitKey(dataType1, value);
            }
        },
        {
            field: 'BSeason', width: 110, align: 'center', title: '时节', formatter: function (value, row) {
                return splitKey(dataSeason1, value);
            }
        }
        ]],
        toolbar: '#bourntool'
    });
    createBournsToolbar();

    //景点
    $("#SearchAltradions").combogrid({
        panelWidth: 640,
        panelHeight: 400,
        url: "/Altradions/GridPageList/",
        iconCls: 'icon-table',
        loadMsg: '正在加载,请稍等。。。',
        rownumbers: true,
        pagination: true,
        pageList: [30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        editable: false,
        idField: 'AltradionId',
        textField: 'AltradionName',
        columns: [[
            { field: 'AltradionId', width: 100, align: 'center', title: '编号' },
            { field: 'RegionName', width: 100, align: 'center', title: '地区' },
            { field: 'AltradionName', width: 110, editor: 'text', align: 'center', title: '名称' },
            {
                field: 'Level', width: 110, align: 'center', title: '等级', formatter: function (value, row) {
                    return indexKey(dataLevel2, value);
                }
            },
            { field: 'IsTui', title: '推荐', width: 50, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">是</span>'; else return '<span style="color:red;">否</span>'; } },
            { field: 'IsUsable', title: '可用', width: 50, align: 'center', formatter: function (value, row) { if (value == true || value == 'true') return '<span style="color:#0a6314;">可用</span>'; else return '<span style="color:red;">不可用</span>'; } },
            { field: 'SortIndex', width: 80, align: 'center', title: '排序' }
        ]], toolbar: '#altradionstool'
    });
    createAltradionsToolbar();

    var dataIsTui = [{ 'key': "", 'value': '全部' }, { 'key': "true", 'value': '是' }, { 'key': "false", 'value': '否' }];
    $("#SearchIsTui").combobox({
        valueField: 'key', textField: 'value', editable: false, data: dataIsTui
    });
    $("#SearchIsUsable").combobox({
        valueField: 'key', textField: 'value', editable: false, data: dataIsTui
    });

    $("#btnClean").linkbutton().click(function () {
        $("#SearchType").combobox("select", "");
        $("#SearchBourn").combogrid("clear");
        $("#SearchAltradions").combogrid("clear");
        $("#SearchIsTui").combobox("select", "");
        $("#SearchIsUsable").combobox("select", "");
        $("#SearchName").val("");
    });

    $("#btnSelect").linkbutton().click(function () {
        var SpecType = $("#SearchType").combobox("getValue");
        var SpecBourn = $("#SearchBourn").combogrid("getValue");
        var SpecAltradion = $("#SearchAltradions").combogrid("getValue");
        var SpecName = $("#SearchName").val();
        var IsTui = $("#SearchIsTui").combobox("getValue");
        var IsUsable = $("#SearchIsUsable").combobox("getValue");
        $("#List").datagrid("load", { 'typeid': SpecType, 'bourn': SpecBourn, 'altradions': SpecAltradion, 'specialtyname': SpecName, 'istui': IsTui, 'isusable': IsUsable });
    });
}

var config = {
    add: {
        text: '新增特产', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增特产', '/specialtyinfo/Edit/', 'icon-add');
        }
    },
    edit: {
        text: '编辑特产', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑特产', '/specialtyinfo/Edit/?specialtyId=' + selectData.SpecialtyId, 'icon-edit');
        }
    },
    remove: {
        text: '删除特产', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("删除提醒", "确定删除吗？删除后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                        url: "/specialtyinfo/DeleteSpecialtyInfo/",
                        type: "post",
                        dataType: "html",
                        data: { specialtyid: selectData.SpecialtyId },
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

//创建顶部工具栏
function createBournsToolbar() {
    $("#bourntool").append("类别：<input style=\"width:150px;\" id=\"BournType\" class=\"easyui-combobox\">&nbsp;");
    $("#bourntool").append("主题：<input style=\"width:150px;\" id=\"BournTheme\" class=\"easyui-combobox\">&nbsp;<br/>");
    $("#bourntool").append("时节：<input style=\"width:150px;\" id=\"BournSeason\" class=\"easyui-combobox\">&nbsp;");
    $("#bourntool").append("名称：<input type=\"text\" id=\"BournName\" style=\"width:140px\">&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"BournSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"BournClean\"  class=\"easyui-linkbutton\" iconcls=\"icon-reset\">重置</a>");
    $("#bourntool>#BournType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/bourninfo/BournTypeComboJson/',
        async: false,
        success: function (data) {
            dataType1 = data;
            data.unshift({ 'key': "", 'value': '请选择类别' });
            $("#bourntool>#BournType").combobox('loadData', data);
        }
    });

    $("#bourntool>#BournTheme").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/bourninfo/BournThemeComboJson/',
        async: false,
        success: function (data) {
            dataTheme1 = data;
            data.unshift({ 'key': "", 'value': '请选择主题' });
            $("#bourntool>#BournTheme").combobox('loadData', data);
        }
    });

    $("#bourntool>#BournSeason").combobox({
        valueField: 'key', textField: 'value', editable: false,data:dataSeason1
    });

    $("#bourntool>#BournClean").linkbutton().click(function () {
        $("#bourntool>#BournType").combobox("select", "");
        $("#bourntool>#BournTheme").combobox("select", "");
        $("#bourntool>#BournSeason").combobox("select", "");
        $("#bourntool>#BournIsTui").combobox("select", "");
    });

    $("#bourntool>#BournSelect").linkbutton().click(function () {
        var BournType = $("#bourntool>#BournType").combobox("getValue");
        var BournTheme = $("#bourntool>#BournTheme").combobox("getValue");
        var BournSeason = $("#bourntool>#BournSeason").combobox("getValue");
        var BournName = $("#bourntool>#BournName").val();
        $("#SearchBourn").combogrid('grid').datagrid("load", { 'btype': BournType, 'btheme': BournTheme, 'bseason': BournSeason, 'bname': BournName });
    });
}

//创建上级景点bar
function createAltradionsToolbar() {
    $("#altradionstool").append("区域：<input style=\"width:120px;\" id=\"Region\">&nbsp;");
    $("#altradionstool").append("主题：<input style=\"width:100px;\" id=\"AltradionsTheme\">&nbsp;");
    $("#altradionstool").append("时节：<input style=\"width:100px;\" id=\"AltradionsSeason\">&nbsp;");
    $("#altradionstool").append("等级：<input style=\"width:100px;\" id=\"AltradionsLevel\"><br />");
    $("#altradionstool").append("推荐：<input style=\"width:50px;\" id=\"AltradionsIsTui\">&nbsp;");
    $("#altradionstool").append("来源：<input style=\"width:50px;\" id=\"Source\">&nbsp;");
    $("#altradionstool").append("可用：<input style=\"width:50px;\" id=\"IsUsable\">&nbsp;");
    $("#altradionstool").append("名称：<input type=\"text\" id=\"AltradionsName\" style=\"width:150px\">&nbsp;");
    $("#altradionstool").append("<a href=\"javascript:void(0)\" id=\"AltradionsSelect\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#altradionstool").append("<a href=\"javascript:void(0)\" id=\"AltradionsClean\" iconcls=\"icon-reset\">重置</a>");

    $("#altradionstool>#Region").combotree({
        url: '/common/RegionComboTreeJson/?idLength=4'
    })

    $("#altradionstool>#AltradionsTheme").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/Altradions/ThemeComboxData/',
        async: false,
        success: function (data) {
            dataTheme2 = data;
            data.unshift({ 'key': "", 'value': '请选择主题' });
            $("#altradionstool>#AltradionsTheme").combobox('loadData', data);
        }
    });
    $("#altradionstool>#AltradionsSeason").combobox({
        valueField: 'key', textField: 'value', editable: false,data:dataSeason1
    });

    $("#altradionstool>#AltradionsLevel").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/Altradions/LevelComboxData/',
        async: false,
        success: function (data) {
            dataLevel2 = data;
            data.unshift({ 'key': "", 'value': '请选择等级' });
            $("#altradionstool>#AltradionsLevel").combobox('loadData', data);
        }
    });

    var dataIsTui2 = [{ key: '', value: '全部' }, { key: "true", value: '是' }, { key: "false", value: '否' }];
    $("#altradionstool>#AltradionsIsTui").combobox({ valueField: 'key', textField: 'value', editable: false, data: dataIsTui2 });
    var source2 = [{ key: '', value: '全部' }, { key: 'true', value: '官方' }, { key: 'false', value: '个人' }];
    $("#altradionstool>#Source").combobox({ valueField: 'key', textField: 'value', editable: false, data: source2 });
    var usable2 = [{ key: '', value: '全部' }, { key: 'true', value: '可用' }, { key: 'false', value: '不可用' }];
    $("#altradionstool>#IsUsable").combobox({ valueField: 'key', textField: 'value', editable: false, data: usable2 });
    $("#altradionstool>#AltradionsClean").linkbutton().click(function () {
        $("#altradionstool>#Region").combotree("setValue", "");
        $("#altradionstool>#AltradionsTheme").combobox("select", "");
        $("#altradionstool>#AltradionsSeason").combobox("select", "");
        $("#altradionstool>#AltradionsLevel").combobox("select", "");
        $("#altradionstool>#AltradionsIsTui").combobox("select", "");
        $("#altradionstool>#Source").combobox("select", "");
        $("#altradionstool>#IsUsable").combobox("select", "");
        $("#altradionstool>#AltradionsName").val("");
    });

    $("#altradionstool>#AltradionsSelect").linkbutton().click(function () {
        var theme = $("#altradionstool>#AltradionsTheme").combobox("getValue");
        var season = $("#altradionstool>#AltradionsSeason").combobox("getValue");
        var name = $("#altradionstool>#AltradionsName").val();
        var level = $("#altradionstool>#AltradionsLevel").combobox("getValue");
        var source = $("#altradionstool>#Source").combobox("getValue");
        var tui = $("#altradionstool>#AltradionsIsTui").combobox("getValue");
        var region = $("#altradionstool>#Region").combotree("getValue");
        var usable = $("#altradionstool>#IsUsable").combobox("getValue");
        $("#SearchAltradions").combogrid("grid").datagrid("load", { 'theme': theme, 'season': season, 'name': name, 'level': level, 'istui': tui, 'source': source, 'regionid': region, 'isusable': usable });
    });
}


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
    if (editData != undefined && editData != null && row.SpecialtyId != editData.SpecialtyId) {
        $('#List').datagrid('endEdit', editDataIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            editData.ReleaseTime = Common.DateTimeFormatter(editData.ReleaseTime);
            editData.AddTime = Common.DateTimeFormatter(editData.AddTime);
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
        url: '/specialtyinfo/EditOrUpdateSpecialtyInfo/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}