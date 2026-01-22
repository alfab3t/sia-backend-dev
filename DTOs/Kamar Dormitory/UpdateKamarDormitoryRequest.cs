using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class UpdateKamarDormitoryRequest
    {
        public int Id { get; set; } 
        public int Lantai { get; set; }
        public string KodeKamar { get; set; }
        public string JenisKamar { get; set; }
        public string? Foto { get; set; }
        public string? Keterangan { get; set; }
        public List<AtributKamarRequest> Atribut { get; set; } = new();

    }
}
