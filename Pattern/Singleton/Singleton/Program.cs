using System;

namespace Singleton
{
    public class LazySingleton
    {
        private static readonly Lazy<LazySingleton> Lazy = new Lazy<LazySingleton>(() => new LazySingleton());

        public Guid Id { get; }
        
        private LazySingleton()
        {
            Console.WriteLine($"ctor LazySingleton {DateTime.Now.TimeOfDay}");
            Id = Guid.NewGuid();
        }

        public static LazySingleton Instance
        {
            get
            {
                Console.WriteLine($"GetInstance {DateTime.Now.TimeOfDay}");
                return Lazy.Value;
            }
        }
    }

    static class Program
    {
        static void Main()
        {
            Console.WriteLine($"Main {DateTime.Now.TimeOfDay}");

            var id = LazySingleton.Instance.Id;
            Console.WriteLine($"Id {id}");
        }
    }
}
