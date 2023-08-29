# LC网站数据库查询接口描述


## v4 1.0.4

*更换了日志框架*

*MicrosoftLogger --> NLog*

*NLog --> LogFile.txt*

*增加了上传程序的下载接口，Flutter for all platform*

*新增 UtilitiesController*

*新增 /Utilities/GetResourceUploader 用来自动将文件名修改为数据库编号（flutter 3 windows)*

*新增 /Utilities/GetTestDataGenerator 用来自动生成各种测试数据（.net framework 4.8)*

**---Update---**

1. double MediaInfo.resource_file_size --> string MediaInfo.resource_file_size_string

2. 新增MediaInfo.video_equipment_tag

3. 新增SearchJSON.video_equipment_tag

4. MediaInfo的普通查询和高级查询中，增加了SearchJSON.video_equipment_tag字段

## v3 1.0.3

*增加四种资源*

*Model：模型资源。DB模型类：ModelInfo，JSON类：ModelInfoJSON*

*AE：AE资源。DB模型类：AEInfo，JSON类：AEInfoJSON*

*IMG：图片资源。DB模型类：IMGInfo，JSON类：IMGInfoJSON*

*DOC：文档资源。DB模型类：DOCInfo，JSON类：DOCInfoJSON*

**---Update---**

1. 修复了一些之前没有验证Header的问题

2. Excel上传后，增加了新的数据单元格验证，根据参数ignore_cell_error，决定验证发生错误时的处理方式

## v2 1.0.2

*根据是否传输数据至服务器，只保留了GET POST*

*返回的StatusCode不为200时，使用RestResultJSON描述错误信息*

*第一次接口对接问题.txt*

*直接导出Excel，对列的编辑在Excel中进行~~~*

*云存储收费问题，视频文件上传目前都是云后台直接操作，不需要接口*

**---Update---**

1. 重命名了Schema，为XXXXXJSON

2. 简化HTTP VERB，使用RestResultJSON返回更加详细的信息

3. 重命名了API URL与parameter的名称

4. 增加MediaInfoJSON.video_url与MediaInfo.video_download_url

5. 查询List类型数据时，增加了page, page_size, total三个属性

6. excel数据上传时，添加了每个单元格的异常处理，新增一个URL参数

7. 增加了一个单个上传MediaInfo的API

## v1 1.0.1

*UserInfo、MediaInfo、ObjectId为后端使用的MongoDB Model*

*UserInfoJSON、MediaInfoJSON、FullTextSearch、Search为前后端通讯用的JSON结构*

*UserInfo为用户信息，MediaInfo为视频资源信息*