using Microsoft.EntityFrameworkCore;
using NlpPipeline.Api.Data;
using NlpPipeline.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddScoped<RabbitMqService>();

var app = builder.Build();

// DOCKER İÇİN OTOMATİK MIGRATION REÇETESİ
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Veritabanı sunucusu henüz tam ayağa kalkmadıysa esneklik sağlamak için bağlantıyı doğrula
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Bekleyen migration'lar algılandı, veritabanına uygulanıyor...");
            context.Database.Migrate();
            Console.WriteLine("Migration'lar başarıyla uygulandı!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Docker veritabanına migration basılırken bir hata meydana geldi!");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();