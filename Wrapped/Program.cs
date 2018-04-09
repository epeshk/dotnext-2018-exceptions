using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

// [assembly: RuntimeCompatibility(WrapNonExceptionThrows = false)]

namespace Wrapped
{
    internal static class ThrowExtension
    {
        private static readonly Action<object> method;
        
        static ThrowExtension()
        {
            var parameter = Expression.Parameter(typeof(object));
            var throwExpr = Expression.Throw(parameter);
            var lambdaExpr = Expression.Lambda<Action<object>>(throwExpr, parameter);
            method = lambdaExpr.Compile();
        }

        public static void Throw<T>(this T e) => method(e);
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                42.Throw();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("!");
        }
    }
}