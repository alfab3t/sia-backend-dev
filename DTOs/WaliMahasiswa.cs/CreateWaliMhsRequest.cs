using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace astratech_apps_backend.DTOs.WaliMahasiswa
{
    public class CreateWaliMhsRequest
    {
        [Required(ErrorMessage = "Pilih Wali Dosen terlebih dahulu.")]
        public int? DosenId { get; set; }

        [Required(ErrorMessage = "Mohon pilih minimal 1 Mahasiswa.")]
        public List<string> MahasiswaIdsToCreate { get; set; } = new List<string>();

        [Required(ErrorMessage = "Pilih Program Studi.")]
        public int? Prodi { get; set; }

        [Required(ErrorMessage = "Angkatan wajib di isi.")]
        [Range(1900, 2100, ErrorMessage = "Angkatan tidak valid.")]
        public string Angkatan { get; set; } = string.Empty;

    }
}
