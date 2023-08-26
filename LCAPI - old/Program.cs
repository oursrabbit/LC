
using LCAPI.Models;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.Reflection;

namespace LCAPI
{
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
            builder.Services.AddSwaggerGen((c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PXQ API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }));

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // Replace with the actual web app

                });
            });

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        builder =>
            //        {
            //            builder.WithOrigins(new string[] {
            //            "https://w6v150011.yicp.fun",
            //            "http://w6v150011.yicp.fun",
            //            "https://pxqpxq.w1.luyouxia.net",
            //            "http://pxqpxq.w1.luyouxia.net",
            //            "https://pxqpxq.w1.luyouxia.net:80",
            //            "http://pxqpxq.w1.luyouxia.net:80",
            //            "https://pxqpxq.w1.luyouxia.net:443",
            //            "http://pxqpxq.w1.luyouxia.net:443",
            //            "https://*.yicp.fun",
            //            "http://*.yicp.fun",
            //            "https://*.luyouxia.net",
            //            "http://*.luyouxia.net",
            //            "https://*.luyouxia.net:80",
            //            "http://*.luyouxia.net:443",
            //            "http://localhost:5100",
            //            "https://localhost:5100"
            //            })
            //                .AllowAnyMethod()
            //                .AllowAnyHeader();
            //        });
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseCors();
            //app.UseCors("AllowAllOrigins");

            app.MapControllers();

            app.Run();
        }
    }
}