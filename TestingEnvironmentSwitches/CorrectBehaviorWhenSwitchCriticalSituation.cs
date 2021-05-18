using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace TestingEnvironmentSwitches
{
    public class CorrectBehaviorWhenSwitchCriticalSituation
    { // Необходимо подключить из внешнего кода StageSemafore, Role, TypeDisrepairSwitch, StateSemaphoreEventArgs
        // SemaphoreColor, Map.GetWaysForSwich, GetObjectStatesEventArgs, SortListEventArgs, SortList, Route, OtcepKsau,
        // CriticalSituationSwitchEventArgs
        public bool isStarted { get; private set; } // началась ли обработка события
        public int standartStopDissolutionTime { get; private set; } // время на остановку роспуска
        public int standartChangeSwitchesTime { get; private set; } // время на изменение маршрутов
        public int standartRestartDissolutionTime { get; private set; } // время на рестарт роспуска
        public DateTime criticalSituationStartTime { get; private set; } // время срабатывания экстренной ситуации
        public DateTime stopDissolutionTime { get; private set; } // время остановки роспуска
        public DateTime restartDissolutionTime { get; private set; } // время рестарта роспуска
        private SemaphoreColor previousColor; // предыдущее значение семафора
        public int penaltyScores { get; private set; } // начисленные штрафные очки
        public int penaltyMultiplicator { get; private set; } // множитель штрафа. по умолчания = 100
        private IEnumerable<string> originWays;

        public CorrectBehaviorWhenSwitchCriticalSituation(int timeForChangeSwitches, int timeForDissolutionStop = 5, int multiplicator = 100, int timeForDissolutionRestart = 5)
        { // конструктор с временем для роспуска/множителем/временем для рестарта роспуска по умолчанию и необходимостью задать время для смены стрелок
            standartStopDissolutionTime = timeForDissolutionStop;
            standartChangeSwitchesTime = timeForChangeSwitches;
            penaltyMultiplicator = multiplicator;
            standartRestartDissolutionTime = timeForDissolutionRestart;
        }

        public void reset()
        { // метод, использующийся только при тестировании, обнуляет все параметры
            penaltyScores = 0;
            isStarted = false;
            criticalSituationStartTime = DateTime.MinValue;
            stopDissolutionTime = DateTime.MinValue;
            restartDissolutionTime = DateTime.MinValue;
            previousColor = SemaphoreColor.Green;
        }
        public void CriticalSituationSwitchHappenedHandler(object sender, CriticalSituationSwitchEventArgs e)
        { // Получаем сообщение о нештатной ситуации, определяем пути, которые следуют за данной стрелкой.
            if (!isStarted)
            { // Если событие ещё не начато, начинаем его, запоминаем время и запрашиваем пути.
                isStarted = true; 
                criticalSituationStartTime = DateTime.Now; 
                originWays = Map.GetWaysForSwich(e.IdObj); // Все пути добавляем в originWays
            }
            else if (isStarted & e.TypeDisrepair == TypeDisrepairSwitch.None)
            { // Если событие было начато и пришла отмена события, то сбрасываем все параметры
                isStarted = false;
                criticalSituationStartTime = DateTime.MinValue;
                stopDissolutionTime = DateTime.MinValue;
                restartDissolutionTime = DateTime.MinValue;
                previousColor = SemaphoreColor.Green;
            }
        }

        public void SemaphoreChangeHappenedHandler(object sender, StateSemaphoreEventArgs e)
        {
            if (isStarted)
            { // Если событие начато
                if (e.ValueColor == SemaphoreColor.Red)
                { // Если сигнал красный, то остановка роспуска 
                    stopDissolutionTime = DateTime.Now; // запоминаем время стопа
                    if ((stopDissolutionTime - criticalSituationStartTime).TotalSeconds > standartStopDissolutionTime)
                    { // Если оператор не уложился в норму
                        int penalty = Convert.ToInt32((stopDissolutionTime - criticalSituationStartTime).TotalSeconds -
                                                      standartStopDissolutionTime) * penaltyMultiplicator; // За каждую секунду сверх нормы к общему штрафу прибавляем единичный штраф
                        penaltyScores += penalty;
                    }

                    previousColor = e.ValueColor;
                }
                else
                {
                    if (previousColor == SemaphoreColor.Red)
                    { // Если предыдущий сигнал был красный, то произошёл рестарт
                        restartDissolutionTime = DateTime.Now; // запоминаем время рестарта
                        if ((restartDissolutionTime - stopDissolutionTime).TotalSeconds >
                            standartRestartDissolutionTime)
                        { // Если оператор не уложился в норму
                            int penalty = Convert.ToInt32((restartDissolutionTime - stopDissolutionTime).TotalSeconds -
                                                          standartRestartDissolutionTime) * penaltyMultiplicator; // За каждую секунду сверх нормы к общему штрафу прибавляем единичный штраф
                            penaltyScores += penalty;
                        }

                        previousColor = e.ValueColor;
                        // TODO: Вызов GetObjectStatesEventArgs
                        // Здесь должен быть вызов GetObjectStatesEventArgs, в ответ на который
                        // произойдёт SortListEventArgs
                        // TODO: Вызов GetObjectStatesEventArgs
                    }
                }
            }
        }

        public void SortListHappenedHandler(object sender, SortListEventArgs e)
        {
            int incorrectWays = 0; 
            foreach (OtcepKsau ok in e.CollectionSortList[0].ListOtcep) // Перебираем все полученные пути
            {
                if (originWays.Contains(ok.Route.IdWay)) // Считаем сколько путей не было переведено
                {
                    incorrectWays++; 
                }
            }
            penaltyScores += incorrectWays * penaltyMultiplicator; // За каждый непереведённый путь прибавляем единичный штраф
        }
    }

    // public class CriticalSituationSwitchEventArgs : EventArgs
    // {
    //     public Guid IdObj { private set; get; }
    //     public string NameAnimation { private set; get; }
    //     public TypeDisrepairSwitch TypeDisrepair { private set; get; }
    //
    //     public CriticalSituationSwitchEventArgs(Guid idObj, string nameAnimation, TypeDisrepairSwitch typeDisrepair)
    //     {
    //         IdObj = idObj;
    //         NameAnimation = nameAnimation;
    //         TypeDisrepair = typeDisrepair;
    //     }
    // }

    // public enum TypeDisrepairSwitch
    // {
    //     None,
    //     Vzrez,
    //     AutomaticReset,
    //     Gab,
    //     LoseControl,
    //     DefectiveRtds,
    //     DefectiveIpd,
    // }

    // public class StateSemaphoreEventArgs : EventArgs
    // {
    //     public Guid IdSemaphore;
    //     public Guid IdObj;
    //     public StageSemafor ButtonStage;
    //     public SemaphoreColor ValueColor;
    //     public double Speed;
    //     public Role Owner;
    // }

    // public enum SemaphoreColor
    // {
    //     Red = 0,
    //     Green = 1,
    //     Yellow = 2,
    //     YellowGreen = 3,
    //     YellowYellow = 4
    // }

    // class Map
    // {
    //     public static IEnumerable<string> GetWaysForSwich(Guid idSwitch)
    //     { // TODO:
    //         return new List<string>{"aa","bb", "cc"};
    //     }
    // }
    

    // public class GetObjectStatesEventArgs : EventArgs
    // {
    //     public IEnumerable<ObjectType> ObjectTypes { get; set; }
    //
    //     public enum ObjectType
    //     {
    //         Retarder,
    //         Switch,
    //         SortList
    //     }
    // }

    // public class SortListEventArgs : EventArgs
    // {
    //     public List<SortList> CollectionSortList;
    //     public Role Owner;
    //     public bool IsNewSL = true;
    // }
    //
    // public class SortList
    // {
    //     public List<OtcepKsau> ListOtcep { get; set; } // временный паблик сет
    // }

    // public class OtcepKsau
    // {
    //     Route _route; // Догадка
    //     public Route Route
    //     {
    //         get { return _route; }
    //         set { _route = value; }
    //     }
    // }

    // public class Route
    // {
    //     string _idWay;
    //
    //     public string IdWay
    //     {
    //         get { return _idWay; }
    //         set { _idWay = value; }
    //     }
    //
    //     public string IdWayWithoutZero;
    // }

    // public class Role
    // {
    //     // Emulated
    // }

    // public class StageSemafor
    // {
    //     // Emulated
    // }
}