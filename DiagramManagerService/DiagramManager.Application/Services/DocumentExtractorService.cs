using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiagramManager.Application.DTOs;
using DiagramManager.Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace DiagramManager.Application.Services
{
    public class DocumentExtractorService : IDocumentExtractorService
    {
        public async Task<List<ExtractedDiagramDto>> ExtractDiagramsAsync(Stream fileStream, string fileName)
        {
            var results = new List<ExtractedDiagramDto>();
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".pdf")
            {
                results.AddRange(ExtractFromPdf(fileStream));
            }
            else if (extension == ".docx")
            {
                results.AddRange(ExtractFromDocx(fileStream));
            }
            else
            {
                throw new NotSupportedException($"Định dạng file {extension} không được hỗ trợ để trích xuất diagram.");
            }

            return await Task.FromResult(results);
        }

        private List<ExtractedDiagramDto> ExtractFromPdf(Stream stream)
        {
            var list = new List<ExtractedDiagramDto>();
            
           
            using (var pdf = PdfDocument.Open(stream))
            {
                foreach (var page in pdf.GetPages())
                {
                    var images = page.GetImages().ToList();
                    int imageIndex = 1;
                    
                    foreach (var image in images)
                    {
                        try
                        {
                            byte[]? bytes = null;
                            string mimeType = "image/png"; // Mặc định PNG

                            if (image.TryGetPng(out var pngBytes))
                            {
                                bytes = pngBytes;
                                mimeType = "image/png";
                            }
                            else
                            {
                                bytes = image.RawBytes.ToArray();
                            }

                            if (bytes != null && bytes.Length >= 8192)
                            {
                                list.Add(new ExtractedDiagramDto
                                {
                                    ImageBytes = bytes,
                                    MimeType = mimeType,
                                    Name = $"pdf_page_{page.Number}_img_{imageIndex}",
                                    PageNumber = page.Number
                                });
                                imageIndex++;
                            }
                        }
                        catch
                        {
                            // Bỏ qua nếu có ảnh lỗi hoặc không thể giải mã
                        }
                    }
                }
            }

            return list;
        }

        private List<ExtractedDiagramDto> ExtractFromDocx(Stream stream)
        {
            var list = new List<ExtractedDiagramDto>();
            
            using (var doc = WordprocessingDocument.Open(stream, false))
            {
                if (doc.MainDocumentPart != null)
                {
                    var imageParts = doc.MainDocumentPart.ImageParts;
                    int imageIndex = 1;

                    foreach (var imagePart in imageParts)
                    {
                        try
                        {
                            using (var partStream = imagePart.GetStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    partStream.CopyTo(ms);
                                    var bytes = ms.ToArray();
                                    
                                    // Chỉ trích xuất các ảnh >= 8KB (loại bỏ icon, logo trang trí rác)
                                    if (bytes.Length >= 8192)
                                    {
                                        list.Add(new ExtractedDiagramDto
                                        {
                                            ImageBytes = bytes,
                                            MimeType = imagePart.ContentType,
                                            Name = $"docx_img_{imageIndex}",
                                            PageNumber = 1
                                        });
                                        imageIndex++;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Bỏ qua nếu lỗi trích xuất ảnh
                        }
                    }
                }
            }

            return list;
        }
    }
}
