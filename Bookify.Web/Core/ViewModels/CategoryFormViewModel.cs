using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Remote("AllowItem",null,AdditionalFields ="Id",ErrorMessage ="Category with The Same Name is Already Exist")]
        public string Name { get; set; } = null!;
    }
}
