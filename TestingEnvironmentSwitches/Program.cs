using System;

namespace TestingEnvironmentSwitches
{
    class Program
    {
        private static Program M;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n1 - подписаться\n2 - CriticalSituationSwitchCall\n3 - Остановить роспуск\n4 - Рестарт роспуска\n5 - \n11 - \n12 - \n13 - ");
                string inp = Console.ReadLine();
                if (inp == "1")
                {
                    M = new Program();
                    CorrectBehaviorWhenSwitchCriticalSituation a = new CorrectBehaviorWhenSwitchCriticalSituation(10);
                    CriticalSituationSwitchHappened += a.CriticalSituationSwitchHappenedHandler;
                    ChangeSemaphoreHappened += a.SemaphoreChangeHappenedHandler;
                }
                else if (inp == "2")
                {
                    M.CriticalSituationSwitchCall();
                }
                else if (inp == "3")
                {
                    M.DissolutionStopCall();
                }
                else if (inp == "4")
                {
                    M.DissolutionRestartCall();
                }
                else if (inp == "5")
                {
                    
                }
                else if (inp == "11")
                {
                    
                }
                else if (inp == "12")
                {
                    
                }
                else if (inp == "13")
                {
                    
                }
            }
            
            
        }

        void DissolutionRestartCall()
        {
            StateSemaphoreEventArgs args = new StateSemaphoreEventArgs();
            args.ValueColor = SemaphoreColor.Green;
            OnChangeSemaphoreHappened(args);
        }
        void DissolutionStopCall()
        {
            StateSemaphoreEventArgs args = new StateSemaphoreEventArgs();
            args.ValueColor = SemaphoreColor.Red;
            OnChangeSemaphoreHappened(args);
        }
        void CriticalSituationSwitchCall()
        {
            CriticalSituationSwitchEventArgs args = new CriticalSituationSwitchEventArgs(
                new Guid(), 
                "MyNameAnimation",
                TypeDisrepairSwitch.Gab
                );
            OnCriticalSituationSwitchHappened(args);
        }
        protected virtual void OnCriticalSituationSwitchHappened(CriticalSituationSwitchEventArgs e)
        {
            EventHandler<CriticalSituationSwitchEventArgs> handler = CriticalSituationSwitchHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnChangeSemaphoreHappened(StateSemaphoreEventArgs e)
        {
            EventHandler<StateSemaphoreEventArgs> handler = ChangeSemaphoreHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public static event EventHandler<CriticalSituationSwitchEventArgs> CriticalSituationSwitchHappened;
        public static event EventHandler<StateSemaphoreEventArgs> ChangeSemaphoreHappened;
    }
}