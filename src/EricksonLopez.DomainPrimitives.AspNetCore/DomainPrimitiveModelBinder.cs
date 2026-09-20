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
public sealed class DomainPrimitiveModelBinder<[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods)] T> : IModelBinder
{
    private static readonly Func<string, IFormatProvider?, T>? CompiledParse2 = CreateParse2Delegate();
    private static readonly Func<string, T>? CompiledParse1 = CreateParse1Delegate();
    private static readonly Func<string, T>? CompiledCreate = CreateCreateDelegate();

    private static Func<string, IFormatProvider?, T>? CreateParse2Delegate()
    {
        var m = typeof(T).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(IFormatProvider) }, null);
        return m is null ? null : (Func<string, IFormatProvider?, T>)Delegate.CreateDelegate(typeof(Func<string, IFormatProvider?, T>), m);
    }

    private static Func<string, T>? CreateParse1Delegate()
    {
        var m = typeof(T).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
        return m is null ? null : (Func<string, T>)Delegate.CreateDelegate(typeof(Func<string, T>), m);
    }

    private static Func<string, T>? CreateCreateDelegate()
    {
        var m = typeof(T).GetMethod("Create", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
        return m is null ? null : (Func<string, T>)Delegate.CreateDelegate(typeof(Func<string, T>), m);
    }

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
            if (CompiledParse2 != null)
            {
                var result = CompiledParse2(rawValue, valueProviderResult.Culture);
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            if (CompiledParse1 != null)
            {
                var result = CompiledParse1(rawValue);
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            if (CompiledCreate != null)
            {
                var result = CompiledCreate(rawValue);
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
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(modelName, ex.Message);
        }

        return Task.CompletedTask;
    }
}





