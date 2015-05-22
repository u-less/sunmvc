$.extend($.fn.validatebox.defaults.rules, {
    isInt: {
        validator: function (value, param) {
            return /^\d+$/g.test(value) && value < 2100000000;
        },
        message: "只允许输入32位整数"
    }
});
$.extend($.fn.validatebox.defaults.rules, {
    /*必须和某个字段相等*/
    equalTo: {
        validator: function (value, param) {
            return $(param[0]).val() == value;
        },
        message: '两次输入数据不一致'
    }

});

$.extend($.fn.validatebox.defaults.rules, {
    dateCompare: {
        validator: function (value, param) {
            var compareTime = $("input[name=" + param[0] + "]").val();
            //因为日期是统一格式的所以可以直接比较字符串 否则需要Date.parse(_date)转换
            switch (param[1]) {
                case 0:return value > compareTime;
                default:return value < compareTime;
            }
        }, message: '结束时间必须大于开始时间'
    }
});