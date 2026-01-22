using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Jurusan
{
    public class UpdateJurusanRequest
    {

        [Required(ErrorMessage = "ID Jurusan harus diisi.")]
        [Range(1, short.MaxValue, ErrorMessage = "ID Jurusan tidak valid.")]
        public short Id { get; set; }

        [Required(ErrorMessage = "Nama jurusan harus diisi.")]
        [StringLength(100)]
        public string NamaJurusan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kepala Jurusan harus diisi.")]
        [StringLength(50)]
        public string KepalaJurusan { get; set; } = string.Empty;

        [Required(ErrorMessage = "No. NPK harus diisi.")]
        [StringLength(50)]
        public string NoNPK { get; set; } = string.Empty;

        [StringLength(250)]
        public string JurusanDeskripsi { get; set; } = string.Empty;
    }
}

