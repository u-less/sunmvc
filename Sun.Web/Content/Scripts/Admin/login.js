function reImg() {
    document.getElementById('CaptchaImg').src = '/admin/common/GetCaptchaImage/?t='+ Math.random();
}
$(function () {
    if (window.top.location.href!=window.location.href)
    {
        window.top.location.href = window.location.href;
    }
    $("#btnSubmit").click(function () {
        if (validate())
            submitData(0);
    })
    $("#capchaSubmit").click(function () {
        if (validate())
            submitData(1);
    })
    $("#txtcaptcha").focus(function () {
        $("#captchaerror").hide();
    })
    $("#txtAccount").focus(function () {
        if ($(this).val() == '账号|邮箱|手机')
            $(this).val('');
    }).blur(function () {
        if ($(this).val() == '') {
            $("#userlabel").html("账号不能为空").css("color", "red");
            $(this).val('账号|邮箱|手机');
        } else {
            $("#userlabel").html('');
            $("#pwdlabel").html('');
        }
    })
    $("#txtPwd").focus(function () {
        $("#pwdlabel").html('');
    })
    $("#txtPwd").blur(function () {
        if ($(this).val() == '')
            $("#pwdlabel").html("密码不能为空").css("color", "red");
    })
})
function submitData(n) {
    if (errors < canerrors || n > 0) {
        $("#captcha").val($("#txtcaptcha").val());
        $.ajax({
            async: false,
            url: "/admin/adminhome/Login/",
            type: "post",
            dataType: "json",
            data: $("#loginForm").serialize(),
            success: function (msg) {
                var result = eval('(' + msg + ')');
                errors = parseInt(result.errors);
                switch (parseInt(result.state)) {
                    case 0: { showform(); removeLogin(); $("#userlabel").html("账号不存在").css("color", "red"); }; break;
                    case 1: { showform(); $("#txtPwd").val(''); $("#pwdlabel").html("密码错误").css("color", "red"); }; break;
                    case 2: { showcaptcha(); $("#captchaerror").show(); }; break;
                    case 3: { showform(); $("#userlabel").html("账号被锁定").css("color", "red"); }; break;
                    default: { window.location.href = "/admin/adminhome/index"; }; break;
                }
            },
            error: function () { alert("操作提醒", "连接服务器失败"); }
        })
    } else {
        showcaptcha();
    }
}
function showcaptcha() {
    $("#loginForm").hide();
    $(".welcome").fadeIn('slow');
}
function showform() {
    $(".welcome").hide();
    $("#loginForm").fadeIn('slow');
}
function validate() {
    if ($("#txtAccount").val() == '账号|邮箱|手机') {
        $("#userlabel").html("账号不能为空").css("color", "red");
        $("#txtAccount").focus();
        return false;
    }
    if ($("#txtPwd").val() == '') {
        $("#pwdlabel").html("密码不能为空").css("color", "red");
        $("#txtPwd").focus();
        return false;
    }
    return true;
}
function removeLogin() {
    $("#txtAccount").val('账号|邮箱|手机').focus();
    $("#txtPwd").val('');
}