namespace Bookify.Web.Core.ViewModels
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool isDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}
