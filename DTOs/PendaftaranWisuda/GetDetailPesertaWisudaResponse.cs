using System;

namespace astratech_apps_backend.DTOs.PendaftaranWisuda
{
    public class GetDetailPesertaWisudaResponse
    {
        public List<DetailDataWisuda> Data { get; set; } = [];
        public int TotalData { get; set; } = 0;
        public int TotalHalaman { get; set; } = 0;
    }
}
