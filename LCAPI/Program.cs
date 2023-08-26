
using LCAPI.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace LCAPI
{
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class MyHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).ControllerName;

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            if (controllerName == "UserInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-USERINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/UserInfo，固定值PXQ",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("PXQ")
                });
            }
            else if (controllerName == "MediaInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-MEDIAINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/MediaInfo，固定值pxq",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("pxq")
                });
            }
            else if (controllerName == "ModelInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-MODELINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/ModelInfo，固定值Pxq",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("Pxq")
                });
            }
            else if (controllerName == "AeInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-AEINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/AeInfo，固定值PxQ",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("PxQ")
                });
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v3", new OpenApiInfo
                {
                    Version = "v4",
                    Title = "LC API",
                    Description = "LC网站数据库查询接口描述\r\n\r\n" +
                    "v4\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "MediaInfo.resource_file_size --> MediaInfo.resource_file_size_string\r\n\r\n" +
                    "新增MediaInfo.video_equipment_tag\r\n\r\n" +
                    "新增SearchJSON.video_equipment_tag\r\n\r\n" +
                    "MediaInfo的普通查询和高级查询中，增加了SearchJSON.video_equipment_tag字段\r\n\r\n" +
                    "v3\r\n\r\n" +
                    "增加四种新的资源\r\n\r\n" +
                    "Model：模型资源。DB模型类：ModelInfo，JSON类：ModelInfoJSON\r\n\r\n" +
                    "AE：AE资源。DB模型类：AEInfo，JSON类：AEInfoJSON\r\n\r\n" +
                    "IMG：图片资源。DB模型类：IMGInfo，JSON类：IMGInfoJSON\r\n\r\n" +
                    "DOC：文档资源。DB模型类：DOCInfo，JSON类：DOCInfoJSON\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "修复了一些之前没有验证Header的问题\r\n\r\n" +
                    "v2\r\n\r\n" +
                    "根据是否传输数据至服务器，只保留了GET POST\r\n\r\n" +
                    "返回的StatusCode不为200时，使用RestResultJSON描述错误信息\r\n\r\n" +
                    "第一次接口对接问题.txt\r\n\r\n" +
                    "直接导出Excel，对列的编辑在Excel中进行~~~\r\n\r\n" +
                    "云存储收费问题，视频文件上传目前都是云后台直接操作，不需要接口\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "重命名了Schema，为XXXXXJSON\r\n\r\n" +
                    "简化HTTP VERB，使用RestResultJSON返回更加详细的信息\r\n\r\n" +
                    "重命名了API URL与parameter的名称\r\n\r\n" +
                    "增加MediaInfoJSON.video_url与MediaInfo.video_download_url\r\n\r\n" +
                    "查询List类型数据时，增加了page, page_size, total三个属性\r\n\r\n" +
                    "excel数据上传时，添加了每个单元格的异常处理，新增一个URL参数\r\n\r\n" +
                    "增加了一个单个上传MediaInfo的API\r\n\r\n" +
                    "v1\r\n\r\n" +
                    "UserInfo、MediaInfo、ObjectId为后端使用的MongoDB Model\r\n\r\n" +
                    "UserInfoJSON、MediaInfoJSON、FullTextSearch、Search为前后端通讯用的JSON结构\r\n\r\n" +
                    "UserInfo为用户信息，MediaInfo为视频资源信息\r\n\r\n",
                });

                options.OperationFilter<MyHeaderFilter>();

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // Replace with the actual web app

                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(c => { 
                    c.SwaggerEndpoint("/swagger/v3/swagger.json", "LC API v3");
                    
                });
            //}

            app.UseAuthorization();
            app.UseCors();
            //app.UseCors("AllowAllOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}