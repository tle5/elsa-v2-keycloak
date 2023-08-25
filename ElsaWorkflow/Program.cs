using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ElsaWorkflow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var elsaSection = builder.Configuration.GetSection("Elsa");
            // Elsa services.
            builder.Services
                .AddElsa(elsa => elsa
                    //.UseEntityFrameworkPersistence(ef => ef.UsePostgreSql(connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!))
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddQuartzTemporalActivities()
                    //.AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
                    .AddWorkflowsFrom<Elsa.Persistence.EntityFramework.Sqlite.Startup>()
                );

            // Elsa API endpoints.
            builder.Services.AddElsaApiEndpoints();

            // For Dashboard.
            builder.Services.AddRazorPages();
            builder.Services.AddKeycloakAuthentication(builder.Configuration, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        //var appContext = context.Request.HttpContext.RequestServices.GetService<AppContext>();
                        //appContext.OnTokenValidated(context);
                        //todo: add code to check
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseStaticFiles();
            app.UseHttpActivities();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers()
               .RequireAuthorization();

            app.UseCors(a => a.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
            app.MapRazorPages();
            app.Run();
        }
    }
}