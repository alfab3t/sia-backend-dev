

using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Program_Studi
{
    public class UpdateProdiRequest
    {

        [Required(ErrorMessage = "Id prodi harus diisi.")]
        public short? Id { get; set; }

        [Required(ErrorMessage = "Jurusan harus dipilih.")]
        public short? JurusanId { get; set; }

        [Required(ErrorMessage = "Nama prodi harus diisi.")]
        [StringLength(150, ErrorMessage = "Nama prodi maksimal 150 karakter.")]
        public string? NamaProdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Singkatan prodi harus diisi.")]
        [StringLength(20, ErrorMessage = "Singkatan maksimal 20 karakter.")]
        public string? Singkatan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal berdiri harus diisi.")]
        public DateTime? TanggalBerdiri { get; set; }

        [Required(ErrorMessage = "Nama Kaprodi harus diisi.")]
        [StringLength(100, ErrorMessage = "Nama Kaprodi maksimal 100 karakter.")]
        public string? NamaKaprodi { get; set; } = string.Empty;

        [Required(ErrorMessage = "NPK Kaprodi harus diisi.")]
        [StringLength(50, ErrorMessage = "NPK Kaprodi maksimal 50 karakter.")]
        public string? NpkKaprodi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama Sekprodi harus diisi.")]
        [StringLength(100)]
        public string NamaSekprodi { get; set; } = string.Empty;

        [Required(ErrorMessage = "NPK Sekprodi harus diisi.")]
        [StringLength(50)]
        public string NpkSekprodi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Singkatan kode matkul harus diisi.")]
        [StringLength(10)]
        public string SingkatanMk { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nomor prodi harus diisi.")]
        [StringLength(20, ErrorMessage = "Nomor prodi maksimal 20 karakter.")]
        public string? NomorProdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Singkatan kode matkul harus diisi.")]
        [StringLength(10, ErrorMessage = "Singkatan kode matkul maksimal 10 karakter.")]
        public string? SingkatanKodeMatkul { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jenjang harus dipilih.")]
        public string? Jenjang { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Masa studi harus diisi.")]
        public int? MasaStudi { get; set; }

        [StringLength(100, ErrorMessage = "Nomor SK Dikti maksimal 100 karakter.")]
        public string? NomorSkDikti { get; set; } = string.Empty;

        public DateTime? TglSkDiktiMulai { get; set; }
        public DateTime? TglSkDiktiSelesai { get; set; }

        [StringLength(100, ErrorMessage = "Nomor akreditasi maksimal 100 karakter.")]
        public string? NomorAkreditasi { get; set; } = string.Empty;

        public DateTime? TglAkreditasiMulai { get; set; }
        public DateTime? TglAkreditasiSelesai { get; set; }

        [Required(ErrorMessage = "Akreditasi harus dipilih.")]
        public string? Akreditasi { get; set; } = string.Empty;

        public string? Deskripsi { get; set; } = string.Empty;
        public string? Visi { get; set; } = string.Empty;
        public string? Misi { get; set; } = string.Empty;
        public string? ProfilLulusan { get; set; } = string.Empty;

    }

}
