using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        public readonly IBasketService _basketService;
        public readonly IRepository<Order> _orderRepo;

        public OrderService(IBasketService basketService, IRepository<Order> orderRepo)
        {
            _basketService= basketService;
            _orderRepo= orderRepo;
        }
        public async Task<Order> CreateOrderAsync(string buyerId, Address shippingAdress)
        {
            var basket = await _basketService.GetOrCreateBasketAsync(buyerId);

            Order order = new Order() { 
                ShipToAdress = shippingAdress,
                BuyerId = buyerId,
                Items=basket.BasketItems.Select(x=> new OrderItem()
                {
                    ProductId=x.ProductId,
                    Quantity = x.Quantity,
                    ProductName=x.product.Name,
                    UnitPrice=x.product.Price,
                    PictureUri=x.product.PictureUri,

                }).ToList()
            
            };

            return await _orderRepo.AddAsync(order);

        }
    }
}
