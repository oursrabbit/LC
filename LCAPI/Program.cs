
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
                    Description = "һ���Զ����ͷ��ֻ���/UserInfo���̶�ֵPXQ",
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
                    Description = "һ���Զ����ͷ��ֻ���/MediaInfo���̶�ֵpxq",
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
                    Description = "һ���Զ����ͷ��ֻ���/ModelInfo���̶�ֵPxq",
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
                    Description = "һ���Զ����ͷ��ֻ���/AeInfo���̶�ֵPxQ",
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
                    Description = "LC��վ���ݿ��ѯ�ӿ�����\r\n\r\n" +
                    "v4\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "MediaInfo.resource_file_size --> MediaInfo.resource_file_size_string\r\n\r\n" +
                    "����MediaInfo.video_equipment_tag\r\n\r\n" +
                    "����SearchJSON.video_equipment_tag\r\n\r\n" +
                    "MediaInfo����ͨ��ѯ�͸߼���ѯ�У�������SearchJSON.video_equipment_tag�ֶ�\r\n\r\n" +
                    "v3\r\n\r\n" +
                    "���������µ���Դ\r\n\r\n" +
                    "Model��ģ����Դ��DBģ���ࣺModelInfo��JSON�ࣺModelInfoJSON\r\n\r\n" +
                    "AE��AE��Դ��DBģ���ࣺAEInfo��JSON�ࣺAEInfoJSON\r\n\r\n" +
                    "IMG��ͼƬ��Դ��DBģ���ࣺIMGInfo��JSON�ࣺIMGInfoJSON\r\n\r\n" +
                    "DOC���ĵ���Դ��DBģ���ࣺDOCInfo��JSON�ࣺDOCInfoJSON\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "�޸���һЩ֮ǰû����֤Header������\r\n\r\n" +
                    "v2\r\n\r\n" +
                    "�����Ƿ�����������������ֻ������GET POST\r\n\r\n" +
                    "���ص�StatusCode��Ϊ200ʱ��ʹ��RestResultJSON����������Ϣ\r\n\r\n" +
                    "��һ�νӿڶԽ�����.txt\r\n\r\n" +
                    "ֱ�ӵ���Excel�����еı༭��Excel�н���~~~\r\n\r\n" +
                    "�ƴ洢�շ����⣬��Ƶ�ļ��ϴ�Ŀǰ�����ƺ�ֱ̨�Ӳ���������Ҫ�ӿ�\r\n\r\n" +
                    "---Update---\r\n\r\n" +
                    "��������Schema��ΪXXXXXJSON\r\n\r\n" +
                    "��HTTP VERB��ʹ��RestResultJSON���ظ�����ϸ����Ϣ\r\n\r\n" +
                    "��������API URL��parameter������\r\n\r\n" +
                    "����MediaInfoJSON.video_url��MediaInfo.video_download_url\r\n\r\n" +
                    "��ѯList��������ʱ��������page, page_size, total��������\r\n\r\n" +
                    "excel�����ϴ�ʱ�������ÿ����Ԫ����쳣��������һ��URL����\r\n\r\n" +
                    "������һ�������ϴ�MediaInfo��API\r\n\r\n" +
                    "v1\r\n\r\n" +
                    "UserInfo��MediaInfo��ObjectIdΪ���ʹ�õ�MongoDB Model\r\n\r\n" +
                    "UserInfoJSON��MediaInfoJSON��FullTextSearch��SearchΪǰ���ͨѶ�õ�JSON�ṹ\r\n\r\n" +
                    "UserInfoΪ�û���Ϣ��MediaInfoΪ��Ƶ��Դ��Ϣ\r\n\r\n",
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