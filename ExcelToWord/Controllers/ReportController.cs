using ExcelToWord.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelToWord.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ReportController(ReportService service) : ControllerBase
{
    [HttpPost]
    public async Task ReadReport(IFormFile file) => await service.ReadFile(file); 
}