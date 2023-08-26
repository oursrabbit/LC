"use strict";

var admin_index_media_list = null;

$(document).ready(function () {
    currentUser = new UserInfo(JSON.parse(sessionStorage.getItem("currentUser")));

    if (currentUser.Id == "") {
        window.location.href = "/Index";
        return;
    }

    $("#moresearch").hide();
    $("#loading").hide();

    $("#admin_index_label_username").html(currentUser.Username + "，您来啦~~");

    createLanguageList($("#admin_index_langage"), "语种");
    createKeywordList($("#admin_index_keyword"), "关键词");
    createAreaList($("#admin_index_area"), "领域");

    admin_index_normalsearchbutton_click();
});

function admin_index_normalsearchbutton_click() {
    $("#loading").html("数据查询中....");
    $("#loading").show();

    //kw1 kw2 kw3 ...
    var searchText = $("#admin_index_normalsearch").val();

    var sendJson = {
        searchText: searchText
    };

    $.ajax({
        type: "POST",
        url: aspApiServer + "/MediaInfo/NormalResourceSearch",
        data: JSON.stringify(sendJson),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                admin_index_media_list = [];
                response.forEach(function (json, index) {
                    admin_index_media_list[index] = new MediaInfo(json);
                });
                $("#loading").hide();
                showMediaListByOrder();

            } catch (e) {
                $("#loading").html("查询失败...." + e);
                $("#loading").show();
            }

        },
        error: function (xhr, status, error) {
            $("#loading").html("服务器问题...." + error);
            $("#loading").show();
        }
    });
}

function admin_index_moresearchbutton_click() {
    $("#loading").html("数据查询中：高级查询....");
    $("#loading").show();
    //null or empty for ignore

    //yyyy.mm.dd-yyyy.mmm.dd
    var publicdate = $("#admin_index_publicdate").val() ?? "";
    //kw1 kw2 kw3 ...
    var keywords = $("#kw_input").val() ?? "";
    //lang1 lang2 lang3 ...
    var langs = $("#lang_input").val() ?? "";
    //a1 a2 a3 ...
    var areas = $("#area_input").val() ?? "";
    //ds1 ds2 ds3 ...
    var description = $("#admin_index_description").val() ?? "";
    //name1 name2 name3
    var filename = $("#admin_index_filename").val() ?? "";

    var sendJson = {
        publicdate: publicdate,
        keywords: keywords,
        langs: langs,
        areas: areas,
        description: description,
        filename: filename,
    };

    $.ajax({
        type: "POST",
        url: aspApiServer + "/MediaInfo/ResourceSearch",
        data: JSON.stringify(sendJson),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                admin_index_media_list = [];
                response.forEach(function (json, index) {
                    admin_index_media_list[index] = new MediaInfo(json);
                });
                $("#loading").hide();
                showMediaListByOrder();

            } catch (e) {
                console.log("ResourceSearch成功，但失败了：" + e);
                $("#loading").html("高级查询失败...." + e);
                $("#loading").show();
            }

        },
        error: function (xhr, status, error) {
            console.log("ResourceSearch失败了：" + error);
            $("#loading").html("高级查询服务器问题...." + error);
            $("#loading").show();
        }
    });
}

function admin_index_showmoresearchbutton_click() {
    $("#normalsearch").hide();
    $("#moresearch").show();
    $("#loading").hide();
}

function admin_index_shownormalsearchbutton_click() {
    $("#normalsearch").show();
    $("#moresearch").hide();
    $("#loading").hide();
}

function showMediaListByOrder() {
    $("#loading").hide();
    if (admin_index_media_list != null) {
        var orderselectValue = $("#admin_index_showorder").val();
        switch (orderselectValue) {
            case "最近上传":
                admin_index_media_list.sort(function (a, b) {
                    var dayA = moment(a["resource_publish_date"], "yyyy.MM.dd");
                    var dayB = moment(b["resource_publish_date"], "yyyy.MM.dd");
                    return dayA >= dayB ? -1 : 0;
                });
                break;
            case "最早上传":
                admin_index_media_list.sort(function (a, b) {
                    var dayA = moment(a["resource_publish_date"], "yyyy.MM.dd");
                    var dayB = moment(b["resource_publish_date"], "yyyy.MM.dd");
                    return dayA >= dayB ? 0 : -1;
                });
                break;
        }

        var rowbackcolor = true;
        // Clear existing table rows
        $("#medialisttable tbody").empty();
        // Iterate over the list and add rows to the table
        admin_index_media_list.forEach(function (item, index) {
            var row = ""
            row += rowbackcolor ? `<tr>` : `<tr style="background-color: #f2f2f2">`;
            rowbackcolor = !rowbackcolor;
            row += `<td><a href="#" id="media_list_${index}" onclick="admin_index_showmediadetail()">${item["resource_file_name_zh"]}</a></td>`;
            row += `<td>${item["resource_keyword"]}</td>`;
            row += `<td>${item["resource_description"]}</td>`;

            var src = aspApiServer + "/Resource/Video?id=" + item["resource_Id"];
            row += `
                <td>
                    <video style="width: 100%;" controls>
                    <source src="${src}" type="video/mp4"/>
                    </video>
                </td>
        `;
            row += `</tr>`;

            $("#medialisttable tbody").append(row);
        });

        $('#loading').hide();
    }
}

function admin_index_showmediadetail() {
    $("#loading").hide();
    var targetElement = event.target; // 获取触发事件的元素
    var targetId = targetElement.id; // 获取目标元素的id
    var splittedId = targetId.split('_'); // 将id以'_'分割成数组
    var index = splittedId[2]; // 获取数组中第三个元素
    var media = admin_index_media_list[index];

    $("#admin_index_detail_resource_Id").html("资源唯一编号：" + media["resource_Id"]);
    $("#admin_index_detail_resource_type").html("资源类型：" + media["resource_type"]);
    $("#admin_index_detail_resource_lang").html("语种：" + media["resource_lang"]);
    $("#admin_index_detail_resource_keyword").html("关键字：" + media["resource_keyword"]);
    $("#admin_index_detail_resource_tag").html("标签（领域）：" + media["resource_tag"]);
    $("#admin_index_detail_resource_publish_date").html("资源产生时间：" + media["resource_publish_date"]);
    $("#admin_index_detail_resource_description").html("资源描述：" + media["resource_description"]);
    $("#admin_index_detail_resource_source_uri").html("资源来源：" + media["resource_source_uri"]);
    $("#admin_index_detail_resource_file_name").html("原始文件名：" + media["resource_file_name"]);
    $("#admin_index_detail_resource_file_name_zh").html("中文文件名：" + media["resource_file_name_zh"]);
    $("#admin_index_detail_resource_file_extension").html("资源文件后缀：" + media["resource_file_extension"]);
    $("#admin_index_detail_resource_file_size").html("文件大小：" + media["resource_file_size"]);
    $("#admin_index_detail_resource_upload_date").html("资源上传时间：" + media["resource_upload_date"]);
    $("#admin_index_detail_resource_upload_user").html("资源上传人：" + media["resource_upload_username"]);
    $("#admin_index_detail_video_clarity").html("视频清晰度：" + media["video_clarity"]);
    $("#admin_index_detail_video_duration").html("视频时长：" + media["video_duration"]);
    $('#admin_index_mediaInfoDetailModel').modal('show');
}

function admin_index_close_resource_detail() {
    $('#admin_index_mediaInfoDetailModel').modal('hide');
    $("#loading").hide();
}

function admin_index_export_media_to_excel() {
    var workbook = XLSX.utils.book_new(); // 创建一个新的工作簿对象
    workbook.SheetNames.push('Sheet1'); // 添加一个新工作表对象
    var ws_data = [];
    ws_data[0] = [];
    ws_data[0][0] = "资源唯一编号";
    ws_data[0][1] = "资源类型";
    ws_data[0][2] = "语种";
    ws_data[0][3] = "关键字";
    ws_data[0][4] = "标签（领域）";
    ws_data[0][5] = "资源产生时间";
    ws_data[0][6] = "资源描述";
    ws_data[0][7] = "资源来源";
    ws_data[0][8] = "原始文件名";
    ws_data[0][9] = "中文文件名";
    ws_data[0][10] = "资源文件后缀";
    ws_data[0][11] = "文件大小";
    ws_data[0][12] = "资源上传时间";
    ws_data[0][13] = "资源上传人编号";
    ws_data[0][14] = "资源上传人";
    ws_data[0][15] = "视频清晰度";
    ws_data[0][16] = "视频时长";

    admin_index_media_list.forEach(function (item, index) {
        ws_data[index + 1] = [];
        ws_data[index + 1][0] = item["resource_Id"];
        ws_data[index + 1][1] = item["resource_type"];
        ws_data[index + 1][2] = item["resource_lang"];
        ws_data[index + 1][3] = item["resource_keyword"];
        ws_data[index + 1][4] = item["resource_tag"];
        ws_data[index + 1][5] = item["resource_publish_date"];
        ws_data[index + 1][6] = item["resource_description"];
        ws_data[index + 1][7] = item["resource_source_uri"];
        ws_data[index + 1][8] = item["resource_file_name"];
        ws_data[index + 1][9] = item["resource_file_name_zh"];
        ws_data[index + 1][10] = item["resource_file_extension"];
        ws_data[index + 1][11] = item["resource_file_size"];
        ws_data[index + 1][12] = item["resource_upload_date"];
        ws_data[index + 1][13] = item["resource_upload_user"];
        ws_data[index + 1][14] = item["resource_upload_username"];
        ws_data[index + 1][15] = item["video_clarity"];
        ws_data[index + 1][16] = item["video_duration"];
    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    workbook.Sheets["Sheet1"] = ws;
    XLSX.writeFile(workbook, 'output.xlsx'); // 将工作簿写入文件中
    console.log('Excel文件已生成！'); // 在控制台中打印消息
}

function admin_index_download_excel_template() {
    var a = document.createElement("a");
    a.href = aspApiServer + "/Resource/Video/ExcelTemplate";
    a.download = "data.xlsx";
    document.body.appendChild(a);
    a.click();
    a.remove();
}

function admin_index_upload_excel() {
    $("#loading").html("Excel上传处理中....");
    $("#loading").show();
    var formData = new FormData();
    var fileInput = $('#admin_index_upload_excel_filepath')[0].files[0];
    formData.append('file', fileInput);
    $.ajax({
        url: aspApiServer + "/Resource/Video/UpdateDB?userId=" + currentUser.Id, 
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response == null || response == "")
            {
                $("#loading").html("更新成功^_^");
                $("#loading").show()
                return;
            }
            else {
                $("#loading").html("更新失败...." + response ?? "");
                $("#loading").show()
                return;
            }
        },
        error: function (xhr, status, error) {
            console.error('Error uploading file:', error);
            $("#loading").html("更新服务器问题...." + error);
            $("#loading").show();
        }
    });
}