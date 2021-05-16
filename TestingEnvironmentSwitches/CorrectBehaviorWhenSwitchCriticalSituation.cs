using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace TestingEnvironmentSwitches
{
    public class CorrectBehaviorWhenSwitchCriticalSituation
    {
        public int standartStopDissolutionTime { get; private set; } // время на остановку роспуска
        public int standartChangeSwitchesTime { get; private set; } // время на изменение маршрутов
        public int standartRestartDissolutionTime { get; private set; }
        public DateTime criticalSituationStartTime { get; private set; } // время срабатывания экстренной ситуации
        public DateTime stopDissolutionTime { get; private set; } // время остановки роспуска
        public DateTime restartDissolutionTime { get; private set; } // время рестарта роспуска
        private SemaphoreColor previousColor; // предыдущее значение семафора
        public int penaltyScores { get; private set; } // начисленные штрафные очки
        public int penaltyMultiplicator { get; private set; } // множитель штрафа. по умолчания = 100
        private IEnumerable<string> originWays;
        private IEnumerable<string> newWays;

        public CorrectBehaviorWhenSwitchCriticalSituation(int timeForChangeSwitches, int timeForDissolutionStop = 5, int multiplicator = 100, int timeForDissolutionRestart = 5)
        {
            standartStopDissolutionTime = timeForDissolutionStop;
            standartChangeSwitchesTime = timeForChangeSwitches;
            penaltyMultiplicator = multiplicator;
            standartRestartDissolutionTime = timeForDissolutionRestart;
        }
        public void CriticalSituationSwitchHappenedHandler(object sender, CriticalSituationSwitchEventArgs e)
        { // Получаем сообщение о нештатной ситуации, определяем пути, которые следуют за данной стрелкой.
            criticalSituationStartTime = DateTime.Now;
            Console.WriteLine("Ситуация сработала " + criticalSituationStartTime);
            Console.WriteLine("Запрос путей");
            // Передали id стрелки в запрос
            // Получили временно статичный ответ
            originWays = Map.GetWaysForSwich(e.IdObj);
            Console.WriteLine("Получены пути:");
            foreach (string var in originWays)
            {
                Console.WriteLine(var);
            }
        }

        public void SemaphoreChangeHappenedHandler(object sender, StateSemaphoreEventArgs e)
        {
            Console.WriteLine("Сработало изменение семафора");
            if (e.ValueColor == SemaphoreColor.Red)
            {
                stopDissolutionTime = DateTime.Now;
                Console.WriteLine("Красный сигнал. Роспуск остановлен спустя " + (stopDissolutionTime - criticalSituationStartTime).TotalSeconds);
                if ((stopDissolutionTime - criticalSituationStartTime).TotalSeconds > standartStopDissolutionTime)
                {
                    int penalty = Convert.ToInt32((stopDissolutionTime - criticalSituationStartTime).TotalSeconds -
                                                  standartStopDissolutionTime) * penaltyMultiplicator;
                    penaltyScores += penalty;
                }
                previousColor = e.ValueColor;
                Console.WriteLine("Всего штрафных баллов: " + penaltyScores);
            }
            else
            {
                if (previousColor == SemaphoreColor.Red)
                {
                    restartDissolutionTime = DateTime.Now;
                    Console.WriteLine("Рестарт роспуска спустя " +
                                      (restartDissolutionTime - stopDissolutionTime).TotalSeconds);
                    if ((restartDissolutionTime - stopDissolutionTime).TotalSeconds > standartRestartDissolutionTime)
                    {
                        int penalty = Convert.ToInt32((restartDissolutionTime - stopDissolutionTime).TotalSeconds -
                                                      standartRestartDissolutionTime) * penaltyMultiplicator;
                        penaltyScores += penalty;
                    }
                    previousColor = e.ValueColor;
                    Console.WriteLine("Всего штрафных баллов: " + penaltyScores);
                    // Рестар произошёл, отправляется сообщение с запросом маршрута
                    Console.WriteLine("Запрос путей");
                    // Передали id стрелки в запрос
                    // Получили временно статичный ответ
                    newWays = Map.GetWaysForSwich(e.IdObj); // Обращаемся не к ID семафора, а к ID пути надвига
                    Console.WriteLine("Получены пути:");
                    foreach (string var in newWays)
                    {
                        Console.WriteLine(var);
                    }

                    foreach (string way in newWays)
                    {
                        if (originWays.Contains(way))
                        {
                            Console.WriteLine("ОДИНАКОВЫЙ ПУТЬ: " + way);
                            penaltyScores += penaltyMultiplicator;
                        }
                        
                    }
                    Console.WriteLine("Всего штрафных баллов: " + penaltyScores);
                }
            }
        }
    }

    public class CriticalSituationSwitchEventArgs : EventArgs
    {
        public Guid IdObj { private set; get; }
        public string NameAnimation { private set; get; }
        public TypeDisrepairSwitch TypeDisrepair { private set; get; }

        public CriticalSituationSwitchEventArgs(Guid idObj, string nameAnimation, TypeDisrepairSwitch typeDisrepair)
        {
            IdObj = idObj;
            NameAnimation = nameAnimation;
            TypeDisrepair = typeDisrepair;
        }
    }

    public enum TypeDisrepairSwitch
    {
        None,
        Vzrez,
        AutomaticReset,
        Gab,
        LoseControl,
        DefectiveRtds,
        DefectiveIpd,
    }

    public class StateSemaphoreEventArgs : EventArgs
    {
        public Guid IdSemaphore;
        public Guid IdObj;
        public StageSemafor ButtonStage;
        public SemaphoreColor ValueColor;
        public double Speed;
        public Role Owner;
    }

    public enum SemaphoreColor
    {
        Red = 0,
        Green = 1,
        Yellow = 2,
        YellowGreen = 3,
        YellowYellow = 4
    }

    class Map
    {
        public static IEnumerable<string> GetWaysForSwich(Guid idSwitch)
        {
            if (DateTime.Now.Second % 2 == 0)
            {
                return new List<string>{"aa","bb", "cc"};
            }
            return new List<string>{"gg","gg", "gg"};
        }
    }
    //public static IEnumerable<string> GetWaysForSwich(Guid idSwitch)
    

    public class GetObjectStatesEventArgs : EventArgs
    {
        public IEnumerable<ObjectType> ObjectTypes { get; set; }

        public enum ObjectType
        {
            Retarder,
            Switch,
            SortList
        }
    }

    public class SortListEventArgs : EventArgs
    {
        public List<SortList> CollectionSortList;
        public Role Owner;
        public bool IsNewSL = true;
    }

    public class SortList
    {
        public List<OtcepKsau> ListOtcep { get; private set; }
    }

    public class OtcepKsau
    {
        string _route; // Догадка
        public string Route
        {
            get { return _route; }
            set { _route = value; }
        }
    }

    public class Route
    {
        string _idWay;

        public string IdWay
        {
            get { return _idWay; }
            set { _idWay = value; }
        }

        public string IdWayWithoutZero;
    }

    public class Role
    {
        // Emulated
    }

    public class StageSemafor
    {
        // Emulated
    }
}