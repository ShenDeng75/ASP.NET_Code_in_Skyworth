
show(); // 显示菜单栏
var dbid = "";
function show() {
    $("#Exemplar_import").removeClass("hide");
}
// 添加
function add() {
    var inputs = [];
    inputs.push($("[name='closedate']"));
    inputs.push($("[name='wlno']"));
    inputs.push($("[name='wlname']"));
    inputs.push($("[name='closepeople']"));
    inputs.push($("[name='wlclass']"));
    inputs.push($("[name='supp']"));

    var ok = verf(inputs, "添加"); // 验证格式

    if (ok) {    // 向服务器发起请求
        var datas = {};
        for (var i in inputs) {
            var ele = inputs[i];
            datas[ele.attr("name")] = ele.val();
        }
        datas["modelno"] = $("[name='modelno']").val();  // 由于模穴号可能为空，所以不参与验证

        $.ajax({
            url: "ImportOne",
            type: "post",
            data: datas,
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                if (result.Result == "成功") {
                    $("#save_button").attr("data-dismiss", "modal");
                    window.location.reload();
                } else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                    $("#save_button").removeAttr("data-dismiss", "modal");
                }
            }
        });
    }
    else
        $("#save_button").removeAttr("data-dismiss", "modal");
}
// 修改
function edit() {
    var inputs = [];
    inputs.push($("[name='wlno2']"));
    inputs.push($("[name='wlname2']"));
    inputs.push($("[name='closepeople2']"));
    inputs.push($("[name='wlclass2']"));
    inputs.push($("[name='supp2']"));

    var ok = verf(inputs, "修改"); // 验证格式

    if (ok) {    // 向服务器发起请求
        var datas = {};
        for (var i in inputs) {
            var ele = inputs[i];
            datas[ele.attr("name")] = ele.val();
        }
        datas["modelno2"] = $("[name='modelno2']").val();
        datas["closetime2"] = $("[name='closetime2']").val();
        datas["dbid"] = dbid;  // DBID

        $.ajax({
            url: "EditOne",
            type: "post",
            data: datas,
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                if (result.Result == "成功") {
                    $("#edit_button").attr("data-dismiss", "modal");
                    window.location.reload();
                } else {
                    $('.alert').removeClass("alert-success")
                    $('.alert').html(result.Result).addClass('alert-danger').show().delay(3000).fadeOut();
                    $("#edit_button").removeAttr("data-dismiss", "modal");
                }
            }
        });
    }
    else
        $("#edit_button").removeAttr("data-dismiss", "modal");
}
//验证格式
function verf(inputs, code) {
    var ok = true;
    var valtdtime = "";
    var otherss = "";
    if (code == "添加") {
        valtdtime = "validtime";
        otherss = "other"
    } else {
        valtdtime = "validtime2";
        otherss = "other2"
    }
    if ($("[name='" + valtdtime + "']").val() != "其他")  // 验证时间格式
        inputs.push($("[name='" + valtdtime + "']"));
    else {
        var other = $("[name='" + otherss + "']");
        var value = other.val().toString().trim();
        var isok = /^\d+$/.test(value.slice(0, value.length - 1));
        if (value.trim().charAt(value.length - 1) != "年" &&
            value.trim().charAt(value.length - 1) != "月" || !isok)
            other.val("");
        inputs.push(other);
    }

    for (var i in inputs) {    // 验证格式
        if (inputs[i].val() == null || inputs[i].val() == "" || inputs[i].val() == "---请选择---") {
            ok = false;
            var span = document.createElement("span");
            span.innerHTML = "<span style='color: red' > 请填写有效内容！</span >"
            if (inputs[i].siblings("span").length == 0)
                inputs[i].after(span);
        }
        else {
            inputs[i].siblings("span").remove();
        }
    }
    return ok;
}
// 其他
function other() {
    var vs = $('#valtime').val();
    if (vs == "其他") {
        $("#other").removeClass("hide");
    }
    else {
        $("#other").addClass("hide");
    }
}
// 其他2
function other2() {
    var vs = $('#valtime2').val();
    if (vs == "其他") {
        $("#other2").removeClass("hide");
    }
    else {
        $("#other2").addClass("hide");
    }
}

// 填充表单
function full(th) {
    var row = th.name;
    dbid = row;  // dbid
    var s = "tr[id='" + row + "'] td";
    var fullele = $(s);
    var time2 = fullele.eq(2).text().replace(/\//g, "-",2);
    $("td input[name='closetime2']").val(time2);
    $("td input[name='wlno2']").val(fullele.eq(3).text());
    $("td input[name='modelno2']").val(fullele.eq(4).text());
    $("td input[name='wlname2']").val(fullele.eq(5).text());
    $("td input[name='closepeople2']").val(fullele.eq(7).text());
    $("td input[name='supp2']").val(fullele.eq(8).text());

    var wlclass2 = fullele.eq(6).text();    // 初始化select
    var patt = `select[name='wlclass2'] option:contains('${wlclass2}')`;
    $(patt).prop("selected", true);

    var patt2 = `select[name='validtime2'] option:contains('其他')`;
    $(patt2).prop("selected", true);
    $("input[name='other2']").removeClass("hide");
    var validtime = fullele.eq(9).text();
    $("input[name='other2']").val(validtime);
}
// 选择导入文件
function choose_file() {
    var file = document.getElementById("file").files[0];
    var formdate = new FormData();
    formdate.append("file", file);
    $.ajax({
        url: "ImportExcel",
        method: "post",
        data: formdate,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.Result == "成功") {
                window.location.reload();
            }
            else{
                $('.alert').removeClass("alert-success")
                $('.alert').html(result.Result).addClass('alert-danger').show().delay(5000).fadeOut();
            }
        },
        error: function (err) {
            alert("上传失败");
        }
    })
}