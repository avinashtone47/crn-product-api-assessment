using Product.Application.DTOs;
using Product.Application.Validators;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Tests.Application
{
    public class ProductValidatorTests
    {
        [Fact]
        public void CreateProductValidator_ShouldFail_WhenProductNameIsEmpty()
        {
            var validator = new CreateProductValidator();
            var dto = new CreateProductDto { ProductName = "", CreatedBy = "tester" };

            var result = validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ProductName");
        }

        [Fact]
        public void CreateProductValidator_ShouldPass_WhenValid()
        {
            var validator = new CreateProductValidator();
            var dto = new CreateProductDto { ProductName = "Widget", CreatedBy = "tester" };

            var result = validator.Validate(dto);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void UpdateProductValidator_ShouldFail_WhenModifiedByMissing()
        {
            var validator = new UpdateProductValidator();
            var dto = new UpdateProductDto { ProductName = "Widget", ModifiedBy = "" };

            var result = validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModifiedBy");
        }
    }
}
