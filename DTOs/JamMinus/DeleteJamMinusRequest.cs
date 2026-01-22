using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Jam_Minus
{
    public class DeleteJamMinusRequest
    {
        [Required(ErrorMessage = "Id wajib diisi")]
        public short Id { get; set; }
    }
}
