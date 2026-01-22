using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Bantuan.FAQ
{
    public class CreateFAQRequest
    {
        [Required(ErrorMessage = "Kategori ID harus diisi.")]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Pertanyaan harus diisi.")]
        [StringLength(500)]
        public string Pertanyaan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jawaban harus diisi.")]
        public string Jawaban { get; set; } = string.Empty;

        public int Urutan { get; set; } = 0;

        public List<string> AksesRoleIds { get; set; } = [];
    }
}