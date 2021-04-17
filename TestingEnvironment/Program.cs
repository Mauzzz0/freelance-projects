using System;
using System.Collections.Generic;
using static System.Console;

namespace TestingEnvironment
{
    /* Сценарии:
     *
     *  1) [Нет нештатной ситуации] ->
     *     [TypeDisrepairGac.None] ->
     * 
     *  2) [Внимание. Тормозить вручную] -> [остановить роспуск 5сек] -> [замедлители ручные 30сек]
     *     [TypeDisrepairGac.ManualBrake] -> [SemaphoreColor.Red]       -> [BrakeModeControl.Manual]
     * 
     *  3) [Внимание!! Любая ситуация] -> [остановить роспуск 5сек] -> [замедлители 30сек]
     *     [TypeDisrepairGac.[ANY]] -> [SemaphoreColor.Red]        -> [BrakeModeControl.Manual] 
     */
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
                WriteLine("\n1 - подписаться\n2 - вызвать нештатную ситуацию\n3 - вызвать зелёный семафор\n4 - брейкмоды\n5 - свичмоды");
                string inp = ReadLine();
                if (inp == "1")
                {
                    CriticalSituationHappened += c_CriticalSituationHappened;
                    StateSemaphoreHappened += c_StateSemaphoreHappened;
                    BrakeModes += c_BrakeModes;
                    SwitchModes += c_SwitcModes;
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
                else if (inp == "4")
                {
                    Program M = new Program();
                    M.BrakeModesManualCall();
                }
                else if (inp == "5")
                {
                    Program M = new Program();
                    M.SwitchModesManualCall();
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
                WriteLine("Роспуск");
            }

            static void c_BrakeModes(object sender, BrakeModesEventArgs e)
            {
                WriteLine("Произошёл вызов тормозов");
            }

            static void c_SwitcModes(object sender, SwitchModesEventArgs e)
            {
                WriteLine("Свичмод");
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

        void BrakeModesManualCall()
        {
            BrakeModesEventArgs args = new BrakeModesEventArgs();
            args.BrakeModes = new Dictionary<Guid, BrakeModeControl>();
            args.BrakeModes[new Guid()] = BrakeModeControl.Manual;
            args.BrakeModes[new Guid()] = BrakeModeControl.Manual;
            args.BrakeModes[new Guid()] = BrakeModeControl.Manual;
            OnBrakeModesHappened(args);
        }

        void SwitchModesManualCall()
        {
            SwitchModesEventArgs args = new SwitchModesEventArgs();
            args.SwitchModes = new Dictionary<Guid, SwitchModeControl>();
            args.SwitchModes[new Guid()] = SwitchModeControl.Manual;
            args.SwitchModes[new Guid()] = SwitchModeControl.Manual;
            OnSwitchModesHappened(args);
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

        protected virtual void OnBrakeModesHappened(BrakeModesEventArgs e)
        {
            EventHandler<BrakeModesEventArgs> handler = BrakeModes;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSwitchModesHappened(SwitchModesEventArgs e)
        {
            EventHandler<SwitchModesEventArgs> handler = SwitchModes;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        public static event EventHandler<CriticalSituationGacEventArgs> CriticalSituationHappened;
        public static event EventHandler<StateSemaphoreEventArgs> StateSemaphoreHappened;
        public static event EventHandler<BrakeModesEventArgs> BrakeModes;
        public static event EventHandler<SwitchModesEventArgs> SwitchModes;
    }
}