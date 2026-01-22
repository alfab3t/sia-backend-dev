using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using astratech_apps_backend.DTOs.Kamar_Dormitory;

namespace astratech_apps_backend.Services.Implementations
{
    public static class ExcelExportService
    {
        static ExcelExportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static byte[] ExportKamarDormitory(
            List<KamarDormitoryExportDto> data,
            DateTime startDate,
            DateTime endDate,
            string prodi,
            string angkatan)
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Dormitory");

            int row = 1;

            ws.Cells[row, 1].Value = "LAPORAN PENGHUNI DORMITORY";
            ws.Cells[row, 1, row, 13].Merge = true;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Size = 14;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row += 2;

            ws.Cells[row, 1].Value = "Periode";
            ws.Cells[row, 2].Value = $"{startDate:dd-MM-yyyy} s/d {endDate:dd-MM-yyyy}";
            row++;
            ws.Cells[row, 1].Value = "Prodi";
            ws.Cells[row, 2].Value = string.IsNullOrEmpty(prodi) ? "Semua" : prodi;
            row++;
            ws.Cells[row, 1].Value = "Angkatan";
            ws.Cells[row, 2].Value = string.IsNullOrEmpty(angkatan) ? "Semua" : angkatan;
            row += 2;

            string[] headers = { "No", "NIM", "Nama", "Prodi", "Angkatan", "No Kamar", "Tanggal Masuk", "Tanggal Keluar", "Terakhir Ditagih", "NIK", "Tagihan", "Pembayaran", "Sisa Tagihan" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[row, i + 1].Value = headers[i];
                ws.Cells[row, i + 1].Style.Font.Bold = true;
                ws.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                ws.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;
            int no = 1;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = no++;
                ws.Cells[row, 2].Value = item.NIM;
                ws.Cells[row, 3].Value = item.Nama;
                ws.Cells[row, 4].Value = item.Prodi;
                ws.Cells[row, 5].Value = item.Angkatan;
                ws.Cells[row, 6].Value = item.NoKamar;
                ws.Cells[row, 7].Value = item.TanggalMasuk?.ToString("dd-MM-yyyy");
                ws.Cells[row, 8].Value = item.TanggalKeluar?.ToString("dd-MM-yyyy");
                ws.Cells[row, 9].Value = item.TerakhirDitagih?.ToString("dd-MM-yyyy");
                ws.Cells[row, 10].Value = item.NIK;
                ws.Cells[row, 11].Value = item.Tagihan;
                ws.Cells[row, 12].Value = item.Pembayaran;
                ws.Cells[row, 13].Value = item.SisaTagihan;

                for (int col = 1; col <= 13; col++)
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                row++;
            }

            ws.Cells.AutoFitColumns();
            ws.Column(11).Style.Numberformat.Format = "#,##0";
            ws.Column(12).Style.Numberformat.Format = "#,##0";
            ws.Column(13).Style.Numberformat.Format = "#,##0";

            return package.GetAsByteArray();
        }
    }
}
