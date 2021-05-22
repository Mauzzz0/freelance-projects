using System;
using System.Collections.Generic;
using System.Threading;
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
                WriteLine("1 - ручное логирование, 2 - автотесты");
                string inp = ReadLine();
                if (inp == "1")
                {
                    WriteLine(
                        "\n1 - подписаться\n2 - BrokenGac\n3 - ManualBrake\n4 - RedCall\n5 - GreenCal\n11 - 3.2.1\n12 - 3.2.2\n13 - 3.2.3");
                    inp = ReadLine();
                    if (inp == "1")
                    {
                        M = new Program();
                        CorrectBehaviorCheck a = new CorrectBehaviorCheck();
                        CriticalSituationHappened += a.CriticalSituationHappened;
                        StateSemaphoreHappened += a.StateSemaphoreHappened;
                        BrakeModes += a.BrakeModesHappened;
                        SwitchModes += a.SwitchModesHappened;
                        GetObjectStatesHappened += c_GetObject4AStatesHappened;
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
                    else if (inp == "11")
                    {
                        M.CriticalSituationBrokenGACCall();
                        Thread.Sleep(4000);
                        M.StateSemaphoreRedCall();
                        Thread.Sleep(7000);
                        M.CriticalSituationManualBrakeCall();
                        Thread.Sleep(45000);
                        M.StateSemaphoreGreenCall();
                        WriteLine();
                    }
                    else if (inp == "12")
                    {
                        M.CriticalSituationManualBrakeCall();
                        Thread.Sleep(4000);
                        M.StateSemaphoreRedCall();
                        Thread.Sleep(26000);
                        M.CriticalSituationBrokenGACCall();
                        Thread.Sleep(15000);
                        M.StateSemaphoreGreenCall();
                    }
                    else if (inp == "13")
                    {
                        M.CriticalSituationManualBrakeCall();
                        Thread.Sleep(4000);
                        //Thread.Sleep(2000);
                        M.StateSemaphoreRedCall();
                        Thread.Sleep(56000);
                        //Thread.Sleep(10000);
                        M.CriticalSituationBrokenGACCall();
                        Thread.Sleep(45000);
                        //Thread.Sleep(4000);
                        M.StateSemaphoreGreenCall();
                    }
                }
                else
                {
                    M = new Program();
                    CorrectBehaviorCheck a = new CorrectBehaviorCheck();
                    CriticalSituationHappened += a.CriticalSituationHappened;
                    StateSemaphoreHappened += a.StateSemaphoreHappened;
                    BrakeModes += a.BrakeModesHappened;
                    SwitchModes += a.SwitchModesHappened;
                    GetObjectStatesHappened += c_GetObject4MStatesHappened;
                    
                    WriteLine("====Тест 1====");
                    WriteLine("Срабатывание гац мн, роспуск, срабатывание тормозить вручную, рестарт, всё в корректных положениях.");
                    WriteLine("Периоды действий: 4/7/45 секунд");
                    WriteLine("Должно получиться 100 штрафных баллов");
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(4000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(7000);
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(45000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 100)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                    
                    a.reset();
                    GetObjectStatesHappened -= c_GetObject4MStatesHappened;
                    GetObjectStatesHappened += c_GetObject4AStatesHappened;
                    WriteLine("====Тест 2====");
                    WriteLine("Срабатывание гац мн, роспуск, срабатывание тормозить вручную, рестарт, 4 элемента не в корректных положениях.");
                    WriteLine("Периоды действий: 4/7/45 секунд");
                    WriteLine("Должно получиться 500 штрафных баллов");
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(4000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(7000);
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(45000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 500)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                    
                    a.reset();
                    GetObjectStatesHappened -= c_GetObject4AStatesHappened;
                    GetObjectStatesHappened += c_GetObject4MStatesHappened;
                    WriteLine("====Тест 3====");
                    WriteLine("Срабатывание тормозить вручную, роспуск, срабатывание гац мн, рестарт, всё в корректных положениях.");
                    WriteLine("Периоды действий: 4/26/15 секунд");
                    WriteLine("Должно получиться 0 штрафных баллов");
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(4000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(26000);
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(15000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 0)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                    
                    a.reset();
                    GetObjectStatesHappened -= c_GetObject4MStatesHappened;
                    GetObjectStatesHappened += c_GetObject4AStatesHappened;
                    WriteLine("====Тест 4====");
                    WriteLine("Срабатывание тормозить вручную, роспуск, срабатывание гац мн, рестарт, 4 элемента не в корректных положениях.");
                    WriteLine("Периоды действий: 4/26/15 секунд");
                    WriteLine("Должно получиться 400 штрафных баллов");
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(4000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(26000);
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(15000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 400)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                    
                    a.reset();
                    GetObjectStatesHappened -= c_GetObject4AStatesHappened;
                    GetObjectStatesHappened += c_GetObject4MStatesHappened;
                    WriteLine("====Тест 5====");
                    WriteLine("Срабатывание тормозить вручную, роспуск, срабатывание гац мн, рестарт, все элементы в корректных положениях.");
                    WriteLine("Периоды действий: 4/56/45 секунд");
                    WriteLine("Должно получиться 400 штрафных баллов");
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(4000);
                    //Thread.Sleep(2000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(56000);
                    //Thread.Sleep(10000);
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(45000);
                    //Thread.Sleep(4000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 400)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                    
                    
                    a.reset();
                    GetObjectStatesHappened -= c_GetObject4MStatesHappened;
                    GetObjectStatesHappened += c_GetObject4AStatesHappened;
                    WriteLine("====Тест 6====");
                    WriteLine("Срабатывание тормозить вручную, роспуск, срабатывание гац мн, рестарт, все элементы в корректных положениях.");
                    WriteLine("Периоды действий: 4/56/45 секунд");
                    WriteLine("Должно получиться 800 штрафных баллов");
                    M.CriticalSituationManualBrakeCall();
                    Thread.Sleep(4000);
                    //Thread.Sleep(2000);
                    M.StateSemaphoreRedCall();
                    Thread.Sleep(56000);
                    //Thread.Sleep(10000);
                    M.CriticalSituationBrokenGACCall();
                    Thread.Sleep(45000);
                    //Thread.Sleep(4000);
                    M.StateSemaphoreGreenCall();
                    M.GetObjectStatesHappenedCall();
                    WriteLine("Получилось: " + a.penaltyScores);
                    if (a.penaltyScores == 800)
                    {
                        WriteLine("===Тест пройден===");
                    }
                    else
                    {
                        WriteLine("===Тест не пройден===");
                    }
                    WriteLine();
                }
            }

            

            

            void c_GetObject4AStatesHappened(object sender, GetObjectStatesEventArgs e)
            {
                WriteLine("Генерации списков");
                BrakeModesEventArgs argsB = new BrakeModesEventArgs();
                argsB.BrakeModes = new Dictionary<Guid, BrakeModeControl>();
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Automatic;
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Automatic;
                SwitchModesEventArgs argsS = new SwitchModesEventArgs();
                argsS.SwitchModes = new Dictionary<Guid, SwitchModeControl>();
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Automatic;
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Automatic;
                M.OnBrakeModesHappened(argsB);
                M.OnSwitchModesHappened(argsS);
            }
            
            void c_GetObject4MStatesHappened(object sender, GetObjectStatesEventArgs e)
            {
                WriteLine("Генерации списков");
                BrakeModesEventArgs argsB = new BrakeModesEventArgs();
                argsB.BrakeModes = new Dictionary<Guid, BrakeModeControl>();
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Manual;
                argsB.BrakeModes[Guid.NewGuid()] = BrakeModeControl.Manual;
                SwitchModesEventArgs argsS = new SwitchModesEventArgs();
                argsS.SwitchModes = new Dictionary<Guid, SwitchModeControl>();
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Manual;
                argsS.SwitchModes[Guid.NewGuid()] = SwitchModeControl.Manual;
                M.OnBrakeModesHappened(argsB);
                M.OnSwitchModesHappened(argsS);
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