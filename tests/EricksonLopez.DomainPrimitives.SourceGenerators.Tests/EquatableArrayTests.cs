// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    public class EquatableArrayTests
    {
        [Fact]
        public void EquatableArray_EqualityWorks()
        {
            var arr1 = new EquatableArray<int>(new[] { 1, 2, 3 });
            var arr2 = new EquatableArray<int>(new[] { 1, 2, 3 });
            var arr3 = new EquatableArray<int>(new[] { 1, 2, 4 });
            var arrEmpty1 = new EquatableArray<int>(new int[0]);
            var arrEmpty2 = new EquatableArray<int>(System.Array.Empty<int>());

            arr1.Equals(arr2).Should().BeTrue();
            arr1.Equals(arr3).Should().BeFalse();

            arr1.GetHashCode().Should().Be(arr2.GetHashCode());
            arr1.GetHashCode().Should().NotBe(arr3.GetHashCode());

            arrEmpty1.Equals(arrEmpty2).Should().BeTrue();
            arrEmpty1.GetHashCode().Should().Be(arrEmpty2.GetHashCode());
            
            arrEmpty1.Values.Should().BeEmpty();
            
            arr1.Length.Should().Be(3);
            arr1[0].Should().Be(1);
        }
        
        [Fact]
        public void EquatableArray_EqualsWithObject()
        {
            var arr = new EquatableArray<int>(new[] { 1 });
            arr.Equals((object)arr).Should().BeTrue();
            arr.Equals(new object()).Should().BeFalse();
        }
    }
}


