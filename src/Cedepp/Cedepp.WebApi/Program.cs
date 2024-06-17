using Cedepp.Application.UseCases.Commands;
using Cedepp.Application.UseCases.Consumers;
using Cedepp.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CedeppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
        {
            IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            AutoRegisterTemplate = true,
            NumberOfReplicas = 1,
            NumberOfShards = 2
        })
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .ReadFrom.Configuration(context.Configuration);
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateLifetime = false;
        options.TokenValidationParameters.RequireExpirationTime = false;
        options.TokenValidationParameters.RequireSignedTokens = false;
        options.TokenValidationParameters.RequireAudience = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;

        options.TokenValidationParameters.SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            var jwtHandler = new JsonWebTokenHandler();
            var jsonToken = jwtHandler.ReadJsonWebToken(token);
            return jsonToken;
        };
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TempData"));

        var jwtBearerSettings = builder.Configuration.GetSection("JwtBearer");
        options.Authority = jwtBearerSettings["Authority"];
        options.Audience = "Cedepp.WebApi";
    });
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User"));
});
builder.Services.AddCors();

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(FinishUserRegistrationCommand).Assembly);

});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((cxt, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

       

        cfg.ReceiveEndpoint("OutOfOffice_UserConsumer_queue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(cxt);
        });
    });

});


var app = builder.Build();
app.UseCors(policyBuilder =>
{
    policyBuilder.WithOrigins("https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
