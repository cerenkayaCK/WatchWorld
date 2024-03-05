using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeViewModelService _homeViewModelService;

        public HomeController(ILogger<HomeController> logger, IHomeViewModelService homeViewModelService,IRepository repository)
        {
            _logger = logger;
            _homeViewModelService = homeViewModelService;

        }

        public async Task<IActionResult> Index(int? categoryId, int? brandId, int pageId = 1)
        {
            var vm = await _homeViewModelService.GetHomeViewModelAsync(categoryId, brandId, pageId);

            var paginationInfo = new PaginationInfoViewModel
            {
                PageId = pageId,
                TotalItems = ?,
                ItemsOnPage = 4
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
