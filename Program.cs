using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Linq;

namespace DateFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get input.
            Console.WriteLine("What is the path of the directory you would like to reorganize?");
            var input = Console.ReadLine();

            // Validate input.
            DirectoryInfo inputDir = null;

            try
            {
                inputDir = new DirectoryInfo(input);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not find directory '{input ?? string.Empty}':\n{e.Message}");
                return;
            }

            if (!inputDir.Exists)
            {
                Console.WriteLine($"Given directory '{inputDir.FullName}' does not exist.");
                return;
            }

            // Enumerate files.
            var headerGroupName = "Header";
            var paidDateGroupName = "PaidDate";
            var dueDateGroupName = "DueDate";
            var amountGroupName = "Amount";
            
            foreach (var file in inputDir.EnumerateFiles())
            {
                // Check if the current file matches the expected format.
                var match = Regex.Match(Path.GetFileNameWithoutExtension(file.Name), @$"^(?<{headerGroupName}>.+?) - Payed (?<{paidDateGroupName}>[\d_]{{4}}-[\d_]{{2}}-[\d_]{{2}}) - Due (?<{dueDateGroupName}>[\d_]{{4}}-[\d_]{{2}}-[\d_]{{2}}) - (?<{amountGroupName}>\$\d+\.\d+)$");

                // If it does, change its name.
                if (match.Success)
                {
                    var oldName = file.Name;

                    var newName = $"{match.Groups[headerGroupName].Value} - Due {match.Groups[dueDateGroupName].Value} - Paid {match.Groups[paidDateGroupName].Value} - {match.Groups[amountGroupName].Value}";

                    var fileExtension = Path.GetExtension(file.Name);

                    var newPath = Path.Combine(file.DirectoryName, $"{newName}{fileExtension}");

                    file.CopyTo(newPath);

                    file.Delete();

                    Console.WriteLine($"RENAMED:\t'{newName}'");
                }
                // Else, ignore it.
                else
                {
                    Console.WriteLine($"IGNORED:\t'{file.Name}'");
                }
            }
        }
    }
}
