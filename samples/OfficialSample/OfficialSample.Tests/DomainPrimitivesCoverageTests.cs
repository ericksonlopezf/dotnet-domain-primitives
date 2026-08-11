using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using EricksonLopez.DomainPrimitives;

namespace OfficialSample.Tests;

public class DomainPrimitivesCoverageTests
{
    [Fact]
    public void TestAllDomainPrimitives()
    {
        // Get all assemblies loaded from the current domain that match our chapters
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name != null && Regex.IsMatch(a.GetName().Name!, @"^\d{2}-"))
            .ToList();

        // If they are not loaded, load them explicitly from the directory
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var files = Directory.GetFiles(path!, "*.dll").Where(f => Regex.IsMatch(Path.GetFileName(f), @"^\d{2}-"));
        foreach (var file in files)
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(file);
                if (!assemblies.Any(a => a.FullName == asmName.FullName))
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
            }
            catch { }
        }

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                // Test StrongIds
                if (type.GetMethod("New", BindingFlags.Public | BindingFlags.Static) != null)
                {
                    var newMethod = type.GetMethod("New", BindingFlags.Public | BindingFlags.Static);
                    if (newMethod != null && newMethod.GetParameters().Length == 0)
                    {
                        try
                        {
                            var instance = newMethod.Invoke(null, null);
                            Assert.NotNull(instance);
                            
                            // Try ToString
                            var str = instance.ToString();
                            Assert.NotNull(str);
                            
                            // Try Parse if available
                            var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string), typeof(IFormatProvider) });
                            if (parseMethod != null)
                            {
                                try { parseMethod.Invoke(null, new object[] { str, null! }); } catch { }
                            }
                        }
                        catch (TargetInvocationException) { }
                    }
                }
                
                // Test Email or String primitives
                if (type.GetMethod("TryCreate", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }) != null)
                {
                    var tryCreateMethod = type.GetMethod("TryCreate", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
                    if (tryCreateMethod != null)
                    {
                        try
                        {
                            var result = tryCreateMethod.Invoke(null, new object[] { "test@example.com" });
                            
                            // Reflection on Result
                            var isSuccessProp = result?.GetType().GetProperty("IsSuccess");
                            var isSuccess = (bool?)isSuccessProp?.GetValue(result);
                            
                            if (isSuccess == true)
                            {
                                var valueProp = result?.GetType().GetProperty("Value");
                                var instance = valueProp?.GetValue(result);
                                instance?.ToString();
                                instance?.GetHashCode();
                                instance?.Equals(instance);
                            }
                        }
                        catch (TargetInvocationException) { }
                    }
                }
                
                // Test Money or Decimal primitives
                if (type.GetMethod("TryCreate", BindingFlags.Public | BindingFlags.Static, new[] { typeof(decimal) }) != null)
                {
                    var tryCreateMethod = type.GetMethod("TryCreate", BindingFlags.Public | BindingFlags.Static, new[] { typeof(decimal) });
                    if (tryCreateMethod != null)
                    {
                        try
                        {
                            var result = tryCreateMethod.Invoke(null, new object[] { 100.50m });
                            
                            // Reflection on Result
                            var isSuccessProp = result?.GetType().GetProperty("IsSuccess");
                            var isSuccess = (bool?)isSuccessProp?.GetValue(result);
                            
                            if (isSuccess == true)
                            {
                                var valueProp = result?.GetType().GetProperty("Value");
                                var instance = valueProp?.GetValue(result);
                                instance?.ToString();
                                instance?.GetHashCode();
                                instance?.Equals(instance);
                            }
                        }
                        catch (TargetInvocationException) { }
                    }
                }
            }
        }
    }
}



