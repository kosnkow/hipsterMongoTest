using System;
using MyCompany.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyCompany.Infrastructure {
    public static class DatabaseConfiguration {
        public static IServiceCollection AddDatabaseModule(this IServiceCollection @this, IConfiguration configuration)
        {
            var entityFrameworkConfiguration = configuration.GetSection("EntityFramework");

            var connection = new SqlConnection(new SqlConnectionStringBuilder {
                DataSource = entityFrameworkConfiguration["DataSource"]
            }.ToString());

            @this.AddDbContext<ApplicationDatabaseContext>(options => options.UseSqlServer(connection));

            return @this;
        }

        public static IApplicationBuilder UseApplicationDatabase(this IApplicationBuilder @this,
            IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            if (environment.IsDevelopment() || environment.IsProduction()) {
                var context = serviceProvider.GetRequiredService<ApplicationDatabaseContext>();
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
            }

            return @this;
        }
    }
}
