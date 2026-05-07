using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles =AppRoles.Reception)]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RentalsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Create(string sKey)
        {
            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetCopyDetails(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var copy = _context.BookCopies.Include(c => c.Book)
                .SingleOrDefault(c => c.SerialNumber.ToString() == model.Value&&!c.IsDeleted&&!c.Book!.isDeleted);

            if (copy is null)
                return NotFound(Errors.InvalidSerialNumber);

            if(!copy.IsAvilableForRental||!copy.Book!.IsAvailableForRental)
                return NotFound(Errors.NotAvilableRental);

            var viewModel = _mapper.Map<BookCopyViewModel>(copy);

            return PartialView("_CopyDetails", viewModel);


        }
    }
}
