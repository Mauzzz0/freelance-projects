using System;
using static System.Console;

namespace TestingEnvironment
{
    class Program
    {
        private static DateTime start;
        private static DateTime stopDissolution;
        private static DateTime restartDissolution;
        private static int penaltyScores = 0;
        static void Main(string[] args)
        {
            while (true)
            {
                WriteLine("\n1 - подписаться\n2 - вызвать нештатную ситуацию\n3 - вызвать зелёный семафор");
                string inp = ReadLine();
                if (inp == "1")
                {
                    CriticalSituationHappened += c_CriticalSituationHappened;
                    StateSemaphoreHappened += c_StateSemaphoreHappened;
                }
                else if (inp == "2")
                {
                    Program M = new Program();
                    M.CriticalSituationBrokenGACCall();
                }
                else if (inp == "3")
                {
                    Program M = new Program();
                    M.StateSemaphoreRedCall();
                }
            }

            static void c_CriticalSituationHappened(object sender, CriticalSituationGacEventArgs e)
            {
                start = DateTime.Now;
                WriteLine("Нештатная ситуация. "+e.TypeDisrepair+" "+Convert.ToString(start.TimeOfDay));
            }

            static void c_StateSemaphoreHappened(object sender, StateSemaphoreEventArgs e)
            {
                stopDissolution = DateTime.Now;
                double difference = (stopDissolution.TimeOfDay - start.TimeOfDay).TotalSeconds;
                penaltyScores = difference < 15 ? 0 : Convert.ToInt32(Math.Floor(100 * (difference - 5)));
                WriteLine("Остановка роспуска. " + stopDissolution.TimeOfDay);
                WriteLine("Время между событиями: " + difference);
                WriteLine("Штрафные баллы: " + penaltyScores);
                // TODO: Чекнуть BrakeControlMode
            }
        }

        void CriticalSituationBrokenGACCall()
        {
            CriticalSituationGacEventArgs args = new CriticalSituationGacEventArgs();
            args.Message = "НЕШТАТНАЯ СИТУАЦИЯ: ГАЦ МН НЕИСПРАВЕН!";
            args.TypeDisrepair = TypeDisrepairGac.BrokenGAC;
            OnCriticalSituationHappened(args);
        }
        void StateSemaphoreRedCall()
        {
            StateSemaphoreEventArgs args = new StateSemaphoreEventArgs();
            args.Owner = new Role();
            args.Speed = 10.15;
            args.ButtonStage = new StageSemaphore();
            args.IdObj = new Guid();
            args.IdSemaphore = new Guid();
            args.ValueColor = SemaphoreColor.Red;
            OnStateSemaphoreHappened(args);
        }

        protected virtual void OnCriticalSituationHappened(CriticalSituationGacEventArgs e)
        {
            EventHandler<CriticalSituationGacEventArgs> handler = CriticalSituationHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnStateSemaphoreHappened(StateSemaphoreEventArgs e)
        {
            EventHandler<StateSemaphoreEventArgs> handler = StateSemaphoreHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public static event EventHandler<CriticalSituationGacEventArgs> CriticalSituationHappened;
        public static event EventHandler<StateSemaphoreEventArgs> StateSemaphoreHappened;
    }
}