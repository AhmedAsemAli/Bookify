using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100 , ErrorMessage ="Length cannot be more than 100 characters"),DisplayName("Category")]
        [Remote("AllowItem",null!,AdditionalFields ="Id",ErrorMessage ="Category with The Same Name is Already Exist")]
        public string Name { get; set; } = null!;
    }
}
