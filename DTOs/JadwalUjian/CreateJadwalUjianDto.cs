using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class CreateJadwalUjianDto
    {
        [Required(ErrorMessage = "ID Grup harus diisi.")]
        public required int GrupId { get; set; }

        [Required(ErrorMessage = "Tahun Akademik harus diisi.")]
        [StringLength(100)]
        public string TahunAkademik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus diisi.")]
        [StringLength(100)]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = " Jenis Ujian harus diisi.")]
        [StringLength(100)]
        public string JenisUjian { get; set; } = string.Empty; 
    }
}