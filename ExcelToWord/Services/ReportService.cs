using BRB.Core.EF.Attributes;
using BRB.Core.OfficeTools.Factory;
using BRB.Core.OfficeTools.Handlers;
using ExcelToWord.Services.Contracts;
using MiniExcelLibs;

namespace ExcelToWord.Services;

[Injectable]
public class ReportService(IWebHostEnvironment environment)
{
    private readonly IWebHostEnvironment _environment = environment;
    private  readonly string _path = Path.Join(environment.ContentRootPath, "Services", "Templates");

    public async Task<MemoryStream> ReadFileByMiniWord(IFormFile file, string filePath)
    {
        var reportList = await this.ReadFromExcel(file);

        var reportListDict = reportList.Select(x => new Dictionary<string, object>
        {
            { "ProjectName", x.ProjectName },
            { "Summary", x.Summary },
            { "DueDate", x.DueDate },
            { "Resolved", x.Resolved },
            { "ProjectLead", x.ProjectLead },
            { "Assignee", x.Assignee },
            { "Status", x.Status },
            { "Number", x.Number }
        }).ToList();
        
        var model = reportList.Select(x => new Dictionary<string, object>
        {
            ["Position"] = "Custom Position",
            ["Assignee"] = reportList.Select(x => x.Assignee).First().ToString(),
            ["Month"] = $"{DateTime.Now.Year} - {DateTime.Now.Month:MMMM}",
            ["Report"] = reportListDict,
        });
        
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsByTemplateAsync(Path.Join(_path, filePath), model);
        memoryStream.Position = 0; 
        
        return memoryStream;
    }
    
    public async Task<MemoryStream> ReadReportByOfficeTool(IFormFile file, string filePath)
    {
        var wordTemplate = await this.ReadFileByMiniWord(file, filePath);
        var model = await ReadFromExcel(file);
        Convert(model, wordTemplate);
        
        return wordTemplate;
    }

    private async Task<List<ReportDto>> ReadFromExcel(IFormFile file)
    {
        var items = await file.OpenReadStream().QueryAsync<ReportDto>(excelType: ExcelType.CSV);
        var reportList = items.ToList();
        if(reportList.Count == 0)
            return reportList;
        
        reportList = reportList.Select((x, i) =>
        {
            x.Number = i + 1;
            x.Resolved = FormatDate(x.Resolved);
            x.DueDate = FormatDate(x.DueDate);
            
            return x;
        }).ToList();
        return reportList;
    }

    private void Convert(List<ReportDto> model, MemoryStream wordFile)
    {
        var placeholders = WordFactory.MakePlaceholders(new WordModel()
            {
                Items = model,
                Assignee1 = model.Select(x => x.Assignee).First().ToString(),
                Position = "Custom Position",
                Month = $"{DateTime.Now.Year} - {DateTime.Now.Month:MMMM}",
            }
        );
        

        var handler = new DocXHandler(wordFile, placeholders);
        handler.ReplaceTableRows();
    }

    private static string FormatDate(string date)
    {
        if (string.IsNullOrEmpty(date))
            return string.Empty;
        
        var format = "dd/MMM/yy hh:mm tt";
        if(DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out var result))
            return result.ToString("dd.MM.yyyy");  

        return string.Empty;
    }
}