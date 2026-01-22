using astratech_apps_backend.DTOs.Pengumuman;
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
    public class PengumumanController(IPengumumanRepository repo) : ControllerBase
    {
        private readonly IPengumumanRepository _repo = repo;
        private const string usernameParam = "namaakun";

        [HttpPost("CreatePengumuman")]
        [RequiresPermission("pengumuman.create")]
        public async Task<IActionResult> CreatePengumuman([FromBody] CreatePengumumanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(usernameParam);
            var roleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId))
            {
                return Unauthorized();
            }

            var newId = await _repo.CreatePengumumanRepo(dto, roleId, username);

            return Ok(new { id = newId });
        }

        [HttpGet("GetAllPengumuman")]
        [RequiresPermission("daftar_pengumuman.view")]
        public async Task<IActionResult> GetAllPengumuman([FromQuery] GetDataPengumumanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (list, totalData) = await _repo.GetAllDataPengumumanRepo(dto);

            var dataDTO = list.Select(m => new PengumumanDto
            {
                IdPengumuman = m.IdPengumuman,
                NamaAplikasi = m.IdAplikasi,
                TanggalPengumumanF = m.TanggalPengumumanF,
                SubyekPengumuman = m.SubyekPengumuman,
                StatusPengumuman = m.StatusPengumuman,
                TanggalPengumuman = m.TanggalMulaiPengumuman
            }).ToList();

            var response = new GetAllPengumumanResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.Limit) + 1
            };

            return Ok(response);
        }

        [HttpPut("EditPengumuman")]
        [RequiresPermission("pengumuman.edit")]
        public async Task<IActionResult> UpdatePengumuman([FromBody] UpdatePengumumanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(usernameParam);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.UpdatePegumumanRepo(dto, username);
            if (!success)
            {
                return NotFound(new { message = "Data pengumuman tidak ditemukan." });
            }
            return Ok();
        }

        [HttpPost("DeletePengumuman/{id}")]
        [RequiresPermission("pengumuman.delete")]
        public async Task<IActionResult> DeletePengumuman(int id)
        {
            var username = User.FindFirstValue(usernameParam);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.SetStatusHapusPengumuman(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data Pengumuman tidak ditemukan." });
            }
            return Ok();
        }

        [HttpGet("GetDataPengumuman")]
        [RequiresPermission("pengumuman.view")]
        public async Task<IActionResult> GetDataPengumuman([FromQuery] GetDataPengumumanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(usernameParam);
            var appId = User.FindFirstValue("idapp");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(appId))
            {
                return Unauthorized();
            }

            var (list, totalData) = await _repo.GetDataPengumumanRepo(dto, username, appId);

            var dataDTO = list.Select(m => new PengumumanDto
            {
                IdPengumuman = m.IdPengumuman,
                NamaAplikasi = m.IdAplikasi,
                TanggalPengumumanF = m.TanggalPengumumanF,
                SubyekPengumuman = m.SubyekPengumuman,
                StatusPengumuman = m.StatusPengumuman,
                TanggalPengumuman = m.TanggalMulaiPengumuman
            }).ToList();

            var response = new GetAllPengumumanResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.Limit) + 1
            };

            return Ok(response);
        }
        [HttpGet("DetailPengumuman/{id}")]
        [RequiresPermission("pengumuman.view")]
        public async Task<IActionResult> GetDetailPengumuman(int id)
        {
            var detailPengumuman = await _repo.GetDetailPengumumanRepo(id);
            if (detailPengumuman == null)
            {
                return NotFound(new { message = "Data pengumuman tidak ditemukan." });
            }

            return Ok(detailPengumuman);
        }

        [HttpPost("HidePengumuman/{id}")]
        [RequiresPermission("pengumuman.edit")]
        public async Task<IActionResult> HidePengumuman(int id)
        {
            var username = User.FindFirstValue(usernameParam);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.HidePengumumanRepo(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data Pengumuman tidak ditemukan." });
            }
            return Ok();
        }

        [HttpPost("ShowPengumuman/{id}")]
        [RequiresPermission("pengumuman.edit")]
        public async Task<IActionResult> ShowPengumuman(int id)
        {
            var username = User.FindFirstValue(usernameParam);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _repo.ShowPengumumanRepo(id, username);
            if (!success)
            {
                return NotFound(new { message = "Data Pengumuman tidak ditemukan." });
            }
            return Ok();
        }

        [HttpGet("GetListRole")]
        [RequiresPermission("pengumuman.create")]
        public async Task<IActionResult> GetListRole()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(usernameParam);
            var roleId = User.FindFirstValue("idrole");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId))
            {
                return Unauthorized();
            }

            var role = await _repo.GetListRoleRepo(username, roleId);

            return Ok(role);
        }

        [HttpGet("GetListAplikasi")]
        [RequiresPermission("pengumuman.create")]
        public async Task<IActionResult> GetListAplikasi()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(usernameParam);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var list = await _repo.GetListAplikasiRepo();

            var dataDTO = list.Select(m => new AplikasiDto
            {
                IdAplikasi = m.AppId,
                NamaAplikasi = m.NamaAplikasi
            }).ToList();

            var response = new GetListAplikasiResponse
            {
                Data = dataDTO
            };

            return Ok(response);
        }

        [HttpPost("GetListRoleByAplikasi")]
        [RequiresPermission("pengumuman.create")]
        public async Task<IActionResult> GetListRoleByAplikasi(string IdAplikasi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var list = await _repo.GetListRoleByAplikasiRepo(IdAplikasi);

            var dataDTO = list.Select(m => new AplikasiDto
            {
                IdRole = m.RoleId,
                NamaAplikasi = m.NamaAplikasi,
                IdAplikasi = m.AppId
            }).ToList();

            var response = new GetListAplikasiResponse
            {
                Data = dataDTO
            };

            return Ok(response);
        }
    }
}
