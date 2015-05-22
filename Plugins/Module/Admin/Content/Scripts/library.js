
//深度克隆
function cloneObj(obj) {
    var result, oClass = isClass(obj);
    //确定result的类型
    if (oClass === "Object") {
        result = {};
    } else if (oClass === "Array") {
        result = [];
    } else {
        return obj;
    }
    for (key in obj) {
        var copy = obj[key];
        if (isClass(copy) == "Object") {
            result[key] = arguments.callee(copy);//递归调用
        } else if (isClass(copy) == "Array") {
            result[key] = arguments.callee(copy);
        } else {
            result[key] = obj[key];
        }
    }
    return result;
}
//返回传递给他的任意对象的类
function isClass(o) {
    if (o === null) return "Null";
    if (o === undefined) return "Undefined";
    return Object.prototype.toString.call(o).slice(8, -1);
}
//对象深度比较
function compareObj(oneObj, twoObj) {
    if (typeof oneObj != 'object') {
        if (typeof twoObj != 'object') {
            if (oneObj == twoObj) return true;
            else
                return false;
        }
        else
            return false;
    }
    else {
        for (var i in oneObj) {
            if (compareObj(oneObj[i], twoObj[i]) == false)
                return false;
        }
        return true;
    }
}
//js时间对象的格式化;
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1,  //month 
        "d+": this.getDate(),     //day 
        "h+": this.getHours(),    //hour 
        "m+": this.getMinutes(),  //minute 
        "s+": this.getSeconds(), //second 
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter 
        "S": this.getMilliseconds() //millisecond 
    }
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}
//点赞
function UpData(id, aobj, url) {
    $.ajax({
        async: false,
        url: url,
        type: "post",
        dataType: "json",
        data: { id: id },
        success: function (backdata) {
            if (backdata == 1) {
                var count = parseInt($("span", aobj).html())
                $("span", aobj).html((count + 1))
            }
        },
        error: function () { alert("( ▼-▼ )网络异常"); }
    });
}

String.prototype.stripHTML = function() {  
    var reTag = /<[^>]+>/igm;
    return this.replace(reTag,"").replace(/&nbsp;/igm, '');
} 
String.prototype.stripScript = function () {
    var script = /<script\b[^>]*>.*<\/script>/igm;
    var style=/<style\b[^>]*>.*<\/style>/igm;
    return this.replace(reTag, "").replace(style,'');
}
String.prototype.Format = function ()
{
    var result = this.toString();
    if (arguments.length > 0) {
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i] != undefined) {
                var re = new RegExp('\\{' + i + '\\}', 'gm');
                result = result.replace(re, arguments[i]);
            }
        }
    }
    return result;
};
function NoHtml()
{
    $('.nohtml').each(function (i, e) { var length = $(this).attr("data-length"); var str = $(this).text().stripHTML(); if (length != undefined && !isNaN(parseInt(length))) { if (str.length > length) str = str.substring(0, length) + '...'; }; $(this).html(str); });
}
var Common = {

    //EasyUI用DataGrid用日期格式化
    TimeFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }
        var val = dateValue.format("yyyy-MM-dd hh:mm:ss");
        return val.substr(11, 5);
    },
    DateTimeFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }
        return dateValue.format("yyyy-MM-dd hh:mm:ss");
    },

    //EasyUI用DataGrid用日期格式化
    DateFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd");
    }
};

//分隔key串，返回value串
function splitKey(dataObj, value) {
    if (value == null)
        return "";
    var arrayObj = value.split(",");
    var strObj = "";
    $.each(dataObj, function (i, item) {
        for (var i = 0; i < arrayObj.length; i++) {
            if (item.key == arrayObj[i].toString()) {
                strObj += item.value + ",";
                break;
            }
        }
    });
    if (strObj.length>1)
        strObj = strObj.substr(0, strObj.length - 1);
    return strObj;
}

function indexKey(dataObj, value) {
    if (value == null)
        return "";
    var index = 0;
    $.each(dataObj, function (i, item) {
        if (item.key == value.toString()) {
            index = i;
            return false;
        }
    });
    return dataObj[index].value;
}
$(function () {
    //回到顶部		
    var scrollTop = $(".scrollTop");
    $(window).scroll(function () {
        if ($(window).scrollTop() > 500) {
            scrollTop.css({ "margin-left": 620 }).show();
            scrollTop.stop().animate({ "bottom": 275 }, 400);
        } else {
            scrollTop.stop().animate({ "bottom": 0 }, 400, function () { scrollTop.hide(); });

        }
    });
    scrollTop.click(function () {
        $(document, "body").scrollTop(0);
    });
    NoHtml();
});
/**   * 替换全部匹配的字符串
* @param {} s1 表达式
* @param {} s2 要替换的字符串
* @return {}  */
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "g"), s2);
}

/**   
* 截取html字符串，保留html格式  
* @param param 要截取的参数  
* @param length 要截取的长度  
* @param endWith 结束的字符串  
*/
function cutHTML(str, length, endWith) {
    str = str.replace(/<\/?[^>]*>/g,''); //去除HTML tag
    str = str.replace(/(^\s*)/g, '\n'); //去除行尾空白
    str = str.replace(/\n[\s| | ]*\r/g,'\n'); //去除多余空行
    str = str.replace(/&nbsp;/ig, '');//去掉&nbsp;
    if (str.length > length)
    {
        str = str.substring(0, length);
        str += endWith;
    }
    return str;
}