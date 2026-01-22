using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class CreatePengumumanRequest
    {
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

        public int StatusBaca { get; set; }

        [Required(ErrorMessage = "harus di isi")]
        public DateTime TanggalMulaiPengumuman { get; set; }

        [Required(ErrorMessage = "harus di isi")]
        public DateTime TanggalSelesaiPengumuman { get; set; }
    }
}
