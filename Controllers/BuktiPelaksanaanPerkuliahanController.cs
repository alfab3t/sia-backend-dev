using astratech_apps_backend.DTOs.BuktiPelaksanaanPerkuliahan;
using astratech_apps_backend.Helpers;
using astratech_apps_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace astratech_apps_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuktiPelaksanaanPerkuliahanController(IBuktiPelaksanaanPerkuliahanRepository repo) : ControllerBase
    {
        private readonly IBuktiPelaksanaanPerkuliahanRepository _repo = repo;

        [HttpGet("GetAllBuktiPelaksanaanPerkuliahan")]
        [RequiresPermission("bukti_pelaksanaan_perkuliahan.view")]
        public async Task<IActionResult> GetAllBuktiPelaksanaan([FromQuery] GetAllBuktiPelaksanaanPerkuliahanRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirstValue("namaakun");
            var role = User.FindFirstValue("idrole") 
                       ?? User.FindFirstValue("role") 
                       ?? User.FindFirstValue("peran");

            Console.WriteLine($"[DEBUG] User: {username}, Role Terbaca: {role}");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var sanitizedDto = SanitizerHelper.EncodeObject(dto);
            if (sanitizedDto == null) return NotFound();

            var (list, totalData) = await _repo.GetAllAsync(sanitizedDto, username, role!);

            var dataDTO = list.Select(m => new BuktiPelaksanaanPerkuliahanDto
            {
                RpsId = m.RpsId,
                KodeNamaDosen = m.KodeNamaDosen,
                ProgramStudi = m.ProgramStudi,
                TahunAkademik = m.TahunAkademik,
                Semester = m.Semester,
                MataKuliah = m.MataKuliah,
                Kelas = m.Kelas,
                JumlahPertemuan = m.JumlahPertemuan,
                AktualPertemuan = m.AktualPertemuan,
                PersentasePertemuan = m.PersentasePertemuan
            }).ToList();

            var response = new GetAllBuktiPelaksanaanPerkuliahanResponse
            {
                Data = dataDTO,
                TotalData = totalData,
                TotalHalaman = (int)Math.Ceiling((double)totalData / dto.PageSize)
            };

            return Ok(response);
        }

        [HttpGet("GetListKonsentrasi")]
        public async Task<IActionResult> GetListKonsentrasi()
        {
            var username = User.FindFirstValue("namaakun");
            var role = User.FindFirstValue("peran");

            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _repo.GetListKonsentrasiAsync(username, role!);
            return Ok(result);
        }

        [HttpGet("GetTahunAkademikAktif")]
        public async Task<IActionResult> GetTahunAkademikAktif()
        {
            var result = await _repo.GetTahunAkademikAktifAsync();
            if (result == null) return NotFound("Tahun akademik aktif tidak ditemukan");
            return Ok(result);
        }

        [HttpGet("GetListSemester")]
        public async Task<IActionResult> GetListSemester()
        {
            var result = await _repo.GetListSemesterAsync();
            return Ok(result);
        }
    }
}