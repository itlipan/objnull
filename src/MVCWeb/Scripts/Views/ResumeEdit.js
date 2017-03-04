//更新MDV
function UpateMDV() {
    $("#MDValue").html("");
    $("#MDValue").html(marked($("#TxtMD").val()));
}

//textare当前位置插入
function InsertAtCaret(id, val) {
    var $txt = jQuery("#" + id);
    var caretPos = $txt[0].selectionStart;
    var textAreaTxt = $txt.val();
    $txt.val(textAreaTxt.substring(0, caretPos) + val + textAreaTxt.substring(caretPos));
}

$(function () {
    //marked选项
    marked.setOptions({
        highlight: function (code) {
            return hljs.highlightAuto(code).value;
        },
        sanitize: false,
        breaks: true
    });

    UpateMDV();

    //实时更新
    $("#TxtTitle").bind("input propertychange", function () {
        $("#MDTitle").html($("#TxtTitle").val());
    });
    $("#TxtMD").bind("input propertychange", function () {
        UpateMDV();
    });

    //选择文件
    $("#TxtChoseFile").click(function () {
        $("#JqueryUpload").click();
    });
    //阻止全局拖拽，使自定义区域拖拽上传生效
    $(document).bind("drop dragover", function (e) {
        e.preventDefault();
    });
    //上传
    var allowExt = [".jpg", ".png", ".bmp", ".gif"];
    var pt = $("#ValPt").val();
    $("#JqueryUpload").fileupload({
        dropZone: $("#TxtMD"),
        pasteZone: $("#TxtMD"),
        add: function (e, data) {
            if (data.files[0].name != null) {
                var ext = data.files[0].name.substr(data.files[0].name.lastIndexOf("."));
                if (allowExt.indexOf(ext) < 0) {
                    swal("图片格式不允许");
                    return;
                }
            }
            if (data.files[0].size > 1024 * 200) {
                swal("因生成PDF中图片耗费大量CPU，所以图片最大为200KB");
                return;
            }
            data.submit();
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $("#UpPercent").html("已上传" + progress + "%");
        },
        dataType: "json",
        done: function (e, data) {
            $("#UpPercent").html("");
            if (data.result.error != "") {
                swal(data.result.error);
            } else {
                var link = "![](http://" + window.location.host + "/File/DownloadImg?pt="+ pt + "&path=" + data.result.path + ")";
                InsertAtCaret("TxtMD", link);
                UpateMDV();
                SetViewer("MDValue");
            }
        },
        fail: function (e, data) {
            $("#UpPercent").html("");
            swal("上传失败");
        }
    });

    //保存
    $("#BtnConfirm").click(function () {
        var ispub = false;
        if($("input[name='ispub']:checked").val() == "1"){
            ispub = true;
        }
        $("#BtnConfirm").attr("disabled", true);
        $.ajax({
            url: "/Resume/EditSave",
            data: { md: $("#TxtMD").val(), html: $("#MDValue").html(), ispub: ispub },
            type: "post",
            success: function (result) {
                $("#BtnConfirm").attr("disabled", false);
                if (result.msg == "done") {
                    window.location.href = "/Resume/See/" + result.gid;
                } else {
                    swal("保存出错");
                }
            }
        });
    });
});
