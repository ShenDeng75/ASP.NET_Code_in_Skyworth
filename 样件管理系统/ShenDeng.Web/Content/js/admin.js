// 全选
function choose_all() {
    if ($("#ch").is(":checked"))
        $("input[type='checkbox']").prop("checked", true);
    else
        $("input[type='checkbox']").prop("checked", false);
}
toRed();
// 变色
function toRed() {
    var trs = $("tbody").children("tr[id]");
    $.each(trs, function (index, item) {
        var time = trs.eq(index).children("td").eq(15).text(); // 如果超期
        if (time == "报废" || time == "超期")
            trs.eq(index).attr("class", "toRed");
    });
}
// 添加账户
function add_account() {
    var username = $("input[id='add_account']").val();
    var jobnumber = $("input[name='jobnumber']").val();
    var roles = [];
    if (username.trim() == "" || jobnumber.trim() == "") {
        $('.alert').removeClass("alert-success")
        $('.alert').html("用户名或工号不能为空！").addClass('alert-danger').show().delay(3000).fadeOut();
        return;
    }
    var inputs = $("table tr td[id='choose_roles'] div input");
    $.each(inputs, function (index, item) {
        if (inputs.eq(index).is(":checked"))
            roles.push(inputs.eq(index).val());
    });
    var datas = {};
    datas["username"] = username;
    datas["jobnumber"] = jobnumber;
    datas["roles"] = roles;
    $.ajax({
        url: "/Admin/Admin/CreateAccount",
        method: "post",
        data: JSON.stringify(datas),
        dataType: "json",
        contentType: "application/json",
        success: function (result) {
            if (result.Result == "成功")
                window.location.reload();
            else {
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
            }
        }
    });
}
// 删除账户
function delete_account(username) {
    var yes = confirm("确认删除该账户吗？");
    if (yes) {
        $.ajax({
            url: `/Admin/Admin/Delete_Account?id=${username}`,
            method: "get",
            success: function (result) {
                window.location.reload();
            }
        });
    }
}
// 重置密码
function reset(username) {
    var yes = confirm("确认重置密码为 1 吗？");
    if (yes) {
        $.ajax({
            url: `/Admin/Admin/ResetPassword?username=${username}`,
            method: "get",
            success: function (result) {
                if (result.Result == "成功")
                    window.location.reload();
                else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                }
            }
        });
    }
}
// 显示权限
show_roles();
function show_roles() {
    var trs = $("tbody[id='show_account'] tr");
    $.each(trs, function (index, item) {
        var td = trs.eq(index).children("td").eq(2);
        var a = td.text();
        var result = int2list(a);
        td.text(result);
    });
}
function int2list(roles) {
    var result = "|";
    if (roles == "31") {
        result = "所有权限";
        return result;
    }
    if ((roles & 1) != 0)
        result += " 查询 |";
    if ((roles & 2) != 0)
        result += " 样件存储 |";
    if ((roles & 4) != 0)
        result += " 样件审核 |";
    if ((roles & 8) != 0)
        result += " 超期处理 |";
    if ((roles & 16) != 0)
        result += " 管理员 |";

    return result;
}
// 查找
function find1() {
    var inputs = $("table[class='my-table'] thead tr td[class!='firsttd'] input");
    var datas = {};
    $.each(inputs, function (index, item) {
        var name = inputs.eq(index).attr("name");
        var value = inputs.eq(index).val();
        datas[name] = value;
    });
    var params = Object.keys(datas).map(k => `${k}=${datas[k]}`).join('&');
    var url = "/Admin/Admin/Find1?" + params;
    window.location.href = url;
}
// 查找
function find2() {
    var inputs = $("table[class='my-table'] thead tr td[class!='firsttd'] input");
    var datas = {};
    $.each(inputs, function (index, item) {
        var name = inputs.eq(index).attr("name");
        var value = inputs.eq(index).val();
        datas[name] = value;
    });
    var params = Object.keys(datas).map(k => `${k}=${datas[k]}`).join('&');
    var url = "/Admin/Admin/Find2?" + params;
    window.location.href = url;
}