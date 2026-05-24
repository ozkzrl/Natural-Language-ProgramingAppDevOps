using Microsoft.EntityFrameworkCore;
using NlpPipeline.Engine;
using NlpPipeline.Engine.Data;

var builder = Host.CreateApplicationBuilder(args);

// Port=5439 bilgisini buraya da ekleyerek Worker'ın doğru veritabanına bakmasını sağlıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5439;Database=nlp_db;Username=nlp_user;Password=nlp_password"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();