using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JadwalUjian
{
    public class CreateJadwalUjianDetailDto
    {
        [Required(ErrorMessage = "ID Jadwal Ujian harus diisi.")]
        public int IdJadwalUjian { get; set; }

        [Required(ErrorMessage = "Mata Kuliah harus diisi.")]
        public int IdMataKuliah { get; set; }

        [Required(ErrorMessage = "Section harus diisi.")]
        public int IdSection { get; set; }

        public int? IdRuangan { get; set; } 

        [Required(ErrorMessage = "Pengawas 1 harus diisi.")]
        public int IdDosen1 { get; set; }

        public int? IdDosen2 { get; set; }

        [Required(ErrorMessage = "Tanggal Ujian harus diisi.")]
        public DateTime Tanggal { get; set; }

        [Required(ErrorMessage = "Waktu Mulai harus diisi.")]
        public TimeSpan WaktuMulai { get; set; }

        [Required(ErrorMessage = "Waktu Selesai harus diisi.")]
        public TimeSpan WaktuSelesai { get; set; }
    }
}