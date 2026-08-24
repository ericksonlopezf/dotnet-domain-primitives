// Copyright © Erickson Lopez. MIT License.
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EricksonLopez.DomainPrimitives.AspNetCore;

/// <summary>
/// Extensions for registering domain primitive model binders in ASP.NET Core MVC.
/// </summary>
public static class DomainPrimitivesMvcBuilderExtensions
{
    /// <summary>
    /// Adds custom model binder support for domain primitives to MVC options.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions"/> instance to configure.</param>
    /// <returns>The same <see cref="MvcOptions"/> instance for chaining additional configuration.</returns>
    public static MvcOptions AddDomainPrimitivesModelBinding(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options;
    }

    /// <summary>
    /// Adds domain primitive model binding to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDomainPrimitivesModelBinding(this IServiceCollection services)
        => services.Configure<MvcOptions>(options => options.AddDomainPrimitivesModelBinding());
}
