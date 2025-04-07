using ExcelToWord.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelToWord.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ReportController(ReportService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ReadReport(IFormFile file)
    {
        var stream = await service.ReadFile(file);

        return this.File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Report");
    }
}