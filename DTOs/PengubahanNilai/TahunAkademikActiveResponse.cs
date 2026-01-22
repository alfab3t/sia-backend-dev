namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class TahunAkademikActiveResponse : BaseResponse
    {
        public List<TahunAkademikActiveDto> Data { get; set; } = new();
    }

    public class TahunAkademikActiveDto
    {
        public string TahunAjaran { get; set; } = "";
        public string Semester { get; set; } = "";
        public string TanggalMulai { get; set; } = "";
    }
}