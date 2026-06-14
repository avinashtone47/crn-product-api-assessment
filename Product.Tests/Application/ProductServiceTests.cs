using Microsoft.EntityFrameworkCore;
using Product.Application.DTOs;
using Product.Application.Services;
using Product.Domain.Exceptions;
using Product.Infrastructure.Data;
using Product.Infrastructure.Repositories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Tests.Application
{
    public class ProductServiceTests
    {
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProduct_AndReturnDto()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            var dto = new CreateProductDto { ProductName = "Widget", CreatedBy = "tester" };

            var result = await service.CreateAsync(dto);

            result.Id.Should().BeGreaterThan(0);
            result.ProductName.Should().Be("Widget");
            result.CreatedBy.Should().Be("tester");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            Func<Task> act = async () => await service.GetByIdAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyProduct_WhenProductExists()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            var created = await service.CreateAsync(new CreateProductDto { ProductName = "Old Name", CreatedBy = "tester" });

            await service.UpdateAsync(created.Id, new UpdateProductDto { ProductName = "New Name", ModifiedBy = "tester2" });

            var updated = await service.GetByIdAsync(created.Id);
            updated.ProductName.Should().Be("New Name");
            updated.ModifiedBy.Should().Be("tester2");
            updated.ModifiedOn.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct_WhenProductExists()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            var created = await service.CreateAsync(new CreateProductDto { ProductName = "ToDelete", CreatedBy = "tester" });

            await service.DeleteAsync(created.Id);

            Func<Task> act = async () => await service.GetByIdAsync(created.Id);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            Func<Task> act = async () => await service.DeleteAsync(999);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedResults()
        {
            using var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductService(unitOfWork);

            for (int i = 1; i <= 5; i++)
            {
                await service.CreateAsync(new CreateProductDto { ProductName = $"Product {i}", CreatedBy = "tester" });
            }

            var page1 = await service.GetAllAsync(pageNumber: 1, pageSize: 2);

            page1.Items.Should().HaveCount(2);
            page1.TotalCount.Should().Be(5);
            page1.TotalPages.Should().Be(3);
        }
    }
}
