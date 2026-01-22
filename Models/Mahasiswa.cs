namespace astratech_apps_backend.Models
{
    public class Mahasiswa
    {
        public string? NoPendaftaran { get; set; }
        public string? Nama { get; set; }
        public string? Konsentrasi { get; set; }
        public string? RFID { get; set; }
        public long? RowNumber { get; set; } = 0;
        public string? TempatLahir { get; set; }
        public string? TanggalLahir { get; set; }
        public string? JenisKelamin { get; set; }
        public string? Alamat { get; set; }
        public string? KodePos { get; set; }
        public string? HP { get; set; }
        public string? Email { get; set; }
        public string? TanggalMasuk { get; set; }
        public string? TanggalLulus { get; set; }
        public string? Angkatan { get; set; }
        public string? StatusKuliah { get; set; }

        public string? NamaAyah { get; set; }
        public string? HPAyah { get; set; }
        public string? StatusAyah { get; set; }
        public string? AlamatAyah { get; set; }
        public string? KodePosAyah { get; set; }
        public string? NikAyah { get; set; } 
        public string? KewarganegaraanAyah { get; set; } 
        public string? AgamaAyah { get; set; } 
        public string? PendidikanAyah { get; set; } 
        public string? PekerjaanAyah { get; set; } 
        public string? PerusahaanAyah { get; set; } 
        public string? AlamatPerusahaanAyah { get; set; } 
        public string? PenghasilanAyah { get; set; } 

        public string? NamaIbu { get; set; }
        public string? HPIbu { get; set; }
        public string? StatusIbu { get; set; }
        public string? AlamatIbu { get; set; }
        public string? KodePosIbu { get; set; }
        public string? NikIbu { get; set; } 
        public string? KewarganegaraanIbu { get; set; } 
        public string? AgamaIbu { get; set; } 
        public string? PendidikanIbu { get; set; } 
        public string? PekerjaanIbu { get; set; } 
        public string? PerusahaanIbu { get; set; } 
        public string? AlamatPerusahaanIbu { get; set; } 
        public string? PenghasilanIbu { get; set; } 

        public string? NamaWali { get; set; }
        public string? HPWali { get; set; }
        public string? StatusWali { get; set; }
        public string? AlamatWali { get; set; }
        public string? KodePosWali { get; set; }
        public string? NikWali { get; set; } 
        public string? KewarganegaraanWali { get; set; } 
        public string? AgamaWali { get; set; } 
        public string? PendidikanWali { get; set; } 
        public string? PekerjaanWali { get; set; } 
        public string? PerusahaanWali { get; set; } 
        public string? AlamatPerusahaanWali { get; set; } 
        public string? PenghasilanWali { get; set; }

        public short? KonsentrasiId { get; set; }
        public string? JenisMahasiswa { get; set; }
        public string? JalurDaftarUlang { get; set; }
        public string? AtasNamaRekening { get; set; }
        public string? Norek { get; set; }
        public string? NamaBank { get; set; }
        public string? Nisn { get; set; }
        public string? KelurahanId { get; set; }
        public string? Nik { get; set; }

        public string? Ttl { get; set; } 
        public string? Agama { get; set; }
        public string? Kewarganegaraan { get; set; } 
        public string? GolonganDarah { get; set; }
        public string? StatusKawin { get; set; }
        public int? UkuranSepatu { get; set; }
        public string? UkuranKemeja { get; set; } 
        public int? TinggiBadan { get; set; } 
        public int? BeratBadan { get; set; } 

        public string? Sd { get; set; } 
        public string? SdTahunLulus { get; set; }
        public string? Smp { get; set; } 
        public string? SmpTahunLulus { get; set; } 
        public string? Sma { get; set; } 
        public string? SmaTahunLulus { get; set; } 
        public string? Pt { get; set; }
        public string? PtTahunLulus { get; set; } 
        public string? Kursus { get; set; } 

        public string? Hobby { get; set; } 
        public string? PengalamanKerja { get; set; } 
        public string? Organisasi { get; set; } 
        public int? JumlahSaudara { get; set; } 
        public int? JumlahKakak { get; set; } 
        public int? JumlahAdik { get; set; } 
        public int? SaudaraSekolah { get; set; }
        public int? SaudaraBekerja { get; set; } 
        public string? AstraGrup { get; set; } 
        public string? AstraHubungan { get; set; } 
        public string? AstraPerusahaan { get; set; } 

        public string? PasFoto { get; set; } 
        public string? KtpSim { get; set; } 
        public string? AktaKelahiran { get; set; } 
        public string? KartuKeluarga { get; set; } 
        public string? Ijazah { get; set; } 
        public string? Skhun { get; set; } 
        public string? BebasNarkoba { get; set; } 
        public string? SanggupBayar { get; set; }
        public string? BuktiBayar { get; set; } 

        public string? VaWisuda { get; set; }
        public string? VaCuti { get; set; } 
        public string? VaIdcard { get; set; } 
        public string? VaLainnya { get; set; } 
        public string? VaSumbangan { get; set; } 
        public string? VaSpp { get; set; } 
        public string? StatusBeasiswa { get; set; } 
        public string? DosenAkademik { get; set; } 
        public string? Status { get; set; } 

        public string? LaporanId { get; set; }
        public string? LaporanNama { get; set; }
        public string? LaporanTanggalKegiatan { get; set; }
        public int? LaporanPoint { get; set; }
        public string? LaporanStatus { get; set; }
        public string? LaporanTahunAjaran { get; set; }
        public short? KegiatanId { get; set; }
        public string? LaporanTingkat { get; set; }

        public int? RfidMahasiswaId { get; set; }
        public string? IdMahasiswa { get; set; }
        public string? RfidMahasiswa { get; set; }
        public string? RfidMahasiswaStatus { get; set; }
        public string? RfidMahasiswaCreatedBy { get; set; }
        public DateTime? RfidMahasiswaCreatedDate { get; set; }
        public string? RfidMahasiswaModifBy { get; set; }
        public DateTime? RfidMahasiswaModifDate { get; set; }

        public string? AlamatJalan { get; set; }
        public string? Kelurahan { get; set; }
        public string? Kecamatan { get; set; }
        public string? Kabupaten { get; set; }
        public string? Provinsi { get; set; }

        public string? AlamatAyahJalan { get; set; }
        public string? KelurahanAyah { get; set; }
        public string? KecamatanAyah { get; set; }
        public string? KabupatenAyah { get; set; }
        public string? ProvinsiAyah { get; set; }

        public string? AlamatIbuJalan { get; set; }
        public string? KelurahanIbu { get; set; }
        public string? KecamatanIbu { get; set; }
        public string? KabupatenIbu { get; set; }
        public string? ProvinsiIbu { get; set; }

        public string? AlamatWaliJalan { get; set; }
        public string? KelurahanWali { get; set; }
        public string? KecamatanWali { get; set; }
        public string? KabupatenWali { get; set; }
        public string? ProvinsiWali { get; set; }
    }
}

