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
        if (time == "超期")
            trs.eq(index).attr("class", "toRed");
    });
}
// 提交
function submit3() {
    var trs = $("tbody tr[id] td input[type='checkbox']");
    var datas = {};
    var ok = true;
    $.each(trs, function (index, item) {
        var inp = trs.eq(index);
        var ischeck = inp.is(':checked');
        if (ischeck) {  // 如果被选中了
            var no = inp.val();
            var space = $(`tr[id='${no}'] td input[name='space']`).val();
            var beizhu = $(`tr[id='${no}'] td input[name='back']`).val();
            if (space == "") {  // 如果存储位置为空
                $('.alert').removeClass("alert-success")
                $('.alert').html("储存位置不能为空！").addClass('alert-danger').show().delay(3000).fadeOut();
                ok = false;
                return;
            }
            datas[no] = space + "&" + beizhu;
        }
    });
    var listkey = [];   // 验证是否变更
    for (var d in datas)
        listkey.push(d);
    if (ok)
    $.ajax({
        url: "/Save/Verfi",
        type: "post",
        data: JSON.stringify(listkey),
        dataType: "json",
        contentType: "application/json",
        async: false,
        success: function (result) {
            if (result.Result == "成功") {
                window.location.reload();
            }
            else {
                ok = false;
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
            }
        }
    });
    if (ok)
        $.ajax({
            url: "/Save/Submit",
            type: "post",
            data: JSON.stringify(datas),
            dataType: "json",
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
        })
}
// 被勾选
function choose(th) {
    var dbid = th.id;
    var input1 = $(`tr[id='${dbid}'] td input[name='space']`);
    var input2 = $(`tr[id='${dbid}'] td input[name='back']`);
    if (input1.hasClass("display2")) {
        input1.removeClass("display2");
        input2.removeClass("display2");
    }
    else {
        input1.addClass("display2");
        input2.addClass("display2");
    }
}
// 查找
function find() {
    var inputs = $("table[class='my-table'] thead tr td[class!='firsttd'] input");
    var datas = {};
    $.each(inputs, function (index, item) {
        var name = inputs.eq(index).attr("name");
        var value = inputs.eq(index).val();
        datas[name] = value;
    });
    var params = Object.keys(datas).map(k => `${k}=${datas[k]}`).join('&');
    var url = "/Save/Find?" + params;
    window.location.href = url;
}
// 退回
function back_(th) {
    var rowno = th.name;
    var backrea = $(`tr[id=${rowno}] td input[name='back']`).val();
    if (backrea.trim() == "") {
        $('.alert').removeClass("alert-success")
        $('.alert').html("备注不能为空!").addClass('alert-danger').show().delay(3000).fadeOut();
        return;
    }
    var ok = true;
    var yes = confirm("确认退回吗？");

    $.ajax({
        url: "/Save/Verfi",
        type: "post",
        data: JSON.stringify([rowno]),
        dataType: "json",
        async: false,
        contentType: "application/json",
        success: function (result) {
            if (result.Result == "成功") {
                window.location.reload();
            }
            else {
                ok = false;
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
            }
        }
    });

    if (yes && ok) {
        url = "/Save/Back?dbid=" + rowno + "&backrea=" + backrea;
        window.location.href = url;
    }
}