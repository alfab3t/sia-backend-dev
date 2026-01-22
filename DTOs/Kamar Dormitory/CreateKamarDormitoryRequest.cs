using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Kamar_Dormitory
{
    public class CreateKamarDormitoryRequest
    {
        public int Lantai { get; set; }
        public string KodeKamar { get; set; }
        public string JenisKamar { get; set; }
        public string? Foto { get; set; }
        public string? Keterangan { get; set; }
        public List<AtributKamarRequest> Atribut { get; set; }
    }

}
