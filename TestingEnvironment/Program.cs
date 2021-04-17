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
        private static DateTime start_time;
        private static DateTime stopDissolution_time;
        private static DateTime restartDissolution_time;
        private static TypeDisrepairGac typeDisrepair;
        private static SemaphoreColor previousColor;
        private static bool isStarted;
        private static int penaltyScores = 0;
        private const int penaltyMultiplicator = 100;
        
        static void Main(string[] args)
        {
            while (true)
            {
                WriteLine("\n1 - подписаться\n2 - вызвать нештатную ситуацию\n3 - вызвать красный семафор\n4 - брейкмоды\n5 - свичмоды");
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
                if (e.TypeDisrepair != TypeDisrepairGac.None)
                {
                    start_time = DateTime.UtcNow;
                    typeDisrepair = e.TypeDisrepair;
                    WriteLine("Время нештатной ситуации: " + start_time);
                    isStarted = true;
                }
            }

            static void c_StateSemaphoreHappened(object sender, StateSemaphoreEventArgs e)
            { // TODO: Рестарт роспуска - это изменение с красного на любой?
                if (e.ValueColor == SemaphoreColor.Red & isStarted)
                {
                    previousColor = e.ValueColor;
                    stopDissolution_time = DateTime.UtcNow;
                    WriteLine("Время роспуска: " + stopDissolution_time);
                    if ((stopDissolution_time - start_time).Seconds > 15) // 5 секунда норма, +10 сек - без штрафа
                    {
                        penaltyScores += ((stopDissolution_time - start_time).Seconds - 5) / 10 * penaltyMultiplicator;
                    }
                    WriteLine("Штрафные баллы: " + penaltyScores);
                }
                else
                {
                    if (previousColor == SemaphoreColor.Red)
                    { // TODO: Это и есть рестарт?
                        previousColor = e.ValueColor;
                        restartDissolution_time = DateTime.UtcNow;
                        WriteLine("Время рестарта: " + restartDissolution_time);
                        if ((restartDissolution_time - start_time).Seconds > 40) // 30 сек норма, +10сек - без штрафа
                        {
                            penaltyScores += ((restartDissolution_time - start_time).Seconds - 30) / 10 *
                                             penaltyMultiplicator;
                        }
                        WriteLine("Штрафные баллы: " + penaltyScores);
                    }
                }
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
            args.TypeDisrepair = TypeDisrepairGac.ManualBrake;
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