namespace System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class PengubahanNilaiRequestDto
{
    [DefaultValue("")]
    public string? SearchTerm { get; set; } = "";

    [DefaultValue("")]
    public string? KonsentrasiId { get; set; } = "";

    [DefaultValue("")]
    public string? TahunAjaran { get; set; } = "";

    [DefaultValue("")]
    public string? Semester { get; set; } = "";

    [DefaultValue("")]
    public string? Status { get; set; } = "";

    [DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "Page harus lebih besar dari 0")]
    public int Page { get; set; } = 1;

    [DefaultValue(10)]
    [Range(1, 100, ErrorMessage = "PageSize harus antara 1 dan 100")]
    public int PageSize { get; set; } = 10;
}