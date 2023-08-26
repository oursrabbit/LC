"use strict";

$(document).ready(function () {
    $("#loginerror").hide();
});

function index_loginbutton_click() {
    //debugVerifyUser();
    verifyUser();
}

function verifyUser() {

    $("#loginerror").html("登录中...请勿刷新或重复点击");
    $("#loginerror").show();
    var username = $("#username").val();
    var password = $("#password").val();

    var sendJson = {
        id: "",
        username: username,
        password: password,
        type: ""
    };

    $.ajax({
        type: "POST",
        url: aspApiServer + "/UserInfo",
        data: JSON.stringify(sendJson),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                currentUser = new UserInfo(response);
                sessionStorage.setItem("currentUser", JSON.stringify(currentUser));
                switch (currentUser.Type) {
                    case "user":
                        window.location.href = "/User/Search";
                        break;
                    case "admin":
                        window.location.href = "/Admin/Index";
                        break;
                }
            } catch (e) {
                console.log("UserInfo成功，但失败了：" + e);
                $("#loginerror").html("用户名或密码错误..." + e);
                $("#loginerror").show();
            }

        },
        error: function (xhr, status, error) {
            console.log("UserInfo失败了：" + error);
            $("#loginerror").html("服务器响应问题..." + error);
            $("#loginerror").show();
        }
    });
}