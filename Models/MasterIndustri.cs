namespace astratech_apps_backend.Models
{
    public class MasterIndustri
    {
        public int Id { get; set; }
        public long RowNumber { get; set; } = 0;
        public string NamaIndustri { get; set; } = string.Empty;
        public string Cabang { get; set; } = string.Empty;
        public string Grup { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
        public string Telepon { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string PIC { get; set; } = string.Empty;
        public string TeleponPIC { get; set; } = string.Empty;
        public string EmailPIC { get; set; } = string.Empty;
        public string NamaPICAtasan { get; set; } = string.Empty;  
        public string TeleponPICAtasan { get; set; } = string.Empty;
        public string EmailPICAtasan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}