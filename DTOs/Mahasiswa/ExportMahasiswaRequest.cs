using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class ExportMahasiswaRequest
    {
        public string? ProgramStudi { get; set; }

        public string? Angkatan { get; set; }

        public string? Tingkat { get; set; }

        public string? JenisMahasiswa { get; set; }

        public string? Status { get; set; }
    }
}
