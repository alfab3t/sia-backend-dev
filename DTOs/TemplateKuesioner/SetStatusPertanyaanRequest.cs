namespace astratech_apps_backend.DTOs.TemplateKuesioner;
using System.ComponentModel.DataAnnotations;

public class SetStatusPertanyaanRequest
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Status harus diisi.")]
    public string Status { get; set; } = String.Empty;
}   