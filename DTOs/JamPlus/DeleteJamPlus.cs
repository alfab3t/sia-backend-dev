using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.JamPlus
{
    public class DeleteJamPlus
    {
        [Required(ErrorMessage = "Id wajib diisi")]
        public int Id { get; set; } = 0;

    }
}
                                