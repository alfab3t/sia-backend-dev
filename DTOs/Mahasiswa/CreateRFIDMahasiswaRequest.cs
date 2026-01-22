using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class CreateRfidMahasiswaRequest
    {
        [Required(ErrorMessage = "Masukan ID Mahasiswa Terlebih Dahulu.")]
        public string NomorRFID { get; set; } = string.Empty;
    }
}