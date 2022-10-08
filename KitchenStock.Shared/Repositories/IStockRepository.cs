using System;
using System.Threading.Tasks;

namespace KitchenStock.Shared.Repositories
{
    public interface IStockRepository
    {
        public Task<StockItem[]> GetAllStockItemsAsync(); 
        public Task<StockItem> AddStockItemAsync(StockItem stockItem); 
        public Task<StockItem> UpdateStockItemAsync(StockItem stockItem); 
        public Task DeleteStockItem(Guid id); 
    }
}
