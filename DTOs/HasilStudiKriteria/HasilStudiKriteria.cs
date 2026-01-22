using System.ComponentModel.DataAnnotations;

namespace astratech_apps_backend.DTOs.HasilStudiKriteria
{
    public class CreateKriteriaRequest
    {
        [Required(ErrorMessage = "Mata kuliah harus dipilih.")]
        public string MataKuliahId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tahun ajaran harus dipilih.")]
        public string TahunAjaran { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester harus dipilih.")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipe kriteria harus dipilih.")]
        public string TipeKriteria { get; set; } = string.Empty;

        public List<KriteriaDetailRequest> KriteriaDetails { get; set; } = new List<KriteriaDetailRequest>();
    }

    public class KriteriaDetailRequest
    {
        public string? KriteriaId { get; set; }

        [Required(ErrorMessage = "Nama kriteria harus diisi.")]
        [StringLength(100)]
        public string Kriteria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Persentase harus diisi.")]
        [Range(0, 100, ErrorMessage = "Persentase harus antara 0-100.")]
        public int Persentase { get; set; }
    }

    public class UpdateKriteriaRequest
    {
        [Required(ErrorMessage = "ID kriteria harus diisi.")]
        public string KriteriaId { get; set; } = string.Empty;

        public List<KriteriaDetailRequest> KriteriaDetails { get; set; } = new List<KriteriaDetailRequest>();
    }

    public class GetAllKriteriaRequest
    {
        public string SearchKeyword { get; set; } = string.Empty;
        public string KonsentrasiId { get; set; } = string.Empty;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Urut { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class KriteriaDto
    {
        public string Id { get; set; } = string.Empty;
        public long RowNumber { get; set; } = 0;
        public string TahunAjaran { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string MataKuliah { get; set; } = string.Empty;
        public string MataKuliahId { get; set; } = string.Empty;
        public string Dosen { get; set; } = string.Empty;
        public string DosenId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AlasanTolak { get; set; } = string.Empty;
        public string TipeKriteria { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
        public List<KriteriaDetailDto> Details { get; set; } = new List<KriteriaDetailDto>();
        public string Konsentrasi { get; set; } = string.Empty;

        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanSubmit { get; set; }
        public bool CanApprove { get; set; }
        public bool CanViewDetail { get; set; } = true;
    }

    public class KriteriaDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string Kriteria { get; set; } = string.Empty;
        public int Persentase { get; set; }
    }

    public class UserPermissionsDto
    {
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanSubmit { get; set; }
        public bool CanApprove { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public class GetAllKriteriaResponse
    {
        public List<KriteriaDto> Data { get; set; } = new List<KriteriaDto>();
        public int TotalData { get; set; }
        public int TotalHalaman { get; set; }
        public UserPermissionsDto UserPermissions { get; set; } = new UserPermissionsDto();
    }

    public class ApproveRejectRequest
    {
        [Required(ErrorMessage = "ID kriteria harus diisi.")]
        public string KriteriaId { get; set; } = string.Empty;

        public string Alasan { get; set; } = string.Empty;
    }

    public class KirimKriteriaRequest
    {
        [Required(ErrorMessage = "ID kriteria harus diisi.")]
        public string KriteriaId { get; set; } = string.Empty;
    }

    public class LookupDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
    }

    public class ListResponse
    {
        public List<LookupDto> Data { get; set; } = new List<LookupDto>();
    }

    public class TemplateKriteriaRequest
    {
        [Required(ErrorMessage = "Tipe template harus dipilih.")]
        public string Tipe { get; set; } = string.Empty;

        public string? MataKuliahId { get; set; }
        public string? TahunAjaran { get; set; }
        public string? Semester { get; set; }
    }

    public class TemplateKriteriaResponse
    {
        public string Tipe { get; set; } = string.Empty;
        public List<KriteriaDetailDto> KriteriaDetails { get; set; } = new List<KriteriaDetailDto>();
    }

    public class KriteriaDetailResponseDto
    {
        public KriteriaDto Kriteria { get; set; } = new KriteriaDto();
        public UserPermissionsDto UserPermissions { get; set; } = new UserPermissionsDto();
    }
}