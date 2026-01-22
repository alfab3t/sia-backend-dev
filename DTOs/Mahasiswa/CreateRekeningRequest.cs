using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class CreateRekeningRequest
    {
        [Required(ErrorMessage = "Masukan ID Mahasiswa Terlebih Dahulu.")]
        public string IdMahasiswa { get; set; } = "";
        public string AtasNama { get; set; } = "";
        public string NoRekening { get; set; } = "";
        public string NamaBank { get; set; } = "";
    }
}
