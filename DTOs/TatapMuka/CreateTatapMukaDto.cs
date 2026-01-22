namespace astratech_apps_backend.DTOs.TatapMuka
{
    public class CreateTatapMukaDto
    {
        public string RuangId { get; set; } = string.Empty;
        public int GrupId { get; set; }               
        public string MataKuliahId { get; set; } = string.Empty;       
        public string SectionId { get; set; } = string.Empty;      
        public string DosenId { get; set; } = string.Empty;       
        public string TahunAjaran { get; set; } = string.Empty;
        public string Tanggal { get; set; } = string.Empty;     
        public string WaktuAwal { get; set; } = string.Empty;   
        public string WaktuAkhir { get; set; } = string.Empty; 
        public string Alasan { get; set; } = string.Empty;  
        public string Status { get; set; } = "Draft"; 
        public string CreatedBy { get; set; } = string.Empty;
    }
}
