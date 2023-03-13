using KEEGames.UnityXUMMAPI.Services;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using UnityXUMMAPI.Helpers;
using UnityXUMMAPI.Models;
using UnityXUMMAPI.Repository;
using XUMM.NET.SDK;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(o => {
});
//builder.Host.ConfigureAppConfiguration(config =>
//{
//    config.AddJsonFile("appse.json");
//});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "XUMMAPI", Version = "v1" });
});
var sql = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(sql));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPlayerService,PlayerService>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IHelperEngine, HelperEngine>();  //change to PlayerService service instead of the test one after testing before deploy  
builder.Services.AddXummNet(builder.Configuration);
builder.Services.Configure<XummConnections>(builder.Configuration.GetSection("Xumm"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XUMMAPI");
});

//init and run
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
