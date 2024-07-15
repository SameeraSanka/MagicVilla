using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logger;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

//mela user login ekak newei me kiynne  system eke log gana. mekata packages 2k insrtal krnna one. eka nuget package manager eke athi
//Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
//	.WriteTo.File("log/villaLogs.txt",rollingInterval: RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog();
// me uda karana log ekama apitama hdnna puluwan. 


builder.Services.AddControllers().AddNewtonsoftJson();
//builder.Services.AddControllers(option => { option.ReturnHttpNotAcceptable = true; }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters(); //hema dnwa postman eka use krddi. eth swagger ekta wda krnne na
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILoggerCustom, LoggerCustom>(); // custom hadapu logger eka mehema register krnna one

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
