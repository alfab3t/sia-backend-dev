using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.MeninggalDunia
{
    public class GetAllMeninggalDuniaRequest
    {
        //optional
        public string? UserId { get; set; }
        public string? SearchKeyword { get; set; }
        public string? Status { get; set; }
        public string? Sort { get; set; }
        public string? RoleId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Page number harus lebih dari 0")]
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 100, ErrorMessage = "Page size harus antara 1-100")]
        public int PageSize { get; set; } = 50;
    }
}
