using System;
using System.Collections.Generic;
using System.Threading;
using Moq;

namespace TestingEnvironmentSwitches
{
    class Program
    {
        private static Program M;
        static void Main(string[] args)
        {
            Console.WriteLine("1 - ручное логирование, 2 - автотесты");
            string inp = Console.ReadLine();
            if (inp == "1")
            {
                while (true)
                {
                    Console.WriteLine(
                        "\n1 - подписаться\n2 - CriticalSituationSwitchCall\n3 - Остановить роспуск\n4 - Рестарт роспуска\n5 - GetObjectStates");
                    inp = Console.ReadLine();
                    if (inp == "1")
                    {
                        M = new Program();
                        CorrectBehaviorWhenSwitchCriticalSituation a =
                            new CorrectBehaviorWhenSwitchCriticalSituation(10);
                        CriticalSituationSwitchHappened += a.CriticalSituationSwitchHappenedHandler;
                        ChangeSemaphoreHappened += a.SemaphoreChangeHappenedHandler;
                        SortListHappened += a.SortListHappenedHandler;
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
                        M.GetObjectStatesCall(1);
                    }
                }
            }
            else
            {
                Console.WriteLine("====Тесты запущены=====\n" +
                                  "Время на изменение стрелок: 5сек\n" +
                                  "Время на остановку роспуска: 5сек\n" +
                                  "Время на рестарт роспуска: 5сек\n" +
                                  "Множитель штрафа: 100\n" +
                                  "========================\n\n");
                
                Console.WriteLine("====1 кейс====\n" +
                                  "Нет опозданий, все стрелки переведены\n" +
                                  "Должно получиться 0 баллов.");
                M = new Program();
                CorrectBehaviorWhenSwitchCriticalSituation a =
                    new CorrectBehaviorWhenSwitchCriticalSituation(10);
                CriticalSituationSwitchHappened += a.CriticalSituationSwitchHappenedHandler;
                ChangeSemaphoreHappened += a.SemaphoreChangeHappenedHandler;
                SortListHappened += a.SortListHappenedHandler;
                M.CriticalSituationSwitchCall();
                M.DissolutionStopCall();
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(0);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 0)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
                
                Console.WriteLine("====2 кейс====\n" +
                                  "Нет опозданий, 3 стрелки не переведены\n" +
                                  "Должно получиться 300 баллов.");
                a.reset();
                M.CriticalSituationSwitchCall();
                M.DissolutionStopCall();
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(1);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 300)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
                
                Console.WriteLine("====3 кейс====\n" +
                                  "Опоздание роспуска на 1сек, все стрелки переведены\n" +
                                  "Должно получиться 100 баллов.");
                a.reset();
                M.CriticalSituationSwitchCall();
                Thread.Sleep(6000);
                M.DissolutionStopCall();
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(0);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 100)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
                
                Console.WriteLine("====4 кейс====\n" +
                                  "Опоздание роспуска на 1сек, 3 стрелки не переведены\n" +
                                  "Должно получиться 400 баллов.");
                a.reset();
                M.CriticalSituationSwitchCall();
                Thread.Sleep(6000);
                M.DissolutionStopCall();
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(1);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 400)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
                
                Console.WriteLine("====5 кейс====\n" +
                                  "Опоздание рестарта на 1сек, все стрелки переведены\n" +
                                  "Должно получиться 100 баллов.");
                a.reset();
                M.CriticalSituationSwitchCall();
                M.DissolutionStopCall();
                Thread.Sleep(1000+ a.standartRestartDissolutionTime*1000);
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(1);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 100)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
                
                Console.WriteLine("====6 кейс====\n" +
                                  "Опоздание рестарта на 1сек, 3 стрелки не переведены\n" +
                                  "Должно получиться 400 баллов.");
                a.reset();
                M.CriticalSituationSwitchCall();
                M.DissolutionStopCall();
                Thread.Sleep(1000+ a.standartRestartDissolutionTime*1000);
                M.DissolutionRestartCall();
                M.GetObjectStatesCall(1);
                Console.WriteLine("Получилось: "+a.penaltyScores);
                if (a.penaltyScores == 400)
                {
                    Console.WriteLine("====Тест пройден====\n\n");
                }
                else
                {
                    Console.WriteLine("==Тест не пройден==\n\n");
                }
            }
        }

        void GetObjectStatesCall(int id)
        {
            if (id == 1)
            {
                SortListEventArgs args = new SortListEventArgs();
                args.CollectionSortList = new List<SortList>() {new SortList()};
                args.CollectionSortList[0].ListOtcep = new List<OtcepKsau>();

                Route r1 = new Route();
                r1.IdWay = "aa";
                OtcepKsau ok1 = new OtcepKsau();
                ok1.Route = r1;

                Route r2 = new Route();
                r2.IdWay = "bb";
                OtcepKsau ok2 = new OtcepKsau();
                ok2.Route = r2;

                Route r3 = new Route();
                r3.IdWay = "cc";
                OtcepKsau ok3 = new OtcepKsau();
                ok3.Route = r3;


                args.CollectionSortList[0].ListOtcep.Add(ok1);
                args.CollectionSortList[0].ListOtcep.Add(ok2);
                args.CollectionSortList[0].ListOtcep.Add(ok3);
                OnSortListHappened(args);
            }
            else
            {
                SortListEventArgs args = new SortListEventArgs();
                args.CollectionSortList = new List<SortList>() {new SortList()};
                args.CollectionSortList[0].ListOtcep = new List<OtcepKsau>();

                Route r1 = new Route();
                r1.IdWay = "dd";
                OtcepKsau ok1 = new OtcepKsau();
                ok1.Route = r1;

                Route r2 = new Route();
                r2.IdWay = "ee";
                OtcepKsau ok2 = new OtcepKsau();
                ok2.Route = r2;

                Route r3 = new Route();
                r3.IdWay = "ff";
                OtcepKsau ok3 = new OtcepKsau();
                ok3.Route = r3;


                args.CollectionSortList[0].ListOtcep.Add(ok1);
                args.CollectionSortList[0].ListOtcep.Add(ok2);
                args.CollectionSortList[0].ListOtcep.Add(ok3);
                OnSortListHappened(args);
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

        protected virtual void OnSortListHappened(SortListEventArgs e)
        {
            EventHandler<SortListEventArgs> handler = SortListHappened;
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
        public static event EventHandler<CriticalSituationSwitchEventArgs> CriticalSituationSwitchHappened;
        public static event EventHandler<StateSemaphoreEventArgs> ChangeSemaphoreHappened;
        public static event EventHandler<SortListEventArgs> SortListHappened;
        public static event EventHandler<GetObjectStatesEventArgs> GetObjectStatesHappened;
    }
}