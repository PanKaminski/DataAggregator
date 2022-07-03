using System.Text.Json.Serialization;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Infrastructure;
using DataAggregator.Bll.Services;
using DataAggregator.WebApi.Authorization;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var emailCreds = builder.Configuration.GetSection("EmailCredentials").Get<EmailCredentials>();
builder.Services.AddSingleton(emailCreds);
builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient<IDataAggregator, TaskDataAggregator>();
builder.Services.AddScoped<IEmailDataSender, EmailDataSender>();
builder.Services.AddScoped<IDataManager, DataManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseJwtMiddleware();

// custom jwt auth middleware
app.UseErrorHandlerMiddleware();

app.MapControllers();

app.Run();
