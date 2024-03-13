using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IBasketService _basketService;
        public BasketController(IBasketViewModelService basketViewModelService, IBasketService basketServis)
        {
            _basketService = basketServis;
            _basketViewModelService = basketViewModelService;
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(int productId,int quantity = 1)
        {
            var basket = await _basketViewModelService.AddItemToBasketAsync(productId, quantity);
            return Json(basket);
        }
        public async Task<IActionResult> Index()
        {   
            var basket =  await _basketViewModelService.GetBasketViewModelAsync();
            return View(basket);
        }
  
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Empty(int itemId)
        {
            await _basketViewModelService.EmptyBasketAsync();
            TempData["Message"] = "Your basket is now empty.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItem(int productId)
        {
            await _basketViewModelService.RemoveItemAsync(productId);
            TempData["Message"] = "Item removed from the basket";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Dictionary<int, int> quantities)
        {
            await _basketViewModelService.SetQuantitiesAsync(quantities);
            TempData["Message"] = "The information for the items on your cart has been updated.";
            return RedirectToAction("index");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketViewModelService.GetBasketViewModelAsync();
            var vm = new CheckoutViewModel() { Basket = basket };
            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _basketViewModelService.CheckoutAsync(vm.Street, vm.City, vm.State, vm.Country, vm.ZipCode);
                return RedirectToAction("OrderConfirmed");
            }

            var basket = await _basketViewModelService.GetBasketViewModelAsync();
            vm.Basket = basket;
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmed()
        {
            return View();
        }
    }
}
