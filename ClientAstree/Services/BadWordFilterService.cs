using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace ClientAstree.Services
{
    public class BadWordFilterService
    {
        private readonly List<string> _badWords = new List<string>
        {
            "badword1",
            "badword2",
            // Add more bad words as needed
        };

        private readonly ILogger<BadWordFilterService> _logger;

        public BadWordFilterService(ILogger<BadWordFilterService> logger)
        {
            _logger = logger;
        }

        public string FilterBadWords(string input)
        {
            foreach (var badWord in _badWords)
            {
                var regex = new Regex($@"\b{Regex.Escape(badWord)}\b", RegexOptions.IgnoreCase);
                var newInput = regex.Replace(input, "****");

                if (input != newInput)
                {
                    _logger.LogInformation($"Replaced '{badWord}' with '****'");
                }

                input = newInput;
            }
            return input;
        }
    }
}
