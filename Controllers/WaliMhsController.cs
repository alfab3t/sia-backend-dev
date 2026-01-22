using astratech_apps_backend.DTOs.WaliMahasiswa;
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
    public class WaliMhsController(IWaliMhsRepository repo) : ControllerBase
    {
        public const string NamaAkun = "namaakun";
        public const string IdRole = "idrole";
        public const string Success = "SUCCESS";
        public const string NotFoundWaliMhs = "Data wali mahasiswa tidak ditemukan.";
        
        private readonly IWaliMhsRepository _repo = repo;

        [HttpGet("GetAllWaliMhs")]
        [RequiresPermission("wali_mahasiswa.view")]
        public async Task<IActionResult> GetAllWaliMhs([FromQuery] GetAllWaliMhsRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var username = User.FindFirst(NamaAkun)?.Value ?? "";
           var role = User.FindFirst(IdRole)?.Value ?? "";

            role = role.ToUpper();

            dto.Username = username;
            dto.Role = role;

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var response = new GetAllWaliMhsResponse
            {
                Data = list.Select(m => new WaliMahasiswaDto
                {
                    Id = m.DosenId,
                    NamaDosen = m.NamaDosen,
                    NamaProdi = m.NamaProdi,
                    prodiId= m.prodiId,
                    Angkatan = m.Angkatan,
                    Status = m.Status
                }).ToList(),
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / dto.PageSize) + 1
            };

            return Ok(response);
        }

        [HttpGet("DetailWaliMhs/{id}/{angkatan}/{prodiId}/{status}")]
        [RequiresPermission("wali_mahasiswa.view")]
        public async Task<IActionResult> GetDetail(int id,string angkatan , int prodiId , string status)
        {
            var data = await _repo.GetDetailAsync(id, angkatan , prodiId , status);

            if (data == null)
                return NotFound(new { message = NotFoundWaliMhs });

            var dto = new WaliMahasiswaDto
            {
                Id = data.Id,
                NamaDosen = data.NamaDosen,
                Angkatan = data.Angkatan,
                ProgramStudi = data.NamaProdi,
                Status = data.Status,
                DaftarMahasiswa = data.DaftarMahasiswa
            };

            return Ok(dto);
        }

        [HttpPost("CreateWaliMhs")]
        [RequiresPermission("wali_mahasiswa.create")]
        public async Task<IActionResult> CreateWaliMhs([FromBody] CreateWaliMhsRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Sesi pengguna tidak valid." });

            int count = await _repo.CreateAsync(dto, username);

            if (count == 0)
                return BadRequest(new { message = "Tidak ada data wali mahasiswa yang berhasil disimpan." });

            return Ok(new
            {
                message = $"SUCCESS. {count} wali mahasiswa berhasil disimpan.",
                totalInserted = count
            });
        }

        [HttpGet("ListMahasiswaAvailable")] 
        [RequiresPermission("wali_mahasiswa.view")]
        public async Task<IActionResult> GetListMahasiswaAvailable([FromQuery] string angkatan, [FromQuery] int prodiId) 
        {        
            if (string.IsNullOrEmpty(angkatan) || angkatan.Length != 4 || prodiId <= 0)
            {
                return BadRequest(new { message = "Filter Angkatan harus 4 digit dan Program Studi harus valid." });
            }

            var mahasiswaList = await _repo.GetMahasiswaAvailableAsync(angkatan, prodiId); 
                if (mahasiswaList == null || !mahasiswaList.Any())
                {
                    return NotFound(new { message = $"Tidak ada mahasiswa yang tersedia untuk Angkatan {angkatan} dan Prodi ID {prodiId}." });
                }
                   return Ok(mahasiswaList);
        }

        [HttpGet("ListAngkatan")]
        [RequiresPermission("wali_mahasiswa.view")]
        public async Task<IActionResult> GetListAngkatan()
        {
            var data = await _repo.GetListAngkatanAsync();
            return Ok(data);
        }

        [HttpGet("ListProdi")]
         [RequiresPermission("wali_mahasiswa.view")]
        public async Task<IActionResult> GetListProdi()
        {
            var data = await _repo.GetListProdiAsync();
            return Ok(data);
        }

        [HttpGet("ListDosen")]
        [RequiresPermission("wali_mahasiswa.view")]
         public async Task<IActionResult> GetListDosen()
        {
            
            var data = await _repo.GetListDosenAsync();
            return Ok(data);
        }
 
        [HttpPost("SetStatusWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> SetStatusWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status
        )
        {
            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.SetStatusAsync(
                id,
                angkatan,
                prodiId,
                status,
                username
            );

            if (!success)
                return NotFound(new { message = NotFoundWaliMhs });
                return Ok(new { message = Success });
        }

        [HttpPut("UpdateWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> UpdateWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status,
            [FromBody] UpdateWaliMhsRequest dto
        )
        {
            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.UpdateAsync(
                id,
                angkatan,
                prodiId,
                status,
                dto,
                username
            );

            if (!success)
                return NotFound(new { message = NotFoundWaliMhs });
            return Ok(new { message = Success });
        }


   
        [HttpDelete("DeleteWaliMhs/{id}")]
        public async Task<IActionResult> DeleteWaliMhs( int id,
        [FromQuery] string angkatan,
        [FromQuery] int prodiId,
        [FromQuery] string status)
        {
            var success = await _repo.DeleteAsync(
                id,
                angkatan,
                prodiId,
                status
            );
            if (!success) return NotFound(new { message = "Data tidak ditemukan." });
            return Ok(new { message = Success });
        }

        [HttpPut("SendWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> SendWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status
        )
        {
            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var detail = await _repo.GetDetailAsync(
                id,
                angkatan,
                prodiId,
                status
            );

            if (detail == null)
                return NotFound(new { message = "Data wali mahasiswa tidak ditemukan." });

            if (detail.Status != "Draft" && detail.Status != "Revisi")
            {
                return BadRequest(new
                {
                    message = $"Data tidak dapat dikirim. Status saat ini '{detail.Status}'."
                });
            }

            var success = await _repo.SendApprovalAsync(
                detail.DosenId,
                detail.Angkatan,
                detail.Prodi,
                detail.Status,
                username
            );

            if (!success)
                return StatusCode(500, new { message = "Gagal mengirim data." });

            return Ok(new
            {
                message = Success,
                status = "Belum Disetujui"
            });
        }


        [HttpPut("ApproveWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> ApproveWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status
        )
        {
            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            
            var detail = await _repo.GetDetailAsync(
                id,
                angkatan,
                prodiId,
                status
            );

            if (detail == null)
                return NotFound(new { message = NotFoundWaliMhs });
            
            if (detail.Status != "Belum Disetujui")
            {
                return BadRequest(new
                {
                    message = $"Data tidak dapat disetujui. Status saat ini '{detail.Status}'."
                });
            }
            var success = await _repo.ApproveAsync(
                detail.DosenId,
                detail.Angkatan,
                detail.Prodi,
                detail.Status,
                username
            );
            if (!success)
                return NotFound(new { message = "Data tidak ditemukan atau gagal disetujui." });
            return Ok(new
            {
                message = Success,
                status = "Aktif"
            });
        }


       [HttpPut("RejectWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> RejectWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status,
            [FromBody] RejectRequest dto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var detail = await _repo.GetDetailAsync(
                id,
                angkatan,
                prodiId,
                status
            );

            if (detail == null)
                return NotFound(new { message = "Data wali mahasiswa tidak ditemukan." });

            var success = await _repo.RejectAsync(
                detail.DosenId,
                detail.Angkatan,
                detail.Prodi,
                detail.Status,
                dto.Reason,
                username
            );

            if (!success)
                return NotFound(new { message = "Data tidak ditemukan atau gagal ditolak." });

            return Ok(new
            {
                message = Success,
                status = "Revisi"
            });
        }


       [HttpPut("DeactivateWaliMhs/{id}")]
        [RequiresPermission("wali_mahasiswa.edit")]
        public async Task<IActionResult> DeactivateWaliMhs(
            int id,
            [FromQuery] string angkatan,
            [FromQuery] int prodiId,
            [FromQuery] string status
        )
        {
            var username = User.FindFirst(NamaAkun)?.Value ?? "";
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.SetStatusAsync(
                id,
                angkatan,
                prodiId,
                status,
                username
            );

            if (!success)
                return NotFound(new { message = "Data tidak ditemukan atau gagal diubah statusnya." });

            return Ok(new { message = Success, status = "Tidak Aktif" });
        }
   }
}

    
