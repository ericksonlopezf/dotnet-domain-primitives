using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OfficialSample.Tests;

public class ExecuteAllProgramsTests
{
    [Fact]
    public void ExecuteAllMains()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var exeFiles = Directory.GetFiles(basePath!, "*.dll").Where(f => Regex.IsMatch(Path.GetFileName(f), @"^\d{2}-"));
        
        foreach (var file in exeFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                var programType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Program");
                if (programType != null)
                {
                    var mainMethod = programType.GetMethod("<Main>$", BindingFlags.NonPublic | BindingFlags.Static) ?? 
                                     programType.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    
                    if (mainMethod != null)
                    {
                        var parameters = mainMethod.GetParameters();
                        if (parameters.Length == 0)
                        {
                            mainMethod.Invoke(null, null);
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
                        {
                            mainMethod.Invoke(null, new object[] { Array.Empty<string>() });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing {file}: {ex}");
            }
        }
    }
}



