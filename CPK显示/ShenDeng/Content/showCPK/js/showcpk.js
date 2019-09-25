refresh_table();
timer();
// 定时器
function timer() {
    window.setInterval(refresh_table, 30 * 1000);
}
// 请求数据
function refresh_table() {
    var time = $("input[name='time']").val();
    var field = $("select[name='choose_cpk']").val();
    var datas = {};
    datas["time"] = time;
    datas["field"] = field;
    $.ajax({
        url: "/ShenDeng/ShowCPK/Find",
        method: "post",
        dataType: "json",
        async: false,
        contentType: "application/json",
        data: JSON.stringify(datas),
        success: function (result) {
            if (result.code) {
                do_success(result);
            }
            else {
                alert("数据获取失败！错误信息：" + result.result);
            }
        }
    });
    set_font_color();   // 改变颜色
}
// 显示数据
function do_success(result) {
    var tbody = $("#mtbody");
    tbody.empty();
    var datas = result.result;
    $.each(datas, function (index, item) {   // 遍历行
        var tr = $("<tr></tr>");
        var a = $("<a href='/Report/SMTAnaly/Index' style='color:#00FFFF;text-decoration:none;'></a>").append(item["lineid"]);
        var td = $("<td></td>").append(a);
        tr.append(td);
        for (var i = 1; i <= 24; i++) {   // 遍历列
            var td = $("<td></td>");
            td.append(item[i.toString()]);
            tr.append(td);
        }
        tbody.append(tr);
    });
    setWidth();
}
// 字体颜色
function set_font_color() {
    var trs = $("tbody tr");
    $.each(trs, function (index, item) {
        var tds = trs.eq(index).children("td");
        $.each(tds, function (index, item) {
            var td = tds.eq(index);
            if (index != 0)   // 除第一行外，其他设置成绿色
                td.addClass("toGreen");
            var val = parseFloat(td.text())
            if (val < 1.33) {
                td.addClass("toRed")
                td.removeClass("toGreen")
                td.addClass("toBlack")
            }
        });
    });
}
setWidth();

// 设置表头宽度
function setWidth() {
    var trs = $("#wrap table tr");
    if (trs.length <= 1)
        return;
    var tds = trs.eq(1).children("td");
    var ths = trs.eq(0).children("th");
    for (var i = 0; i < tds.length; i++) {
        var td = tds.eq(i).width();
        var csstext = "width:" + td + "px !important;"
        ths.eq(i).css("cssText", csstext);
    }
    var td_len = trs.eq(1).width();
    trs.eq(0).width(td_len);
}
// 浏览器大小改变时触发
$(window).resize(function () {
    setWidth();
});

// 设置表头位置，即固定表头
var obj_th;
window.onload = function () {
    obj_th = document.getElementById("th");
};
window.onscroll = function () {  // 滑动页面时触发
    // 这里的top是页面顶部纵坐标的值，整个页面在第四象限。
    var top = document.documentElement.scrollTop;
    if (top >= 100) {    // 当表头到第一象限时，使表头停在页首
        obj_th.style.top = top - 100 + "px";
    }
    if (top == 0) {
        obj_th.style.top = 0 + "px";
    }
};