using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.Wisuda
{
    public class GetAllWisudaRequest
    {
        [Required(ErrorMessage = "Nomor halaman harus diisi.")]
        public int PageNumber { get; set; } = 1;

        [Required(ErrorMessage = "Ukuran data per halaman harus diisi.")]
        public int PageSize { get; set; } = 10;

    }
}
