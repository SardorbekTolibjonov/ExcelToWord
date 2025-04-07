using MiniExcelLibs.Attributes;

namespace ExcelToWord.Services.Contracts;

public class ReportDto
{
    [ExcelColumn(Index = 0)] public string Summary { get; set; } = default!;
    [ExcelColumn(Index = 1)] public string IssueKey { get; set; } = default!;
    [ExcelColumn(Index = 2)] public string IssueId { get; set; } = default!;
    [ExcelColumn(Index = 3)] public string IssueType { get; set; } = default!;
    [ExcelColumn(Index = 4)] public string Status { get; set; } = default!;
    [ExcelColumn(Index = 5)] public string ProjectKey { get; set; } = default!;
    [ExcelColumn(Index = 6)] public string ProjectName { get; set; } = default!;
    [ExcelColumn(Index = 7)] public string ProjectType { get; set; } = default!;
    [ExcelColumn(Index = 8)] public string ProjectLead { get; set; } = default!;
    [ExcelColumn(Index = 9)] public string ProjectLeadId { get; set; } = default!;
    [ExcelColumn(Index = 10)] public string ProjectDescription { get; set; } = default!;
    [ExcelColumn(Index = 11)] public string Priority { get; set; } = default!;
    [ExcelColumn(Index = 12)] public string Resolution { get; set; } = default!;
    [ExcelColumn(Index = 13)] public string Assignee { get; set; } = default!;
    [ExcelColumn(Index = 14)] public string AssigneeId { get; set; } = default!;
    [ExcelColumn(Index = 15)] public string Reporter { get; set; } = default!;
    [ExcelColumn(Index = 16)] public string ReportId { get; set; } = default!;
    [ExcelColumn(Index = 17)] public string Creator { get; set; } = default!;
    [ExcelColumn(Index = 18)] public string CreatorId { get; set; } = default!;
    [ExcelColumn(Index = 19)] public string Created { get; set; } = default!;
    [ExcelColumn(Index = 20)] public string Updated { get; set; } = default!;
    [ExcelColumn(Index = 21)] public string LastViewed { get; set; } = default!;
    [ExcelColumn(Index = 22)] public string Resolved { get; set; } = default!;
    [ExcelColumn(Index = 23)] public string DueDate { get; set; } = default!;
    [ExcelColumn(Index = 24)] public string Votes { get; set; } = default!;
    [ExcelColumn(Index = 25)] public string Labels { get; set; } = default!;
    [ExcelColumn(Index = 26)] public string Description { get; set; } = default!;
    [ExcelColumn(Index = 27)] public string Environment { get; set; } = default!;
    public int Number { get; set; }
    //   ==ProjectLead==
}
public class WordModel
{
    public List<ReportDto> Items { get; set; }
    public string Position { get; set; } = default!;
    public string Assignee { get; set; } = default!;
    public string Month { get; set; } = default!;
}