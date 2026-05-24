using Microsoft.AspNetCore.Mvc;
using NlpPipeline.Api.Data;
using NlpPipeline.Api.Models;
using NlpPipeline.Api.Services;

namespace NlpPipeline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitMqService _rabbitMqService;

        public AnalysisController(AppDbContext context, RabbitMqService rabbitMqService)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitText([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return BadRequest("Metin boş olamaz.");

            var request = new TextAnalysisRequest { RawText = text };
            _context.AnalysisRequests.Add(request);
            await _context.SaveChangesAsync();

            _rabbitMqService.PublishAnalysisTask(request.Id, request.RawText);
            return Ok(new { RequestId = request.Id, Status = "Kuyruğa Eklendi" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(Guid id)
        {
            var request = await _context.AnalysisRequests.FindAsync(id);
            return request == null ? NotFound() : Ok(request);
        }
    }
}