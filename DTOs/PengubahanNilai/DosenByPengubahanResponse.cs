namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class DosenByPengubahanResponse : BaseResponse
    {
        public List<DosenByPengubahanDto> Data { get; set; } = new();
        public int TotalRecords { get; set; }
    }

    public class DosenByPengubahanDto
    {
        public string Username { get; set; } = "";
        public string Nama { get; set; } = "";
    }
}