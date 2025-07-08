using BusinessLayer.DTOs.Table;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstractions;

public interface ITableService
{
    Task<List<TableGetDTO>> GetAllTablesAsync();
    Task<TableGetDTO> GetTableByIdAsync(Guid id); 
    Task<TableGetDTO> CreateTableAsync(TablePostDTO createDto);
    Task<TableGetDTO> UpdateTableAsync(Guid id, TablePutDTO updateDto); 
    Task<bool> DeleteTableAsync(Guid id); 
}
