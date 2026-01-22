using astratech_apps_backend.DTOs.TatapMuka;
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
    public class TatapMukaController : ControllerBase
    {
        private readonly ITatapMukaRepository _repo;

        private const string RoleProdi = "ROL22";      
        private const string RoleKaprodi = "ROL27";     
        private const string RoleDAAK = "ROL21";       
        private const string RoleGA = "ROL20";        
        private const string RoleDosen = "ROL25";    

        private const string StatusDraft = "draft";
        private const string StatusDiterima = "disetujui";
        private const string StatusBelumDisetujuiProdi = "belum disetujui prodi";
        private const string StatusBelumDisetujuiDAAK = "belum disetujui daak";
        private const string StatusBelumDisetujuiGA = "belum disetujui ga";

        private const string UnauthenticatedMessage = "User tidak terautentikasi";
        private const string DataNotFoundMessage = "Data tidak ditemukan";
        private const string ErrorMessage = "ERROR";

        private const string MessageSuccess = "SUCCESS";
        private const string PermissionView = "tatap_muka_tambahan.view";
        private const string PermissionApproveReject = "tatap_muka_tambahan.approve_reject";
        private const string PermissionEdit = "tatap_muka_tambahan.edit";


        public TatapMukaController(ITatapMukaRepository repo)
        {
            _repo = repo;
        }

        private static string GetRoleDisplayName(string roleCode)
        {
            return roleCode?.ToUpper() switch
            {
                "ROL22" => "Program Studi",
                "ROL27" => "Kaprodi",
                "ROL21" => "DAAK",
                "ROL20" => "GA (General Affairs)",
                "ROL25" => "Dosen",
                "ROL23" => "Mahasiswa",
                _ => roleCode ?? "Unknown Role"
            };
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

        [HttpPost("Create")]
        [RequiresPermission("tatap_muka_tambahan.create")]
        public async Task<IActionResult> Create([FromBody] CreateTatapMukaDto dto)
        {

            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                if (userRole != RoleDosen)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Role tidak diizinkan membuat Tatap Muka"
                    });
                }

                dto.CreatedBy = username;
                await _repo.CreateTatapMukaAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Tatap Muka berhasil dibuat",
                    user_role_display = roleDisplay,
                    display_name = displayName

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpGet("List")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> List(
            [FromQuery] string mode = "",
            [FromQuery] string search = "",
            [FromQuery] string orderBy = "",
            [FromQuery] string konsentrasiId = "",
            [FromQuery] string tahunAjaran = "",
            [FromQuery] string semester = "",
            [FromQuery] string status = "")
        {

            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                var prodiCode = User.FindFirst("konsentrasi_id")?.Value ?? "";
                if (userRole == RoleProdi || userRole == RoleKaprodi)
                {
                    konsentrasiId = prodiCode;
                }

                var rawData = await _repo.GetDataTatapMukaAsync(
                    mode,
                    search,
                    orderBy,
                    konsentrasiId,
                    tahunAjaran,
                    semester,
                    userRole,
                    status,
                    username
                );

                var dataList = rawData?.ToList() ?? new List<TatapMukaListItemDto>();

                var dataWithActions = dataList.Select(item => new
                {
                    item.TatapMukaId,
                    item.KonsentrasiSingkatan,
                    item.TahunAjaran,
                    item.Semester,
                    item.MataKuliah,
                    item.Kelas,
                    item.DosenPengampu,
                    item.JadwalTatapMuka,
                    item.Ruangan,
                    item.Status,
                    item.KonsentrasiId,

                    can_edit = CanEditTatapMuka(item, userRole),
                    can_delete = CanDeleteTatapMuka(item, userRole),
                    can_sent = CanSentTatapMuka(item, userRole),
                    can_approve = CanApproveTatapMuka(item, userRole),
                    can_reject = CanRejectTatapMuka(item, userRole),
                    can_cancel = CanCancelInRiwayat(item, userRole),
                    need_ruangan_on_approve = userRole == RoleGA && (item.Status?.ToLower() ?? "") == StatusBelumDisetujuiGA

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
                    message = $"ERROR: {ex.Message}"
                });
            }

        }

        [HttpGet("Detail/{id}")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User tidak terautentikasi"
                    });
                }

                var data = await _repo.GetDetailTatapMukaAsync(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage,
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = MessageSuccess,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpPut("Edit")]
        [RequiresPermission(PermissionEdit)]
        public async Task<IActionResult> Edit([FromBody] EditTatapMukaDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(dto.TatapMukaId);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage
                    });
                }

                if (userRole == RoleDosen)
                {
                    if ((dataExist.Status?.ToLower() ?? "") != StatusDraft)
                        return BadRequest(new
                        {
                            success = false,
                            message = "Hanya dapat mengedit data dengan status Draft"
                        });
                }
                else
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Role tidak diizinkan untuk mengedit"
                    });
                }

                if (string.IsNullOrEmpty(dto.ModifBy))
                {
                    dto.ModifBy = username;
                }

                await _repo.EditTatapMukaAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil diperbarui",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpDelete("Delete/{id}")]
        [RequiresPermission("tatap_muka_tambahan.delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User tidak terautentikasi"
                    });
                }

                if (userRole != RoleDosen)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Hanya Dosen yang dapat menghapus pengajuan"
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(id);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Data Tatap Muka tidak ditemukan"
                    });
                }

                if ((dataExist.Status?.ToLower() ?? "") != StatusDraft)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Hanya dapat menghapus data dengan status Draft"
                    });
                }

                await _repo.DeleteTatapMukaAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil dihapus",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpPut("ApproveTatapMuka")]
        [RequiresPermission(PermissionApproveReject)]
        public async Task<IActionResult> Approve([FromBody] ApproveTatapMukaRequestDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(dto.TatapMukaId);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage
                    });
                }

                var currentStatus = dataExist.Status?.ToLower() ?? "";
                var approverRole = "";

                if (currentStatus == StatusBelumDisetujuiProdi)
                {
                    if (!(userRole == RoleProdi || userRole == RoleKaprodi))
                    {
                        return StatusCode(403, new
                        {
                            success = false,
                            message = "Hanya Prodi yang dapat menyetujui pada tahap ini"
                        });
                    }
                    approverRole = "Prodi";
                }
                else if (currentStatus == StatusBelumDisetujuiDAAK)
                {
                    if (userRole != RoleDAAK)
                    {
                        return StatusCode(403, new
                        {
                            success = false,
                            message = "Hanya DAAK yang dapat menyetujui pada tahap ini"
                        });
                    }
                    approverRole = "DAAK";
                }
                else if (currentStatus == StatusBelumDisetujuiGA)
                {
                    if (userRole != RoleGA)
                    {
                        return StatusCode(403, new
                        {
                            success = false,
                            message = "Hanya GA yang dapat menyetujui pada tahap ini"
                        });
                    }

                    if (string.IsNullOrEmpty(dto.RuangIdGA))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Ruangan wajib dipilih untuk approval GA"
                        });
                    }
                    approverRole = "GA";

                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Status tidak valid untuk approval: {dataExist.Status}"
                    });
                }

                dto.UserRole = approverRole;
                dto.ApprovedBy = username;
                await _repo.ApproveTatapMukaAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil disetujui",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpPut("CancelTatapMuka")]
        [RequiresPermission(PermissionEdit)]
        public async Task<IActionResult> Cancel([FromBody] CancelTatapMukaRequestDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(dto.TatapMukaId);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage
                    });
                }

                var currentStatus = dataExist.Status?.ToLower() ?? "";
                Console.WriteLine($"[DEBUG] Cancel check: status='{dataExist.Status}', trimmed+lower='{currentStatus}', userRole='{userRole}'");
                if (!CanCancelStatus(currentStatus, userRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = GetCancelErrorMessage(userRole)
                    });
                }

                dto.ModifBy = username;
                dto.IdRole = userRole;
                await _repo.CancelTatapMukaAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil dibatalkan",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }


        [HttpPut("RejectTatapMuka")]
        [RequiresPermission(PermissionApproveReject)]
        public async Task<IActionResult> Reject([FromBody] RejectTatapMukaRequestDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(dto.TatapMukaId);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage
                    });
                }

                var currentStatus = dataExist.Status?.ToLower() ?? "";
                if (!CanRejectStatus(currentStatus, userRole))
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = $"Role {userRole} tidak diizinkan untuk menolak pada status {currentStatus}"
                    });
                }

                dto.ModifBy = username;
                await _repo.RejectTatapMukaAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil ditolak",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpPut("SentTatapMuka")]
        [RequiresPermission(PermissionEdit)]
        public async Task<IActionResult> Sent([FromBody] SentTatapMukaRequestDto dto)
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = UnauthenticatedMessage
                    });
                }

                if (!(userRole == RoleDosen))
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Hanya Dosen yang dapat mengirim pengajuan"
                    });
                }

                var dataExist = await _repo.GetDetailTatapMukaAsync(dto.TatapMukaId);
                if (dataExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = DataNotFoundMessage
                    });
                }

                if ((dataExist.Status?.ToLower() ?? "") != StatusDraft)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Hanya data dengan status Draft yang dapat diajukan"
                    });
                }

                await _repo.SentTatapMukaAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Data Tatap Muka berhasil dikirim",
                    user_role_display = roleDisplay,
                    display_name = displayName
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}"
                });
            }
        }

        [HttpGet("GetUserInfo")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)] 
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
                    message = $"{ErrorMessage}: {ex.Message}"
                });
            }
        }

        [HttpGet("GetUserPermissions")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
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
                    message = $"{ErrorMessage}: {ex.Message}"
                });
            }
        }

        private static List<string> GetPermissionsByRole(string roleCode)
        {
            return roleCode?.ToUpper() switch
            {
                "ROL22" => new List<string> { PermissionView, PermissionApproveReject, PermissionEdit },
                "ROL27" => new List<string> { PermissionView, PermissionApproveReject, PermissionEdit },
                "ROL21" => new List<string> { PermissionView, PermissionApproveReject, PermissionEdit },
                "ROL20" => new List<string> { PermissionView, PermissionApproveReject, PermissionEdit },
                "ROL25" => new List<string> { "tatap_muka_tambahan.create", PermissionEdit, "tatap_muka_tambahan.delete", PermissionView },
                "ROL23" => new List<string> { PermissionView },
                _ => new List<string> { PermissionView }
            };
        }

        [HttpGet("TahunAkademikActive")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetActiveTA()
        {
            var data = await _repo.GetTahunAjaranAktifAsync();

            if (data == null)
                return NotFound(new { success = false, message = "Tidak ditemukan tahun akademik aktif." });

            return Ok(new { success = true, data });
        }

        [HttpGet("GetListTahunAkademik")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetListTahunAkademikAsync()
        {
            var data = await _repo.GetListTahunAkademikAsync();

            if (data == null)
                return NotFound(new { success = false, message = "Tidak ditemukan tahun akademik aktif." });

            return Ok(new { success = true, data });
        }

        [HttpGet("GetAllProdi")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetAllListProdi()
        {
            try
            {
                var data = await _repo.GetAllListProdiAsync();

                return Ok(new
                {
                    message = "SUCCESS",
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

        [HttpGet("GetProdiByUser")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetProdiByUser()
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = "User tidak terautentikasi" });
                }
                var data = await _repo.GetProdiByUserAsync(username);

                return Ok(new
                {
                    message = "true",
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

        [HttpGet("GetProdiByNIM")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetProdiByNIM()
        {
            try
            {
                var (username, userRole, displayName, roleDisplay) = GetUserInfo();

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = "User tidak terautentikasi" });
                }
                var data = await _repo.GetProdiByNIMAsync(username);

                return Ok(new
                {
                    message = "true",
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

        [HttpGet("GetListRuanganMahasiswa")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetListRuanganMahasiswa()
        {
            var data = await _repo.GetListRuanganMahasiswaAsync();
            return Ok(data);
        }

        [HttpGet("Prodi")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetKonsentrasiByDosen()
        {
            var (username, userRole, displayName, roleDisplay) = GetUserInfo();

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false, message = UnauthenticatedMessage });
            }

            var data = await _repo.GetListKonsentrasiByDosenAsync(username);
            return Ok(data);
        }

        [HttpGet("GetMataKuliah")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetMataKuliah(
            [FromQuery] MataKuliahRequestDto dto)
        {
            var (username, userRole, displayName, roleDisplay) = GetUserInfo();

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false, message = UnauthenticatedMessage });
            }

            var data = await _repo.GetListMatkulByDosenAsync(dto, username);

            if (data == null || data.Count == 0)
                return NotFound(new { message = "Data mata kuliah tidak ditemukan." });

            return Ok(new { message = "success", data });
        }

        [HttpGet("GetKelasByMatkulSec")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetKelasByMatkulSec(
            [FromQuery] KelasRequestDto dto)
        {
            var (username, userRole, displayName, roleDisplay) = GetUserInfo();

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false, message = UnauthenticatedMessage });
            }

            dto.Role = userRole;
            dto.Username = username;

            var data = await _repo.GetKelasByMatkulSecAsync(dto);
            return Ok(new { message = MessageSuccess, data });
        }

        [HttpGet("GetDosenByJadwalMatkul")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetDosenByJadwalMatkul(
            [FromQuery] NamaDosenRequestDto dto)
        {
            var data = await _repo.GetDosenByJadwalMatkulAsync(dto);
            return Ok(new { message = MessageSuccess, data });
        }

        [HttpGet("GetIdDosenByUser")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetIdDosenByUser()
        {
            var (username, userRole, displayName, roleDisplay) = GetUserInfo();

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { success = false, message = UnauthenticatedMessage });
            }

            var data = await _repo.GetIdDosenByUserAsync(username);
            return Ok(new { message = MessageSuccess, data });
        }

        [HttpGet("checkTatapMuka")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> CheckTatapMuka(
            [FromQuery] CheckBentrokRequestDto dto)
        {
            var data = await _repo.CheckTatapMukaAsync(dto);
            return Ok(new { message = MessageSuccess, data });
        }

        [HttpGet("checkJadwalBentrokTatapMuka")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> CheckJadwalBentrokTatapMuka(
            [FromQuery] BentrokJadwalTatapMukaRequestDto dto)
        {
            var data = await _repo.CheckJadwalBentrokTatapMukaAsync(dto);
            return Ok(new { message = MessageSuccess, data });
        }

        [HttpGet("GetGrup")]
        [RequiresPermission(PermissionView)]
        public async Task<IActionResult> GetGrup([FromQuery] GrupRequestDto dto)
        {
            var result = await _repo.GetListGrupAsync(dto);
            return Ok(new { message = MessageSuccess, data = result });
        }

        private static bool CanCancelStatus(string status, string userRole)
        {
            status = status?.Trim().ToLower() ?? "";
            userRole = userRole?.Trim().ToUpper() ?? "";

            if (userRole == RoleDosen)
            {
                var allowedStatus = new[]
                {
                    StatusBelumDisetujuiProdi,
                    StatusBelumDisetujuiDAAK,
                    StatusBelumDisetujuiGA
                }.Select(s => s.Trim().ToLower()); 

                return allowedStatus.Contains(status);
            }
            else if (userRole == RoleKaprodi || userRole == RoleProdi)
            {
                return status == StatusBelumDisetujuiDAAK.Trim().ToLower();
            }
            else if (userRole == RoleDAAK)
            {
                return status == StatusBelumDisetujuiGA.Trim().ToLower();
            }
            else if (userRole == RoleGA)
            {
                return status == StatusDiterima.Trim().ToLower();
            }
            return false;
        }

        private static string GetCancelErrorMessage(string userRole)
        {
            if (userRole == RoleDosen)
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
            }
            return "Role tidak diizinkan untuk membatalkan";
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
        private static bool CanEditTatapMuka(TatapMukaListItemDto item, string userRole)
        {
            return userRole == RoleDosen
                && (item.Status?.ToLower() ?? "") == StatusDraft;
        }
        private static bool CanDeleteTatapMuka(TatapMukaListItemDto item, string userRole)
        {
            return (userRole == RoleDosen)
                && (item.Status?.ToLower() ?? "") == StatusDraft;
        }
        private static bool CanSentTatapMuka(TatapMukaListItemDto item, string userRole)
        {
            if (item == null) return false;

            string role = userRole?.Trim().ToUpper() ?? "";
            string status = item.Status?.Trim().ToLower() ?? "";

            return role == RoleDosen && status == StatusDraft;
        }

        private static bool CanApproveTatapMuka(TatapMukaListItemDto item, string userRole)
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

        private static bool CanRejectTatapMuka(TatapMukaListItemDto item, string userRole)
        {
            return CanApproveTatapMuka(item, userRole);
        }
        private static bool CanCancelInRiwayat(TatapMukaListItemDto item, string userRole)
        {
            var status = item.Status?.Trim().ToLower() ?? "";
            userRole = userRole?.Trim().ToUpper() ?? "";

            if (userRole == RoleDosen)
            {
                var allowedStatus = new[]
                {
                    StatusBelumDisetujuiProdi,
                    StatusBelumDisetujuiDAAK,
                    StatusBelumDisetujuiGA
                }.Select(s => s.Trim().ToLower());

                return allowedStatus.Contains(status);
            }

            if ((userRole == RoleProdi || userRole == RoleKaprodi)
                && status == StatusBelumDisetujuiDAAK)
            {
                return true;
            }

            if (userRole == RoleDAAK && status == StatusBelumDisetujuiGA)
            {
                return true;
            }

            if (userRole == RoleGA && status == StatusDiterima)
            {
                return true;
            }

            return false;
        }

    }
}