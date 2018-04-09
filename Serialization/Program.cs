using System;
using System.Runtime.Serialization;

namespace Serialization
{
    public class CustomException : Exception { }
    
    [Serializable]
    public class CustomSerializableException : Exception { }

    [Serializable]
    public class CustomSerializableExceptionWithCtor : Exception
    {
        public CustomSerializableExceptionWithCtor() { }
        
        public CustomSerializableExceptionWithCtor(SerializationInfo info, StreamingContext context)
            : base(info, context){}
    }
    
    internal class Program
    {
        private static void ThrowInOtherDomain<T>(AppDomain appDomain)
            where T : Exception, new()
        {
            try {
                appDomain.DoCallBack(() => throw new T());
            }
            catch (Exception e) {
                Console.WriteLine($"Expected: {typeof(T).Name}, Actual: {e.GetType().Name}");
            }
        }
        public static void Main(string[] args)
        {
            var appd = AppDomain.CreateDomain(Guid.NewGuid().ToString());
            ThrowInOtherDomain<Exception>(appd);
            ThrowInOtherDomain<CustomException>(appd);
            ThrowInOtherDomain<CustomSerializableException>(appd);
            ThrowInOtherDomain<CustomSerializableExceptionWithCtor>(appd);
        }
    }
}