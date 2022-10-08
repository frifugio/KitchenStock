using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitchenStock.Shared.Repositories
{
    public class MockStockRepository : IStockRepository
    {
        private List<StockItem> stockItems = new List<StockItem>
        {
            new StockItem
            {
                Id = Guid.NewGuid(),
                Name = "StockItem1",
                Quantity = 5,
                NextRefillDate = DateTime.Now.AddDays(30),
            },
            new StockItem
            {
                Id = Guid.NewGuid(),
                Name = "StockItem2",
                Quantity = 100,
                NextRefillDate = null
            },
        };

        public Task<StockItem> AddStockItemAsync(StockItem stockItem)
        {
            stockItems.Add(stockItem);
            return Task.FromResult(stockItem);
        }

        public Task DeleteStockItem(Guid id)
        {
            stockItems.RemoveAt(stockItems.FindIndex(f => f.Id == id));
            return Task.CompletedTask;
        }

        public Task<StockItem[]> GetAllStockItemsAsync()
        {
            return Task.FromResult(stockItems.ToArray());
        }

        public Task<StockItem> UpdateStockItemAsync(StockItem stockItem)
        {
            stockItems[stockItems.FindIndex(f => f.Id == stockItem.Id)] = stockItem;
            return Task.FromResult(stockItem);
        }
    }
}
