namespace Bookify.Web.Core.ViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }
        public string? BookTitel { get; set; }
        public bool IsAvilableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } 
       
    }
}
