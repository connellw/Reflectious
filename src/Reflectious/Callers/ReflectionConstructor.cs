﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflectious
{
    internal class ReflectionConstructor : IMethod
    {
        private readonly ConstructorInfo _ctorInfo;

        public ReflectionConstructor(ConstructorInfo ctorInfo)
        {
            _ctorInfo = ctorInfo;
        }

        public object Invoke(object instance, object[] args)
        {
            if (instance != null)
                throw new InvalidOperationException("A constructor cannot be called on an object that has already been instantiated.");

            return _ctorInfo.Invoke(args);
        }

        public MethodInfo GetMethodInfo()
        {
            throw new InvalidOperationException("Cannot get the MethodInfo for a constructor.");
        }

        public IEnumerable<Type> GetParameterTypes()
        {
            return _ctorInfo.GetParameters().Select(p => p.ParameterType);
        }
    }
}