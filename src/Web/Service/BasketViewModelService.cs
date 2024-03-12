using ApplicationCore.Interfaces;
using System.Security.Claims;
using Web.Interfaces;
using Web.Models;
using System.Security.Claims;
using System.Runtime.InteropServices;
using Web.Extensions;
using ApplicationCore.Services;
using ApplicationCore.Entities;

namespace Web.Service
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpContext HttpContext => _httpContextAccessor.HttpContext!;
        private string? UserId => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? AnonId => HttpContext.Request.Cookies[Constants.BASKET_COOKIE];
        private string BuyerId => UserId ?? AnonId ?? CreateAnonymousId();

        private string _createdAnonId = null!;
        private string CreateAnonymousId()
        {
            if(_createdAnonId != null) return _createdAnonId;

            _createdAnonId = Guid.NewGuid().ToString();

            HttpContext.Response.Cookies.Append(Constants.BASKET_COOKIE, _createdAnonId, new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(1),
                IsEssential = true
            });

            return _createdAnonId;

        }

        public BasketViewModelService(IBasketService basketService,IHttpContextAccessor httpContextAccessor,IOrderService orderService)
        {
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
        }
        public async Task<BasketViewModel> AddItemToBasketAsync(int productId, int quantity)
        {
            var basket = await _basketService.AddItemToBasketAsync(BuyerId, productId, quantity);

            return basket.ToBasketViewModel();
        }

        public async Task<BasketViewModel> GetBasketViewModelAsync()
        {
            var basket = await _basketService.GetOrCreateBasketAsync(BuyerId);
            return basket.ToBasketViewModel();
        }

        public async Task EmptyBasketAsync()
        {
            await _basketService.EmptyBasketAsync(BuyerId);
        }

    
        public async Task RemoveItemAsync(int productId)
        {
           await _basketService.DeleteBasketItemsAsync(BuyerId, productId);
   
        }

        public async Task<BasketViewModel> SetQuantitiesAsync(Dictionary<int, int> quantities)
        {
            var basket = await _basketService.SetQuantitiesAsync(BuyerId, quantities);
            return basket.ToBasketViewModel();
        }

        public async Task TransferBasketAsync()
        {
            if(AnonId == null || UserId == null) return;
            await _basketService.TransferBasketAsync(AnonId, UserId);
            HttpContext.Response.Cookies.Delete(Constants.BASKET_COOKIE);

        }

        public async Task CheckoutAsync(string street, string city, string? state, string country, string zipCode)
        {
            Address adres = new Address
            {
                Street = street,
                City = city,
                State = state,
                Country = country,
                ZipCode = zipCode
            };
            await _orderService.CreateOrderAsync(BuyerId, adres);
            await _basketService.EmptyBasketAsync(BuyerId);
        }
    }
}
 