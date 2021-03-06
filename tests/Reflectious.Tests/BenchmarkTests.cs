using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Reflectious.Tests
{
    public class BenchmarkTests
    {
        [Fact(Skip = "Doesn't work yet")]
        public void InvokeMethod_Stub_AsFastAsReflection()
        {
            var stub = new Stub();
            const string methodName = nameof(Stub.DoInstanceMethod);

            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        string returnValue = Reflect.Instance(stub)
                            .GetMethod(methodName, Assume.UnambiguousName)
                            .ReturnsType<string>()
                            .Invoke();
                    },
                    NativeCode = () =>
                    {
                        string returnValue = (string) typeof(Stub)
                            .GetMethod(methodName)
                            .Invoke(stub, new object[] { });
                    }
                }
                .AssertFasterOrEqual();
        }
        
        [Fact(Skip = "Doesn't work yet")]
        public void ConstructObject_Stub_AsFastAsReflection()
        {
            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        Stub stub = Reflect.Type<Stub>()
                            .GetConstructor()
                            .Invoke();
                    },
                    NativeCode = () =>
                    {
                        Stub stub = (Stub)typeof(Stub)
                            .GetConstructor(new Type[] { })
                            .Invoke(new object[] { });
                    }
                }
                .AssertFasterOrEqual();
        }
        
        [Fact(Skip = "Doesn't work yet")]
        public void ConstructObject_Stub_AsFastAsActivator()
        {
            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        Stub stub = Reflect.Type<Stub>()
                            .CreateInstance();
                    },
                    NativeCode = () =>
                    {
                        Stub stub = Activator.CreateInstance<Stub>();
                    }
                }
                .AssertFasterOrEqual();
        }

        [Fact(Skip = "Doesn't work yet")]
        public void MakeGeneric_ListOfStubs_AsFastAsReflection()
        {
            var stubType = typeof(Stub);
            
            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        int count = Reflect.Type(typeof(List<>))
                            .MakeGeneric(stubType)
                            .WithNewInstance()
                            .GetProperty("Count")
                            .OfType<int>()
                            .GetValue();
                    },
                    NativeCode = () =>
                    {
                        var type = typeof(List<>).MakeGenericType(stubType);
                        var list = Activator.CreateInstance(type);
                        int count = (int)type.GetProperty("Count").GetValue(list);
                    }
                }
                .AssertFasterOrEqual();
        }

        [Fact]
        public void FindMethod_Stub_AsFastAsReflection()
        {
            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        MethodInfo anyMethod = Reflect.Type(typeof(Enumerable))
                            .GetMethod("Any")
                            .MakeGeneric<Stub>()
                            .WithParameters<IEnumerable<Stub>, Func<Stub, bool>>() // TODO handle List and arrays etc ?
                            .MethodInfo;
                    },
                    NativeCode = () =>
                    {
                        MethodInfo anyMethod = GetGenericMethod(typeof(Enumerable), "Any", new[] {typeof(Stub)},
                            new[] {typeof(IEnumerable<Stub>), typeof(Func<Stub, bool>)},
                            BindingFlags.Static | BindingFlags.Public);
                    }
                }
                .AssertFasterOrEqual();

            MethodInfo GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] paramTypes, BindingFlags flags)
            {
                IEnumerable<MethodInfo> methods = type.GetMethods(flags)
                    .Where(m => m.Name == name)
                    .Where(m => m.GetGenericArguments().Length == typeArgs.Length)
                    .Where(m => m.GetParameters().Length == paramTypes.Length)
                    .Select(m => m.MakeGenericMethod(typeArgs));

                return methods.First();
            }
        }

        [Fact(Skip = "Doesn't work yet")]
        public void CreateComplexInstance_StringAndExpression_AsFastAsReflection()
        {
            var propertyReturnType = typeof(string);
            Expression<Func<Stub, string>> getLengthExpr = s => s.InstanceProperty;
            
            new BenchmarkActions
                {
                    LibraryCode = () =>
                    {
                        ITypeParamStub<Stub> reader = Reflect.Type(typeof(TypeParamStub<,>))
                            .MakeGeneric(typeof(Stub), propertyReturnType)
                            .CastTo<ITypeParamStub<Stub>>()
                            .CreateInstance(getLengthExpr);
                    },
                    NativeCode = () =>
                    {
                        Type readerFieldValueType = typeof(TypeParamStub<,>).MakeGenericType(typeof(Stub), propertyReturnType);
                        ITypeParamStub<Stub> reader = (ITypeParamStub<Stub>) Activator.CreateInstance(readerFieldValueType, getLengthExpr);
                    }
                }
                .AssertFasterOrEqual();
        }

        private class BenchmarkActions
        {
            public Action LibraryCode { get; set; }
            public Action NativeCode { get; set; }

            public void AssertFasterOrEqual()
            {
                BenchmarkTests.AssertFasterOrEqual(Benchmark(LibraryCode), Benchmark(NativeCode));
            }
        }

        private static long Benchmark(Action action, int iterations = 100000)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                action.Invoke();
            }

            stopwatch.Stop();
            return stopwatch.ElapsedTicks / iterations;
        }

        private static void AssertFasterOrEqual(long library, long native)
        {
            double ratio = (double)library / native;
            
            Assert.True(library <= native,
                $"{ratio:#.0}x slower than native. Library {library} ticks. Native {native} ticks.");
        }
    }

    public interface ITypeParamStub<T>
    {
    }

    public class TypeParamStub<T, T1> : ITypeParamStub<T>
    {
        private readonly Expression<Func<T, T1>> _expr;

        public TypeParamStub(Expression<Func<T, T1>> expr)
        {
            _expr = expr;
        }
    }
}