using System.Text.Json;
using BRB.Core.EF.Attributes;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelToWord.Services.Contracts;
using MiniExcelLibs;

namespace ExcelToWord.Services;

[Injectable]
public class ReportService
{
    public async Task ReadFile(IFormFile file)
    {
        var items = await file.OpenReadStream().QueryAsync<ReportDto>(excelType: ExcelType.CSV);
        var reportDtos = items.ToList();
        if(reportDtos.Count == 0)
            return;
        var jsonString = JsonSerializer.Serialize(reportDtos);
        Console.WriteLine(jsonString);
    }
}