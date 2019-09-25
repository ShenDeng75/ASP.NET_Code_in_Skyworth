// 登录
function login() {
    var username = $("input[name='username']").val();
    var password = $("input[name='password']").val();
    var un = $("input[name='username']");
    var ok = true;
    if (username == "") {
        ok = false;
        var span = document.createElement("span");
        span.innerHTML = "<span style='color: red' > 用户名不能为空！</span >"
        if (un.siblings("span").length == 0)
            un.after(span);
    }
    else {
        un.siblings("span").remove();
    }
    if (ok) {
        var datas = {};
        datas["username"] = username;
        datas["password"] = password;
        $.ajax({
            url: "/Account/Ajax_Login",
            type: "post",
            data: JSON.stringify(datas),
            contentType: "application/json",
            dataType: "json",
            success: function (result) {
                if (result.Result == "成功")
                    window.location.reload();
                else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                    $("#login_button").removeAttr("data-dismiss", "modal");
                }
            }
        });
    }
    else
       $("#login_button").removeAttr("data-dismiss", "modal");
}

// 修改密码
function resetpwd() {
    var oldpwd = $("td input[type='password'][name='oldpwd']").val();
    var newpwd = $("td input[type='password'][name='newpwd']").val();
    var conpwd = $("td input[type='password'][name='conpwd']").val();
    var errmsg = "";
    if (newpwd != conpwd)
        errmsg = "两次密码不一致！";
    if (oldpwd == "" || newpwd == "" || conpwd == "")
        errmsg = "密码不能为空"
    if (errmsg != "") {
        $('.alert').removeClass("alert-success")
        $('.alert').html(errmsg).addClass('alert-danger').show().delay(3000).fadeOut();
        $("#login_button").removeAttr("data-dismiss", "modal");
        return;
    }
    $.ajax({
        url: "/Account/ResetPwd",
        method: "post",
        type: "json",
        contentType: "application/json",
        data: JSON.stringify([oldpwd, newpwd]),
        success: function (result) {
            if (result.Result == "成功") {
                window.location.href = "/Account/SignOut"
            }
            else {
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                $("#login_button").removeAttr("data-dismiss", "modal");
            }
        }
    });

}