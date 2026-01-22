using astratech_apps_backend.DTOs.RekapitulsiIndustri;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RekapitulasiIndustriController : ControllerBase
    {
        private readonly IRekapitulasiIndustriRepository _repo;

        public RekapitulasiIndustriController(IRekapitulasiIndustriRepository repo){ _repo = repo;}

        [HttpGet("GetListTahunAjaran")]
        public async Task<IActionResult> GetListTahunAjaran()
        {
            var result = await _repo.GetListTahunAjaranAsync();
            return Ok(result);
        }

        [HttpGet("GetActiveTahunAjaran")]
        public async Task<IActionResult> GetActiveTahunAjaran()
        {
            var result = await _repo.GetActiveTahunAjaranAsync();
            return Ok(new { tahunAjaran = result });
        }

        [HttpGet("GetAllRekapitulasi")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRekapitulasiIndustriRequest request)
        {
            var result = await _repo.GetAllAsync(request);
            return Ok(result);
        }

        [HttpGet("GetAllPivot")]
        public async Task<IActionResult> GetPivot([FromQuery] GetAllRekapitulasiIndustriPivotRequest request)
        {
            var sanitized = SanitizerHelper.EncodeObject(request);
            var result = await _repo.GetPivotAsync(sanitized);
            return Ok(result);
        }

       [HttpGet("DetailRekapitulasi")]
       public async Task<IActionResult> GetDetail([FromQuery] GetDetailRekapitulasiIndustriRequest request)
       {
            var result = await _repo.GetDetailMahasiswaAsync(request.NamaIndustri, request.TahunAjaran);
            
            return Ok(new GetDetailRekapitulasiIndustriResponse
            {
                Data = result.ToList(),
                TotalData = result.Count()
            });
       }
    }
}
