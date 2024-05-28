using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace ClientAstree.Services
{
    public class ExcelService
    {
        private readonly string _brandsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/brands.xlsx");

        public List<string> GetCarBrands()
        {
            using (var package = new ExcelPackage(new FileInfo(_brandsFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var carBrands = new List<string>();
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    carBrands.Add(worksheet.Cells[row, 2].Text); // Column B
                }
                return carBrands;
            }
        }

        public List<string> GetCarModels(string brand)
        {
            var brandFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/files/{brand}.xlsx");
            if (!File.Exists(brandFilePath))
            {
                return new List<string>();
            }

            using (var package = new ExcelPackage(new FileInfo(brandFilePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var carModels = new List<string>();
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    carModels.Add(worksheet.Cells[row, 1].Text); // Column A
                }
                return carModels;
            }
        }
    }
}
