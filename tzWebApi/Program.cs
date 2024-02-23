
using tzWepApi.Interfaces;
using tzWepApi.Services;

namespace tzWepApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IFileService, FileService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Any", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                        .WithOrigins(new[] { "http://localhost:5173", "http://localhost:5174", "https://tz-web-app-client.azurewebsites.net" })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });

            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            /*else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dispatch API V1");
                    c.RoutePrefix = string.Empty;
                });
            }*/

            app.UseHttpsRedirection();
            
            app.UseCors("Any");
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
