using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ReactProjectsAuthApi.Data;
using ReactProjectsAuthApi.Data.Security;
using ReactProjectsAuthApi.Hubs;
using ReactProjectsAuthApi.Profiles;
using Serilog;
using System.Text;
using ReactProjectsAuthApi.Middlewares;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.PostgreSql;
using ReactProjectsAuthApi;
using Microsoft.AspNetCore.Identity.UI.Services;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("PORT"));

IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;

services.AddDbContext<ChatDbContext>(opts =>
{
    opts.UseNpgsql(configuration.GetConnectionString("ChatConn"));
});

services.AddDbContext<AppDbContext>()
    .AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.User.RequireUniqueEmail = true;
        opts.SignIn.RequireConfirmedEmail = true;
    }).AddRoles<IdentityRole>()
    .AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

services.AddCors();

services.AddControllers().AddNewtonsoftJson(opts =>
{
    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSignalR();

services.AddFluentValidation(opts =>
{
    opts.RegisterValidatorsFromAssemblyContaining<RegisterModelValidator>();
});
services.AddAutoMapper(typeof(MainProfile));

services.AddHangfire(opts =>
{
    opts.UsePostgreSqlStorage(configuration.GetConnectionString("HangConn"));
});
services.AddHangfireServer();

services.AddScoped<IEmailSender, FluentEmailSender>();
services.AddScoped<HangfireJobs>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseCors((policyOpts) =>
{
    policyOpts.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
});
app.UseRouting();
app.UseWSRequestAuth();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MainHub>("/hub");


app.Run();
