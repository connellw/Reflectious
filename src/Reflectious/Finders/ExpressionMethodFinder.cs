﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflectious
{
    internal class ExpressionMethodFinder<TSource, TReturn> : IMethodFinder
    {
        private readonly MethodInfo _methodInfo;

        public ExpressionMethodFinder(Expression<Func<TSource, TReturn>> expression)
        {
            _methodInfo = Reflect.Expression(expression).GetMethodInfo();
        }

        public Type[] GenericArguments
        {
            get { throw new NotSupportedException(); }
            set { }

        }

        public Type[] ParameterTypes
        {
            get { throw new NotSupportedException(); }
            set { }
        }

        public bool WantsParameterTypes { get; } = false;

        public string GetCacheKey()
        {
            return _methodInfo.DeclaringType.FullName + "." + _methodInfo.Name;
        }

        public IMethod Find()
        {
            return new ReflectionMethod(_methodInfo);
        }
    }
}