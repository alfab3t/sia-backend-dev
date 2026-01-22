namespace DTOs.Perwalian.Pelaksanaan;

public class GetAllPelaksanaanRequestDto
{
    public string? SearchKeyword { get; set; }
    public string? Urut { get; set; } 
    public string Role { get; set; } = ""; 
    public string Username { get; set; } = "";
}
