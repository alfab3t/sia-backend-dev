using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JenisKuesioner
{
    public class UpdateJenisKuesionerRequest
    {
        [Required(ErrorMessage = "ID jenis kuesioner harus diisi.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama jenis kuesioner harus diisi.")]
        [StringLength(50)]
        public string NamaJenis { get; set; } = string.Empty;
    }
}