using astratech_apps_backend.DTOs.PendaftaranWisuda;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PendaftaranWisudaController : ControllerBase
    {
        private const string CLAIM_NAMAAKUN = "namaakun";

        private readonly IPendaftaranWisudaRepository _repo;

        public PendaftaranWisudaController(IPendaftaranWisudaRepository repo)
        {
            _repo = repo;
        }
        
        [HttpPost("CreatePendaftaranWisuda")]
        [RequiresPermission("pendaftaran_wisuda.create")]
        public async Task<IActionResult> Create([FromBody] CreatePendaftaranWisudaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? createdBy = User.FindFirstValue(CLAIM_NAMAAKUN);
            if (string.IsNullOrWhiteSpace(createdBy))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest("Invalid input");

            var result = await _repo.CreatePendaftaranWisudaAsync(sanitizedDto, createdBy);

            return result == "SUCCESS"
                ? Ok(new { message = "Pendaftaran wisuda berhasil dibuat." })
                : BadRequest(new { message = "Pendaftaran wisuda gagal dibuat." });
        }

        [HttpGet("TahunAkademik")]
        public async Task<IActionResult> GetListTahunAkademik()
        {
            var data = await _repo.GetListTahunAkademikAsync();
            return Ok(data);
        }

        [HttpGet("GetAllPendaftaranWisuda")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPendaftaranWisudaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var response = new GetAllPendaftaranWisudaResponse
            {
                Data = list.Select(m => new PendaftaranWisudaDto
                {
                    Id = m.Id,
                    TahunAkademik = m.TahunAkademik,
                    TanggalMulaiPendaftaran = m.TanggalMulaiPendaftaran,
                    TanggalAkhirPendaftaran = m.TanggalAkhirPendaftaran,
                    KuotaUndanganTambahan = m.KuotaUndanganTambahan
                }).ToList(),
                TotalData = totalData,
                TotalHalaman = (int)Math.Ceiling((double)totalData / dto.PageSize)
            };

            return Ok(response);
        }

        [HttpGet("GetAll/{id}")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetById(int id)
        {
            var (list, _) = await _repo.GetByIdAsync(id);

            if (!list.Any())
                return NotFound(new { message = "Data pendaftaran wisuda tidak ditemukan." });

            return Ok(list.First());
        }

        [HttpPut("UpdatePendaftaranWisuda")]
        [RequiresPermission("pendaftaran_wisuda.edit")]
        public async Task<IActionResult> Update([FromBody] UpdatePendaftaranWisudaRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? updatedBy = User.FindFirstValue(CLAIM_NAMAAKUN);
            if (string.IsNullOrWhiteSpace(updatedBy))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest("Invalid input");

            var success = await _repo.UpdateAsync(sanitizedDto, updatedBy);

            return success
                ? Ok(new { message = "Pendaftaran wisuda berhasil diupdate." })
                : NotFound(new { message = "Data wisuda tidak ditemukan." });
        }

        [HttpGet("Pendaftaran/detail/{id}")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetDetailPeserta(
            int id,
            [FromQuery] GetDetailPesertaWisudaRequest? req)
        {
            var (headList, _) = await _repo.GetByIdAsync(id);

            var head = headList.FirstOrDefault();
            if (head == null)
                return NotFound(new { message = "Data wisuda tidak ditemukan." });

            req ??= new GetDetailPesertaWisudaRequest();

            var pageSize = req.PageSize <= 0 ? 10 : req.PageSize;
            req.PageSize = pageSize;

            var (data, totalData) = await _repo.GetDetailPesertaAsync(id, req);

            return Ok(new GetDetailPendaftaranWisudaResponse
            {
                Head = new PendaftaranWisudaDto
                {
                    Id = head.Id,
                    TahunAkademik = head.TahunAkademik,
                    TanggalMulaiPendaftaran = head.TanggalMulaiPendaftaran,
                    TanggalAkhirPendaftaran = head.TanggalAkhirPendaftaran,
                    KuotaUndanganTambahan = head.KuotaUndanganTambahan
                },
                Data = data,
                TotalData = totalData,
                TotalHalaman = (int)Math.Ceiling((double)totalData / pageSize)
            });
        }

        [HttpGet("Pendaftar/detail")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetWisudaDetailAdmin([FromQuery] string wisId)
        {
            if (string.IsNullOrEmpty(wisId))
                return BadRequest("wisId wajib diisi");

            var data = await _repo.GetDetailWisudaAsync(wisId);

            return data == null
                ? NotFound("Detail wisuda tidak ditemukan")
                : Ok(data);
        }

        [HttpGet("ExportPendaftaranWisuda/{id}")]
        [RequiresPermission("pendaftaran_wisuda.export")]
        public async Task<IActionResult> ExportPesertaWisuda(int id)
        {
            var data = await _repo.ExportPesertaWisudaAsync(id);

            if (data == null || !data.Any())
                return NotFound(new { message = "Data peserta wisuda tidak ditemukan." });

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Peserta Wisuda");

            string[] headers =
            {
                "Nomor Pendaftaran",
                "Tanggal Daftar",
                "Prodi",
                "NIM",
                "Nama Mahasiswa",
                "Kelengkapan Berkas",
                "Status Pembayaran",
                "Tanggal Bayar",
                "Undangan Tambahan"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = item.NomorPendaftaran ?? "";

                if (item.TanggalDaftar.HasValue)
                {
                    ws.Cells[row, 2].Value = item.TanggalDaftar.Value;
                    ws.Cells[row, 2].Style.Numberformat.Format = "dd MMMM yyyy";
                }

                ws.Cells[row, 3].Value = item.Prodi ?? "";
                ws.Cells[row, 4].Value = item.NIM ?? "";
                ws.Cells[row, 5].Value = item.NamaMahasiswa ?? "";
                ws.Cells[row, 6].Value = item.KelengkapanBerkas ?? "";
                ws.Cells[row, 7].Value = item.StatusPembayaran ?? "";

                if (item.TanggalBayar.HasValue)
                {
                    ws.Cells[row, 8].Value = item.TanggalBayar.Value;
                    ws.Cells[row, 8].Style.Numberformat.Format = "dd MMMM yyyy";
                }

                ws.Cells[row, 9].Value = item.UndanganTambahan;
                row++;
            }

            ws.Cells.AutoFitColumns();

            return File(
                package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Export_Peserta_Wisuda_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpPost("CreateWisudaMahasiswa")]
        [RequiresPermission("pendaftaran_wisuda.create")]
        public async Task<IActionResult> CreateWisuda([FromBody] CreateWisudaRequest dto)
        {
            var username = User.FindFirstValue(CLAIM_NAMAAKUN);

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var newId = await _repo.CreateWisudaMahasiswaAsync(dto, username);

            return Ok(new
            {
                message = "Pendaftaran wisuda berhasil disimpan",
                id = newId
            });
        }

        [HttpGet("GetDataWisudaMahasiswa")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetMyWisuda()
        {
            var mhsId = User.FindFirstValue(CLAIM_NAMAAKUN);

            if (string.IsNullOrEmpty(mhsId))
                return Unauthorized();

            var data = await _repo.GetByMahasiswaAsync(mhsId);

            if (data == null)
                return NotFound(new { message = "Mahasiswa belum mendaftar wisuda" });

            return Ok(data);
        }

        [HttpGet("DetailWisudaMahasiswa")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetWisudaDetailMahasiswa([FromQuery] string wisId)
        {
            if (string.IsNullOrEmpty(wisId))
                return BadRequest("wisId wajib diisi");

            var data = await _repo.GetDetailWisudaAsync(wisId);

            return data == null
                ? NotFound("Detail wisuda mahasiswa tidak ditemukan")
                : Ok(data);
        }

        [HttpPost("ApproveWisudaMahasiswa")]
        [RequiresPermission("pendaftaran_wisuda.approve_reject")]
        public async Task<IActionResult> ApproveWisuda([FromBody] ApproveWisudaRequest req)
        {
            if (string.IsNullOrEmpty(req.WisudaId))
                return BadRequest(new { success = false, message = "WisudaId wajib diisi" });

            var user = User.FindFirstValue(CLAIM_NAMAAKUN);
            if (string.IsNullOrEmpty(user))
                return Unauthorized();

            await _repo.ApproveAsync(
                req.WisudaId,
                user,
                req.Role,
                req.TanggalBayar
            );

            var message = req.Role == "DAKAP"
                ? "Pembayaran wisuda berhasil, QR dapat diakses"
                : "Berkas wisuda berhasil diverifikasi";

            return Ok(new { success = true, message });
        }

        [HttpGet("QrCodeWisudaMahasiswa")]
        [RequiresPermission("pendaftaran_wisuda.view")]
        public async Task<IActionResult> GetQrWisuda()
        {
            var mhsId = User.FindFirstValue(CLAIM_NAMAAKUN);
            if (string.IsNullOrEmpty(mhsId))
                return Unauthorized();

            var guid = Guid.NewGuid().ToString();
            var (status, fileName) = await _repo.GetQrInfoAsync(mhsId, guid);

            if (status == "NOT_ELIGIBLE")
                return BadRequest("Wisuda belum lunas");

            var payload = $"QRWISUDA#{mhsId}#{DateTime.UtcNow:O}";
            var base64 = ""; // TODO: Implement QR generation when QrGeneratorService is available

            return Ok(new QrWisudaResponse
            {
                FileName = fileName,
                Base64 = base64
            });
        }
    }
}
