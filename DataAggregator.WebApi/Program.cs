using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Infrastructure;
using DataAggregator.Bll.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var emailCreds = builder.Configuration.GetSection("EmailCredentials").Get<EmailCredentials>();
builder.Services.AddSingleton(emailCreds);
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

app.MapControllers();

app.Run();
