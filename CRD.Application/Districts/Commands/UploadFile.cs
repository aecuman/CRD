using CRD.Application.Common;
using CRD.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Districts.Commands
{
    // File Upload
    public record UploadFileCommand(IFormFile File, int RefId) : IRequest<int>;
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, int>
    {
        private readonly IRepository<CRDFile> _context;
        public UploadFileHandler(IRepository<CRDFile> context)
        {
            _context = context;
        }

        public async Task<int> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            
                var filePath = Path.Combine("wwwroot/uploads/districtrates", request.File.FileName);
                var directory = Path.GetDirectoryName(filePath);
                var sanitizedFileName = Path.GetFileName(request.File.FileName); // Ensures a clean file name
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }
                var crdFile = new CRDFile
                {
                    FileClass = "DistrictRate",
                    Name = sanitizedFileName,
                    Url = filePath
                };
                _context.AddWithoutSaving(crdFile);
            await _context.SaveAsync();
            // await _context.SaveChangesAsync();
            return crdFile.Id;
            
            
        }
    }
}
