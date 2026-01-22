namespace astratech_apps_backend.Models
{
    public class Wisuda
    {
        public long RowNumber { get; set; }
        public int TahunLulus { get; set; }
        public int JumlahLulusan { get; set; }
        public int JumlahTerima { get; set; }

        public string? MahasiswaId { get; set; }         
        public string? MahasiswaNama { get; set; }      
        public string? ProdiSingkatan { get; set; }  
        public string? DetailStatus { get; set; }     
        public string? Status { get; set; }   
        public short KonsentrasiId { get; set; }  
        public string?  Foto { get; set; }       
        public int Count { get; set; }         
        public string KonsentrasiNama { get; set; } = string.Empty;      
        public string KonsentrasiSingkatan { get; set; } = string.Empty; 
        public string KonsentrasiStatus { get; set; } = string.Empty;     
         public string CreatedBy { get; set; } = default!;


    }
}
