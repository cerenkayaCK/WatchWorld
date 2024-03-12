using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRepository<Basket> _basketRepo;
        private readonly IRepository<BasketItem> _basketItemRepo;
        private readonly IRepository<Product> _productRepo;

        public BasketService(IRepository<Basket> basketRepo, IRepository<BasketItem> basketItemRepo,IRepository<Product> productRepo)
        {
            _basketItemRepo= basketItemRepo;
            _basketRepo = basketRepo; 
            _productRepo= productRepo;
        }
        public async Task<Basket> AddItemToBasketAsync(string buyerId, int productId, int quantity)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);
            var basketItem = basket.BasketItems.FirstOrDefault(x => x.ProductId == productId);

            if(basketItem != null)
            {
                basketItem.Quantity += quantity;
            }
            else
            {
                var product = await _productRepo.GetByIdAsync(productId);
                basketItem = new BasketItem()
                {
                    ProductId = productId,
                    Quantity = quantity,
                    product = product!
                };
                basket.BasketItems.Add(basketItem);
            }
            await _basketRepo.UpdateAsync(basket);
            return basket;

        }

        public async Task DeleteBasketItemsAsync(string buyerId, int productId)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);
            var basketItem = basket.BasketItems.FirstOrDefault(a => a.ProductId == productId);

            if(basketItem != null)
            {
                await _basketItemRepo.DeleteAsync(basketItem);
            }
        }

        public async Task EmptyBasketAsync(string buyerId)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);
            foreach ( var item in basket.BasketItems.ToList())
            {
                await _basketItemRepo.DeleteAsync(item);
            }
        }

        public async Task<Basket> GetOrCreateBasketAsync(string buyerId)
        {
            var specBasket = new BasketwithItemsSpecification(buyerId);

            var basket = await _basketRepo.FirstOrDefaultAsync(specBasket);

            if (basket == null)
            {
                basket = new Basket()
                {
                    BuyerId = buyerId,
                };

                basket = await _basketRepo.AddAsync(basket);
            }
          return basket;
        }

        public async Task<Basket> SetQuantitiesAsync(string buyerId, Dictionary<int, int> quantities)
        {
           var basket =await GetOrCreateBasketAsync(buyerId);
           
            foreach( var item in basket.BasketItems)
            {
                if (quantities.ContainsKey(item.ProductId))
                {
                    item.Quantity = quantities[item.ProductId];
                    await _basketItemRepo.UpdateAsync(item);
                }
            }
            return basket;

        }

        public async Task TransferBasketAsync(string sourceBuyerId, string destinationBuyerId)
        {
            var specSourceBasket = new BasketwithItemsSpecification(sourceBuyerId);
            var sourceBasket = await _basketRepo.FirstOrDefaultAsync(specSourceBasket);
            if (sourceBasket == null) { return; }

            var destinationBasket = await GetOrCreateBasketAsync(destinationBuyerId);

            foreach (var item in sourceBasket.BasketItems)
            {
                var targetItem = destinationBasket.BasketItems.FirstOrDefault(a => a.ProductId == item.ProductId);
                if(targetItem != null)
                {
                    targetItem.Quantity += item.Quantity;
                }
                else
                {

                    destinationBasket.BasketItems.Add(new BasketItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    });
                }
            }

            await _basketRepo.UpdateAsync(destinationBasket);
            await _basketRepo.DeleteAsync(sourceBasket);
        }
    }
}
