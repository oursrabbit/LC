var mediaListJson = null;

$('.loading').hide();
function showMediaListByOrder() {
    if (mediaListJson != null) {
        var orderselectValue = $("#orderselect").val();
        switch (orderselectValue) {
            case "评分":
                mediaListJson.sort(function (a, b) {
                    var starA = parseFloat(a["mediaaverstar"]);
                    var starB = parseFloat(b["mediaaverstar"]);
                    return starB - starA;
                });
                break;
            case "热度":
                mediaListJson.sort(function (a, b) {
                    var playA = parseFloat(a["mediaplaycount"]);
                    var playB = parseInt(b["mediaplaycount"]);
                    return playA - playB;
                });
                break;
            default: // 发布时间
                mediaListJson.sort(function (a, b) {
                    var dayA = moment(a["mediapublishdate"], "yyyy.MM.dd");
                    var dayB = moment(b["mediapublishdate"], "yyyy.MM.dd");
                    return dayA >= dayB ? -1 : 0;
                });
                break;
        }

        var keywordsArray = $("#keywords").val().split(" ");

        var rowbackcolor = true;
        // Clear existing table rows
        $("#medialisttable tbody").empty();
        // Iterate over the list and add rows to the table
        mediaListJson.forEach(function (item) {
            var row = ""
            row += rowbackcolor ? `<tr>` : `<tr style="background-color: #f2f2f2">`;
            rowbackcolor = !rowbackcolor;
            row += `<td>${item["mediatitle"]}</td>`;
            row += `<td>${item["mediatitle_zh"]}</td>`;
            row += `<td>`;
            item["mediakeyword"].split(" ").forEach(function (keywordinitem) {
                if (keywordsArray.includes(keywordinitem)) {
                    row += `<strong><span style="color:red;">${keywordinitem}</span></strong> `;
                }
                else {
                    row += `${keywordinitem} `;
                }
            });
            row += `</td>`;
            row += `<td>${item["mediainfo"]}</td>`;
            row += `<td>${item["mediatime"]}</td>`;
            row += `
                <td>
                    <video style="width: 100%;" controls>
                    <source src="${item["mediapath"]}" type="video/${item["mediaextension"]}" />
                    </video>
                </td>
        `;

            row += `
                <td value="${item["Id"]}">
                    <button class="btn btn-link" type="button" value="${item["Id"]}" onclick="onMediaInfoDetailOpen(this)">修改</button>
                    <button class="btn btn-danger" type="button" value="${item["Id"]}" onclick="onDeleteButtonClick(this)">删除</button>
                </td>
        `;
            row += `</tr>`;

            $("#medialisttable tbody").append(row);
        });

        $('.loading').hide();
        $('.tablelist').show();
    }
}

connection.on("SearchPageForMediaList", function (listjsonstring) {
    try {
        mediaListJson = JSON.parse(listjsonstring);
        showMediaListByOrder();
    } catch (e) {
        $(".temph1").html("失败，请检查查询信息，并重新检索");
    }
});

connection.on("AdminIndexPageDeleteMedia", function (res) {
    mediaListJson = mediaListJson.filter(item => item["Id"] != deleteMedia["Id"]);
    showMediaListByOrder();
});

connection.on("AdminIndexPageInsertMedia", function (res) {
    if(res == "")
        $(".loading").html("<h1>上传结束，刷新查看结果</h1>");
    else
        $(".loading").html(`<h1>${res}</h1>`);
});

connection.on("AdminIndexPageUpdateMedia", function (res) {
    if (res == "")
        $(".loading").html("<h1>更新结束，刷新查看结果</h1>");
    else
        $(".loading").html(`<h1>${res}</h1>`);
});

function getmediainfo() {
    $('.tablelist').hide();
    $('.loading').show();

    var mediaType_video = $("#video_check").prop("checked");
    var mediaType_ae = $("#ae_check").prop("checked");
    var mediaType_image = $("#image_check").prop("checked");
    var mediaType_document = $("#document_check").prop("checked");


    var titleInputValue = $("#titleinput").val();
    var publishdateValue = $("#publishdate").val();
    var keywordsValue = $("#keywords").val();
    var areaselectedValue = $("#areaselected").val();

    var sendjson = {};
    sendjson.mediaType_video = mediaType_video;
    sendjson.mediaType_ae = mediaType_ae;
    sendjson.mediaType_image = mediaType_image;
    sendjson.mediaType_document = mediaType_document;
    sendjson.titleInputValue = titleInputValue;
    sendjson.publishdateValue = publishdateValue;
    sendjson.keywordsValue = keywordsValue;
    sendjson.areaselectedValue = areaselectedValue;

    connection.invoke("SearchPageForMediaList", JSON.stringify(sendjson));
}

var deleteMedia = null;
function onDeleteButtonClick(button) {
    mediaListJson.forEach(function (item) {
        if (item["Id"] == button.value) {
            deleteMedia = item;
            return;
        }
    });
    $("#deletemediainfo").empty().append(deleteMedia['mediatitle']);
    $('#deleteConfirmModal').modal('show');
}

function onDeleteCancel() {
    $('#deleteConfirmModal').modal('hide');
}

function onDeleteConfirm() {
    $('#deleteConfirmModal').modal('hide');
    $('.tablelist').hide();
    $('.loading').show();

    connection.invoke("AdminIndexPageDeleteMedia", deleteMedia["Id"]);
}

function findNameByValueInOptions(value, findoptions) {
    for (const o of findoptions) {
        if (o.value === parseInt(value)) {
            return o.name;
        }
        if (o.children) {
            const result = findNameByValueInOptions(value, o.children);
            if (result != null) {
                return result;
            }
        }
    }
    return null;
}

function onExportTableMediaList()
{
    var workbook = XLSX.utils.book_new(); // 创建一个新的工作簿对象
    workbook.SheetNames.push('Sheet1'); // 添加一个新工作表对象
    var ws_data = [];
    ws_data[0] = [];
    ws_data[0][0] = "类型";
    ws_data[0][1] = "领域";
    ws_data[0][2] = "画面信息";
    ws_data[0][3] = "关键词";
    ws_data[0][4] = "文件位置";
    ws_data[0][5] = "发布时间";
    ws_data[0][6] = "质量";
    ws_data[0][7] = "时长";
    ws_data[0][8] = "原始标题";
    ws_data[0][9] = "中文标题";

    mediaListJson.forEach(function (item, index) {
        ws_data[index + 1] = [];
        ws_data[index + 1][0] = item["mediatype"];

        ws_data[index + 1][1] = "";
        item["mediaarea"].split(",").forEach(function (area) {
            var name = findNameByValueInOptions(area, options);
            if (name != null)
                ws_data[index + 1][1] += name + " ";
        });

        ws_data[index + 1][2] = item["mediainfo"];
        ws_data[index + 1][3] = item["mediakeyword"];
        ws_data[index + 1][4] = item["mediapath"];
        ws_data[index + 1][5] = item["mediapublishdate"];
        ws_data[index + 1][6] = item["mediaquality"];
        ws_data[index + 1][7] = item["mediatime"];
        ws_data[index + 1][8] = item["mediatitle"];
        ws_data[index + 1][9] = item["mediatitle_zh"];
    });

    var ws = XLSX.utils.aoa_to_sheet(ws_data);
    workbook.Sheets["Sheet1"] = ws;
    XLSX.writeFile(workbook, 'output.xlsx'); // 将工作簿写入文件中
    console.log('Excel文件已生成！'); // 在控制台中打印消息
}

function setupMediaInfoDetail(updateMedia, createNew) {
    if (createNew == true) {
        $('#detail_mediaID').hide();
    } else {
        $('#detail_mediaID').show();
    }

    if (updateMedia == null) {
        clearMediaInfoDetail();
    }
    else
    {
        $('#detail_mediaID').html("文件编号s：" + updateMedia["Id"]);

        $('#detail_mediatype').val(`${updateMedia["mediatype"]}`);
        $('#detail_mediatypeerror').html("");

        var areaList = [];
        updateMedia["mediaarea"].split(',').forEach(function (item, index) {
            areaList[index] = parseInt(item);
        });
        treeselect_detail.updateValue(areaList);
        inputhidden_detail.value = treeselect_detail.value;
        $('#detail_mediaareaerror').html("");;

        $('#detail_mediaaverstar').val(`${updateMedia["mediaaverstar"]}`);
        $('#detail_mediaaverstarerror').val("-1分表示新素材123，还未被评分");

        $('#detail_mediaextension').val(`${updateMedia["mediaextension"]}`);
        $('#detail_mediaextensionerror').html("");

        $('#detail_mediainfo').val(`${updateMedia["mediainfo"]}`);
        $('#detail_mediainfoerror').html("");

        $('#detail_mediakeyword').val(`${updateMedia["mediakeyword"]}`);
        $('#detail_mediakeyworderror').val("多个关键词请用空格分隔");

        $('#detail_mediapath').val(`${updateMedia["mediapath"]}`);
        $('#detail_mediapatherror').html("");

        $('#detail_mediaplaycount').val(`${updateMedia["mediaplaycount"]}`);
        $('#detail_mediaplaycounterror').html("");

        var publishDate = new Date(`${updateMedia["mediapublishdate"]}`);
        var formattedDate = publishDate.getFullYear() + '-' + (publishDate.getMonth() + 1).toString().padStart(2, '0') + '-' + publishDate.getDate().toString().padStart(2, '0');
        $('#detail_mediapublishdate').val(formattedDate);
        $('#detail_mediapublishdateerror').html("");

        $('#detail_mediaquality').val(`${updateMedia["mediaquality"]}`);
        $('#detail_mediaqualityerror').html("");

        $('#detail_mediatime').val(`${updateMedia["mediatime"]}`);
        $('#detail_mediatimeerror').val("时:分:秒");

        $('#detail_mediatitle').val(`${updateMedia["mediatitle"]}`);
        $('#detail_mediatitleerror').html("");

        $('#detail_mediatitle_zh').val(`${updateMedia["mediatitle_zh"]}`);
        $('#detail_mediatitle_zherror').html("");
    }
}

function clearMediaInfoDetail() {
    $('#detail_mediaID').html("文件编号c：");

    $('#detail_mediatype').val("视频素材123");
    $('#detail_mediatypeerror').html("");

    treeselect_detail.updateValue([]);
    $('#detail_mediaarea').val("");
    $('#detail_mediaareaerror').html("");;

    $('#detail_mediaaverstar').val("-1");
    $('#detail_mediaaverstarerror').html("-1分表示新素材123，还未被评分");

    $('#detail_mediaextension').val("mp4");
    $('#detail_mediaextensionerror').html("");

    $('#detail_mediainfo').val("");
    $('#detail_mediainfoerror').html("");

    $('#detail_mediakeyword').val("");
    $('#detail_mediakeyworderror').html("多个关键词请用空格分隔");

    $('#detail_mediapath').val("");
    $('#detail_mediapatherror').html("");

    $('#detail_mediaplaycount').val("0");
    $('#detail_mediaplaycounterror').html("");

    $('#detail_mediapublishdate').val('2000-10-04');
    $('#detail_mediapublishdateerror').html("");

    $('#detail_mediaquality').val("4K");
    $('#detail_mediaqualityerror').html("");

    $('#detail_mediatime').val("00:00:00");
    $('#detail_mediatimeerror').html("时:分:秒");

    $('#detail_mediatitle').val("");
    $('#detail_mediatitleerror').html("");

    $('#detail_mediatitle_zh').val("");
    $('#detail_mediatitle_zherror').html("");
}

var editingMedia = null;
function onMediaInfoDetailOpen(button) {
    clearMediaInfoDetail();
    if (button == null) // upload file clear all controls
    {
        editingMedia = null
        setupMediaInfoDetail(null, true)
    }
    else // update file set up all controls
    {
        editingMedia = null;
        mediaListJson.forEach(function (item) {
            if (item["Id"] == button.value) {
                editingMedia = item;
                return;
            }
        });
        setupMediaInfoDetail(editingMedia, false);
    }

    //$("#deletemediainfo").empty().append(deleteMedia['mediatitle']);
    $('#mediaInfoDetailModel').modal('show');
}

function onMediaInfoDetailClose() {
    $('#mediaInfoDetailModel').modal('hide');
    clearMediaInfoDetail();
}

// 还未启用
function onMediaInfoDetailTempSave() {
    $('#mediaInfoDetailModel').modal('hide');
}

// 还未启用
function onMediaInfoDetailUploadAndNew() {
    if (verifyAndCreateJsonFromMediaInfoDetail()[0] == true) {
        clearMediaInfoDetail();
        setupMediaInfoDetail(null, true);

        $('#deleteConfirmModal').modal('hide');
        $('.tablelist').hide();
        $('.loading').show();

        connection.invoke("AdminIndexPageDeleteMedia", deleteMedia["Id"]);
    }
}

function onMediaInfoDetailUpload() {
    var res = verifyAndCreateJsonFromMediaInfoDetail();
    if (res[0] == true) {
        $('#mediaInfoDetailModel').modal('hide');
        clearMediaInfoDetail();

        $('.tablelist').hide();
        $('.loading').show();
        $(".loading").html("<h1>处理中...</h1>");

        var sendJson = res[1];
        if (editingMedia == null) {
            sendJson.mediaId = "";
            connection.invoke("AdminIndexPageInsertMedia", JSON.stringify(sendJson));
        }
        else {
            sendJson.mediaId = editingMedia["Id"];
            connection.invoke("AdminIndexPageUpdateMedia", JSON.stringify(sendJson));
        }
    }
}

function verifyAndCreateJsonFromMediaInfoDetail()
{
    var sendjson = {};

    sendjson.mediatype = $('#detail_mediatype').val();
    sendjson.mediatitle = $('#detail_mediatitle').val();
    sendjson.mediatitle_zh = $('#detail_mediatitle_zh').val();
    sendjson.mediakeyword = $('#detail_mediakeyword').val();
    sendjson.mediaarea = inputhidden_detail.value;
    sendjson.mediaplaycount = $('#detail_mediaplaycount').val();
    sendjson.mediaaverstar = $('#detail_mediaaverstar').val();
    sendjson.mediaextension = $('#detail_mediaextension').val();
    sendjson.mediainfo = $('#detail_mediainfo').val();
    sendjson.mediapath = $('#detail_mediapath').val();
    sendjson.mediapublishdate = $('#detail_mediapublishdate').val();
    sendjson.mediaquality = $('#detail_mediaquality').val();
    sendjson.mediatime = $('#detail_mediatime').val();

    sendjson.mediauploaduserid = $('#currentuserlabel').attr("name");

    return [true, sendjson];
}