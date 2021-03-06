using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Reflectious.Tests
{
    public class InvokerTests
    {
        [Fact]
        public void GetMethod_MethodExpression_CorrectValue()
        {
            var stub = new Stub();
            
            string returnValue = Reflect.Instance(stub)
                .GetMethod(s => s.DoInstanceMethod())
                .Invoke();
            
            Assert.Equal(Stub.MethodExecutedString, returnValue);
        }

        [Fact]
        public void GetMethod_Nameof_CorrectValue()
        {
            var stub = new Stub();

            string returnValue = Reflect.Instance(stub)
                .GetMethod(nameof(Stub.DoInstanceMethod))
                .ReturnsType<string>()
                .Invoke();

            Assert.Equal(Stub.MethodExecutedString, returnValue);
        }

        [Fact]
        public void GetVoidMethod_Nameof_Executes()
        {
            var stub = new Stub();

            Reflect.Instance(stub)
                .GetMethod(nameof(Stub.DoInstanceVoidMethod))
                .Invoke("Test arg");

            Assert.Equal("Test arg", stub.InstanceProperty);
        }

        [Fact]
        public void GetProperty_Expression_CorrectValue()
        {
            var stub = new Stub();
            
            string returnValue = Reflect.Instance(stub)
                .GetProperty(s => s.InstanceProperty)
                .GetValue();
            
            Assert.Equal(Stub.InitialPropertyValue, returnValue);
        }
        
        [Fact]
        public void SetProperty_Expression_ChangesValue()
        {
            var stub = new Stub();
            
            Reflect.Instance(stub)
                .GetProperty(s => s.InstanceProperty)
                .SetValue("Test change");
            
            Assert.Equal("Test change", stub.InstanceProperty);
        }
        
        [Fact]
        public void SetStaticProperty_Expression_ChangesValue()
        {            
            Reflect.Type<Stub>()
                .GetProperty(nameof(Stub.StaticProperty))
                .OfType<string>()
                .SetValue("Test change");
            
            Assert.Equal("Test change", Stub.StaticProperty);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetStaticMethod_LinqEnumerableType_FindsAnyMethod(bool value)
        {
            var stub = new Stub();
            
            var result = Reflect.Type(typeof(Enumerable))
                .GetMethod(nameof(Enumerable.Any))
                .ReturnsType<bool>()
                .MakeGeneric<Stub>()
                .WithParameters<IEnumerable<Stub>, Func<Stub, bool>>()
                .Invoke(new[] { stub }, s => value);
            
            Assert.Equal(value, result);
        }
        
        [Fact]
        public void ConstructorMakeGeneric_GenericList_CreatesObject()
        {
            var result = Reflect.Type(typeof(List<>))
                .GetConstructor()
                .MakeGeneric<Stub>()
                .Invoke();
            
            Assert.IsType<List<Stub>>(result);
        }
        
        [Fact]
        public void ConstructorWithParameter_GenericList_CreatesObject()
        {
            var arr = new[] {new Stub()};

            var result = Reflect.Type(typeof(List<>))
                .GetConstructor()
                .MakeGeneric<Stub>()
                .Invoke(arr.AsEnumerable());

            var castedResult = Assert.IsType<List<Stub>>(result);
            Assert.Equal(arr[0], castedResult[0]);
        }
        
        [Fact]
        public void TypeMadeGenericConstructor_GenericList_CreatesObject()
        {
            var result = Reflect.Type(typeof(List<>))
                .MakeGeneric<Stub>()
                .GetConstructor()
                .WithParameters()
                .Invoke();
            
            Assert.IsType<List<Stub>>(result);
        }
        
        [Fact]
        public void AlreadyGenericTypeConstructor_GenericList_CreatesObject()
        {
            var result = Reflect.Type(typeof(List<Stub>))
                .GetConstructor()
                .WithParameters()
                .Invoke();
            
            Assert.IsType<List<Stub>>(result);
        }
        
        [Fact]
        public void StrongStaticInvoker_GenericList_CreatesObject()
        {
            var result = Reflect.Type<List<Stub>>()
                .GetConstructor()
                .WithParameters()
                .Invoke();
            
            Assert.IsType<List<Stub>>(result);
        }
        
        [Fact]
        public void StrongStaticCreateInstance_GenericList_CreatesObject()
        {
            var result = Reflect.Type<List<Stub>>()
                .CreateInstance();
            
            Assert.IsType<List<Stub>>(result);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExtensionMethod_LinqAny_ReturnsPredicate(bool predicateValue)
        {
            IEnumerable<Stub> stubs = new[] { new Stub() };

            bool hasAny = Reflect.Instance(stubs)
                .GetExtensionMethod(typeof(Enumerable), "Any")
                .ReturnsType<bool>()
                .WithParameters<Func<Stub, bool>>()
                .Invoke(p => predicateValue);
            
            Assert.Equal(predicateValue, hasAny);
        }
    }
}