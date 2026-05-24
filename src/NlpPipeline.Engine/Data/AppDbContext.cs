using Microsoft.EntityFrameworkCore;
using NlpPipeline.Engine.Data;
using NlpPipeline.Engine.Models;

namespace NlpPipeline.Engine.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TextAnalysisRequest> AnalysisRequests { get; set; }
    }
}