using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.WaliMahasiswa
{
    public class UpdateWaliMhsRequest
    {
        [Required(ErrorMessage = "Nama dosen harus dipilih.")]
        public int DosenId { get; set; }

        [Required(ErrorMessage = "Pilih Program Studi.")]
        public int Prodi { get; set; }

        [Required(ErrorMessage = "Nama mahasiswa harus dipilih minimal satu.")]
        public List<string> MahasiswasIds { get; set; } = new List<string>();
    }
}
