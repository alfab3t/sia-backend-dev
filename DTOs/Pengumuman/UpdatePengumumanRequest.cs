using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class UpdatePengumumanRequest
    {
        [Required(ErrorMessage = "ID pengumuman harus diisi.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID pengumuman tidak valid.")]
        public int IdPengumuman { get; set; }

        [Required(ErrorMessage = "Harus di pilih")]
        [StringLength(30)]
        public string IdAplikasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "")]
        [StringLength(30)]
        public string KpdPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "harus di isi")]
        public string SubyekPengumuman { get; set; } = string.Empty;

        [Required(ErrorMessage = "harus di isi")]
        public string IsiPengumuman { get; set; } = string.Empty;

        [Required]
        public int StatusBaca { get; set; } = 0;

        [Required(ErrorMessage = "harus di isi")]
        public DateTime TanggalMulaiPengumuman { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "harus di isi")]
        public DateTime TanggalSelesaiPengumuman { get; set; } = DateTime.Now;
    }
}