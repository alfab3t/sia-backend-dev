using astratech_apps_backend.DTOs.HasilStudiKriteria;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Models.HasilStudiKriteria;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KriteriaController : ControllerBase
    {
        private readonly IKriteriaRepository _repo;

        public KriteriaController(IKriteriaRepository kriteriaRepository)
        {
            _repo = kriteriaRepository;
        }

        private (string username, string userRole, string displayName) GetUserInfo()
        {
            var username = User.FindFirstValue("namaakun") ?? "";
            var userRole = User.FindFirstValue("idrole") ?? "";
            var displayName = User.FindFirstValue(ClaimTypes.Name) ?? username;

            return (username, userRole, displayName);
        }

        [HttpGet("GetAllKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetAllKriteria([FromQuery] GetAllKriteriaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sanitizedDto = SanitizerHelper.EncodeObject(request);
                if (sanitizedDto == null) return NotFound();

                var (username, userRole, _) = GetUserInfo();

                var (data, totalData) = await _repo.GetAllAsync(request, username, userRole);

                var kriteriaDtos = data.Select(k => new KriteriaDto
                {
                    Id = k.Id,
                    RowNumber = k.RowNumber,
                    TahunAjaran = k.TahunAjaran,
                    Semester = k.Semester,
                    MataKuliah = k.MataKuliah,
                    MataKuliahId = k.MataKuliahId,
                    Dosen = k.Dosen,
                    DosenId = k.DosenId,
                    Status = k.Status,
                    AlasanTolak = k.AlasanTolak,
                    TipeKriteria = k.TipeKriteria,
                    CreatedBy = k.CreatedBy,
                    CreatedDate = k.CreatedDate,
                    UpdatedBy = k.UpdatedBy,
                    UpdatedDate = k.UpdatedDate,
                    Konsentrasi = k.Konsentrasi,
                    Details = k.Details.Select(d => new KriteriaDetailDto
                    {
                        Id = d.Id,
                        Kriteria = d.Kriteria,
                        Persentase = d.Persentase
                    }).ToList()
                }).ToList();

                var totalHalaman = ((totalData - 1) / request.PageSize) + 1;

                return Ok(new
                {
                    success = true,
                    message = "Data kriteria berhasil diambil",
                    data = kriteriaDtos,
                    TotalData = totalData,
                    TotalHalaman = totalHalaman
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("DetailKriteria/{id}")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetKriteriaDetail(string id)
        {
            try
            {
                var kriteria = await _repo.GetByIdAsync(id);
                if (kriteria == null)
                {
                    return NotFound(new { success = false, message = "Data kriteria tidak ditemukan" });
                }

                var kriteriaDto = new KriteriaDto
                {
                    Id = kriteria.Id,
                    RowNumber = kriteria.RowNumber,
                    TahunAjaran = kriteria.TahunAjaran,
                    Semester = kriteria.Semester,
                    MataKuliah = kriteria.MataKuliah,
                    MataKuliahId = kriteria.MataKuliahId,
                    Dosen = kriteria.Dosen,
                    DosenId = kriteria.DosenId,
                    Status = kriteria.Status,
                    AlasanTolak = kriteria.AlasanTolak,
                    TipeKriteria = kriteria.TipeKriteria,
                    CreatedBy = kriteria.CreatedBy,
                    CreatedDate = kriteria.CreatedDate,
                    UpdatedBy = kriteria.UpdatedBy,
                    UpdatedDate = kriteria.UpdatedDate,
                    Konsentrasi = kriteria.Konsentrasi,
                    Details = kriteria.Details.Select(d => new KriteriaDetailDto
                    {
                        Id = d.Id,
                        Kriteria = d.Kriteria,
                        Persentase = d.Persentase
                    }).ToList()
                };

                return Ok(new
                {
                    success = true,
                    data = kriteriaDto,
                    message = "Detail kriteria berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("CreateKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.create")]
        public async Task<IActionResult> CreateKriteria([FromBody] CreateKriteriaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sanitizedDto = SanitizerHelper.EncodeObject(request);
                if (sanitizedDto == null) return NotFound();

                var (username, userRole, _) = GetUserInfo();


                if (request.KriteriaDetails != null && request.KriteriaDetails.Count > 0 && request.KriteriaDetails.Sum(d => d.Persentase) != 100)
                {
                    return BadRequest(new { success = false, message = "Total persentase harus 100%" });
                }

                var kriteriaId = await _repo.CreateAsync(request, username, userRole);

                return Ok(new { success = true, data = new { id = kriteriaId }, message = "Kriteria berhasil dibuat" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("UpdateKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.edit")]
        public async Task<IActionResult> UpdateKriteria([FromBody] UpdateKriteriaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sanitizedDto = SanitizerHelper.EncodeObject(request);
                if (sanitizedDto == null) return NotFound();

                var (username, _, _) = GetUserInfo();

                var kriteria = await _repo.GetByIdAsync(request.KriteriaId);
                if (kriteria == null)
                {
                    return NotFound(new { success = false, message = "Data kriteria tidak ditemukan" });
                }


                if (request.KriteriaDetails != null && request.KriteriaDetails.Count > 0 && request.KriteriaDetails.Sum(d => d.Persentase) != 100)
                {
                    return BadRequest(new { success = false, message = "Total persentase harus 100%" });
                }

                var success = await _repo.UpdateAsync(request, username);
                if (!success)
                {
                    return BadRequest(new { success = false, message = "Gagal mengupdate kriteria" });
                }

                return Ok(new { success = true, message = "Kriteria berhasil diupdate" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteKriteria/{id}")]
        [RequiresPermission("kriteria_penilaian_dosen.edit")]
        public async Task<IActionResult> DeleteKriteria(string id)
        {
            try
            {
                var (username, _, _) = GetUserInfo();

                var kriteria = await _repo.GetByIdAsync(id);
                if (kriteria == null)
                {
                    return NotFound(new { success = false, message = "Data kriteria tidak ditemukan" });
                }

                var success = await _repo.DeleteAsync(id, username);
                if (!success)
                {
                    return BadRequest(new { success = false, message = "Gagal menghapus kriteria" });
                }

                return Ok(new { success = true, message = "Kriteria berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("SentKriteria/{id}")]
        [RequiresPermission("kriteria_penilaian_dosen.edit")]
        public async Task<IActionResult> SentKriteria(string id)
        {
            try
            {
                var (username, _, _) = GetUserInfo();

                var isValid = await _repo.CheckTotalPersentaseAsync(id);
                if (!isValid)
                {
                    return BadRequest(new { success = false, message = "Kriteria tidak dapat dikirim karena total presentase belum 100%" });
                }

                var success = await _repo.KirimAsync(id, username);
                if (!success)
                {
                    return BadRequest(new { success = false, message = "Gagal mengirim kriteria" });
                }

                return Ok(new { success = true, message = "Kriteria berhasil dikirim" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("ApproveKriteria/{id}")]
        [RequiresPermission("kriteria_penilaian_dosen.approve_reject")]
        public async Task<IActionResult> ApproveKriteria(string id)
        {
            try
            {
                var (username, _, _) = GetUserInfo();

                var success = await _repo.ApproveAsync(id, username);
                if (!success)
                {
                    return BadRequest(new { success = false, message = "Gagal menyetujui kriteria" });
                }

                return Ok(new { success = true, message = "Kriteria berhasil disetujui" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("RejectKriteria/{id}")]
        [RequiresPermission("kriteria_penilaian_dosen.approve_reject")]
        public async Task<IActionResult> RejectKriteria(string id, [FromBody] ApproveRejectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sanitizedDto = SanitizerHelper.EncodeObject(request);
                if (sanitizedDto == null) return NotFound();

                var (username, _, _) = GetUserInfo();

                request.KriteriaId = id;
                var success = await _repo.RejectAsync(request, username);
                if (!success)
                {
                    return BadRequest(new { success = false, message = "Gagal menolak kriteria" });
                }

                return Ok(new { success = true, message = "Kriteria berhasil ditolak" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("Dropdown/KonsentrasiKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetDropdownKonsentrasi()
        {
            try
            {
                var data = await _repo.GetListKonsentrasiAsync();

                return Ok(new { success = true, data = new ListResponse { Data = data }, message = "Data konsentrasi berhasil diambil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("Dropdown/TahunAjaranKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetDropdownTahunAjaran()
        {
            try
            {
                var data = await _repo.GetListTahunAjaranAsync();

                return Ok(new
                {
                    success = true,
                    data = data,
                    message = "Data tahun ajaran berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("Dropdown/MataKuliahKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetDropdownMataKuliah([FromQuery] string tahunAjaran, [FromQuery] string semester)
        {
            try
            {
                var (username, userRole, _) = GetUserInfo();

                var data = await _repo.GetListMataKuliahAsync(username, userRole, tahunAjaran ?? "", semester ?? "");

                return Ok(new
                {
                    success = true,
                    Data = data,
                    count = data.Count,
                    message = "Data mata kuliah berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("ActivePeriodKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetActivePeriod()
        {
            try
            {
                var activePeriode = await _repo.GetActivePeriodeByTanggalAsync() ??
                                   await _repo.GetActiveTahunAkademikByTanggalAsync();

                if (string.IsNullOrEmpty(activePeriode))
                {
                    return Ok(new
                    {
                        success = true,
                        data = new { tahunAjaran = "", semester = "" },
                        message = "Tidak ada periode aktif"
                    });
                }

                var periodParts = activePeriode.Split('|');
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        tahunAjaran = periodParts.Length > 0 ? periodParts[0] : "",
                        semester = periodParts.Length > 1 ? periodParts[1] : ""
                    },
                    message = "Periode aktif berhasil diambil"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("TemplateKriteria")]
        [RequiresPermission("kriteria_penilaian_dosen.view")]
        public async Task<IActionResult> GetTemplate([FromQuery] TemplateKriteriaRequest request)
        {
            try
            {

                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Request tidak boleh null. Silakan berikan parameter yang valid."
                    });
                }

                var (username, userRole, _) = GetUserInfo();
                var template = await _repo.GetTemplateKriteriaAsync(request, userRole, username);

                if (template == null)
                {
                    var tempmessage = request.Tipe?.ToUpper() switch
                    {
                        "RPS" => "Template kriteria berdasarkan RPS belum tersedia",
                        "TAHUNLALU" => "Template kriteria dari tahun akademik sebelumnya tersedia",
                        "UUT" => "Template UTS, UAS, Tugas tersedia",
                        "KUSTOM" => "Silakan isi kriteria secara manual",
                        _ => "Template tidak tersedia"
                    };

                    return Ok(new
                    {
                        success = true,
                        data = new TemplateKriteriaResponse
                        {
                            Tipe = request.Tipe ?? string.Empty,
                            KriteriaDetails = []
                        },
                        message = tempmessage
                    });
                }

                return Ok(new { success = true, data = template, message = "Template kriteria berhasil diambil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("CheckTotalKriteria/{id}")]

        public async Task<IActionResult> CheckTotalProsentase(string id)
        {
            try
            {
                var isValid = await _repo.CheckTotalPersentaseAsync(id);

                return Ok(new { success = true, data = new { isValid }, message = "Check total berhasil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}