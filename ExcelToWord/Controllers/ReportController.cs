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
        var stream = await service.ReadFileByMiniWord(file, "MiniwordTemplate.docx");

        return this.File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Report");
    }
    
    [HttpPost]
    public async Task<IActionResult> ReadReportByOfficeTool(IFormFile file)
    {
        var stream = await service.ReadReportByOfficeTool(file, "ReportTemplate.docx");

        return this.File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Report");
    }
    
}