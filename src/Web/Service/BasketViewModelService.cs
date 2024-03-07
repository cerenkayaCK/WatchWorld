using ApplicationCore.Interfaces;
using System.Security.Claims;
using Web.Interfaces;
using Web.Models;

using System.Security.Claims;
using System.Runtime.InteropServices;
using Web.Extensions;

namespace Web.Service
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IBasketService _basketService;
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

        public BasketViewModelService(IBasketService basketService,IHttpContextAccessor httpContextAccessor)
        {
            _basketService = basketService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BasketViewModel> AddItemToBasketAsync(int productId, int quantity)
        {
            var basket = await _basketService.AddItemToBasketAsync(BuyerId, productId, quantity);

            return basket.ToBasketViewModel();
        }
    }
}
 