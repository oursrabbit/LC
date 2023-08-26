using LCMVC.DatabaseHelper.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;

namespace LCMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            builder.Services.AddSession();

            builder.Services.AddDistributedMemoryCache();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Index}/{action=Login}/{id?}");
            app.UseSession();

            app.UseEndpoints(endpoints => { endpoints.MapHub<LCMVCHUB>("/LCMVCHUB"); });

            app.Run();
        }
    }
}