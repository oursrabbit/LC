// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

"use strict";

class UserInfo {
    constructor(json) {
        this.Id = json.Id ?? "";
        this.Username = json.Username ?? "";
        this.Type = json.Type ?? "";
    }
}

var currentUser = new UserInfo({ Id: "", Username: 'Alice' });

class MediaInfo {
    constructor(json) {
        // 资源通用信息表
        this.resource_Id = json.Id ?? ""; // 资源唯一编号
        this.resource_type = json.resource_type ?? ""; // 资源类型  
        this.resource_lang = json.resource_lang ?? ""; // 语种
        this.resource_keyword = json.resource_keyword ?? ""; // 关键字
        this.resource_tag = json.resource_tag ?? ""; // 标签（领域）
        this.resource_publish_date = json.resource_publish_date ?? "1999.1.1"; // 资源产生时间
        this.resource_description = json.resource_description ?? ""; // 资源描述
        this.resource_source_uri = json.resource_source_uri ?? ""; // 资源来源

        this.resource_file_name = json.resource_file_name ?? ""; // 原始文件名
        this.resource_file_name_zh = json.resource_file_name_zh ?? ""; // 中文文件名
        this.resource_file_extension = json.resource_file_extension ?? ""; // 资源文件后缀
        this.resource_file_size = json.resource_file_size ?? ""; // 文件大小

        this.resource_upload_date = json.resource_upload_date ?? ""; // 资源上传时间
        this.resource_upload_user = json.resource_upload_user ?? ""; // 资源上传人
        this.resource_upload_username = json.resource_upload_username ?? "";

        // 视频资源表
        this.video_clarity = json.video_clarity ?? ""; // 视频清晰度
        this.video_duration = json.video_duration ?? ""; // 视频时长
    }
}

// 后端服务器地址
var aspApiServer = "http://114.115.220.129:5000";