using Microsoft.EntityFrameworkCore;
using GasApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost\\SQLEXPRESS01;Database=GasDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔥 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGasWeb",
        policy =>
        {
            policy.WithOrigins("https://localhost:7234")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔥 CORS (activar)
app.UseCors("AllowGasWeb");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();