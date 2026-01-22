namespace astratech_apps_backend.DTOs.Mahasiswa
{
    public class RfidMahasiswaDto
    {
        public int RfidMahasiswaId { get; set; }
        public string? IdMahasiswa { get; set; }
        public string? RfidMahasiswa { get; set; }
        public string? RfidMahasiswaStatus { get; set; }
        public string? RfidMahasiswaCreatedBy { get; set; }
        public DateTime RfidMahasiswaCreatedDate { get; set; }
    }
}
