using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Linq.Dynamic.Core;

namespace Bookify.Web.Controllers
{
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(SearchFormViewModel model) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Subscriber = _context.Subscripers
                .SingleOrDefault(s=>s.MobileNumber==model.Value||
                s.Email == model.Value ||
                s.NationalId == model.Value );
            var viewModel=_mapper.Map<SubscriberSearchResultViewModel>(Subscriber);
            return PartialView( "_Result", viewModel);
        }

        public IActionResult Create() 
        {
           

            return View("Form",PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form",PopulateViewModel(model));
       
            var subscriber=_mapper.Map<Subscriper>(model);

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var imagePath = "/images/Subscribers";
            var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);
            if (!isUploaded)
            {
                ModelState.AddModelError("Image", errorMessage!);
                return View("Form", PopulateViewModel(model));
            }

            subscriber.ImageUrl = $"{imagePath}/{imageName}";
            subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Add(subscriber);
            _context.SaveChanges();

           

            return RedirectToAction(nameof(Details), new { id = subscriber.Id });
        }


        
        public IActionResult Details(int id) 
        {
            var subscriber=_context.Subscripers.Include(s=>s.Governorate)
                .Include(s=>s.Area).FirstOrDefault(s=>s.Id==id);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberViewModel>(subscriber);
            return View(viewModel);


        }

        public IActionResult Edit(int id)
        {
            var subscriber=_context.Subscripers.Find(id);
            if (subscriber is null)
                return NotFound();

            var model = _mapper.Map<SubscriberFormViewModel>(subscriber);
            var viewModel=PopulateViewModel(model);

            return View("Form", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var Subscriber = _context.Subscripers.Find(model.Id);

            if (Subscriber is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(Subscriber.ImageUrl))
                    _imageService.Delete(Subscriber.ImageUrl, Subscriber.ImageThumbnailUrl);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var imagePath = "/images/Subscribers";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError("Image", errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }

                model.ImageUrl = $"{imagePath}/{imageName}";
                model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            }

            else if (!string.IsNullOrEmpty(Subscriber.ImageUrl))
            {
                model.ImageUrl = Subscriber.ImageUrl;
                model.ImageThumbnailUrl = Subscriber.ImageThumbnailUrl;
            }

            Subscriber = _mapper.Map(model, Subscriber);
            Subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            Subscriber.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = Subscriber.Id });
        }

        [AjaxOnly]
        public IActionResult GetAreas(int governorateId)
        {
            var areas = _context.Areas
                .Where(a => a.GovernorateId == governorateId&&!a.IsDeleted) 
                .OrderBy(x => x.Name)
                .ToList();

            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));

        }

        public IActionResult AllowNationalId(SubscriberFormViewModel model) 
        {
            var subscriber = _context.Subscripers.SingleOrDefault(s => s.NationalId == model.NationalId);
            bool isAllowed = subscriber is null||subscriber.Id.Equals(model.Id);
            return Json(isAllowed);

        }

        public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
        {
            var subscriber = _context.Subscripers.SingleOrDefault(s => s.MobileNumber == model.MobileNumber);
            bool isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);
            return Json(isAllowed);

        }

        public IActionResult AllowEmail(SubscriberFormViewModel model)
        {
            var subscriber = _context.Subscripers.SingleOrDefault(s => s.Email == model.Email);
            bool isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);
            return Json(isAllowed);

        }

        private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
        {
            SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() : model;

            var governorates = _context.Governorates.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
            viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

            if (model?.GovernorateId > 0)
            {
                var areas = _context.Areas
                    .Where(a => a.GovernorateId == model.GovernorateId && !a.IsDeleted)
                    .OrderBy(a => a.Name)
                    .ToList();

                viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
            }

            return viewModel;
        }
    }
}
