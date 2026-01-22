using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MasterIndustri
{
    public class UpdateMasterIndustriRequest
    {
        [Required(ErrorMessage = "ID industri harus diisi.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID industri tidak valid.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama industri harus diisi.")]
        [StringLength(100)]
        public string NamaIndustri { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Cabang { get; set; }

        [Required(ErrorMessage = "Grup industri harus diisi.")]
        [StringLength(50)]
        public string Grup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat harus diisi.")]
        [StringLength(200)]
        public string Alamat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telepon harus diisi.")]
        [StringLength(50)]
        public string Telepon { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Fax { get; set; }

        [Required(ErrorMessage = "Nama PIC harus diisi.")]
        [StringLength(100)]
        public string PIC { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TeleponPIC { get; set; }

        [RegularExpression(@"(^$)|(^\s*$)|(^\s*[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}(\s*,\s*[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,})*\s*$)",
        ErrorMessage = "Format email PIC tidak valid.")]
        [StringLength(255)]
        public string? EmailPIC { get; set; }
        
        [StringLength(100)]
        public string? NamaPICAtasan { get; set; }

        [StringLength(50)]
        public string? TeleponPICAtasan { get; set; }

        [RegularExpression(@"(^$)|(^\s*$)|(^\s*[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}(\s*,\s*[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,})*\s*$)",
         ErrorMessage = "Format email PIC atasan tidak valid.")]
        [StringLength(255)]
        public string? EmailPICAtasan { get; set; }

    }
}
