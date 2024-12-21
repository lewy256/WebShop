using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OrderApi.Infrastructure;
using System.Linq.Expressions;

namespace OrderApi.UnitTests;

public static class TestExtensions {
    public static IPropertyValidator[] GetValidatorsForMember<T, TProperty>(
        this IValidator<T> validator, Expression<Func<T, TProperty>> expression) {
        var descriptor = validator.CreateDescriptor();
        var expressionMemberName = expression.GetMember()?.Name;

        return descriptor.GetValidatorsForMember(expressionMemberName).Select(x => x.Validator).ToArray();
    }

    public static IPropertyValidator[] GetValidatorsForMember<T>(
        this IValidator<T> validator, string memberName) {
        var descriptor = validator.CreateDescriptor();
        return descriptor.GetValidatorsForMember(memberName).Select(x => x.Validator).ToArray();
    }

    public static EntityTypeBuilder<TEntity> GetEntityTypeBuilder<TEntity, TEntityConfiguration>()
        where TEntity : class
        where TEntityConfiguration : IEntityTypeConfiguration<TEntity>, new() {
        var builder = new DbContextOptionsBuilder<OrderContext>();

        builder.UseInMemoryDatabase("TestDb");

        var context = new OrderContext(builder.Options);
        var conventionSet = ConventionSet.CreateConventionSet(context);
        var modelBuilder = new ModelBuilder(conventionSet);

        var entityBuilder = modelBuilder.Entity<TEntity>();
        var entityConfig = new TEntityConfiguration();
        entityConfig.Configure(entityBuilder);

        return entityBuilder;
    }
}