using astratech_apps_backend.DTOs.Wisuda;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WisudaController(IWisudaRepository repo) : ControllerBase
    {
        private readonly IWisudaRepository _repo = repo;

        [HttpGet("GetAllTahunLulus")]
        [RequiresPermission("tanda_terima_ijazah.view")]
        public async Task<IActionResult> GetAllTahunLulus([FromQuery] GetAllWisudaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDto = list.Select(m => new WisudaDto
            {
                RowNumber = m.RowNumber,
                TahunLulus = m.TahunLulus,
                JumlahLulusan = m.JumlahLulusan,
                JumlahTerima = m.JumlahTerima
            }).ToList();

            var response = new GetAllWisudaResponse
            {
                Data = dataDto,
                TotalData = totalData,
                TotalHalaman = sanitizedDto.PageSize <= 0
                    ? 0
                    : ((totalData - 1) / sanitizedDto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("GetAllTandaTerima")]
        [RequiresPermission("tanda_terima_ijazah.view")]
        public async Task<IActionResult> GetAllTandaTerima([FromQuery] GetAllTandaTerimaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var (list, totalData) = await _repo.GetAllTandaTerimaAsync(sanitizedDto);

            var dataDto = list.Select(x => new WisudaDto
            {
                RowNumber = x.RowNumber,
                MahasiswaId = x.MahasiswaId,
                MahasiswaNama = x.MahasiswaNama,
                ProdiSingkatan = x.ProdiSingkatan,
                DetailStatus = x.DetailStatus,
                Status = x.Status,
                KonsentrasiId = x.KonsentrasiId,
                Foto = x.Foto,
                Count = x.Count
            }).ToList();

            var response = new GetAllTandaTerimaResponse
            {
                Data = dataDto,
                TotalData = totalData,
                TotalHalaman = sanitizedDto.PageSize <= 0
                    ? 0
                    : ((totalData - 1) / sanitizedDto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("GetListLulusan")]
        [RequiresPermission("tanda_terima_ijazah.view")]
        public async Task<IActionResult> GetListLulusan([FromQuery] int tahunLulus)
        {
            if (tahunLulus <= 0)
                return BadRequest("TahunLulus tidak valid.");

            var list = await _repo.GetListLulusanAsync(tahunLulus);

            var dataDto = list.Select(x => new WisudaDto
            {
                MahasiswaId = x.MahasiswaId,
                MahasiswaNama = x.MahasiswaNama
            }).ToList();

            return Ok(new
            {
                Data = dataDto,
                TotalData = dataDto.Count
            });
        }

        [HttpGet("GetListKonsentrasi")]
        [RequiresPermission("tanda_terima_ijazah.view")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            var username = User.FindFirstValue("namaakun");
            var roleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId))
                return Unauthorized();

            var list = await _repo.GetListKonsentrasiAsync(username, roleId);

            var dataDto = list.Select(x => new WisudaDto
            {
                KonsentrasiId = x.KonsentrasiId,
                KonsentrasiNama = x.KonsentrasiNama ?? "",
                KonsentrasiSingkatan = x.KonsentrasiSingkatan ?? "",
                KonsentrasiStatus = x.KonsentrasiStatus ?? ""
            }).ToList();

            return Ok(new
            {
                Data = dataDto,
                TotalData = dataDto.Count
            });
        }

        [HttpPost("CreateTandaTerimaIjazah")]
        [RequiresPermission("tanda_terima_ijazah.create")]
        public async Task<IActionResult> CreateTandaTerimaIjazah([FromBody] CreateWisudaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return NotFound();

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.CreateTandaTerimaIjazahAsync(
                username, dto
            );

            if (!success)
            {
                return NotFound(new
                {
                    message = "Mahasiswa dengan mahasiswa_id tersebut tidak ditemukan."
                });
            }

            return Ok(new
            {
                message = "SUCCESS"
            });
        }
    }
    
}
