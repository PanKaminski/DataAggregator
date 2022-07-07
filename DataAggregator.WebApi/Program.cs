using System.Text.Json.Serialization;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Infrastructure;
using DataAggregator.Bll.Services;
using DataAggregator.Dal.Contract.Repositories;
using DataAggregator.Dal.Repositories;
using DataAggregator.WebApi.Authorization;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Services;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// AddAsync services to the container.

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
builder.Services.AddSingleton(_ => new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient<IDataAggregator, TaskDataAggregator>();
builder.Services.AddScoped<IEmailDataSender, EmailDataSender>();
builder.Services.AddScoped<IDataManager, DataManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApiTasksService, ApiTasksService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IApiTasksRepository, ApiTasksRepository>();
builder.Services.AddAutoMapper(typeof(AggregatorApiMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// custom jwt auth middleware
//app.UseErrorHandlerMiddleware();

// global error handler
app.UseJwtMiddleware();

app.MapControllers();

app.Run();
