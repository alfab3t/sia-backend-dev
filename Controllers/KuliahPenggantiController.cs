using astratech_apps_backend.DTOs.KuliahPengganti;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using astratech_apps_backend.Helpers;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KuliahPenggantiController : ControllerBase
    {
        private readonly IKuliahPenggantiRepository _repo;

        private const string RoleProdi = "ROL22";
        private const string RoleKaprodi = "ROL27";
        private const string RoleDAAK = "ROL21";
        private const string RoleGA = "ROL20";
        private const string RoleDosen = "ROL25";
        private const string RoleMahasiswa = "ROL23";

        private const string StatusDraft = "draft";
        private const string StatusDiterima = "diterima";
        private const string StatusBelumDisetujuiProdi = "belum disetujui prodi";
        private const string StatusBelumDisetujuiDAAK = "belum disetujui daak";
        private const string StatusBelumDisetujuiGA = "belum disetujui ga";

        private const string UnauthenticatedMessage = "User tidak terautentikasi";
        private const string DataNotFoundMessage = "Data tidak ditemukan";

        
        private const string PermissionView = "kuliah_pengganti.view";
        private const string PermissionCreate = "kuliah_pengganti.create";
        private const string PermissionEdit = "kuliah_pengganti.edit";
        private const string PermissionDelete = "kuliah_pengganti.delete";
        private const string PermissionApproveReject = "kuliah_pengganti.approve_reject";


        private const string CLAIM_ROLE = "idrole";
        private const string RESPONSE_SUCCESS = "SUCCESS";
        private const string RESPONSE_ERROR = "ERROR";
        private const string RESPONSE_UNAUTHORIZED = "UNAUTHORIZED";
        private string GetUsername()
            {
                return User.Identity?.Name
                    ?? throw new UnauthorizedAccessException("User belum login");
            }
        public KuliahPenggantiController(IKuliahPenggantiRepository repo)
        {
            _repo = repo;
        }

        private (string username, string userRole, string displayName, string roleDisplay) GetUserInfo()
        {
            var username = User.FindFirst("namaakun")?.Value
                ?? User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.Identity?.Name
                ?? "";

            var userRole = User.FindFirst("role")?.Value
                ?? User.FindFirst("idrole")?.Value
                ?? User.FindFirst(ClaimTypes.Role)?.Value
                ?? "";

            var displayName = User.FindFirst("displayname")?.Value
                ?? User.FindFirst(ClaimTypes.Name)?.Value
                ?? username;

            var roleDisplay = GetRoleDisplayName(userRole);

            return (username, userRole, displayName, roleDisplay);
        }

        private static string GetRoleDisplayName(string roleCode)
        {
            return roleCode?.ToUpper() switch
            {
                "ROL22" => "Prodi",
                "ROL27" => "Kaprodi",
                "ROL21" => "DAAK",
                "ROL20" => "GA (General Affairs)",
                "ROL25" => "Dosen",
                "ROL23" => "Mahasiswa",
                _ => roleCode ?? "Unknown Role"
            };
        }

        [HttpPost("CreatePengajuanKP")]
        [RequiresPermission(PermissionCreate)]
        public async Task<IActionResult> CreatePengajuanKP([FromBody] CreatePengajuanKPDto dto)
        {
           
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                if (!(userRole == RoleDosen || userRole == RoleMahasiswa))
                {
                    return StatusCode(403, new { success = false, message = "Role tidak diizinkan untuk membuat pengajuan" });
                }

                var result = await _repo.CreatePengajuanKPAsync(dto);

                if (!result)
                    return BadRequest(new { success = false, message = "Gagal membuat pengajuan kuliah pengganti" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan kuliah pengganti berhasil dibuat",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
        
        }

        [HttpPut("EditPengajuanKP")]
        [RequiresPermission(PermissionEdit)]
        public async Task<IActionResult> EditPengajuanKP([FromBody] EditPengajuanKPDto dto)
        {
       
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                var existingData = await _repo.GetDetailKPAsync(dto.KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });


                if (userRole == RoleDosen || userRole == RoleMahasiswa || userRole == RoleGA)
                {
                    var Status = (existingData.Status ?? "").ToLower();

                    if (!(Status == StatusDraft.ToLower() || Status == StatusBelumDisetujuiGA.ToLower()))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Hanya dapat mengedit data dengan status Draft atau Belum Disetujui GA"
                        });
                    }
                }
                else
                {
                    return StatusCode(403, new { success = false, message = "Role tidak diizinkan untuk mengedit" });
                }

                if (string.IsNullOrEmpty(dto.ModifiedBy))
                {
                    dto.ModifiedBy = username;
                }

                var result = await _repo.EditPengajuanKPAsync(dto);

                if (!result)
                    return BadRequest(new { success = false, message = "Gagal mengubah pengajuan kuliah pengganti" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan kuliah pengganti berhasil diubah",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
        }

        [HttpPost("ApprovePengajuanKP")]
        [RequiresPermission(PermissionApproveReject)]
        public async Task<IActionResult> ApprovePengajuanKP([FromBody] ApprovePengajuanKPDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }
                var existingData = await _repo.GetDetailKPAsync(dto.KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });
                var currentStatus = existingData.Status?.ToLower() ?? "";
                var approverRole = "";
                var nextStatus = "";
                if (currentStatus == StatusBelumDisetujuiProdi)
                {
                    if (!(userRole == RoleProdi || userRole == RoleKaprodi))
                        return StatusCode(403, new { success = false, message = "Hanya Prodi yang dapat menyetujui pada tahap ini" });

                    approverRole = "Prodi";
                    nextStatus = "Belum Disetujui DAAK";
                }
                else if (currentStatus == StatusBelumDisetujuiDAAK)
                {
                    if (userRole != RoleDAAK)
                        return StatusCode(403, new { success = false, message = "Hanya DAAK yang dapat menyetujui pada tahap ini" });

                    approverRole = "DAAK";
                    nextStatus = "Belum Disetujui GA";
                }
                else if (currentStatus == StatusBelumDisetujuiGA)
                {
                    if (userRole != RoleGA)
                        return StatusCode(403, new { success = false, message = "Hanya GA yang dapat menyetujui pada tahap ini" });

                    approverRole = "GA";
                    nextStatus = "Diterima";
                }
                else
                {
                    return BadRequest(new { success = false, message = $"Status tidak valid untuk approval: {existingData.Status}" });
                }

                dto.Role = approverRole;
                dto.Status = nextStatus;
                dto.Approver = username;

                var ok = await _repo.ApprovePengajuanKPAsync(dto);

                if (!ok)
                    return BadRequest(new { success = false, message = "Gagal approve pengajuan kuliah pengganti" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan kuliah pengganti berhasil di-approve",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("SentPengajuanKP")]
        [RequiresPermission(PermissionApproveReject)]
        public async Task<IActionResult> SentPengajuanKP([FromBody] SentPengajuanKPDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                if (!(userRole == RoleDosen || userRole == RoleMahasiswa))
                {
                    return StatusCode(403, new { success = false, message = "Hanya Dosen dan Mahasiswa yang dapat mengirim pengajuan" });
                }

                var existingData = await _repo.GetDetailKPAsync(dto.KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                if ((existingData.Status?.ToLower() ?? "") != StatusDraft)
                    return BadRequest(new { success = false, message = "Hanya data dengan status Draft yang dapat diajukan" });

                
                if (string.IsNullOrEmpty(dto.ModifiedBy))
                {
                    dto.ModifiedBy = username;
                }

                var result = await _repo.SentPengajuanKPAsync(dto);

                if (!result)
                    return BadRequest(new { success = false, message = "Gagal mengirim pengajuan" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan berhasil dikirim",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("DeletePengajuanKP")]
        [RequiresPermission(PermissionDelete)]
        public async Task<IActionResult> DeletePengajuanKP(string KuliahPenggantiId)
        {
        
                if (string.IsNullOrWhiteSpace(KuliahPenggantiId))
                    return BadRequest(new { success = false, message = "id kuliah pengganti wajib diisi" });

                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                if (!(userRole == RoleDosen || userRole == RoleMahasiswa))
                {
                    return StatusCode(403, new { success = false, message = "Hanya Dosen dan Mahasiswa yang dapat menghapus pengajuan" });
                }

                var existingData = await _repo.GetDetailKPAsync(KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                if ((existingData.Status?.ToLower() ?? "") != StatusDraft)
                    return BadRequest(new { success = false, message = "Hanya dapat menghapus data dengan status Draft" });

                await _repo.DeletePengajuanKPAsync(KuliahPenggantiId, username);

                return Ok(new
                {
                    success = true,
                    message = "Data berhasil dihapus",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
     
        }

        [HttpPost("RejectPengajuanKP")]
        [RequiresPermission(PermissionApproveReject)]
        public async Task<IActionResult> RejectPengajuanKP([FromBody] RejectPengajuanKPDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                var existingData = await _repo.GetDetailKPAsync(dto.KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                var currentStatus = existingData.Status?.ToLower() ?? "";

                
                if (!CanRejectStatus(currentStatus, userRole))
                {
                    return StatusCode(403, new { success = false, message = $"Role {userRole} tidak diizinkan untuk menolak pada status {currentStatus}" });
                }

                dto.ModifiedBy = username;

                var result = await _repo.RejectPengajuanKPAsync(dto);

                if (!result)
                    return BadRequest(new { success = false, message = "Gagal menolak pengajuan" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan ditolak",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("CancelPengajuanKP")]
        [RequiresPermission(PermissionEdit)]
        public async Task<IActionResult> CancelPengajuanKP([FromBody] CancelPengajuanKPDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                var existingData = await _repo.GetDetailKPAsync(dto.KuliahPenggantiId);

                if (existingData == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                var currentStatus = existingData.Status?.ToLower() ?? "";

                
                if (!CanCancelStatus(currentStatus, userRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = GetCancelRESPONSE_ERROR(userRole)
                    });
                }

                dto.ModifiedBy = username;

                var result = await _repo.CancelPengajuanKPAsync(dto);

                if (!result)
                    return BadRequest(new { success = false, message = "Gagal membatalkan pengajuan" });

                return Ok(new
                {
                    success = true,
                    message = "Pengajuan berhasil dibatalkan",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("GetDataKP")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetDataKP([FromQuery] GetKPRequest request)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                if (string.IsNullOrEmpty(request.Status))
                {
                    request.Status = GetStatusFilterByRole(userRole);
                }

                var data = await _repo.GetDataKPAsync(request);

                
                var dataList = data as List<GetDataKP> ?? data?.ToList() ?? new List<GetDataKP>();

                
                var dataWithActions = dataList.Select(item => new
                {
                    
                    item.KuliahPenggantiId,
                    item.KonsentrasiSingkatan,
                    item.TahunAjaran,
                    item.Semester,
                    item.NamaMataKuliah,
                    item.DosenKode,
                    item.JadwalPertemuan,
                    item.JadwalRencana,
                    item.JadwalPengganti,
                    item.NamaRuangan,
                    item.Status,
                    item.KonsentrasiId,

                    can_edit = CanEditKP(item, userRole),
                    can_delete = CanDeleteKP(item, userRole),
                    can_sent = CanSentKP(item, userRole),
                    can_approve = CanApproveKP(item, userRole),
                    can_reject = CanRejectKP(item, userRole),
                    can_cancel = CanCancelKP(item, userRole),
                    can_view_detail = true
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Data berhasil diambil",
                    data = dataWithActions,
                    user_role = userRole,
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        [HttpGet("GetRiwayat")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetRiwayat(
            [FromQuery] string search = "",
            [FromQuery] string tahunAkademik = "",
            [FromQuery] string semester = "",
            [FromQuery] string konsentrasiId = "",
            [FromQuery] string sortBy = "kpe_tanggal desc")
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                var request = new GetKPRiwayatRequest
                {
                    SearchText = search,
                    SortBy = sortBy,
                    KonsentrasiId = konsentrasiId,
                    TahunAjaran = tahunAkademik,
                    Semester = semester,
                    RoleCode = userRole,
                    Username = username,
                    Role = userRole
                };

                var data = await _repo.GetDataKPRiwayatAsync(request);

                
                var dataList = data as List<GetDataKPRiwayat> ?? data?.ToList() ?? new List<GetDataKPRiwayat>();

                
                var dataWithActions = dataList.Select(item => new
                {
                    item.KuliahPenggantiId,
                    item.KonsentrasiSingkatan,
                    item.TahunAjaran,
                    item.Semester,
                    item.NamaMataKuliah,
                    item.DosenKode,
                    item.JadwalPertemuan,
                    item.JadwalRencana,
                    item.JadwalPengganti,
                    item.NamaRuangan,
                    item.Status,
                    item.TanggalPengajuan,
                    item.KonsentrasiId,

                    
                    can_edit = false, 
                    can_delete = false, 
                    can_sent = false, 
                    can_approve = false, 
                    can_reject = false, 
                    can_cancel = CanCancelInRiwayat(item, userRole),
                    can_view_detail = true 
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Data riwayat berhasil diambil",
                    data = dataWithActions,
                    user_role = userRole,
                    user_role_display = roleDisplay,  
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        private static bool CanEditKP(GetDataKP item, string userRole)
        {
            if (item == null) return false;

            var Status = (item.Status ?? "").ToLower();

            var isCreatorRole =
                userRole == RoleDosen ||
                userRole == RoleMahasiswa ||
                userRole == RoleGA;

            var isEditableStatus =
                Status == StatusDraft.ToLower() ||
                Status == StatusBelumDisetujuiGA.ToLower();

            return isCreatorRole && isEditableStatus;
        }

        private static bool CanDeleteKP(GetDataKP item, string userRole)
        {
            
            return (userRole == RoleDosen || userRole == RoleMahasiswa)
                && (item.Status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanSentKP(GetDataKP item, string userRole)
        {
            
            return (userRole == RoleDosen || userRole == RoleMahasiswa)
                && (item.Status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanApproveKP(GetDataKP item, string userRole)
        {
            var status = item.Status?.ToLower() ?? "";

            if (userRole == RoleProdi || userRole == RoleKaprodi)
            {
                return status == StatusBelumDisetujuiProdi;
            }
            else if (userRole == RoleDAAK)
            {
                return status == StatusBelumDisetujuiDAAK;
            }
            else if (userRole == RoleGA)
            {
                return status == StatusBelumDisetujuiGA;
            }
            return false;
        }

        private static bool CanRejectKP(GetDataKP item, string userRole)
        {
            
            return CanApproveKP(item, userRole);
        }

        private static bool CanCancelKP(GetDataKP item, string userRole)
        {
            var status = item.Status?.ToLower() ?? "";

            if (userRole == RoleDosen || userRole == RoleMahasiswa)
            {
                
                var allowedStatus = new[]
                {
                    StatusBelumDisetujuiProdi,
                    StatusBelumDisetujuiDAAK,
                    StatusBelumDisetujuiGA
                };
                return allowedStatus.Contains(status);
            }
            else if (userRole == RoleDAAK)
            {
                
                return status == StatusBelumDisetujuiGA;
            }
            else if (userRole == RoleGA)
            {
                
                return status == StatusDiterima;
            }
            else if (userRole == RoleProdi || userRole == RoleKaprodi )
            {
                return status == StatusBelumDisetujuiDAAK;
            }
            return false;
        }

        private static bool CanCancelInRiwayat(GetDataKPRiwayat item, string userRole)
        {
            var status = item.Status?.ToLower() ?? "";

            
            if (userRole == RoleDAAK && status == StatusBelumDisetujuiGA)
            {
                return true; 
            }
            else if (userRole == RoleGA && status == StatusDiterima)
            {
                return true; 
            }else if ((userRole == RoleProdi || userRole == RoleKaprodi) && status == StatusBelumDisetujuiDAAK)
            {
                return true; 
            }
            return false;
        }

        private static bool CanRejectStatus(string status, string userRole)
        {
            if (status == StatusBelumDisetujuiProdi)
            {
                return userRole == RoleProdi || userRole == RoleKaprodi;
            }
            else if (status == StatusBelumDisetujuiDAAK)
            {
                return userRole == RoleDAAK;
            }
            else if (status == StatusBelumDisetujuiGA)
            {
                return userRole == RoleGA;
            }
            return false;
        }

        private static bool CanCancelStatus(string status, string userRole)
        {
            if (userRole == RoleDosen || userRole == RoleMahasiswa)
            {
                var allowedStatus = new[]
                {
                    StatusBelumDisetujuiProdi,
                    StatusBelumDisetujuiDAAK,
                    StatusBelumDisetujuiGA
                };
                return allowedStatus.Contains(status);
            }
            else if (userRole == RoleDAAK)
            {
                return status == StatusBelumDisetujuiGA;
            }
            else if (userRole == RoleGA)
            {
                return status == StatusDiterima;
            }
            else if (userRole == RoleProdi || userRole == RoleKaprodi )
            {
                return status == StatusBelumDisetujuiDAAK;
            }
            return false;
        }

        private static string GetStatusFilterByRole(string userRole)
        {
            switch (userRole)
            {
                case RoleProdi:
                case RoleKaprodi:
                    return "Belum Disetujui Prodi";
                case RoleDAAK:
                    return "Belum Disetujui DAAK";
                case RoleGA:
                    return "Belum Disetujui GA";
                case RoleDosen:
                case RoleMahasiswa:
                    return "";
                default:
                    return "";
            }
        }

        private static string GetCancelRESPONSE_ERROR(string userRole)
        {
            if (userRole == RoleDosen || userRole == RoleMahasiswa)
            {
                return "Hanya dapat membatalkan pengajuan yang masih dalam proses approval";
            }
            else if (userRole == RoleDAAK)
            {
                return "Hanya dapat membatalkan pengajuan dengan status 'Belum Disetujui GA'";
            }
            else if (userRole == RoleGA)
            {
                return "Hanya dapat membatalkan pengajuan dengan status 'Diterima'";
            } else if (userRole == RoleProdi || userRole == RoleKaprodi )
            {
                return "Hanya dapat membatalkan pengajuan dengan status 'Belum Disetujui DAAK'";
            }
            return "Role tidak diizinkan untuk membatalkan";
        }

        [HttpGet("detail/{id}")]
        [RequiresPermission(PermissionView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetailKP(string id)
        {
            try
            {
                var (username, _, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                var data = await _repo.GetDetailKPAsync(id);

                if (data == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                return Ok(new
                {
                    success = true,
                    message = "Detail berhasil diambil",
                    data,
                    user_role_display = roleDisplay,  
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        [HttpGet("pertemuan/{id}")]
        [RequiresPermission(PermissionView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetailPertemuanKP(string id)
        {
            try
            {
                var (username, _, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                var data = await _repo.GetDetailPertemuanKPAsync(id);

                if (data == null)
                    return NotFound(new { success = false, message = DataNotFoundMessage });

                return Ok(new
                {
                    success = true,
                    message = "Detail pertemuan berhasil diambil",
                    data,
                    user_role_display = roleDisplay,  
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        [HttpGet("GetUserInfo")]
        [RequiresPermission(PermissionView)]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public IActionResult GetUserInfoEndpoint()
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        username,
                        user_role = userRole,
                        user_role_display = roleDisplay,
                        display_name = displayName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        [HttpGet("GetUserPermissions")]
        [RequiresPermission(PermissionView)]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public IActionResult GetUserPermissions()
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                var permissions = GetPermissionsByRole(userRole);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        username,
                        user_role = userRole,
                        user_role_display = roleDisplay,
                        display_name = displayName,
                        permissions = permissions
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        private static List<string> GetPermissionsByRole(string roleCode)
        {
            return roleCode?.ToUpper() switch
            {
                "ROL22" => new List<string> { PermissionView, PermissionApproveReject },
                "ROL27" => new List<string> { PermissionView, PermissionApproveReject },
                "ROL21" => new List<string> { PermissionView, PermissionApproveReject },
                "ROL20" => new List<string> { PermissionView, PermissionApproveReject },
                "ROL25" => new List<string> { PermissionCreate, PermissionEdit, PermissionDelete, PermissionApproveReject, PermissionView },
                "ROL23" => new List<string> { PermissionCreate, PermissionEdit, PermissionDelete, PermissionApproveReject, PermissionView },
                _ => new List<string> { PermissionView }
            };
        }

        [HttpGet("GetDataKPByDosen")]
        [RequiresPermission(PermissionCreate)]
        public async Task<IActionResult> GetDataKPByDosen([FromQuery] GetKPByDosenRequestDto request)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                if (userRole != RoleDosen)
                {
                    return StatusCode(403, new { success = false, message = "Hanya dosen yang dapat mengakses data ini" });
                }

                var data = await _repo.GetDataKPByDosenAsync(username, request);

                
                var dataList = data as List<GetKPByDosenResponseDto> ?? data?.ToList() ?? new List<GetKPByDosenResponseDto>();

                
                var dataWithActions = dataList.Select(item => new
                {
                    
                    item.KuliahPenggantiId,
                    item.KonsentrasiSingkatan,
                    item.TahunAjaran,
                    item.Semester,
                    item.NamaMataKuliah,
                    item.DosenKode,
                    item.JadwalPertemuan,
                    item.JadwalRencana,
                    item.JadwalPengganti,
                    item.NamaRuangan,
                    item.status,
                    item.TanggalPengajuan,

                    
                    can_edit = CanEditKPByDosen(item, userRole),
                    can_delete = CanDeleteKPByDosen(item, userRole),
                    can_sent = CanSentKPByDosen(item, userRole),
                    can_approve = false, 
                    can_reject = false, 
                    can_cancel = CanCancelKPByDosen(item, userRole),
                    can_view_detail = true 
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Data kuliah pengganti oleh dosen berhasil diambil",
                    data = dataWithActions,
                    user_role = userRole,
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        [HttpGet("GetDataKPByNIM")]
        [RequiresPermission(PermissionCreate)]
        public async Task<IActionResult> GetDataKPByNIM([FromQuery] GetKPByNimRequestDto request)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = UnauthenticatedMessage });
                }

                
                if (userRole != RoleMahasiswa)
                {
                    return StatusCode(403, new { success = false, message = "Hanya mahasiswa yang dapat mengakses data ini" });
                }

                
                if (userRole == RoleMahasiswa && string.IsNullOrEmpty(request.NIM))
                {
                    
                    request.NIM = username;
                }

                var data = await _repo.GetDataKPByNIMAsync(request);

                
                var dataList = data as List<GetKPByNimResponseDto> ?? data?.ToList() ?? new List<GetKPByNimResponseDto>();

                
                var dataWithActions = dataList.Select(item => new
                {
                    
                    item.KuliahPenggantiId,
                    item.KonsentrasiSingkatan,
                    item.TahunAjaran,
                    item.Semester,
                    item.NamaMataKuliah,
                    item.DosenKode,
                    item.JadwalPertemuan,
                    item.JadwalRencana,
                    item.JadwalPengganti,
                    item.NamaRuangan,
                    item.Status,
                    item.TanggalPengajuan,

                    
                    can_edit = CanEditKPByMahasiswa(item, userRole),
                    can_delete = CanDeleteKPByMahasiswa(item, userRole),
                    can_sent = CanSentKPByMahasiswa(item, userRole),
                    can_approve = false, 
                    can_reject = false, 
                    can_cancel = CanCancelKPByMahasiswa(item, userRole),
                    can_view_detail = true 
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Data kuliah pengganti oleh mahasiswa berhasil diambil",
                    data = dataWithActions,
                    user_role = userRole,
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"{RESPONSE_ERROR}: {ex.Message}"
                });
            }
        }

        private static bool CanEditKPByDosen(GetKPByDosenResponseDto item, string userRole)
        {
            return userRole == RoleDosen && (item.status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanDeleteKPByDosen(GetKPByDosenResponseDto item, string userRole)
        {
            
            return userRole == RoleDosen && (item.status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanSentKPByDosen(GetKPByDosenResponseDto item, string userRole)
        {
            
            return userRole == RoleDosen && (item.status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanCancelKPByDosen(GetKPByDosenResponseDto item, string userRole)
        {
            if (userRole == RoleDosen)
            {
                var status = item.status?.ToLower() ?? "";
                var allowedStatus = new[]
                {
            StatusBelumDisetujuiProdi,
            StatusBelumDisetujuiDAAK,
            StatusBelumDisetujuiGA
        };
                return allowedStatus.Contains(status);
            }
            return false;
        }

        private static bool CanEditKPByMahasiswa(GetKPByNimResponseDto item, string userRole)
        {
            
            return userRole == RoleMahasiswa && (item.Status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanDeleteKPByMahasiswa(GetKPByNimResponseDto item, string userRole)
        {
            
            return userRole == RoleMahasiswa && (item.Status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanSentKPByMahasiswa(GetKPByNimResponseDto item, string userRole)
        {
            
            return userRole == RoleMahasiswa && (item.Status?.ToLower() ?? "") == StatusDraft;
        }

        private static bool CanCancelKPByMahasiswa(GetKPByNimResponseDto item, string userRole)
        {
            
            if (userRole == RoleMahasiswa)
            {
                var status = item.Status?.ToLower() ?? "";
                var allowedStatus = new[]
                {
            StatusBelumDisetujuiProdi,
            StatusBelumDisetujuiDAAK,
            StatusBelumDisetujuiGA
        };
                return allowedStatus.Contains(status);
            }
            return false;
        }

        [HttpGet("checkJadwalBentrokKP")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> CheckJadwalBentrok(
            [FromQuery] CheckJadwalBentrokKPRequestDto dto)
        {
            var result = await _repo.CheckJadwalBentrokKPAsync(dto);

            if (!string.IsNullOrEmpty(result))
            {
                return Ok(new
                {
                    isBentrok = true,
                    subjek = result
                });
            }

            return Ok(new
            {
                isBentrok = false,
                subjek = ""
            });
        }

        [HttpGet("check-bentrok-edit")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> CheckJadwalBentrok(
            [FromQuery] CheckJadwalBentrokRequestDto dto)
        {
            var result = (await _repo.CheckJadwalBentrokAsync(dto)).ToList();

            if (!result.Any() || !result[0].IsBentrok)
            {
                return Ok(new
                {
                    isBentrok = false,
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                isBentrok = true,
                data = result
            });
        }
    
    




        [HttpGet("active")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetActiveTA()
        {
            var data = await _repo.GetTahunAjaranAktifAsync();

            if (data == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Tidak ditemukan tahun akademik aktif."
                });
            }

            return Ok(new
            {
                success = true,
                data
            });
        }


        [HttpGet("GetAllDosen")]
        [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetAllListDosen()
        {
            try
            {
                var data = await _repo.GetAllListDosenAsync();

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }
   
           [HttpGet("GetListGrup")]
            [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetListGrup(
            string KonsentrasiId,
            string KelasId,
            string GrupTahunAjaran,
            string GrupSemester
        )
        {
            try
            {
                var data = await _repo.GetListGrupAsync(KonsentrasiId, KelasId, GrupTahunAjaran, GrupSemester);

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }
   
        [HttpGet("GetListKelas")]
                [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetListKelas(
            [FromQuery] string KonsentrasiId,
            [FromQuery] string TahunAjaran,
            [FromQuery] string jenis
        )
        {
            try
            {
                var username = User.Identity?.Name;
                var role = User.Claims.FirstOrDefault(c => c.Type == CLAIM_ROLE)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = RESPONSE_UNAUTHORIZED });
                }

                IEnumerable<dynamic> data;

                if (role == RoleMahasiswa)
                {
                    data = await _repo.GetKelasByNIMAsync(KonsentrasiId, TahunAjaran, username);
                }
                else if (role == RoleDosen)
                {
                    data = await _repo.GetKelasByDosenAsync(KonsentrasiId, TahunAjaran, username);
                }
                else
                {
                    data = await _repo.GetListKelasAsync(KonsentrasiId, TahunAjaran, jenis);
                }

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    role,
                    username,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }



        [HttpGet("GetMataKuliahKP")]
        [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetMataKuliahKP(
            [FromQuery] string KonsentrasiId,
            [FromQuery] string KelasId,
            [FromQuery] string TahunAjaran = "",
            [FromQuery] string Semester = "")
        {
            try
            {
                var username = User.Identity?.Name ?? string.Empty;
                var role = User.Claims.FirstOrDefault(c => c.Type == CLAIM_ROLE)?.Value;

                if (string.IsNullOrEmpty(KonsentrasiId) || string.IsNullOrEmpty(KelasId))
                {
                    return BadRequest(new
                    {
                        message = RESPONSE_ERROR,
                        error = "Parameter KonsentrasiId dan KelasId wajib diisi"
                    });
                }

                object data;

                if (role == RoleMahasiswa)
                {
                    if (string.IsNullOrEmpty(TahunAjaran) || string.IsNullOrEmpty(Semester))
                    {
                        return BadRequest(new
                        {
                            message = RESPONSE_ERROR,
                            error = "Parameter TahunAjaran dan Semester wajib diisi"
                        });
                    }

                    data = await _repo.GetMataKuliahByNIMAsync(username, TahunAjaran, Semester);
                }
                else if (role == RoleDosen)
                {
                    if (string.IsNullOrEmpty(TahunAjaran) || string.IsNullOrEmpty(Semester))
                    {
                        return BadRequest(new
                        {
                            message = RESPONSE_ERROR,
                            error = "Parameter TahunAjaran dan Semester wajib diisi"
                        });
                    }

                    data = await _repo.GetMataKuliahByDosenAsync(
                        username, KelasId, TahunAjaran, Semester, "");
                }
                else
                {
                    data = await _repo.GetMataKuliahKPAsync(KonsentrasiId, KelasId);
                }

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    role,
                    username,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = RESPONSE_ERROR, error = ex.Message });
            }
        }

        
        [HttpGet("GetDetail")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetDetail(
            [FromQuery] string MataKuliahId,
            [FromQuery] string KonsentrasiId,
            [FromQuery] string TahunAjaran,
            [FromQuery] string Semester,
            [FromQuery] string GrupId,
            [FromQuery] string SectionId)
        {
            try
            {
                var data = await _repo.GetMataKuliahDetailAsync(
                    MataKuliahId, KonsentrasiId, TahunAjaran, Semester, GrupId, SectionId);

                return Ok(new { message = RESPONSE_SUCCESS, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = RESPONSE_ERROR, error = ex.Message });
            }
        }

        
        [HttpGet("GetMataKuliahForFilter")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetMataKuliahForFilter(
            [FromQuery] string KonsentrasiId,
            [FromQuery] string TahunAjaran = "",
            [FromQuery] string semester = "")
        {
     try
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == CLAIM_ROLE)?.Value;

        if (string.IsNullOrEmpty(KonsentrasiId))
        {
            return BadRequest(new
            {
                message = RESPONSE_ERROR,
                error = "Parameter KonsentrasiId wajib diisi"
            });
        }

        var username = User.Identity?.Name ?? string.Empty;
        object data;

                if (role == RoleMahasiswa)
                {
                    data = await _repo.GetMataKuliahByNIMAsync(username, TahunAjaran, semester);
                }
                else if (role == RoleDosen)
                {
                    data = await _repo.GetMataKuliahByDosenAsync(username, "", TahunAjaran, semester, "");
                }
                else
                {
                                    return Ok(new
                                    {
                                        message = RESPONSE_SUCCESS,
                                        role,
                                        data = new List<object>() 
                                    });                }

                return Ok(new { message = RESPONSE_SUCCESS, role, username, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = RESPONSE_ERROR, error = ex.Message });
            }
        }
   
   
        [HttpGet("GetListPertemuan")]
        [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetListPertemuan(
            string KonsentrasiId,
            string TahunAjaran,
            string Semester,
            string MataKuliahId,
            string GrupId,
            string SectionId)
        {
            var data = await _repo.GetListPertemuanAsync(KonsentrasiId, TahunAjaran, Semester, MataKuliahId, GrupId, SectionId);

            return Ok(new
            {
                message = RESPONSE_SUCCESS,
                data
            });
        }

        [HttpGet("GetAllProdi")]
        [RequiresPermission(PermissionView)]

        public async Task<IActionResult> GetAllListProdi()
        {
            try
            {
                var username = GetUsername();
                var role = User.Claims.FirstOrDefault(c => c.Type == "idrole")?.Value;

                IEnumerable<dynamic> data;

                if (role == RoleMahasiswa) 
                {
                    data = await _repo.GetKonsentrasiByNIMAsync(username);
                }
                else if (role == RoleDosen) 
                {
                    data = await _repo.GetKonsentrasiByDosenAsync(username);
                }
                else
                {
                    data = await _repo.GetAllListProdiAsync();
                }

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    role,
                    username,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }
    
        [HttpGet("GetAllRuangan")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetAllListRuangan()
        {
            try
            {
                var data = await _repo.GetAllListRuanganAsync();

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }
  

        [HttpGet("GetSection")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetSection(
            string KonsentrasiId,
            string KelasId,
            string GrupId,
            string TahunAjaran,
            string Semester)
        {
            var data = await _repo.GetSectionByGrupAsync(KonsentrasiId, KelasId, GrupId, TahunAjaran, Semester);

            return Ok(new
            {
                message = RESPONSE_SUCCESS,
                data
            });
        }

        [HttpGet("GetTahunSemesterActive")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetTahunSemesterActive()
        {
            try
            {
                var data = await _repo.GetTahunSemesterActiveAsync();

                return Ok(new
                {
                    message = RESPONSE_SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = RESPONSE_ERROR,
                    error = ex.Message
                });
            }
        }
    

    }
}