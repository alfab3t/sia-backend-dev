namespace astratech_apps_backend.Models
{
    public class Remedial
    {
        public long RowNumber { get; set; }
        public int NmaId { get; set; }              
        public string MhsId { get; set; } = string.Empty;
        public string MhsNama { get; set; } = string.Empty;
        public double NilaiAkhir { get; set; }
        public string AngkaMutu { get; set; } = string.Empty;
        public double? NilaiRemedial { get; set; }
        public string? AngkaMutuRemedial { get; set; }
        public bool CanEdit { get; set; }


        public int? MataKuliahId { get; set; }       
        public int? SectionId { get; set; }            
        public string KelasId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
    }
}