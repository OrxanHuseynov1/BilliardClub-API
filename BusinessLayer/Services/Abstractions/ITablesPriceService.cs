using BusinessLayer.DTOs.TablesPrice;

namespace BusinessLayer.Services.Abstractions;

public interface ITablesPriceService
{
    Task<List<TablesPriceGetDTO>> GetAllTablesPricesAsync();
    Task<TablesPriceGetDTO> GetTablesPriceByIdAsync(Guid id);
    Task<TablesPriceGetDTO> CreateTablesPriceAsync(TablesPricePostDTO createDto);
    Task<TablesPriceGetDTO> UpdateTablesPriceAsync(Guid id, TablesPricePutDTO updateDto);
    Task<bool> DeleteTablesPriceAsync(Guid id);
    Task InitializeDefaultTablesPrices(Guid companyId, string createdBy);
}