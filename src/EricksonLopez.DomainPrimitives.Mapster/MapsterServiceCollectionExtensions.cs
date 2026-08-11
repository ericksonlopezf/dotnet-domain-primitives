using System;
using Mapster;

namespace EricksonLopez.DomainPrimitives.Mapster;

/// <summary>
/// Provides extension methods for registering domain primitive mappings with Mapster.
/// </summary>
/// <remarks>
/// <para>
/// These extensions work together with the source-generated
/// <c>DomainPrimitivesMapsterRegister</c> class (in <c>EricksonLopez.DomainPrimitives.Mapster.Generated</c>
/// namespace) to automatically register bidirectional mappings for every domain primitive
/// in the assembly — without any reflection at runtime.
/// </para>
/// <para>
/// The source generator runs at compile time and produces a class implementing
/// <see cref="IRegister"/> for all detected domain primitives. This extension
/// simply discovers and applies all <see cref="IRegister"/> implementations.
/// </para>
/// </remarks>
public static class MapsterServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Mapster <see cref="IRegister"/> implementations found by scanning the assembly of the specified marker type
    /// (including the source-generated <c>DomainPrimitivesMapsterRegister</c>) and registers them
    /// with the global <see cref="TypeAdapterConfig.GlobalSettings"/>.
    /// </summary>
    /// <typeparam name="TMarker">A type from the assembly containing your domain primitives.</typeparam>
    /// <param name="config">The <see cref="TypeAdapterConfig"/> to register mappings in.</param>
    /// <returns>The same <see cref="TypeAdapterConfig"/> for chaining.</returns>
    public static TypeAdapterConfig AddDomainPrimitivesMapster<TMarker>(this TypeAdapterConfig config)
    {
        return AddDomainPrimitivesMapster(config, typeof(TMarker).Assembly);
    }

    /// <summary>
    /// Registers all Mapster <see cref="IRegister"/> implementations found by scanning the specified assembly
    /// (including the source-generated <c>DomainPrimitivesMapsterRegister</c>) and registers them
    /// with the global <see cref="TypeAdapterConfig.GlobalSettings"/>.
    /// </summary>
    /// <param name="config">The <see cref="TypeAdapterConfig"/> to register mappings in.</param>
    /// <param name="assembly">The assembly containing your domain primitives.</param>
    /// <returns>The same <see cref="TypeAdapterConfig"/> for chaining.</returns>
    /// <remarks>
    /// <b>NativeAOT / Trimming Warning:</b> <c>config.Scan(assembly)</c> uses reflection
    /// (<c>Assembly.GetTypes()</c>) to discover all <see cref="IRegister"/> implementations.
    /// This is NOT compatible with NativeAOT or aggressive trimming.
    /// <para>
    /// For NativeAOT compliance, the <c>MapsterSourceGenerator</c> emits a concrete
    /// <c>DomainPrimitivesMapsterRegister : IRegister</c> class at compile time.
    /// Register it explicitly: <c>new DomainPrimitivesMapsterRegister().Register(config)</c>
    /// instead of relying on assembly scanning.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("config.Scan() uses reflection to discover IRegister implementations and is not NativeAOT-safe. For NativeAOT, explicitly instantiate the source-generated DomainPrimitivesMapsterRegister.")]
    public static TypeAdapterConfig AddDomainPrimitivesMapster(
        this TypeAdapterConfig config,
        System.Reflection.Assembly assembly)
    {
        // Register all IRegister implementations in the target assembly
        // This picks up the source-generated DomainPrimitivesMapsterRegister automatically.
        // NOTE: config.Scan() uses reflection — see [RequiresUnreferencedCode] above.
        config.Scan(assembly);

        return config;
    }

    /// <summary>
    /// Registers mappings directly from a provided pair of delegates.
    /// Use this when the source generator is not available or for manual override.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="config">The <see cref="TypeAdapterConfig"/>.</param>
    /// <param name="fromValue">Factory delegate to create TPrimitive from TValue.</param>
    /// <param name="toValue">Projection delegate to extract TValue from TPrimitive.</param>
    public static TypeAdapterConfig AddDomainPrimitiveMapping<TPrimitive, TValue>(
        this TypeAdapterConfig config,
        System.Linq.Expressions.Expression<Func<TValue, TPrimitive>> fromValue,
        System.Linq.Expressions.Expression<Func<TPrimitive, TValue>> toValue)
        where TPrimitive : struct
        where TValue : notnull
    {
        config.NewConfig<TValue, TPrimitive>().MapWith(fromValue);
        config.NewConfig<TPrimitive, TValue>().MapWith(toValue);
        return config;
    }
}
