using System;
using System.Collections.Generic;
using static System.Console;
// using cbc = CorrectBehaviorWhenCriticalSituationGAC.CorrectBehaviorCheck;


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
        private static Program M;
        
        static void Main(string[] args)
        {
            while (true)
            {
                WriteLine("\n1 - подписаться\n2 - BrokenGac\n3 - ManualBrake\n4 - RedCall\n5 - GreenCall");
                string inp = ReadLine();
                if (inp == "1")
                {
                    M = new Program();
                    CorrectBehaviorCheck a = new CorrectBehaviorCheck();
                    CriticalSituationHappened += a.CriticalSituationHappened;
                    StateSemaphoreHappened += a.StateSemaphoreHappened;
                    BrakeModes += a.BrakeModesHappened;
                    SwitchModes += a.SwitchModesHappened;
                    GetObjectStatesHappened += c_GetObjectStatesHappened;
                }
                else if (inp == "2")
                {
                    M.CriticalSituationBrokenGACCall();
                }
                else if (inp == "3")
                {
                    M.CriticalSituationManualBrakeCall();
                }
                else if (inp == "4")
                {
                    M.StateSemaphoreRedCall();
                }
                else if (inp == "5")
                {
                    M.StateSemaphoreGreenCall();
                }
            }

            void c_CriticalSituationHappened(object sender, CriticalSituationGacEventArgs e)
            {
                if (e.TypeDisrepair != TypeDisrepairGac.None)
                {
                    start_time = DateTime.UtcNow;
                    typeDisrepair = e.TypeDisrepair;
                    WriteLine("Crit sit time: " + start_time);
                    WriteLine("Crit:" + e.TypeDisrepair);
                    isStarted = true;
                }
            }

            void c_StateSemaphoreHappened(object sender, StateSemaphoreEventArgs e)
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
                        WriteLine((restartDissolution_time - start_time).Seconds);
                        if ((restartDissolution_time - start_time).Seconds > 40) // 30 сек норма, +10сек - без штрафа
                        {
                            penaltyScores += ((restartDissolution_time - start_time).Seconds - 30) / 10 *
                                             penaltyMultiplicator;
                        }
                        WriteLine("Штрафные баллы: " + penaltyScores);
                        M.GetObjectStatesHappenedCall();
                    }
                }
            }

            void c_GetObjectStatesHappened(object sender, GetObjectStatesEventArgs e)
            {
                WriteLine("Генерации списков");
                BrakeModesEventArgs argsB = new BrakeModesEventArgs();
                argsB.BrakeModes = new Dictionary<Guid, BrakeModeControl>();
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Manual;
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Automatic;
                SwitchModesEventArgs argsS = new SwitchModesEventArgs();
                argsS.SwitchModes = new Dictionary<Guid, SwitchModeControl>();
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Automatic;
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Manual;
                M.OnBrakeModesHappened(argsB);
                M.OnSwitchModesHappened(argsS);
            }

            static void c_BrakeModes(object sender, BrakeModesEventArgs e)
            {
                int NumberOfIncorrectBrakes = 0;
                WriteLine("Brakes:");
                foreach (KeyValuePair<Guid,BrakeModeControl> Brake in e.BrakeModes)
                {
                    WriteLine("-"+Brake.Value);
                    if (Brake.Value != BrakeModeControl.Manual)
                    {
                        NumberOfIncorrectBrakes++;
                    }
                }
                penaltyScores += NumberOfIncorrectBrakes * penaltyMultiplicator;
                WriteLine("incorrect brakes:" + NumberOfIncorrectBrakes);
            }

            static void c_SwitcModes(object sender, SwitchModesEventArgs e)
            {
                //#if (typeDisrepair != TypeDisrepairGac.ManualBrake)
                //{ // TODO: Срабатывание ГАЦ МН, затем ТОРМОЗИТЬ ВРУЧНУЮ приведёт к нон-чеку этой секции
                    int NumberOfIncorrectSwitches = 0;
                    WriteLine("Switches:");
                    foreach (KeyValuePair<Guid, SwitchModeControl> Switch in e.SwitchModes)
                    {
                        WriteLine("-"+Switch.Value);
                        if (Switch.Value != SwitchModeControl.Manual)
                        {
                            NumberOfIncorrectSwitches++;
                        }
                    }

                    penaltyScores += NumberOfIncorrectSwitches * penaltyMultiplicator;
                    WriteLine("incorrect switches:" + NumberOfIncorrectSwitches);
                //}
            }
        }

        void CriticalSituationBrokenGACCall()
        {
            CriticalSituationGacEventArgs args = new CriticalSituationGacEventArgs();
            args.Message = "НЕШТАТНАЯ СИТУАЦИЯ: ГАЦ МН НЕИСПРАВЕН!";
            args.TypeDisrepair = TypeDisrepairGac.BrokenGAC;
            OnCriticalSituationHappened(args);
        }

        void CriticalSituationManualBrakeCall()
        {
            CriticalSituationGacEventArgs args = new CriticalSituationGacEventArgs();
            args.Message = "НЕШТАТНАЯ СИТУАЦИЯ: ТОРМОЗИТЬ ВРУЧНУЮ!";
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
        void StateSemaphoreGreenCall()
        {
            StateSemaphoreEventArgs args = new StateSemaphoreEventArgs();
            args.Owner = new Role();
            args.Speed = 10.15;
            args.ButtonStage = new StageSemaphore();
            args.IdObj = new Guid();
            args.IdSemaphore = new Guid();
            args.ValueColor = SemaphoreColor.Green;
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

        public void GetObjectStatesHappenedCall()
        {
            WriteLine("Объекты запрошены");
            GetObjectStatesEventArgs args = new GetObjectStatesEventArgs();
            OnGetObjectStatesHappened(args);
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

        protected virtual void OnGetObjectStatesHappened(GetObjectStatesEventArgs e)
        {
            EventHandler<GetObjectStatesEventArgs> handler = GetObjectStatesHappened;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        public static event EventHandler<CriticalSituationGacEventArgs> CriticalSituationHappened;
        public static event EventHandler<StateSemaphoreEventArgs> StateSemaphoreHappened;
        public static event EventHandler<BrakeModesEventArgs> BrakeModes;
        public static event EventHandler<SwitchModesEventArgs> SwitchModes;
        public static event EventHandler<GetObjectStatesEventArgs> GetObjectStatesHappened;
    }
}