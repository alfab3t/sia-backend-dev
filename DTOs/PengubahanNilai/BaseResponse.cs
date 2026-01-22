namespace astratech_apps_backend.DTOs.PengubahanNilai
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

    }

}
