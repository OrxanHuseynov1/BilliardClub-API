// BusinessLayer.Services.Abstractions/ISessionProductService.cs
using BusinessLayer.DTOs.SessionProduct;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Abstractions;

public interface ISessionProductService
{
    Task<SessionProductGetDTO?> AddOrUpdateProductAsync(SessionProductPostDTO dto);
    Task<bool> DeleteProductAsync(Guid sessionProductId);
    Task<List<SessionProductGetDTO>> GetSessionProductsAsync(Guid tableSessionId);
}