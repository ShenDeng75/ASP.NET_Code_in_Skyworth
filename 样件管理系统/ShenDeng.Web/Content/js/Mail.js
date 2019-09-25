// 删除
function delete_mail(dbid) {
    var url = "/Admin/Mail/Delete?dbid=" + dbid;
    window.location.href = url;
}
// 添加
function add_mail() {
    var name = $("td input[name='name']").val();
    var duty = $("td select[name='duty']").val();
    var mailaddre = $("td input[name='mailaddre']").val();
    var mailpwd = $("td input[name='mailpwd']").val();
    var msg = "";
    if (!/^.+?@.+?\..+?$/.test(mailaddre))
        msg = "请输入有效的邮箱地址";
    if (duty == "发送邮箱" && mailpwd.trim() == "")
        msg = "密码不能为空";
    if (name.trim() == "" || mailaddre.trim() == "")
        msg = "姓名或邮箱地址不能为空";
    if (msg != "") {
        $('.alert').removeClass("alert-success")
        $('.alert').html(msg).addClass('alert-danger').show().delay(3000).fadeOut();
        return;
    }
    datas = {};
    datas["name"] = name;
    datas["duty"] = duty;
    datas["mailaddre"] = mailaddre
    datas["mailpwd"] = mailpwd;

    $.ajax({
        url: "/Admin/Mail/CreateMail",
        method: "post",
        data: JSON.stringify(datas),
        type: "json",
        contentType: "application/json",
        success: function (result) {
            if (result.Result == "成功") {
                window.location.reload();
            }
            else {
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
            }
        }
    });
}
// 下拉框
function choosepwd() {
    var choose = $("#select_duty").val();
    if (choose == "发送邮箱") {
        $("#show_pwd").removeClass("hide");
    }
    else {
        $("#show_pwd").addClass("hide");
    }
}