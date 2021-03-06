﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Reflectious
{
    public class StaticReflector : StaticReflector<object>
    {
        internal StaticReflector(Type type)
            : base(new NullInstanceGetter(type))
        {
        }

        [PublicAPI]
        public new InstanceReflector WithInstance(object instance)
        {
            return new InstanceReflector(new WeakInstanceGetter(instance));
        }

        [PublicAPI]
        public StaticReflector<T> CastTo<T>() where T : class
        {
            return new StaticReflector<T>(InstanceGetter);
        }
    }

    public class StaticReflector<TType> : InstanceReflector<TType>
        where TType : class
    {
        public StaticReflector()
            : base(new NullInstanceGetter(typeof(TType)))
        {
        }
        
        internal StaticReflector(IInstanceGetter instanceGetter)
            : base(instanceGetter)
        {
        }
        
        internal StaticReflector(Type type)
            : base(new NullInstanceGetter(type))
        {
            Debug.Assert(typeof(TType).IsAssignableFrom(type));
        }

        [PublicAPI]
        public ConstructorReflector<TType> GetConstructor()
        {
            var finder = FinderUtility.GetConstructorFinder(InstanceGetter.Type, Assume.Nothing);
            return new ConstructorReflector<TType>(finder);
        }

        [PublicAPI]
        public TType CreateInstance()
        {
            return GetConstructor()
                .WithParameters()
                .Invoke();
        }

        [PublicAPI]
        public TType CreateInstance<TArg1>(TArg1 arg1)
        {
            return GetConstructor()
                .WithParameters<TArg1>()
                .Invoke(arg1);
        }

        [PublicAPI]
        public TType CreateInstance<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            return GetConstructor()
                .WithParameters<TArg1, TArg2>()
                .Invoke(arg1, arg2);
        }

        [PublicAPI]
        public TType CreateInstance<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return GetConstructor()
                .WithParameters<TArg1, TArg2, TArg3>()
                .Invoke(arg1, arg2, arg3);
        }

        [PublicAPI]
        public TType CreateInstance<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return GetConstructor()
                .WithParameters<TArg1, TArg2, TArg3, TArg4>()
                .Invoke(arg1, arg2, arg3, arg4);
        }

        [PublicAPI]
        public TType CreateInstance(params object[] args)
        {
            return GetConstructor()
                //.WithParameters(args.Select(a => a?.GetType())) // already happening in .Invoke
                .Invoke(args);
        }

        [PublicAPI]
        public NewInstanceReflector<TType> WithNewInstance()
        {
            return new NewInstanceReflector<TType>(new ConstructorInstanceGetter(CreateInstance, InstanceGetter.Type));
        }

        [PublicAPI]
        public InstanceReflector<TType> WithInstance(TType instance)
        {
            return new InstanceReflector<TType>(new StrongInstanceGetter<TType>(instance));
        }

        [PublicAPI]
        public InstanceReflector<TType> WithInstance(Func<TType> getInstanceFunc)
        {
            return new InstanceReflector<TType>(new StrongInstanceFuncGetter<TType>(getInstanceFunc));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric(params Type[] types)
        {
            var genericType = InstanceGetter.Type.MakeGenericType(types);
            return new StaticReflector(genericType);
        }
        
        [PublicAPI]
        public StaticReflector MakeGeneric<T1>()
        {
            return MakeGeneric(typeof(T1));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric<T1, T2>()
        {
            return MakeGeneric(typeof(T1), typeof(T2));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric<T1, T2, T3>()
        {
            return MakeGeneric(typeof(T1), typeof(T2), typeof(T3));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric<T1, T2, T3, T4>()
        {
            return MakeGeneric(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric<T1, T2, T3, T4, T5>()
        {
            return MakeGeneric(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric<T1, T2, T3, T4, T5, T6>()
        {
            return MakeGeneric(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }

        [PublicAPI]
        public StaticReflector MakeGeneric(IEnumerable<Type> types)
        {
            return MakeGeneric(types.ToArray());
        }
    }
}