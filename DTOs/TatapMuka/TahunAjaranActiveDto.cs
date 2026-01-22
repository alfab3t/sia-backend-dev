namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class TahunAjaranActiveDto
    {
        public string TahunAjaran { get; set; } = string.Empty;
        public string GanjilGenap { get; set; } = string.Empty;
        public DateTime? TanggalMulai { get; set; }
    }

    public class ListTahunAkademikActiveDto
    {
        public string TahunAkademik { get; set; } = string.Empty;
    }
}

