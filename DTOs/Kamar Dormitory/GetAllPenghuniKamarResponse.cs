namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class GetAllPenghuniKamarResponse
    {
        public List<PenghuniKamarDto> Data { get; set; } = new();
        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
        public int JumlahAktif { get; set; }
    }
}