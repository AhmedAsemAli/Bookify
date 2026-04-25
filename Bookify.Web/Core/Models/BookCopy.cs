namespace Bookify.Web.Core.Models
{
    public class BookCopy
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public bool IsAvilableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }

    }
}
