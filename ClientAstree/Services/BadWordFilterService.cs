using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace ClientAstree.Services
{
    public class BadWordFilterService
    {
        private readonly List<string> _badWords = new List<string>();

        private readonly ILogger<BadWordFilterService> _logger;

        public BadWordFilterService(ILogger<BadWordFilterService> logger)
        {
            _logger = logger;
            LoadBadWordsFromFile();
        }

        private void LoadBadWordsFromFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Words.xlsx");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"The file {filePath} does not exist. No bad words loaded.");
                return;
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet

                    int row = 1;
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        var word = worksheet.Cells[row, 1].Value.ToString();
                        _badWords.Add(word);
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading bad words from the Excel file.");
            }

            _logger.LogInformation($"Loaded {_badWords.Count} bad words from the Excel file.");
        }

        public string FilterBadWords(string input)
        {
            foreach (var badWord in _badWords)
            {
                // Creating the regex pattern to match whole words only (case insensitive)
                string pattern = $@"\b{Regex.Escape(badWord)}\b";
                
                // Using Regex.Replace to replace the bad word with '****'
                string newInput = Regex.Replace(input, pattern, "****", RegexOptions.IgnoreCase);

                // Logging the replacement if a bad word was found and replaced
                if (input != newInput)
                {
                    _logger.LogInformation($"Replaced '{badWord}' with '****'");
                }

                // Updating the input string to reflect the replacements
                input = newInput;
            }
            return input;
        }
    }
}
