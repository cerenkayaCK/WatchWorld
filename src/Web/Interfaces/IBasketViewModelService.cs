
using Web.Models;

namespace Web.Interfaces
{
    public interface IBasketViewModelService
    {
        public Task<BasketViewModel> GetBasketViewModelAsync();
        public Task<BasketViewModel> AddItemToBasketAsync(int productId, int quantity);
    }
}
