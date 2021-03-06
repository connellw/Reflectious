﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reflectious
{
    internal class ActivatorConstructor : IMethod
    {
        private readonly Type _type;

        public ActivatorConstructor(Type type)
        {
            _type = type;
        }

        public object Invoke(object instance, object[] args)
        {
            if (instance != null)
                throw new InvalidOperationException("A constructor cannot be called on an object that has already been instantiated.");

            return Activator.CreateInstance(_type, args);
        }

        public MethodInfo GetMethodInfo()
        {
            throw new NotSupportedException("Cannot get method info for an Activator constructor.");
        }

        public IEnumerable<Type> GetParameterTypes()
        {
            throw new NotSupportedException("Cannot get parameter types for an Activator constructor.");
        }
    }
}