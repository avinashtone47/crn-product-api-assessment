using System;
using System.Linq;
using System.Threading.Tasks;
using Product.Application.DTOs;
using Product.Application.Interfaces;
using Product.Domain.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var products = await _unitOfWork.Products.GetPagedAsync(pageNumber, pageSize);
            var total = await _unitOfWork.Products.CountAsync();

            return new PagedResult<ProductDto>
            {
                Items = products.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithItemsAsync(id);
            if (product == null)
                throw new NotFoundException(nameof(Product.Domain.Entities.Product), id);

            return MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var entity = new Product.Domain.Entities.Product
            {
                ProductName = dto.ProductName,
                CreatedBy = dto.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var entity = await _unitOfWork.Products.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException(nameof(Product.Domain.Entities.Product), id);

            entity.ProductName = dto.ProductName;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedOn = DateTime.UtcNow;

            _unitOfWork.Products.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Products.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException(nameof(Product.Domain.Entities.Product), id);

            _unitOfWork.Products.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static ProductDto MapToDto(Product.Domain.Entities.Product p) => new()
        {
            Id = p.Id,
            ProductName = p.ProductName,
            CreatedBy = p.CreatedBy,
            CreatedOn = p.CreatedOn,
            ModifiedBy = p.ModifiedBy,
            ModifiedOn = p.ModifiedOn,
            Items = p.Items?.Select(i => new ItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList() ?? new()
        };
    }
}