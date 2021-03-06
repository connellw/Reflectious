﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflectious
{
    public class ReflectionMethod : IMethod
    {
        private readonly MethodInfo _methodInfo;

        public ReflectionMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public object Invoke(object instance, object[] args)
        {
            return _methodInfo.Invoke(instance, args);
        }

        public MethodInfo GetMethodInfo()
        {
            return _methodInfo;
        }

        public IEnumerable<Type> GetParameterTypes()
        {
            return _methodInfo.GetParameters().Select(p => p.ParameterType);
        }
    }
}