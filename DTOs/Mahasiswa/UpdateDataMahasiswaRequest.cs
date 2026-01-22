using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class UpdateDataMahasiswaRequest
    {
        [Required(ErrorMessage = "ID Mahasiswa wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Mahasiswa maksimal 10 karakter")]
        public string idMahasiswa { get; set; } = string.Empty;

        [Required(ErrorMessage = "No Pendaftaran wajib diisi")]
        [StringLength(10, ErrorMessage = "No Pendaftaran maksimal 10 karakter")]
        public string NoPendaftaran { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Nama maksimal 200 karakter")]
        public string? Nama { get; set; }

        [StringLength(1, ErrorMessage = "Jenis Kelamin maksimal 1 karakter")]
        [RegularExpression("^[LP]?$", ErrorMessage = "Jenis Kelamin harus 'L' atau 'P'")]
        public string? JenisKelamin { get; set; }

        [StringLength(100, ErrorMessage = "Tempat Lahir maksimal 100 karakter")]
        public string? TempatLahir { get; set; }

        public DateTime? TanggalLahir { get; set; }

        [StringLength(500, ErrorMessage = "Alamat maksimal 500 karakter")]
        public string? Alamat { get; set; }

        [StringLength(5, ErrorMessage = "Kode Pos maksimal 5 karakter")]
        [RegularExpression("^([0-9]{5})?$", ErrorMessage = "Kode Pos harus 5 digit angka")]
        public string? KodePos { get; set; }

        [StringLength(15, ErrorMessage = "HP maksimal 15 karakter")]
        [RegularExpression("^[0-9+\\-\\s()]*$", ErrorMessage = "Format HP tidak valid")]
        public string? Hp { get; set; }

        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Nama Ayah maksimal 100 karakter")]
        public string? NamaAyah { get; set; }

        [StringLength(15, ErrorMessage = "HP Ayah maksimal 15 karakter")]
        [RegularExpression("^[0-9+\\-\\s()]*$", ErrorMessage = "Format HP Ayah tidak valid")]
        public string? HpAyah { get; set; }

        [StringLength(50, ErrorMessage = "Status Ayah maksimal 50 karakter")]
        public string? StatusAyah { get; set; }

        [StringLength(500, ErrorMessage = "Alamat Ayah maksimal 500 karakter")]
        public string? AlamatAyah { get; set; }

        [StringLength(5, ErrorMessage = "Kode Pos Ayah maksimal 5 karakter")]
        [RegularExpression("^([0-9]{5})?$", ErrorMessage = "Kode Pos Ayah harus 5 digit angka")]
        public string? KodePosAyah { get; set; }

        [StringLength(100, ErrorMessage = "Nama Ibu maksimal 100 karakter")]
        public string? NamaIbu { get; set; }

        [StringLength(15, ErrorMessage = "HP Ibu maksimal 15 karakter")]
        [RegularExpression("^[0-9+\\-\\s()]*$", ErrorMessage = "Format HP Ibu tidak valid")]
        public string? HpIbu { get; set; }

        [StringLength(50, ErrorMessage = "Status Ibu maksimal 50 karakter")]
        public string? StatusIbu { get; set; }

        [StringLength(500, ErrorMessage = "Alamat Ibu maksimal 500 karakter")]
        public string? AlamatIbu { get; set; }

        [StringLength(5, ErrorMessage = "Kode Pos Ibu maksimal 5 karakter")]
        [RegularExpression("^([0-9]{5})?$", ErrorMessage = "Kode Pos Ibu harus 5 digit angka")]
        public string? KodePosIbu { get; set; }

        [StringLength(100, ErrorMessage = "Nama Wali maksimal 100 karakter")]
        public string? NamaWali { get; set; }

        [StringLength(15, ErrorMessage = "HP Wali maksimal 15 karakter")]
        [RegularExpression("^[0-9+\\-\\s()]*$", ErrorMessage = "Format HP Wali tidak valid")]
        public string? HpWali { get; set; }

        [StringLength(50, ErrorMessage = "Status Wali maksimal 50 karakter")]
        public string? StatusWali { get; set; }

        [StringLength(500, ErrorMessage = "Alamat Wali maksimal 500 karakter")]
        public string? AlamatWali { get; set; }

        [StringLength(5, ErrorMessage = "Kode Pos Wali maksimal 5 karakter")]
        [RegularExpression("^([0-9]{5})?$", ErrorMessage = "Kode Pos Wali harus 5 digit angka")]
        public string? KodePosWali { get; set; }

        [StringLength(20, ErrorMessage = "NISN maksimal 20 karakter")]
        public string? Nisn { get; set; }
    }
}
