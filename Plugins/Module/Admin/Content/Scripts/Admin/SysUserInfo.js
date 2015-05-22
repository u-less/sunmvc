
var editData;//用于编辑的数据
var selectData = new Object();//原始选中数据
var saveCount = 0;//防止重复提交表单
var editDataIndex = 0;//编辑行的索引
var dataStates;
var dataType;

$(function () {
    $("#AddorUpdate").dialog("close");
    var height = window.parent.window.$("#tabs").height();
    height = height - 46;
    $("#List").datagrid({
         url: "/admin/sysuserinfo/UserGridJson/",
        title: '用户管理中心',
        width: 'auto',
        height: height,
        iconCls: 'icon-table',
        rownumbers: true,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        singleSelect: true,
        idField: 'userid',
        columns: [[
            { field: 'UserId', width: 50, align: 'center', title: '编号' },
            { field: 'LoginId', width: 100, align: 'center', title: '账号' },
            { field: 'UserName', width: 100, align: 'center', title: '姓名' },
            { field: 'NickName', width: 100, align: 'center', title: '昵称' },
            { field: 'RegionName', width: 110, align: 'center', title: '地区' },
            { field: 'OrganName', width: 110, align: 'center', title: '机构' },
            { field: 'RoleName', width: 80, align: 'center', title: '角色' },
            { field: 'UserType', width: 80, align: 'center', title: '类型', formatter: function (value, row) { return indexKey(dataType, value); } },
            { field: 'Sex', width: 50, align: 'center', title: '性别',formatter: function (value, row) { if (value == '1') return '男'; else return '女'; }},
            { field: 'AddTime', width: 110, align: 'center', title: '注册时间', formatter: function (value, row) { return Common.DateTimeFormatter(value); } },
            { field: 'LastLoginTime', width: 110, align: 'center', title: '最后登录时间', formatter: function (value, row) { return Common.DateTimeFormatter(value); } },
            { field: 'States', title: '状态', width: 50, align: 'center', formatter: function (value, row) {return indexKey(dataStates,value); }}
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
    $(".datagrid-toolbar").append("姓名/账号/昵称：<input type=\"text\" style=\"width:150px;\" id=\"SearchName\">&nbsp;");
    $(".datagrid-toolbar").append("机构：<input type=\"text\" style=\"width:150px;\" id=\"OrganId\">&nbsp;");
    $(".datagrid-toolbar").append("区域：<input style=\"width:130px;\" id=\"Region\">&nbsp;");
    $(".datagrid-toolbar").append("类型：<input style=\"width:auto;\" id=\"SearchType\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("状态：<input style=\"width:auto;\" id=\"SearchStates\" class=\"easyui-combobox\">&nbsp;<br/>");
    $(".datagrid-toolbar").append("最后登录时间&nbsp;&nbsp;：<input type=\"text\" style=\"width:auto;\" id=\"SearchStartTime\">&nbsp;");
    $(".datagrid-toolbar").append("<input type=\"text\" style=\"width:auto;\" id=\"SearchEndTime\">&nbsp;&nbsp;&nbsp;");
    $(".datagrid-toolbar").append("性别：<input style=\"width:auto;\" id=\"SearchSex\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("角色：<input style=\"width:auto;\" id=\"SearchRole\" class=\"easyui-combobox\">&nbsp;");
    $(".datagrid-toolbar").append("<a href=\"javascript:void(0)\" id=\"btnSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;<a href=\"javascript:void(0)\" id=\"btnClean\" class=\"easyui-linkbutton\"  iconCls=\"icon-reset\">重置</a>");

    $("#Region").combotree({
         url: '/admin/common/RegionComboTreeJson/?idLength=4'
    })

    $("#SearchType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetUserTypeCombox/',
        async: false,
        success: function (data) {
            dataType = data;
            data.unshift({ 'key':"", 'value': '请选择类型' });
            $("#SearchType").combobox('loadData', data);
        }
    });

    $("#SearchStates").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetUserStatusCombox/',
        async: false,
        success: function (data) {
            dataStates = data;
            data.unshift({ 'key': "", 'value': '请选择状态' });
            $("#SearchStates").combobox('loadData', data);
        }
    });

    $("#SearchRole").combobox({
        valueField: 'key', textField: 'value', editable: false
    });

    $.ajax({
        type: "post",
         url: '/admin/SysUserInfo/GetRoleCombox/',
        async: false,
        success: function (data) {
            data.unshift({ 'key': "", 'value': '请选择角色' });
            $("#SearchRole").combobox('loadData', data);
        }
    });

    $("#SearchSex").combobox({
        data: [{key:'',value:'全部'},{ key: '1', value: '男' }, { key: '0', value: '女' }], valueField: 'key', textField: 'value', editable: false
    });

    $('#SearchStartTime').datetimebox({
        showSeconds: true,
        editable:false
    });

    $('#SearchEndTime').datetimebox({
        showSeconds: true,
        editable: false
    });
    $("#btnClean").linkbutton().click(function () {
        $("#Region").combotree("setValue", "");
        $("#SearchSex").combobox("select", "");
        $("#SearchStates").combobox("select", "");
        $("#SearchType").combobox("select", "");
        $("#SearchRole").combobox("select", "");
        $("#SearchName").val("");
        $("#SearchStartTime").datetimebox("setValue","");
        $("#SearchEndTime").datetimebox("setValue", "");
    });

    $("#btnSelect").linkbutton().click(function () {
        var Region = $("#Region").combotree("getValue");
        var UserSex = $("#SearchSex").combobox("getValue");
        var UserStates = $("#SearchStates").combobox("getValue");
        var UserType = $("#SearchType").combobox("getValue");
        var UserRole = $("#SearchRole").combobox("getValue");
        var UserName = $("#SearchName").val();
        var StartTime = $("#SearchStartTime").datetimebox("getValue");
        var EndTime = $("#SearchEndTime").datetimebox("getValue");
        $("#List").datagrid("load", { 'regionid': Region, 'sex': UserSex, 'states': UserStates, 'username': UserName, 'usertype': UserType, 'roleid': UserRole, 'starttime': StartTime, 'endtime': EndTime });
    });
    $('#OrganId').combogrid({
        panelWidth: 600,
        panelHeight: 400,
         url: '/admin/AdminCommon/OrganComboGridJson/',
        idField: 'OrganId',
        textField: 'OrganName',
        editable: false,
        pagination: true,
        pageList: [20, 30, 40, 50, 80],
        fitColumns: true,
        columns: [[
        { field: 'OrganId', title: '编号', width: 60 },
        { field: 'OrganName', title: '名称', width: 100 },
        { field: 'TypeName', title: '类别', width: 90 },
        { field: 'LevelName', title: '等级', width: 90 }
        ]],
        toolbar: '#organbar'
    });
    createOrganTopToolbar();
}
//创建机构工具栏
function createOrganTopToolbar() {
    $("#organbar").append("类别：<input style=\"width:85px;\" id=\"OrganType\" class=\"easyui-combobox\">&nbsp;");
    $("#organbar").append("等级：<input style=\"width:85px;\" id=\"OrganLevel\" class=\"easyui-combobox\">&nbsp;");
    $("#organbar").append("名称：<input type=\"text\" id=\"OrganName\" style=\"width:100px\">&nbsp;");
    $("#organbar").append("<a href=\"javascript:void(0)\" id=\"OrganSelect\" class=\"easyui-linkbutton\" iconCls=\"icon-search\">搜索</a>&nbsp;");
    $("#organbar").append("<a href=\"javascript:void(0)\" id=\"OrganClean\" class=\"easyui-linkbutton\" iconCls=\"icon-reset\">重置</a>");
    $("#OrganType").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/sysorgan/OrganTypeComboJson/',
        function (data, textStatus) {
            data.unshift({ "key": "", 'value': '请选择类别' });
            $("#OrganType").combobox('loadData', data);
        },
        "json"
    );

    $("#OrganLevel").combobox({
        valueField: 'key', textField: 'value', editable: false
    });
    $.post
    (
        '/admin/sysorgan/OrganLevelComboJson/',
        function (data, textStatus) {
            data.unshift({ 'key': "", 'value': '请选择等级' });
            $("#OrganLevel").combobox('loadData', data);
        },
        "json"
    );
    $("#OrganSelect").linkbutton().click(function () {
        var OrganType = $("#OrganType").combobox("getValue");
        var OrganLevel = $("#OrganLevel").combobox("getValue");
        var OrganName = $("#OrganName").val();
        $('#OrganId').combogrid('grid').datagrid("load", { 'TypeId': OrganType, 'Level': OrganLevel, 'OrganName': OrganName });
    });

    $("#OrganClean").linkbutton().click(function () {
        $("#OrganType").combobox("select", "");
        $("#OrganLevel").combobox("select", "");
        $("#OrganName").val("");
    });
}
var config = {
    add: {
        text: '新增用户', id: 'addBtn', iconCls: 'icon-add', handler: function () {
            addTab('新增用户', '/admin/sysuserinfo/Edit/', 'icon-add');
        }
    },
    edit: {
        text: '编辑用户', id: 'editBtn', iconCls: 'icon-edit', disabled: true, handler: function () {
            addTab('编辑用户', '/admin/sysuserinfo/Edit/?userId=' + selectData.UserId, 'icon-edit');
        }
    },
    remove: {
        text: '锁定用户', id: 'deleteBtn', iconCls: 'icon-remove', disabled: true, handler: function () {
            $.messager.confirm("锁定提醒", "确定锁定吗？锁定后将不可恢复", function (r) {
                if (r) {
                    $.ajax({
                        async: false,
                         url: "/admin/sysuserinfo/DeleteUser/",
                        type: "post",
                        dataType: "html",
                        data: { userId: selectData.UserId },
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
                    })
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
    $("#editBtn").linkbutton({ disabled: true });
    $("#deleteBtn").linkbutton({ disabled: true });
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
    if (editData != undefined && editData != null && row.UserId != editData.UserId) {
        $('#List').datagrid('endEdit', editDataIndex);
        if (!compareObj(editData, selectData) && editData != null) {
            editData.AddTime = Common.DateTimeFormatter(editData.AddTime);
            editData.LastLoginTime = Common.DateTimeFormatter(editData.LastLoginTime);
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
         url: '/admin/sysuserinfo/EditOrUpdateUser/',
        type: "post",
        dataType: "html",
        data: data,
        success: sucFunc,
        error: function () { $.messager.alert('操作提醒', '连接服务器失败，所做的修改不能保存') }
    })
}