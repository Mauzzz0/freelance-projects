using System;
using static System.Console;

namespace TestingEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            
            while (true)
            {
                WriteLine("1 - подписаться\n2-вызвать событие");
                string inp = Console.ReadLine();
                if (inp == "1")
                {
                    CriticalSituationHappened += c_CriticalSituationHappened;
                }
                else if (inp == "2")
                {
                    Program M = new Program();
                    M.EventCall();
                }
            }

            static void c_CriticalSituationHappened(object sender, CriticalSituationGacEventArgs e)
            {
                Console.WriteLine("Событие произошло");
            }
        }

        void EventCall()
        {
            CriticalSituationGacEventArgs args = new CriticalSituationGacEventArgs();
            args.Message = "НЕШТАТНАЯ СИТУАЦИЯ: ГАЦ МН НЕИСПРАВЕН!";
            args.TypeDisrepair = TypeDisrepairGac.BrokenGAC;
            OnCriticalSituationHappened(args);
        }

        protected virtual void OnCriticalSituationHappened(CriticalSituationGacEventArgs e)
        {
            EventHandler<CriticalSituationGacEventArgs> handler = CriticalSituationHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public static event EventHandler<CriticalSituationGacEventArgs> CriticalSituationHappened;
    }
}