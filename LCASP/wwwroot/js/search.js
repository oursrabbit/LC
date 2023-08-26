"use strict";

var search_media_list = null;

$(document).ready(function () {
    currentUser = new UserInfo(JSON.parse(sessionStorage.getItem("currentUser")));

    if (currentUser.Id == "") {
        window.location.href = "/Index";
        return;
    }

    $("#moresearch").hide();
    $("#loading").hide();

    createLanguageList($("#search_langage"), "语种");
    createKeywordList($("#search_keyword"), "关键词");
    createAreaList($("#search_area"), "领域");

    search_normalsearchbutton_click();
});


function search_normalsearchbutton_click() {
    $("#loading").html("数据查询中....");
    $("#loading").show();

    //kw1 kw2 kw3 ...
    var searchText = $("#search_normalsearch").val();

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
                search_media_list = [];
                response.forEach(function (json, index) {
                    search_media_list[index] = new MediaInfo(json);
                });
                $("#loading").hide();
                showMediaListByOrder();

            } catch (e) {
                console.log("NormalResourceSearch成功，但失败了：" + e);
                $("#loading").html("查询失败...." + e);
                $("#loading").show();
            }

        },
        error: function (xhr, status, error) {
            console.log("NormalResourceSearch失败了：" + error);
            $("#loading").html("服务器问题...." + error);
            $("#loading").show();
        }
    });
}

function search_moresearchbutton_click() {
    $("#loading").html("数据查询中：高级查询....");
    $("#loading").show();
    //null or empty for ignore

    //yyyy.mm.dd-yyyy.mmm.dd
    var publicdate = $("#search_publicdate").val() ?? "";
    //kw1 kw2 kw3 ...
    var keywords = $("#kw_input").val() ?? "";
    //lang1 lang2 lang3 ...
    var langs = $("#lang_input").val() ?? "";
    //a1 a2 a3 ...
    var areas = $("#area_input").val() ?? "";
    //ds1 ds2 ds3 ...
    var description = $("#search_description").val() ?? "";
    //name1 name2 name3
    var filename = $("#search_filename").val() ?? "";

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
                search_media_list = [];
                response.forEach(function (json, index) {
                    search_media_list[index] = new MediaInfo(json);
                });
                $("#loading").hide();
                showMediaListByOrder();

            } catch (e) {
                console.log("ResourceSearch成功，但失败了：" + e);
                $("#loading").html("高级查询失败...."  + e);
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

function search_showmoresearchbutton_click() {
    $("#normalsearch").hide();
    $("#moresearch").show();
    $("#loading").hide();
}

function search_shownormalsearchbutton_click() {
    $("#normalsearch").show();
    $("#moresearch").hide();
    $("#loading").hide();
}

function showMediaListByOrder() {
    $("#loading").hide();
    if (search_media_list != null) {
        var orderselectValue = $("#search_showorder").val();
        switch (orderselectValue) {
            case "最近上传":
                search_media_list.sort(function (a, b) {
                    var dayA = moment(a["resource_publish_date"], "yyyy.MM.dd");
                    var dayB = moment(b["resource_publish_date"], "yyyy.MM.dd");
                    return dayA >= dayB ? -1 : 0;
                });
                break;
            case "最早上传":
                search_media_list.sort(function (a, b) {
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
        search_media_list.forEach(function (item,index) {
            var row = ""
            row += rowbackcolor ? `<tr>` : `<tr style="background-color: #f2f2f2">`;
            rowbackcolor = !rowbackcolor;
            row += `<td><a href="#" id="media_list_${index}" onclick="search_showmediadetail()">${item["resource_file_name_zh"]}</a></td>`;
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

function search_showmediadetail() {
    $("#loading").hide();
    var targetElement = event.target; // 获取触发事件的元素
    var targetId = targetElement.id; // 获取目标元素的id
    var splittedId = targetId.split('_'); // 将id以'_'分割成数组
    var index = splittedId[2]; // 获取数组中第三个元素
    var media = search_media_list[index];

    $("#search_detail_resource_Id").html("资源唯一编号：" + media["resource_Id"]);
    $("#search_detail_resource_type").html("资源类型：" + media["resource_type"]);
    $("#search_detail_resource_lang").html("语种：" + media["resource_lang"]);
    $("#search_detail_resource_keyword").html("关键字：" + media["resource_keyword"]);
    $("#search_detail_resource_tag").html("标签（领域）：" + media["resource_tag"]);
    $("#search_detail_resource_publish_date").html("资源产生时间：" + media["resource_publish_date"]);
    $("#search_detail_resource_description").html("资源描述：" + media["resource_description"]);
    $("#search_detail_resource_source_uri").html("资源来源：" + media["resource_source_uri"]);
    $("#search_detail_resource_file_name").html("原始文件名：" + media["resource_file_name"]);
    $("#search_detail_resource_file_name_zh").html("中文文件名：" + media["resource_file_name_zh"]);
    $("#search_detail_resource_file_extension").html("资源文件后缀：" + media["resource_file_extension"]);
    $("#search_detail_resource_file_size").html("文件大小：" + media["resource_file_size"]);
    $("#search_detail_resource_upload_date").html("资源上传时间：" + media["resource_upload_date"]);
    $("#search_detail_resource_upload_user").html("资源上传人：" + media["resource_upload_username"]);
    $("#search_detail_video_clarity").html("视频清晰度：" + media["video_clarity"]);
    $("#search_detail_video_duration").html("视频时长：" + media["video_duration"]);
    $('#search_mediaInfoDetailModel').modal('show');
}

function search_close_resource_detail() {
    $('#search_mediaInfoDetailModel').modal('hide');
    $("#loading").hide();
}