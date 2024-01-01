using FluentValidation;

namespace ProductApi.Service.Extensions;

public static class ValidatorExtensions {
    public static IRuleBuilderOptions<T, TProperty> InOrNull<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] validOptions) {
        string formatted = $"{string.Join(", ", validOptions.Select(x => x.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions.Last()}";

        return ruleBuilder
            .Must(x => x is null || validOptions.Contains(x))
            .WithMessage($"Property can be null or must be one of these values: {formatted}");
    }
}
