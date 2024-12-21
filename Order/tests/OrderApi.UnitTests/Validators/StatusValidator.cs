using FluentAssertions;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore.Metadata;
using OrderApi.Entities;
using OrderApi.Features.Statuses;
using OrderApi.Infrastructure.Configurations;
using Xunit;

namespace OrderApi.UnitTests.Validators;

public class StatusValidator {
    private readonly CreateStatus.Validator _validator = new();

    [Fact]
    public void Validator_MaxLengthRules_ShouldHaveSameLengthAsEfEntity() {
        var propertiesToValidate = new[]{
            nameof(Status.Description),
        };

        var entityTypeBuilder = TestExtensions
            .GetEntityTypeBuilder<Status, StatusConfiguration>();

        Dictionary<string, ILengthValidator> validatorsDict = propertiesToValidate
            .Select(p => new { Key = p, Validator = _validator.GetValidatorsForMember(p).OfType<ILengthValidator>().First() })
            .ToDictionary(key => key.Key, value => value.Validator);

        Dictionary<string, IMutableProperty> expectedDbProperties = propertiesToValidate
            .Select(p => new { Key = p, FieldMetadata = entityTypeBuilder.Metadata.FindDeclaredProperty(p) })
            .ToDictionary(key => key.Key, value => value.FieldMetadata);

        foreach(var propValidator in validatorsDict) {
            var expectedDbMetadata = expectedDbProperties[propValidator.Key];

            expectedDbMetadata.GetMaxLength().Should().Be(propValidator.Value.Max);
        }
    }
}
