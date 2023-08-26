
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
            else if (controllerName == "IMGInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-IMGINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/IMGInfo，固定值pXq",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("pXq")
                });
            }
            else if (controllerName == "DocInfo")
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LCAPI-DOCINFO",
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = "一个自定义个头，只针对/DocInfo，固定值PXq",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Example = new OpenApiString("PXq")
                });
            }
        }
    }

    public class LoggerWriter : BackgroundService
    {
        public static Queue<String> log = new Queue<String>();
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (true)
            {
                Console.WriteLine("task");
                Thread.Sleep(1000);
            }
            return Task.CompletedTask;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v4", new OpenApiInfo
                {
                    Version = "v4",
                    Title = "LC API",
                    Description =  @"<a href='https://github.com/oursrabbit/LC'>LC网站数据库查询接口描述</a>"
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
                    c.SwaggerEndpoint("/swagger/v4/swagger.json", "LC API v4");
                    
                });
            //}

            app.UseAuthorization();
            app.UseCors();
            //app.UseCors("AllowAllOrigins");

            app.MapControllers();

            logger.Info("App set up end!");

            app.Run();
        }
    }
}