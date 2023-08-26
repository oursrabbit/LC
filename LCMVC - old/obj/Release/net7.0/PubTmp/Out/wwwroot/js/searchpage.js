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
            var starValue = parseFloat(item["mediaaverstar"]);
            row += `
                <td class="star-rating" value="${item["Id"]}">
                    <span class='fa fa-star ${starValue > 0 ? "checked" : ""}' mediaid='${item["Id"]}' value='1'></span>
                    <span class='fa fa-star ${starValue > 1 ? "checked" : ""}' mediaid='${item["Id"]}' value='2'></span>
                    <span class='fa fa-star ${starValue > 2 ? "checked" : ""}' mediaid='${item["Id"]}' value='3'></span>
                    <span class='fa fa-star ${starValue > 3 ? "checked" : ""}' mediaid='${item["Id"]}' value='4'></span>
                    <span class='fa fa-star ${starValue > 4 ? "checked" : ""}' mediaid='${item["Id"]}' value='5'></span>
                </td>
        `;
            row += `</tr>`;

            $("#medialisttable tbody").append(row);
        });

        $('.loading').hide();
        $('.tablelist').show();

        $('.fa-star').mouseover(function () {
            $(this).prevAll().addBack().addClass('checked');
            $(this).nextAll().removeClass('checked');
        });

        $('.fa-star').click(function () {
            var sendjson = {};
            sendjson.id = $(this).attr('mediaid');
            sendjson.value = $(this).attr('value');
            connection.invoke("SearchPageUpdateStar", JSON.stringify(sendjson));
            alert("还没设计好如何处理数据");
        });
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