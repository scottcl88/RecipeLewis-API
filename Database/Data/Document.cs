using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Document : EntityDataUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }
        public string? Name { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public string? ContentType { get; set; }
        public long ByteSize { get; set; }
        public byte[] Bytes { get; set; }
        public Guid DocumentKey { get; set; }
    }
}
