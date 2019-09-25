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
        var back = trs.eq(index).children("td").eq(19).text();  // 被退回
        if (back.trim() != "")
            trs.eq(index).attr("class", "toRed2");
    });
}
// 提交
function submit2() {
    var trs = $("tbody tr[id] td input[type='checkbox']");
    var datas = {};
    var ok = true;
    $.each(trs, function (index, item) {
        var inp = trs.eq(index);
        var ischeck = inp.is(':checked');
        if (ischeck) {  // 如果被选中了
            var no = inp.val();
            var result = $(`tr[id=${no}] td select`).val();
            var ngdes = $(`tr[id='${no}'] td input[type='text']`).val();
            if (result == "NG" && ngdes == "") {  // 如果NG没有填写不良描述
                $('.alert').removeClass("alert-success")
                $('.alert').html("NG的不良描述不能为空！").addClass('alert-danger').show().delay(3000).fadeOut();
                ok = false;
                return;
            }
            if (result == "OK")
                ngdes = "";
            datas[no] = ngdes;
        }
    });
    if (ok)
    $.ajax({
        url: "/Check/Submit",
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
    var select1 = $(`tr[id='${dbid}'] td select`);
    var input1 = $(`tr[id='${dbid}'] td input[type='text']`);
    if (input1.hasClass("display2")) {
        input1.removeClass("display2");
        select1.removeClass("display2");
    }
    else {
        input1.addClass("display2");
        select1.addClass("display2");
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
    var url = "/Check/Find?" + params;
    window.location.href = url;
}
