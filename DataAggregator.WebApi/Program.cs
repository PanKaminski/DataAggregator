using System.Text.Json.Serialization;
using DataAggregator.WebApi.Authorization;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// AddAsync services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// configure DI for application services
builder.Services.AddBackgroundServices(builder.Configuration);

builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddBllServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AggregatorApiMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// global cors policy
app.UseCors("EnableCORS");

// global error handler
app.UseErrorHandlerMiddleware();

// custom jwt auth middleware
app.UseJwtMiddleware();

app.MapControllers();

app.Run();
