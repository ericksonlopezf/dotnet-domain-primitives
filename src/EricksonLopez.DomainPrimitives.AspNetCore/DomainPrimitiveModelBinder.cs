// Copyright © Erickson Lopez. MIT License.
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EricksonLopez.DomainPrimitives.AspNetCore;

/// <summary>
/// Model binder for domain primitives that seamlessly binds values from HTTP route, query, or form parameters.
/// </summary>
/// <typeparam name="T">The domain primitive type.</typeparam>
[System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Fallback reflection binder for non-generated primitive model binding.")]
[System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2087", Justification = "Fallback reflection binder for non-generated primitive model binding.")]
[System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2090", Justification = "Fallback reflection binder for non-generated primitive model binding.")]
public sealed class DomainPrimitiveModelBinder<T> : IModelBinder
{
    private static readonly MethodInfo? Parse2ParamMethod = typeof(T).GetMethod(
        "Parse",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(string), typeof(IFormatProvider) },
        null);

    private static readonly MethodInfo? Parse1ParamMethod = typeof(T).GetMethod(
        "Parse",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(string) },
        null);

    private static readonly MethodInfo? CreateMethod = typeof(T).GetMethod(
        "Create",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(string) },
        null);

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Fallback reflection binder for non-generated primitive model binding.")]
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2087", Justification = "Fallback reflection binder for non-generated primitive model binding.")]
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var rawValue = valueProviderResult.FirstValue;

        if (rawValue is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            if (Parse2ParamMethod != null)
            {
                var result = (T)Parse2ParamMethod.Invoke(null, new object?[] { rawValue, valueProviderResult.Culture })!;
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            if (Parse1ParamMethod != null)
            {
                var result = (T)Parse1ParamMethod.Invoke(null, new object?[] { rawValue })!;
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            if (CreateMethod != null)
            {
                var result = (T)CreateMethod.Invoke(null, new object?[] { rawValue })!;
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
            {
                var result = (T)converter.ConvertFrom(null, valueProviderResult.Culture, rawValue)!;
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.TryAddModelError(modelName, $"The value '{rawValue}' is not valid for {typeof(T).Name}.");
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            bindingContext.ModelState.TryAddModelError(modelName, ex.InnerException.Message);
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(modelName, ex.Message);
        }

        return Task.CompletedTask;
    }
}





