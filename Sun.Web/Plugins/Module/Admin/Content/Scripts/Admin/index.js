$(function () {
    var menutype = parseInt($("#layout").attr('data-menutype'));
    if (menutype == 0)
        createMenu();
    else
    { createTopMenu(); }
    layout(menutype);
    //绑定tabs的右键菜单
    $("#tabs").tabs({
        onContextMenu: function (e, title) {
            e.preventDefault();
            $('#tabsMenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            }).data("tabTitle", title);
        }
    });

    //实例化menu的onClick事件
    $('#tabsMenu').menu({
        onClick: function (item) {
            closeTab(item.id);
        }
    });
});
function layout(menutype) {
    $("#layout").layout();
    $("#tabs").tabs({ border: false, fit: true });
    $("#menus").accordion({ fit: true, border: false });
    clockon();
    if (menutype == 0)
    {
        $(".menu_tree li").each(function () {
            $(this).on("click", function () {
                $(".menu_tree li").each(function () {
                    $(this).removeClass("hover");
                });
                $(this).addClass("hover");
            });
        });
    }
    $('#updatePassword').dialog("close");
    initLoginOut();
}
function refresh() {
    window.frames["mainFrame"].location.reload();
}
//创建菜单
function createMenu() {
    var mResult = "";
    var menus = eval("(" + $("#layout").attr("data-menus") + ")");
    $.each(menus, function (i, n) {
        if (n.menus != undefined) {
            mResult += "<div title=\"" + n.name + "\" iconCls=\"" + n.icon + "\"><div class=\"menu_tree\"><ul>";
            $.each(n.menus, function (j, x) {
                mResult += getMenuContent(x);
            })
            mResult += "</ul></div></div>";
        }
    })
    $("#menus").html(mResult);
}
function createTopMenu()
{
    var mResult = "";
    var menus = eval("(" + $("#layout").attr("data-menus") + ")");
    $.each(menus, function (i, n) {
        if (n.menus != undefined) {
            mResult += "<a id=\"btn-edit\" data-href=\"" + n.url + "\" onclick=\"mnClick(this)\" title='"+n.name+"' class=\"easyui-menubutton\" data-options=\"menu:'#topm_" + n.id + "',iconCls:'" + n.icon + "',size:'large',iconAlign:'left',plain:false\">" + n.name + "</a>";
            CreateTopMenuCont(n.id, n.menus);
        } else {
            mResult += "<a id=\"btn-edit\" data-href=\"" + n.url + "\" onclick=\"mnClick(this)\" title='" + n.name + "' class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-large-picture',size:'large',iconAlign:'left',plain:false\">" + n.name + "</a>";
        }
    });
    $("#topmenuwrap").html(mResult);
    $("#topmenuwrap").find('.easyui-menubutton').menubutton();
    $("#topmenuwrap").find('.easyui-linkbutton').linkbutton();
}
function CreateTopMenuCont(id,menus)
{
    var result = '<div id="topm_' + id + '" style="width:150px;">';
    $.each(menus, function (j, v) {
        result += '<div data-href=' + v.url + ' title="' + v.name + '" onclick="mnClick(this)"  data-options="iconCls:\'' + v.icon + '\'">' + v.name + '</div>';
    });
    result += '</div>';
    $("#layout").append(result);
}
function mnClick(a) {
    var href = $(a).attr("data-href");
    var title = $(a).attr("title");
    if (href != "" && title != "")
        addTab(title, href);
}
function getMenuContent(x) {
    var result = "<li><div class=\"" + x.icon + "\"></div><a href=\"#\" data-href=\"" + x.url + "\"  onclick=\"mnClick(this)\"  title=\"" + x.name + "\">" + x.name + "</a></li>";
    return result;
}
function addTab(title, url) {
    if (!$('#tabs').tabs('exists', title)) {
        var scroll = 'scrolling="no"';
        if (url.lastIndexOf('scroll=yes') > 0)
            scroll = '';
        $('#tabs').tabs('add', {
            title: title,
            iconCls: 'icon-table',
            content: '<iframe '+scroll+' frameborder="0"  src="' + url + '" style="overflow:hidden;width:100%;height:99%;"></iframe> ',
            closable: true
        });
    }
    else {
        $('#tabs').tabs('select', title);
    }
}
//修改密码
function savePassword() {
    var newpass = $('#txtPassword');
    var rePass = $('#txtRePassword');
    if (newpass.val() == '') {
        $.messager.alert("密码修改错误提醒", "新密码不允许为空");
        newpass.focus();
        return false;
    }
    if (rePass.val() == '') {
        $.messager.alert("密码修改错误提醒", "请再一次输入新密码");
        rePass.focus();
        return false;
    }
    if (newpass.val() != rePass.val()) {
        $.messager.alert("密码修改错误提醒", "两次输入密码不一致");
        rePass.focus();
        return false;
    }
    return true;
}
//修改密码
function openUpdatePass() {
    $('#txtPassword').val('');
    $('#txtRePassword').val('');
    saveCount = 0;
    $('#updatePassword').dialog({
        resizable: 'false', shadow: 'false', iconCls: 'icon-key', title: '修改密码',
        buttons: [{
            text: '修改', id: 'addBtn', iconCls: 'icon-save', handler: function () {
                $("#passwordEdit").form("submit", {
                    url: "admin/adminhome/UpdatePassword/",
                    onSubmit: function () {
                        var goon = $("#passwordEdit").form("validate");
                        goon = savePassword();
                        if (goon == false) return false;
                        if (saveCount == 0) saveCount = 1;
                        else {
                            $.messager.alert("操作重复提醒", "请不要两次点击保存按钮");
                            return false;
                        }
                    },
                    success: function (data) {
                        if (data == 'True') {
                            $.messager.alert("操作提醒", "密码修改成功,2秒后请重新登陆。");
                            window.setTimeout("window.location='/AdminHome/Login/'", 2000);
                        }
                        else
                            $.messager.alert("操作提醒", data);
                    }
                })
            }
        }, {
            text: '取消', id: 'cancelBtn', iconCls: 'icon-cancel', handler: function () { $("#updatePassword").dialog("close"); }
        }]
    }).dialog("move", { top: 85, left: $(document).width() - 460 });
}
function initLoginOut() {
    $("#loginOut").click(function () {
        $.messager.confirm("退出系统提醒", "确定要退出系统吗？", function (r) {
            if (r) {
                $.ajax({
                    async: false,
                    url: "admin/adminhome/LoginOut/",
                    type: "post",
                    dataType: "html",
                    data: {},
                    success: function (msg) {
                        if (msg == "True") {
                            window.location.href = "/AdminHome/Login/";
                        }
                        else {
                            $.messager.alert("退出提醒", msg);
                        }
                    },
                    error: function () { $.messager.alert("操作提醒", "连接服务器失败"); }
                })
            }
        })
    })
}
//本地时钟
function clockon() {
    var now = new Date();
    var year = now.getFullYear(); //getFullYear getYear
    var month = now.getMonth();
    var date = now.getDate();
    var day = now.getDay();
    var hour = now.getHours();
    var minu = now.getMinutes();
    var sec = now.getSeconds();
    var week;
    month = month + 1;
    if (month < 10) month = "0" + month;
    if (date < 10) date = "0" + date;
    if (hour < 10) hour = "0" + hour;
    if (minu < 10) minu = "0" + minu;
    if (sec < 10) sec = "0" + sec;
    var arr_week = new Array("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");
    week = arr_week[day];
    var time = "";
    time ="【"+ year + "年" + month + "月" + date + "日" + " " + hour + ":" + minu + ":" + sec + " " + week+"】";

    $("#clocktime").html(time);

    var timer = setTimeout("clockon()", 200);
}

//几个关闭事件的实现
function closeTab(action) {
    var onlyOpenTitle = "工作中心";
    var alltabs = $('#tabs').tabs('tabs');
    var currtab_title = $('#tabsMenu').data('tabTitle');
    var currentTab = $('#tabs').tabs('getTab', currtab_title);//$('#tabs').tabs('getSelected');
    var allTabtitle = [];
    $.each(alltabs, function (i, n) {
        allTabtitle.push($(n).panel('options').title);
    })


    switch (action) {
        case "close":
            //var currtab_title =currentTab.panel('options').title;
            $('#tabs').tabs('close', currtab_title);
            break;
        case "closeall":
            $.each(allTabtitle, function (i, n) {
                if (n != onlyOpenTitle) {
                    $('#tabs').tabs('close', n);
                }
            });
            break;
        case "closeother":
            //var currtab_title = currentTab.panel('options').title;
            $.each(allTabtitle, function (i, n) {
                if (n != currtab_title && n != onlyOpenTitle) {
                    $('#tabs').tabs('close', n);
                }
            });
            break;
        case "closeright":
            var tabIndex = $('#tabs').tabs('getTabIndex', currentTab);

            if (tabIndex == alltabs.length - 1) {
                alert('后面没有啦');
                return false;
            }
            $.each(allTabtitle, function (i, n) {
                if (i > tabIndex) {
                    if (n != onlyOpenTitle) {
                        $('#tabs').tabs('close', n);
                    }
                }
            });

            break;
        case "closeleft":
            var tabIndex = $('#tabs').tabs('getTabIndex', currentTab);
            if (tabIndex == 1) {
                alert('前面没有啦');
                return false;
            }
            $.each(allTabtitle, function (i, n) {
                if (i < tabIndex) {
                    if (n != onlyOpenTitle) {
                        $('#tabs').tabs('close', n);
                    }
                }
            });

            break;
    }
}