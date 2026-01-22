using astratech_apps_backend.DTOs.Program_Studi;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdiController(IProdiRepository repo) : ControllerBase
    {
        private readonly IProdiRepository _repo = repo;

        [HttpGet("GetAllProdi")]
        [RequiresPermission("program_studi.view")]
        public async Task<IActionResult> GetAllProdi([FromQuery] GetAllProdiRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();


            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(prodi => new ProdiDto
            {
                Id = prodi.Id,
                NamaProdi = prodi.NamaProdi,
                Singkatan = prodi.Singkatan,
                NamaKaprodi = prodi.NamaKaprodi,
                Jenjang = prodi.Jenjang,
                MasaStudi = prodi.MasaStudi,
                Status = prodi.Status
            }).ToList();

            var response = new GetAllProdiResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("DetailProdi/{id}")]
        [RequiresPermission("program_studi.view")]
        public async Task<IActionResult> GetProdiById(short id)
        {
            var dataDto = await _repo.GetByIdAsync(id);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data prodi tidak ditemukan." });
            }
            return Ok(dataDto);
        }

        [HttpPost("CreateProdi")]
        [RequiresPermission("program_studi.create")]
        public async Task<IActionResult> CreateProdi([FromBody] CreateProdiRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var newId = await _repo.CreateAsync(dto, username);

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPut("EditProdi")]
        [RequiresPermission("program_studi.edit")]
        public async Task<IActionResult> EditProdi([FromBody] UpdateProdiRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(state => state.Value?.Errors.Count > 0)
                    .Select(state => new {
                        Field = state.Key,
                        Errors = state.Value?.Errors.Select(error => error.ErrorMessage).ToArray()
                    })
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Data tidak valid",
                    errors
                });
            }

            var username = User.FindFirstValue("namaakun");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
            {
                return BadRequest(new { success = false, message = "Invalid data format" });
            }

            var success = await _repo.UpdateAsync(sanitizedDto, username);
            if (!success)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Data prodi tidak ditemukan atau tidak ada perubahan data."
                });
            }

            return Ok(new
            {
                success = true,
                message = "SUCCESS"
            });
        }

        [HttpPost("SetStatusProdi/{id}")]
        [RequiresPermission("program_studi.edit")]
        public async Task<IActionResult> SetStatusProdi(short id)
        {
            var username = User.FindFirstValue("namaakun");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusAsync(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data prodi tidak ditemukan." });
            }
            return Ok(new { message = "SUCCESS" });
        }

        [HttpGet("CheckNPK/{npk}")]
        [RequiresPermission("program_studi.view")]

        public async Task<IActionResult> CheckNPK(string npk)
        {
            if (string.IsNullOrWhiteSpace(npk))
            {
                return BadRequest(new { message = "NPK tidak boleh kosong." });
            }

            try
            {
                var isValid = await _repo.CheckNPKAsync(npk);

                return Ok(new
                {
                    isValid,
                    message = isValid ? "NPK valid" : "NPK tidak ditemukan dalam database karyawan"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat memvalidasi NPK.", error = ex.Message });
            }
        }

        [HttpGet("GetListKaryawan")]
        [RequiresPermission("program_studi.view")]
        public async Task<IActionResult> GetListKaryawan()
        {
            try
            {
                var karyawanList = await _repo.GetListKaryawanAsync();

                var response = karyawanList.Select(k => new
                {
                    KaryawanId = k.KaryawanId,
                    KaryawanNama = k.KaryawanNama,
                    Nama = k.Nama
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal mengambil data karyawan", error = ex.Message });

            }
        }

        [HttpGet("CheckNomorProdi/{nomorProdi}")]
        [RequiresPermission("program_studi.view")]
        public async Task<IActionResult> CheckNomorProdi(string nomorProdi)
        {
            if (string.IsNullOrWhiteSpace(nomorProdi))
            {
                return BadRequest(new { message = "Nomor Program Studi tidak boleh kosong." });
            }

            try
            {
                var result = await _repo.CheckNomerProdiAsync(nomorProdi);

                if (result != null) 
                {
                    return Ok(new
                    {
                        isValid = false,
                        message = $"Nomor Program Studi sudah digunakan oleh prodi: {result.ProNama}",
                        data = new
                        {
                            exists = true,
                            proNomor = result.ProNomor,
                            proNama = result.ProNama
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        isValid = true,
                        message = "Nomor Program Studi tersedia",
                        data = new
                        {
                            exists = false
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Terjadi kesalahan saat memvalidasi Nomor Program Studi.",
                    error = ex.Message
                });
            }
        }
    }
}
