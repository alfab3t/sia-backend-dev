namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class ProdiByMatkulResponse : BaseResponse
    {
        public List<ProdiByMatkulDto> Data { get; set; } = new();
        public int TotalRecords { get; set; }
    }

    public class ProdiByMatkulDto
    {
        public string CoordinatorUsername { get; set; } = "";
    }
}