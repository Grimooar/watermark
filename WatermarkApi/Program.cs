using Microsoft.EntityFrameworkCore;
using WatermarkApi.DbContext;
using WatermarkApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataDbContext>(options => options.UseSqlite("Filename=MyTestedDb.db"));

builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();
DataDbInitializer.Initialize(app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

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