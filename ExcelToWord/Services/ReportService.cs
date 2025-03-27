using System.Text.Json;
using BRB.Core.EF.Attributes;
using BRB.Core.OfficeTools;
using BRB.Core.OfficeTools.Factory;
using BRB.Core.OfficeTools.Handlers;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelToWord.Services.Contracts;
using MiniExcelLibs;

namespace ExcelToWord.Services;

[Injectable]
public class ReportService(IWebHostEnvironment environment)
{
    public async Task<MemoryStream> ReadFile(IFormFile file)
    {
        var items = await file.OpenReadStream().QueryAsync<ReportDto>(excelType: ExcelType.CSV);
        var reportList = items.ToList();
        if(reportList.Count == 0)
            return null;
        
        reportList = reportList.Select((x, i) =>
        {
            x.Number = i + 1;
            return x;
        }).ToList();
        var model = new Dictionary<string, object>
        {
            ["N"] = 1,
            ["Report"] = reportList
        };
        var memoryStream = new MemoryStream();
        var path = Path.Join(environment.ContentRootPath, "Services", "Templates", "ReportTemplate000.docx");
        await memoryStream.SaveAsByTemplateAsync(path, model);
        // memoryStream.Position = 0;
        await Convert(reportList, memoryStream);
        return memoryStream;
    }

    private async Task Convert(List<ReportDto> model, MemoryStream wordFile)
    {
        var plh = WordFactory.MakePlaceholders(new WordModel()
            {
                Items = model
            }
        );
        var handler = new DocXHandler(wordFile, plh);
        wordFile = handler.ReplaceTableRows();
    }
}

public class WordModel
{
    public List<ReportDto> Items { get; set; }
}