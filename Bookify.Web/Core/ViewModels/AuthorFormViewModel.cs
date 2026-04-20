using System.ComponentModel;

namespace Bookify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "Length cannot be more than 100 characters"), DisplayName("Author")]
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = "Author with The Same Name is Already Exist")]
        public string Name { get; set; } = null!;
    }
}
