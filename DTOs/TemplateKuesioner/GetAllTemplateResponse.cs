namespace astratech_apps_backend.DTOs.TemplateKuesioner
{
    public class GetAllTemplateResponse
    {
        public List<TemplateDto> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}