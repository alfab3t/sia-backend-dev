using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class CreatePengubahanNilaiDto
    {
        [Required(ErrorMessage = "Tahun ajaran harus diisi")]
        public string TahunAjaran { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus diisi")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mata kuliah harus diisi")]
        public string MataKuliahId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kelas harus diisi")]
        public string KelasId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alasan pengajuan harus diisi")]
        public string AlasanPengajuan { get; set; } = string.Empty;
    }
}