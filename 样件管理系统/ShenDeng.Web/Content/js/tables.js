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
        var back = trs.eq(index).children("td").eq(14).text();  // 被退回
        if (back.trim() != "")
            trs.eq(index).attr("class", "toRed2");
    });
}
// 提交
function submit1() {  
    var trs = $("tbody tr[id] td input");
    var datas = [];
    $.each(trs, function (index, item) {
        var inp = trs.eq(index);
        var ischeck = inp.is(':checked');
        if (ischeck) { // 如果被选中了
            var no = inp.val();
            datas.push(no);
        }
    });
    $.ajax({
        url: "Submit",
        type: "post",
        data: JSON.stringify(datas),
        dataType: "json",
        contentType: "application/json",
        success: function (result) {
            if (result.Result == "成功") {
                window.location.reload();
            }
        }
    })
}
// 撤销
function delete_(th) {
    var rowno = th.name;
    var back = $(`tr[id=${rowno}] td input[type='text']`).val();
    if (back == "" || back == null) {
        $('.alert').removeClass("alert-success")
        $('.alert').html("撤销理由不能为空！").addClass('alert-danger').show().delay(3000).fadeOut();
        return;
    }
    var yes = confirm("确认撤销吗？");
    var datas = [rowno, back];
    if (yes)
        $.ajax({
            url: "Back",
            type: "post",
            data: JSON.stringify(datas),
            dataType: "json",
            contentType: "application/json",
        });
    window.location.reload();
}
// 被勾选
function choose(th) {
    var dbid = th.id;
    var input1 = $(`tr[id='${dbid}'] td input[type='text']`);
    if (input1.hasClass("display2"))
        input1.removeClass("display2");
    else
        input1.addClass("display2");
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
    var url = "/Home/Find?" + params;
    window.location.href = url;
}