using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives
{
    public class DapperAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }
    public class PercentageAttribute : System.Attribute { }
    public class SmartEnumAttribute : System.Attribute { }
    public class SmartEnumAttribute<T> : System.Attribute { }
}
