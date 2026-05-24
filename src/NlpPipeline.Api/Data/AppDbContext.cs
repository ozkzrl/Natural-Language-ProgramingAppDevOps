using Microsoft.EntityFrameworkCore;
using NlpPipeline.Api.Models;

namespace NlpPipeline.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TextAnalysisRequest> AnalysisRequests { get; set; }
    }
}