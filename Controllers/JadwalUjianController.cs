using astratech_apps_backend.DTOs.JadwalUjian;
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

    public class JadwalUjianController(IJadwalUjianRepository repo) : ControllerBase
    {
        private readonly IJadwalUjianRepository _repo = repo;

        
        private const string CLAIM_TYPE_NAMA_AKUN = "namaakun";
        private const string ERROR_SERVER = "Terjadi kesalahan pada server.";
        private const string SUCCESS_MESSAGE = "SUCCESS";
        private const string ERROR_MESSAGE = "ERROR";

        

        [HttpPost("CreateJadwalUjian")]
        [RequiresPermission("jadwal_ujian.create")]
        
        public async Task<IActionResult> CreateJadwalUjian([FromBody] CreateJadwalUjianDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(CLAIM_TYPE_NAMA_AKUN);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var newId = await _repo.CreateAsyncUjian(dto, username);

            return Ok(new { message = SUCCESS_MESSAGE, id = newId });
        }


        [HttpPost("CreateJadwalUjianDetail")]
        [RequiresPermission("jadwal_ujian.create")]
        public async Task<IActionResult> CreateJadwalUjianDetail([FromBody] CreateJadwalUjianDetailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(CLAIM_TYPE_NAMA_AKUN);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var newId = await _repo.CreateAsyncDetail(dto, username);

            return Ok(new { message = SUCCESS_MESSAGE, id = newId });
        }



   


        [HttpGet("GetAllJadwalUjian")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetJadwalUjian([FromQuery] GetAllJadwalUjianRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userRole = User.FindFirst("idrole")?.Value ?? "";
            dto.UserRole = userRole;

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

                var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

                var dataDTO = list.Select(m => new JadwalUjianDto
                {
                    IdJadwalUjian = m.IdJadwalUjian,
                    Prodi = m.Prodi,
                    TahunAkademik = m.TahunAkademik,
                    Semester = m.Semester,
                    Kelas = m.Kelas,
                    JenisUjian = m.JenisUjian,
                    Status = m.Status
                }).ToList();

             var response = new GetAllJadwalUjianResponse
                {
                    Data = dataDTO,
                    TotalData = totalData,
                };


                return Ok(response);
        }

        [HttpGet("DetailJadwalUjian/{id}")]
        [RequiresPermission("jadwal_ujian.view")]       
         public async Task<IActionResult> GetJadwalUjianById(int id)
        {
            try
            {
                var data = await _repo.GetDetailAsync(id);
                if (data == null)
                {
                    return NotFound(new { Message = "Jadwal ujian tidak ditemukan." });
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ERROR_SERVER, Error = ex.Message });
            }
        }

        [HttpGet("DetailJadwalUjianDetail/{id}")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetJadwalUjianDetail(int id)
        {
            try
            {
                var data = await _repo.GetDataAsyncDetail(id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ERROR_SERVER, Error = ex.Message });
            }
        }

        [HttpGet("DataMatkulJadwalUjian")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetDataMatkulDetail([FromQuery] GetDataMatkul dto)
            {
                try
                {
                    var data = await _repo.GetDataMatkulAllAsync(dto); 
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = ERROR_SERVER, Error = ex.Message });
                }
            }





        [HttpPut("EditJadwalUjian")]
        [RequiresPermission("jadwal_ujian.edit")]
        public async Task<IActionResult> UpdateJadwalUjian([FromBody] UpdateJadwalUjianRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(CLAIM_TYPE_NAMA_AKUN);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(dto, username);
            if (!success)
            {
                return NotFound(new { message = "Data Jadwal Ujian tidak ditemukan." });
            }
            return Ok(new { message = SUCCESS_MESSAGE });
        }

        [HttpPut("FinalisasiJadwalUjian")]
        [RequiresPermission("jadwal_ujian.edit")]
        public async Task<IActionResult> FinalizeJadwalUjian([FromBody] string[] ids)
        {
            try
            {
                var modifiedBy = User.Identity?.Name;
                if (string.IsNullOrEmpty(modifiedBy))
                {
                    return Unauthorized(new { Message = "Tidak dapat mengidentifikasi pengguna dari token." });
                }

                var success = await _repo.FinalizeAsync(ids, modifiedBy);
                if (!success)
                {
                    
                    return BadRequest(new { Message = "Gagal memfinalisasi jadwal. Pastikan ID valid." });
                }

                return Ok(new { Message = "Jadwal ujian berhasil difinalisasi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ERROR_SERVER, Error = ex.Message });
            }
        }


    


        [HttpGet("check-bentrok")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> CheckBentrok([FromQuery] DateTime tanggal, [FromQuery] TimeSpan mulai, [FromQuery] TimeSpan selesai)
        {
            try
            {
                
                if (selesai <= mulai)
                {
                    return BadRequest(new { Message = "Waktu selesai harus setelah waktu mulai." });
                }

                var bentrokData = await _repo.CheckJadwalBentrok(tanggal, mulai, selesai);
                return Ok(bentrokData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ERROR_SERVER, Error = ex.Message });
            }
        }



        [HttpGet("check-jadwal-ujian")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> CheckJadwalUjian([FromQuery] string grupId, [FromQuery] string tahunAkademik, [FromQuery] string semester,[FromQuery] string jenis)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(grupId))
                {
                    return BadRequest(new { Message = "Grup ID harus diisi." });
                }

                if (string.IsNullOrWhiteSpace(tahunAkademik))
                {
                    return BadRequest(new { Message = "Tahun akademik harus diisi." });
                }

                if (string.IsNullOrWhiteSpace(semester))
                {
                    return BadRequest(new { Message = "Semester harus diisi." });
                }

                if (string.IsNullOrWhiteSpace(jenis))
                {
                    return BadRequest(new { Message = "Jenis ujian harus diisi." });
                }

                var result = await _repo.CheckJadwalUjian(grupId, tahunAkademik, semester, jenis);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Message = ERROR_SERVER, 
                    Error = ex.Message 
                });
            }
        }






        [HttpPut("DeleteJadwalUjian")]
        [RequiresPermission("jadwal_ujian.create")]
        public async Task<IActionResult> DeleteJadwalUjianAsync([FromBody] DeleteJadwalUjianDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modifiedBy = User.Identity?.Name 
                            ?? User.FindFirstValue(CLAIM_TYPE_NAMA_AKUN);

            if (string.IsNullOrEmpty(modifiedBy))
            {
                return Unauthorized(new
                {
                    error = true,
                    message = "Tidak dapat mengidentifikasi pengguna."
                });
            }

            var success = await _repo.DeleteJadwalUjianAsync(dto.IdJadwal, modifiedBy);

            if (!success)
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Gagal menghapus jadwal ujian."
                });
            }

            return Ok(new
            {
                error = false,
                message = "Jadwal ujian berhasil dihapus.",
                deletedId = dto.IdJadwal
            });
        }

        [HttpGet("GetAllDosen")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetAllListDosen()
        {
            try
            {
                var data = await _repo.GetAllListDosenAsync();

                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ERROR_MESSAGE,
                    error = ex.Message
                });
            }
        }
        [HttpGet("GetListGrup")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetListGrup(string KonsentrasiId, string KelasId, string GrupTahunAjaran, string GrupSemester)
        {
            try
            {
                var data = await _repo.GetListGrupAsync(KonsentrasiId, KelasId, GrupTahunAjaran, GrupSemester);

                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ERROR_MESSAGE,
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetAllProdi")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetAllListProdi()
        {
             try
            {
                var   data = await _repo.GetAllListProdiAsync();
                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ERROR_MESSAGE,
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetAllRuangan")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetAllListRuangan()
        {
            try
            {
                var data = await _repo.GetAllListRuanganAsync();

                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ERROR_MESSAGE,
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetSection")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetSection(string KonsentrasiId, string KelasId, string GrupId, string TahunAjaran, string Semester)
        {
            var data = await _repo.GetSectionByGrupAsync(KonsentrasiId, KelasId, GrupId, TahunAjaran, Semester);

            return Ok(new
            {
                message = SUCCESS_MESSAGE,
                data
            });
        }

        [HttpGet("GetListKelas")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetListKelas([FromQuery] string KonsentrasiId, [FromQuery] string TahunAjaran, [FromQuery] string jenis)
        {
            try
            {
                var  data = await _repo.GetListKelasAsync(KonsentrasiId, TahunAjaran, jenis);

                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "ERROR",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetTahunSemesterActive")]
        [RequiresPermission("jadwal_ujian.view")]
        public async Task<IActionResult> GetTahunSemesterActive()
        {
            try
            {
                var data = await _repo.GetTahunSemesterActiveAsync();

                return Ok(new
                {
                    message = SUCCESS_MESSAGE,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "ERROR",
                    error = ex.Message
                });
            }
        }



    }
}