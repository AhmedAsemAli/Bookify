namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public bool isDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }
    }
}
