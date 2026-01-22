using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Bantuan.FAQ
{
    public class UpdateFAQRequest
    {
        [Required(ErrorMessage = "ID FAQ harus diisi.")]
        public int FaqId { get; set; }

        [Required(ErrorMessage = "ID Kategori harus diisi.")]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Pertanyaan harus diisi.")]
        [StringLength(500)]
        public string Pertanyaan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jawaban harus diisi.")]
        public string Jawaban { get; set; } = string.Empty;

        public int Urutan { get; set; }

        [Required(ErrorMessage = "Status harus diisi.")]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public List<string> AksesRoleIds { get; set; } = [];
    }
}