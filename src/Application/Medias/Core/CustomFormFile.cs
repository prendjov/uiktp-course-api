using Microsoft.AspNetCore.Http;

namespace Application.Medias.Core
{
    public class CustomFormFile : IFormFile
    {
        private readonly Stream _stream;

        public CustomFormFile(Stream stream, long length, string name, string fileName)
        {
            // Copy the original stream to a new MemoryStream
            _stream = new MemoryStream();
            stream.CopyTo(_stream);

            // Reset the position of the new MemoryStream
            _stream.Position = 0;

            Length = length;
            Name = name;
            FileName = fileName;
            ContentType = "application/octet-stream";
            Headers = new HeaderDictionary();
        }

        public string ContentType { get; }
        public string ContentDisposition => $"form-data; name={Name}; filename={FileName}";
        public IHeaderDictionary Headers { get; }
        public long Length { get; }
        public string Name { get; }
        public string FileName { get; }

        public void CopyTo(Stream target)
        {
            _stream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return _stream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            return _stream;
        }
    }
}
