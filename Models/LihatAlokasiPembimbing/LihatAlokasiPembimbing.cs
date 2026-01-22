namespace astratech_apps_backend.Models.LihatAlokasiPembimbing
{
    public class LihatAlokasiPembimbing
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string Tanggal { get; set; } = string.Empty;
        public string Konsentrasi { get; set; } = string.Empty;
        public string Industri { get; set; } = string.Empty;
        public string NamaKelompok { get; set; } = string.Empty;
        public string AnggotaKelompok { get; set; } = string.Empty;
        public int DosenId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string JudulProposal { get; set; } = string.Empty;
        public string JudulProposalAktual { get; set; } = string.Empty;
        public string TahunAkademik { get; set; } = string.Empty;
        public DateTime TanggalCreated { get; set; }
        public DateTime TanggalApprove { get; set; }
        public int? KonsentrasiId { get; set; }
        public string NamaDosenPembimbing { get; set; } = string.Empty;
    }
}
