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
    var url = "/Ledger/Find?" + params;
    window.location.href = url;
}
// 被勾选
function choose(th) {
    var dbid = th.id;
    var td = $(`tr[id='${dbid}'] td[name='back']`);
    if (td.children("input").hasClass("display2")) {
        td.children("input").removeClass("display2");
        td.children("span").addClass("display2");
    }
    else {
        td.children("input").addClass("display2");
        td.children("span").removeClass("display2");
    }
}
// 撤销
function backout(th) {
    var dbid = th.id;
    var backrea = $(`tr[id=${dbid}] td[name='back'] input`).val();
    if (backrea.trim() == "") {
        $('.alert').removeClass("alert-success")
        $('.alert').html("备注不能为空！").addClass('alert-danger').show().delay(3000).fadeOut();
        return;
    }
    window.location.href = `/Admin/Admin/Backout?dbid=${dbid}&backrea=${backrea}`;
}
// 历史数据导入
function import_file(th) {
    var file = th.files[0];
    var formdata = new FormData();
    formdata.append("file", file);
    $.ajax({
        url: "HistoryData",
        method: "post",
        data: formdata,
        contentType: false,
        processData: false,
        success: function (result) {
            window.location.reload();
        }
    });
}
// 导出
function Output() {
    var trs = $("tbody tr[id] td input[type='checkbox']");
    var datas = [];
    var ok = true;
    $.each(trs, function (index, item) {
        var inp = trs.eq(index);
        var ischeck = inp.is(':checked');
        if (ischeck) {  // 如果被选中了
            var no = inp.val();
            datas.push(no);
        }
    });
    if (datas.length == 0)
        return;
    if (ok)
        $.ajax({
            url: "/Ledger/Output2Excel",
            type: "post",
            data: JSON.stringify(datas),
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                if (result.Result == "成功") {
                    window.location.href = result.filepath;
                }
                else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                }
            }
        })
}