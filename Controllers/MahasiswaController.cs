using astratech_apps_backend.DTOs.Mahasiswa;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MahasiswaController(IMahasiswaRepository repo) : ControllerBase
    {
        private readonly IMahasiswaRepository _repo = repo;
        private const string ClaimNamaAkun = "namaakun";
        private const string ClaimIdRole = "idrole";
        private const string ClaimRoleId = "roleid";
        private const string MessageSuccess = "Success";
        private const string MessageInvalidIdMahasiswa = "ID Mahasiswa tidak valid";


        [HttpGet("GetAllMahasiswa")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetAllMahasiswa([FromQuery] GetAllMahasiswaRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            sanitizedDto.RoleId = User.FindFirst(ClaimIdRole)?.Value ?? "";
            sanitizedDto.Username = User.FindFirst(ClaimNamaAkun)?.Value ?? "";

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto);

            var dataDTO = list.Select(mahasiswa => new MahasiswaDto
            {
                NIM = mahasiswa.IdMahasiswa ?? "",
                Nama = mahasiswa.Nama ?? "",
                Prodi = mahasiswa.Konsentrasi ?? "",
                Angkatan = mahasiswa.Angkatan ?? "",
                JenisMahasiswa = mahasiswa.JenisMahasiswa ?? "",
                Status = mahasiswa.StatusKuliah ?? "",
                RFID = mahasiswa.RFID ?? ""
            }).ToList();

            var response = new GetAllMahasiswaResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = ((totalData - 1) / (sanitizedDto.PageSize ?? 10)) + 1
            };

            return Ok(response);
        }

        [HttpGet("DetailMahasiswa/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetByIdAsync(string mahasiswaId)
        {
            var dataDto = await _repo.GetByIdAsync(mahasiswaId);
            if (dataDto == null)
            {
                return NotFound(new { message = "Data mahasiswa tidak ditemukan." });
            }
            return Ok(dataDto);
        }

        [HttpGet("KegiatanPrestasi/{idMahasiswa}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetKegiatanPrestasi(string idMahasiswa)
        {
            var roleId = User.FindFirstValue(ClaimIdRole);
            if (string.IsNullOrEmpty(roleId))
                return Unauthorized();

            var request = new GetLaporanKegiatanRequest
            {
                Search = "",
                Status = "Disetujui",
                OrderBy = "lke_tanggal_kegiatan asc",
                Tahun = "",
                KegiatanId = "",
                IdMahasiswa = idMahasiswa,
                Role = roleId,
                Halaman = 1,
                Limit = 10
            };

            var result = await _repo.GetLaporanKegiatanMahasiswaAsync(request);

            if (result == null)
            {
                return Ok(new
                {
                    data = new List<object>(),
                    totalData = 0,
                    error = false,
                    message = "Tidak ada data kegiatan/prestasi"
                });
            }

            var kegiatanList = result.Select((mahasiswa, index) => new
            {
                no = index + 1,
                id = mahasiswa.LaporanId ?? $"keg-{index}",
                namaKegiatan = mahasiswa.LaporanNama ?? "",
                tanggalPelaksanaan = mahasiswa.LaporanTanggalKegiatan ?? "",
                tingkat = mahasiswa.LaporanTingkat ?? "1",
                poin = mahasiswa.LaporanPoint,
                status = mahasiswa.LaporanStatus ?? "",
                tahunAjaran = mahasiswa.LaporanTahunAjaran ?? "",
                kegId = mahasiswa.KegiatanId
            }).ToList();

            return Ok(new
            {
                data = kegiatanList,
                totalData = kegiatanList.Count,
                error = false,
                message = MessageSuccess
            });
        }


        [HttpPut("EditMahasiswa")]
        [RequiresPermission("mahasiswa.edit")]
        public async Task<IActionResult> UpdateMahasiswa([FromBody] UpdateDataMahasiswaRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.FindFirstValue(ClaimNamaAkun);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var success = await _repo.UpdateAsync(sanitizedDto, username);

            if (!success) return NotFound(new { message = "Data mahasiswa tidak ditemukan." });
            return Ok(new { message = "SUCCESS" });
        }


        [HttpGet("GetAllProdi")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetAllProdi()
        {
            var username = User.FindFirstValue(ClaimNamaAkun);
            var roleId = User.FindFirstValue(ClaimIdRole);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleId))
            {
                return Unauthorized();
            }

            var result = await _repo.GetListKonsentrasiAsync(username, roleId);

            var prodiList = result.Select(mahasiswa => new
            {
                id = mahasiswa.KonsentrasiId,
                namaProdi = mahasiswa.KonsentrasiNama
            }).ToList();

            return Ok(new
            {
                data = prodiList,
                error = false,
                message = MessageSuccess
            });
        }


        [HttpGet("GetAngkatan")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetAngkatan()
        {
            var result = await _repo.GetListAngkatanAktifAsync();

            var angkatanList = result.Select(x => new
            {
                angkatan = x
            }).ToList();

            return Ok(new
            {
                data = angkatanList,
                error = false,
                message = MessageSuccess
            });
        }


        [HttpGet("GetJenisMahasiswa")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetJenisMahasiswa()
        {
            await Task.CompletedTask;

            return Ok(new JenisMahasiswaResponse
            {
                Data = new List<JenisMahasiswaItemDto>
                {
                    new() { Id = "1", NamaJenis = "Reguler" },
                    new() { Id = "2", NamaJenis = "Beasiswa" }
                },
                Error = false,
                Message = MessageSuccess
            });
        }

        [HttpPost("CreateRFID")]
        [RequiresPermission("mahasiswa.create")]
        public async Task<IActionResult> CreateRFID(
            [FromBody] CreateRfidMahasiswaRequest dto,
            [FromQuery] string mahasiswaId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue(ClaimNamaAkun);
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
                return BadRequest(new { message = "Invalid data" });

            var existingRfid = await _repo.CreateRfidMahasiswaAsync(
                mahasiswaId,
                dto.NomorRFID,
                username
            );

            if (!string.IsNullOrEmpty(existingRfid))
            {
                return BadRequest(new
                {
                    message = $"RFID sudah digunakan oleh mahasiswa: {existingRfid}"
                });
            }

            return Ok(new { message = "SUCCESS" });
        }

        [HttpPost("SetStatusRFID/{rmaId}")]
        [RequiresPermission("mahasiswa.edit")]
        public async Task<IActionResult> SetStatusRFID(string rmaId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest(new { message = "Status wajib diisi." });

            var resultStatus = await _repo.SetStatusRfidAsync(status, rmaId);

            if (resultStatus == null)
                return NotFound(new { message = "Data RFID tidak ditemukan." });

            if (!resultStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Gagal mengubah status RFID." });

            var resultMsg = status == "Aktif"
                ? "RFID berhasil diaktifkan."
                : "RFID berhasil dinonaktifkan.";

            return Ok(new { message = resultMsg });
        }

        [HttpGet("ExportMahasiswa")]
        [RequiresPermission("mahasiswa.export")]
        public async Task<IActionResult> ExportMahasiswa([FromQuery] ExportMahasiswaRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue(ClaimNamaAkun);
            var roleId = User.FindFirstValue(ClaimRoleId);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
            {
                return BadRequest(new { message = "Data request tidak valid" });
            }

            var fileBytes = await _repo.ExportMahasiswaExcelAsync(
                sanitizedDto,
                roleId ?? string.Empty,
                username
            );

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound(new { message = "Tidak ada data yang ditemukan untuk export" });
            }

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DataMahasiswa_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }


        [HttpGet("ExportRFIDMahasiswa")]
        [RequiresPermission("mahasiswa.import")]
        public async Task<IActionResult> ExportRFID([FromQuery] ImportRfidRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null)
            {
                return BadRequest(new { message = "Data request tidak valid" });
            }

            var fileBytes = await _repo.ExportMahasiswaRFIDExcelAsync(
                sanitizedDto.KonsentrasiId ?? string.Empty,
                sanitizedDto.Urut ?? "NIM asc"
            );

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound(new { message = "Tidak ada data RFID yang ditemukan untuk export" });
            }

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DataRFID_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }


        [HttpGet("GetRiwayatKuliah/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetRiwayatKuliah(string mahasiswaId)
        {
            var result = await _repo.GetDataRiwayatKuliahAsync(mahasiswaId);
            return Ok(new { data = result });
        }

        [HttpGet("GetRiwayatStudi/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetRiwayatStudi(string mahasiswaId)
        {
            if (string.IsNullOrWhiteSpace(mahasiswaId))
            {
                return BadRequest(new { error = true, message = "Bentuk Payload Tidak Valid!" });
            }

            var result = await _repo.GetDataRiwayatStudiAsync(mahasiswaId);
            return Ok(new { data = result });

        }

        [HttpGet("GetDataRFID/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataRFID(string mahasiswaId)
        {
            var result = await _repo.GetDataRfidAsync(mahasiswaId);

            return Ok(new
            {
                data = result.Select(Rfid => new
                {
                    Rfid.RfidMahasiswaId,
                    Rfid.IdMahasiswa,
                    Rfid.RfidMahasiswa,
                    Rfid.RfidMahasiswaStatus,
                    Rfid.RfidMahasiswaCreatedBy,
                    Rfid.RfidMahasiswaCreatedDate
                }).ToList()
            });

        }

        [HttpGet("GetDataRiwayatStatusKuliah/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataRiwayatStatusKuliah(string mahasiswaId)
        {
            if (string.IsNullOrEmpty(mahasiswaId))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Payload Tidak Valid!"
                });
            }

            var list = await _repo.GetDataRiwayatKuliahAsync(mahasiswaId);

            var dataList = list.Select((item, index) => new
            {
                No = index + 1,
                Semester = item.Semester,
                Status = item.Status,
                SKS = item.Sks
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Data riwayat status kuliah berhasil diambil",
                data = new
                {
                    list = dataList,
                    totalData = dataList.Count
                }
            });

        }

        [HttpGet("GetDataPerformaIP/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataPerformaIP(string mahasiswaId)
        {
            if (string.IsNullOrWhiteSpace(mahasiswaId))
            {
                return BadRequest(new { message = MessageInvalidIdMahasiswa });
            }

            var data = await _repo.GetDataPerformaIpAsync(mahasiswaId);

            if (!data.Any())
            {
                return Ok(new { data = new List<object>() });
            }

            var result = data.Select((item, index) => new
            {
                no = index + 1,
                semester = item.Semester,
                ip = item.Ip,
                ipk = item.Ipk,
                publish = item.Publish
            });

            return Ok(new { data = result });
        }

        [HttpGet("GetDataPerformaKehadiran/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataPerformaKehadiran(string mahasiswaId)
        {
            if (string.IsNullOrWhiteSpace(mahasiswaId))
            {
                return BadRequest(new { message = MessageInvalidIdMahasiswa });
            }

            var data = await _repo.GetDataPerformaKehadiranAsync(mahasiswaId);

            if (!data.Any())
            {
                return Ok(new { data = new List<object>() });
            }

            var result = data.Select((item, index) => new
            {
                no = index + 1,
                semester = item.Semester,
                persentase = item.Persentase
            });

            return Ok(new { data = result });
        }

        [HttpGet("GetDataPerformaJamMinus/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataPerformaJamMinus(string mahasiswaId)
        {
            if (string.IsNullOrWhiteSpace(mahasiswaId))
            {
                return BadRequest(new { message = MessageInvalidIdMahasiswa });
            }

            var data = await _repo.GetDataPerformaJamMinusAsync(mahasiswaId);

            if (!data.Any())
            {
                return Ok(new { data = new List<object>() });
            }

            var result = data.Select((item, index) => new
            {
                no = index + 1,
                semester = item.Semester,
                jamMinus = item.Minus
            });

            return Ok(new { data = result });
        }

        [HttpGet("GetDataPerformaPelanggaran/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetDataPerformaPelanggaran(string mahasiswaId)
        {
            if (string.IsNullOrWhiteSpace(mahasiswaId))
            {
                return BadRequest(new { message = MessageInvalidIdMahasiswa });
            }

            var data = await _repo.GetDataPerformaPelanggaranAsync(mahasiswaId);

            if (!data.Any())
            {
                return Ok(new { data = new List<object>() });
            }

            var result = data.Select((item, index) => new
            {
                no = index + 1,
                semester = item.Semester,
                total = item.Total,
                teguran = item.Teguran,
                sp1 = item.Sp1,
                sp2 = item.Sp2,
                sp3 = item.Sp3
            });

            return Ok(new { data = result });
        }

        [HttpGet("GetProvinsi")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetProvinsi()
        {
            var result = await _repo.GetListProvinsiAsync();

            return Ok(new
            {
                data = result,
                error = false,
                message = MessageSuccess
            });
        }

        [HttpGet("GetKabupaten")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetKabupaten([FromQuery] string provinsi)
        {
            if (string.IsNullOrWhiteSpace(provinsi))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Provinsi wajib diisi"
                });
            }

            var result = await _repo.GetListKabupatenAsync(provinsi.ToUpper());

            return Ok(new
            {
                data = result,
                error = false,
                message = MessageSuccess
            });
        }

        [HttpGet("GetKecamatan")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetKecamatan([FromQuery] string provinsi, [FromQuery] string kabupaten)
        {
            if (string.IsNullOrWhiteSpace(provinsi) || string.IsNullOrWhiteSpace(kabupaten))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Provinsi dan Kabupaten wajib diisi"
                });
            }

            var result = await _repo.GetListKecamatanAsync(
                provinsi.ToUpper(),
                kabupaten.ToUpper()
            );

            return Ok(new
            {
                data = result,
                error = false,
                message = MessageSuccess
            });
        }

        [HttpGet("GetKelurahan")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetKelurahan([FromQuery] string provinsi, [FromQuery] string kabupaten, [FromQuery] string kecamatan)
        {
            if (string.IsNullOrWhiteSpace(provinsi)
            || string.IsNullOrWhiteSpace(kabupaten)
            || string.IsNullOrWhiteSpace(kecamatan))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Provinsi, Kabupaten, dan Kecamatan wajib diisi"
                });
            }

            var result = await _repo.GetListKelurahanAsync(
                provinsi.ToUpper(),
                kabupaten.ToUpper(),
                kecamatan.ToUpper()
            );

            return Ok(new
            {
                data = result,
                error = false,
                message = MessageSuccess
            });
        }

        [HttpGet("GetFotoMahasiswa/{mahasiswaId}")]
        [RequiresPermission("mahasiswa.view")]
        public async Task<IActionResult> GetFotoMahasiswa(string mahasiswaId)
        {
            string? namaFileFoto = await _repo.GetNamaFileFotoAsync(mahasiswaId);

            if (string.IsNullOrEmpty(namaFileFoto) || namaFileFoto == "-")
            {
                return Ok(new
                {
                    message = "Foto tidak ditemukan di database",
                    mhsId = mahasiswaId,
                    status = "NO_PHOTO_IN_DB"
                });
            }

            return Ok(new
            {
                message = "Foto tersedia di database",
                mhsId = mahasiswaId,
                fileName = namaFileFoto,
                status = "PHOTO_EXISTS_IN_DB"
            });
        }

        [HttpPost("CreateRekening")]
        [RequiresPermission("mahasiswa.edit")]
        public async Task<IActionResult> CreateRekening([FromBody] CreateRekeningRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.IdMahasiswa))
                return BadRequest(new { error = true, message = "ID Mahasiswa wajib diisi" });

            var sanitized = SanitizerHelper.EncodeObject(dto);
            if (sanitized == null)
                return BadRequest(new { error = true, message = "Data tidak valid" });

            var username = User.FindFirstValue(ClaimNamaAkun);
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var success = await _repo.CreateRekeningAsync(
                sanitized.IdMahasiswa,
                sanitized.AtasNama ?? "",
                sanitized.NoRekening ?? "",
                sanitized.NamaBank ?? "",
                username
            );

            if (!success)
                return StatusCode(500, new { error = true, message = "Gagal menyimpan rekening" });

            return Ok(new { error = false, message = "SUCCESS" });
        }
    }
}