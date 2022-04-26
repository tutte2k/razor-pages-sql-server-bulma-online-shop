﻿using OnlineShop.Domain.Infrastructure;
using OnlineShop.Domain.Models;
using System.Threading.Tasks;

namespace OnlineShop.Application.Cart
{
    [Service]
    public class AddToCart
    {
        private readonly ISessionManager _sessionManager;
        private readonly IStockManager _stockManager;

        public AddToCart(ISessionManager sessionManager, IStockManager stockManager)
        {
            _sessionManager = sessionManager;
            _stockManager = stockManager;
        }
        public class Request
        {

            public int StockId { get; set; }
            public int Qty { get; set; }
        }

        public async Task<bool> Do(Request request)
        {


            if (!_stockManager.EnoughStock(request.StockId, request.Qty))
            {
                return false;
            }

            await _stockManager.PutStockOnHold(request.StockId, request.Qty, _sessionManager.GetId());

            var stock = _stockManager.GetStockWithProduct(request.StockId);

            var cartProduct = new CartProduct()
            {
                ProductId = stock.ProductId,
                ProductName = stock.Product.Name,
                StockId = stock.Id,
                Qty = request.Qty,
                Value = stock.Product.Value
            };

            _sessionManager.AddProduct(cartProduct);

            return true;
        }

    }
}