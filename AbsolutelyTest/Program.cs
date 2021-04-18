using System;

namespace AbsolutelyTest
{
    class EA : EventArgs
    {
        public string Message { get; set; }
        
    }
    class Program
    {
        static void Main(string[] args)
        {
            Person p = new Person();
            Console.WriteLine("Hello World!");
            Program m = new Program();
            getsss += c_aa;
            m.call();
        }

        static void c_aa(object sender, EA e)
        {
            Console.WriteLine("СРАБОТАЛО ЕБАТЬ " + e.Message);
        }
        
        public void call()
        {
            EA args = new EA();
            args.Message = "ЖОПА ХУЙ";
            OnEventHappened(args);
        }
        protected virtual void OnEventHappened(EA e)
        {
            EventHandler<EA> handler = getsss;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public static event EventHandler<EA> getsss;
    }

    class Person
    {
        private static string Name { get; set; }
        private static int Age { get; set; }

        public void method(object sender, EA e)
        {
            Console.WriteLine("СРАБОТАЛО ИЗ КЛАССА");
        }
    }
}