using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Jurusan
{
    public class CreateJurusanRequest
    {

        [Required(ErrorMessage = "Nama jurusan harus diisi.")]
        [StringLength(100)]
        public string NamaJurusan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kepala Jurusan harus diisi.")]
        [StringLength(50)]
        public string KepalaJurusan { get; set; } = string.Empty;

        [Required(ErrorMessage = "No. NPK harus diisi.")]
        [StringLength(50)]
        public string NoNPK { get; set; } = string.Empty;

        [StringLength(255)]
        public string JurusanDeskripsi { get; set; } = string.Empty;

    }
}
