
var saveCount = 0;//防止重复提交表单
var dataTheme1;
var dataType1;
var dataTheme2;
var dataLevel2;
var dataSeason1 = [{ 'key': "", 'value': '请选择时节' }, { 'key': "1", 'value': '一月' }, { 'key': "2", 'value': '二月' }, { 'key': "3", 'value': '三月' }, { 'key': "4", 'value': '四月' }, { 'key': "5", 'value': '五月' }, { 'key': "6", 'value': '六月' }, { 'key': "7", 'value': '七月' }, { 'key': "8", 'value': '八月' }, { 'key': "9", 'value': '九月' }, { 'key': "10", 'value': '十月' }, { 'key': "11", 'value': '十一月' }, { 'key': "12", 'value': '十二月' }];
function submitForm() {
    $("#editform").form("submit", {
        url: "/WebBanner/EditOrUpdateBanner/?bannerkey=" + bannerKey,
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
                    var id = $("#BannerId").val();
                    if (id==''||parseInt(id) == 0) {
                        $('#editform').form('clear');
                        saveCount = 0;
                    } else {
                        if (window.parent.window.$('#tabs').tabs('exists', '编辑Banner')) {
                            window.parent.window.$('#tabs').tabs("close", '编辑Banner');
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
    $("#edit").panel("maximize").panel({ border: false, fit: true, style: { padding: 5 } });
    if (btype==2) {
        $('#RelateId').combogrid({
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
    }
    else if (btype ==3) {
        //景点
        $('#RelateId').combogrid({
            panelWidth: 640,
            panelHeight: 400,
            url: '/Altradions/GridPageList/',
            idField: 'AltradionId',
            textField: 'AltradionName',
            editable: false,
            pagination: true,
            pageList: [5, 10],
            fitColumns: true,
            columns: [[
            { field: 'AltradionId', title: '编号', width: 60 },
                { field: 'RegionName', width: 100, align: 'center', title: '地区' },
                {
                    field: 'ParentName', width: 100, align: 'center', title: '上级景点', formatter: function (value, row) {
                        if (value != '' && value != null)
                            return value;
                        else
                            return '无上级景点';
                    }
                },
                { field: 'AltradionName', width: 110, align: 'center', title: '名称' },
                {
                    field: 'Level', width: 110, align: 'center', title: '等级', formatter: function (value, row) {
                        return indexKey(dataLevel2, value);
                    }
                },
                {
                    field: 'ATheme', width: 110, align: 'center', title: '主题', formatter: function (value, row) {
                        return splitKey(dataTheme2, value);
                    }
                },
                {
                    field: 'ASeason', width: 110, align: 'center', title: '时节', formatter: function (value, row) {
                        return splitKey(dataSeason1, value);
                    }
                },
                {
                    field: 'Source', width: 60, align: 'center', title: '来源', formatter: function (value, row) {
                        if (value == true)
                            return '<span style="color:red;">官方</span>';
                        else
                            return '<span style="color:#0a6314;">玩家</span>';
                    }
                }
            ]],
            toolbar: '#altradionstool'
        });
        createAltradionsToolbar();
    }
})

//创建顶部工具栏
function createBournsToolbar() {
    $("#bourntool").append("类别：<input style=\"width:150px;\" id=\"SearchType\" class=\"easyui-combobox\">&nbsp;");
    $("#bourntool").append("主题：<input style=\"width:150px;\" id=\"SearchTheme\" class=\"easyui-combobox\">&nbsp;<br/>");
    $("#bourntool").append("时节：<input style=\"width:150px;\" id=\"SearchSeason\" class=\"easyui-combobox\">&nbsp;");
    $("#bourntool").append("名称：<input type=\"text\" id=\"SearchName\" style=\"width:140px\">&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#bourntool").append("<a href=\"javascript:void(0)\" id=\"btnClean\"  class=\"easyui-linkbutton\" iconcls=\"icon-reset\">重置</a>");
    $("#bourntool>#SearchType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/bourninfo/BournTypeComboJson/',
        async: false,
        success: function (data) {
            dataType1 = data;
            data.unshift({ 'key': "", 'value': '请选择类别' });
            $("#bourntool>#SearchType").combobox('loadData', data);
        }
    });

    $("#bourntool>#SearchTheme").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/bourninfo/BournThemeComboJson/',
        async: false,
        success: function (data) {
            dataTheme1 = data;
            data.unshift({ 'key': "", 'value': '请选择主题' });
            $("#bourntool>#SearchTheme").combobox('loadData', data);
        }
    });

    $("#bourntool>#SearchSeason").combobox({
        valueField: 'key', textField: 'value', editable: false, data: dataSeason1
    });

    $("#bourntool>#btnClean").linkbutton().click(function () {
        $("#bourntool>#SearchType").combobox("select", "");
        $("#bourntool>#SearchTheme").combobox("select", "");
        $("#bourntool>#SearchSeason").combobox("select", "");
        $("#bourntool>#SearchName").val("");
    });
    $("#bourntool>#btnSelect").linkbutton().click(function () {
        var BournType = $("#bourntool>#SearchType").combobox("getValue");
        var BournTheme = $("#bourntool>#SearchTheme").combobox("getValue");
        var BournSeason = $("#bourntool>#SearchSeason").combobox("getValue");
        var BournName = $("#bourntool>#SearchName").val();
        $("#RelateId").combogrid('grid').datagrid("load", { 'btype': BournType, 'btheme': BournTheme, 'bseason': BournSeason, 'bname': BournName });
    });
}

//创建顶部工具栏
function createAltradionsToolbar() {
    $("#altradionstool").append("主题：<input style=\"width:150px;\" id=\"SearchTheme2\">&nbsp;");
    $("#altradionstool").append("时节：<input style=\"width:150px;\" id=\"SearchSeason2\">&nbsp;<br/>");
    $("#altradionstool").append("等级：<input style=\"width:150px;\" id=\"SearchLevel2\">&nbsp;");
    $("#altradionstool").append("名称：<input type=\"text\" id=\"SearchName2\" style=\"width:140px\">&nbsp;");
    $("#altradionstool").append("<a href=\"javascript:void(0)\" id=\"btnSelect2\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#altradionstool").append("<a href=\"javascript:void(0)\" iconcls=\"icon-reset\" id=\"btnClean2\">重置</a>");
    $("#altradionstool>#SearchTheme2").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
        url: '/Altradions/ThemeComboxData/',
        async: false,
        success: function (data) {
            data.unshift({ 'key': "", 'value': '请选择主题' });
            dataTheme2 = data;
            $("#altradionstool>#SearchTheme2").combobox('loadData', data);
        }
    });
    $("#altradionstool>#SearchSeason2").combobox({
        valueField: 'key', textField: 'value', editable: false, data: dataSeason1
    });

    $("#altradionstool>#SearchLevel2").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.ajax({
        type: "post",
        url: '/Altradions/LevelComboxData/',
        async: false,
        success: function (data) {
            data.unshift({ 'key': "", 'value': '请选择等级' });
            dataLevel2 = data
            $("#altradionstool>#SearchLevel2").combobox('loadData', data);
        }
    });
    $("#altradionstool>#btnClean2").linkbutton().click(function () {
        $("#altradionstool>#SearchTheme2").combobox("select", "");
        $("#altradionstool>#SearchSeason2").combobox("select", "");
        $("#altradionstool>#SearchLevel2").combobox("select", "");
        $("#altradionstool>#SearchName2").val("");
    });

    $("#altradionstool>#btnSelect2").linkbutton().click(function () {
        var theme = $("#altradionstool>#SearchTheme2").combobox("getValue");
        var season = $("#altradionstool>#SearchSeason2").combobox("getValue");
        var level = $("#altradionstool>#SearchLevel2").combobox("getValue");
        var name = $("#altradionstool>#SearchName2").val();
        $("#RelateId").combogrid('grid').datagrid("load", { 'theme': theme, 'season': season, 'name': name, 'level': level });
    });
}
