using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Pengumuman
{
    public class GetDataPengumumanRequest 
    {
        [Required(ErrorMessage = "Jenis Filter harus diisi.")]
        
        [StringLength(50)]
        public string Filter { get; set; } = string.Empty;

        public string IdAplikasi { get; set; } = string.Empty;

        public string InputCari { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]

        public int Halaman { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]

        public int Limit { get; set; } = 10;

    }
}
