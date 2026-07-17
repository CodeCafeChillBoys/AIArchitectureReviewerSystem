using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Services;

namespace AIArchitectureReviewer.Infrastructure.Services
{
    public class CodeExtractorService : ICodeExtractorService
    {
        private static readonly string[] AllowedExtensions = { ".cs", ".java", ".ts", ".js", ".py", ".cpp", ".h" };

        public async Task<string> ExtractCodeAsync(Stream stream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".zip")
            {
                var sb = new StringBuilder();
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue; // Directory

                        var entryExt = Path.GetExtension(entry.FullName).ToLower();
                        if (Array.Exists(AllowedExtensions, ext => ext == entryExt))
                        {
                            using (var entryStream = entry.Open())
                            using (var reader = new StreamReader(entryStream, Encoding.UTF8))
                            {
                                var content = await reader.ReadToEndAsync();
                                sb.AppendLine($"<file path=\"{entry.FullName}\">");
                                sb.AppendLine(content);
                                sb.AppendLine("</file>");
                                sb.AppendLine();
                            }
                        }
                    }
                }
                return sb.ToString();
            }
            else if (Array.Exists(AllowedExtensions, ext => ext == extension))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                {
                    var content = await reader.ReadToEndAsync();
                    var sb = new StringBuilder();
                    sb.AppendLine($"<file path=\"{fileName}\">");
                    sb.AppendLine(content);
                    sb.AppendLine("</file>");
                    return sb.ToString();
                }
            }

            throw new ArgumentException($"Unsupported file type: {extension}. Only source code files (.cs, .java, .ts, etc.) or .zip archives are supported.");
        }
    }
}
