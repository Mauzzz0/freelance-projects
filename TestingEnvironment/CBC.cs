using System;
using System.Collections.Generic;
using static System.Console;

namespace TestingEnvironment
{ // Необходимо подтянуть все классы из остальной части программы
    // А именно, BrakeModeControl, SwitchModeControl, SwitchModesEventArgs, BrakeModesEventArgs, Role, StageSemaphore,
    // SemaphoreColor, StateSemaphoreEventArgs, TypeDisrepairGac, CriticalSituationGacEventArgs, GetObjectStatesEventArgs
    public class CorrectBehaviorCheck
    {
        public int standartStopDissolutionTime { get; private set; }
        public int standartRestartDissolutionTime { get; private set; }
        public DateTime criticalSituationStartTime { get; private set; } // время срабатывания экстренной ситуации
        public DateTime stopDissolutionTime { get; private set; } // время остановки роспуска
        public DateTime restartDissolutionTime { get; private set; } // время рестарта роспуска
        private TypeDisrepairGac TypeDisrepair; // тип экстренной ситуации
        private SemaphoreColor previousColor; // предыдущее значение семафора
        public bool isStarted; // сработала ли экстренная ситуация
        public bool isBrakeDone { get; private set; }
        public bool isSwitchDone { get; private set; }
        public bool checkBrakes { get; private set; }
        public bool checkSwitches { get; private set; }
        public int penaltyScores { get; private set; } // начисленные штрафные очки
        public int penaltyMultiplicator { get; private set; } // множитель штрафа. по умолчания = 100

        public CorrectBehaviorCheck(int multiplicator = 100, int stopDissolutionTime = 5,
            int restartDissolutionTime = 30) // стандартный конструктор, задаёт множитель, время для остановки
        {                                           // роспуска, время для рестарта роспуска
            penaltyMultiplicator = multiplicator;
            standartStopDissolutionTime = stopDissolutionTime;
            standartRestartDissolutionTime = restartDissolutionTime;
        }
        
        

        /// <summary>
        /// Обработчик события "критическая ситуация" (CriticalSituationGacEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void CriticalSituationHappened(object sender, CriticalSituationGacEventArgs e)
        {
            if (e.TypeDisrepair != TypeDisrepairGac.None)
            {
                if (isStarted) // Если уже началась какая-то ситация, то нужно проверить не нарушил ли оператор время на её реагирование.
                    // Ситуация 3.2.3 -> Вторая ситуация спустя минуту после первой. Оператор не успел зарестартить роспуск во время первой.
                {
                    DateTime now = DateTime.Now;
                    if ((now - criticalSituationStartTime).TotalSeconds > standartRestartDissolutionTime + 10)
                    {
                        penaltyScores += (Convert.ToInt32((now - criticalSituationStartTime).TotalSeconds) - 
                                        standartRestartDissolutionTime) / 10 * penaltyMultiplicator;
                    }
                    WriteLine("penalty: "+penaltyScores);
                }
                
                TypeDisrepair = e.TypeDisrepair;
                checkBrakes = true;
                if (e.TypeDisrepair != TypeDisrepairGac.ManualBrake)
                {
                    checkSwitches = true;
                }
                criticalSituationStartTime = DateTime.Now;
                WriteLine("СРАБОТАЛА НЕШТАТНАЯ СИТУАЦИЯ");
                WriteLine("Crit sit time: " + criticalSituationStartTime);
                WriteLine("Crit:" + e.TypeDisrepair);
                isStarted = true;
            }
        }
        
        /// <summary>
        /// Обработчик события "смена сигнала семафора" (StateSemaphoreEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void StateSemaphoreHappened(object sender, StateSemaphoreEventArgs e)
        {
            // TODO: Рестарт роспуска - это изменение с красного на любой?
            if (e.ValueColor == SemaphoreColor.Red & isStarted)
            {
                previousColor = e.ValueColor;
                stopDissolutionTime = DateTime.Now;
                WriteLine("---" + Convert.ToString(criticalSituationStartTime - DateTime.Now) + "---");
                WriteLine("Время роспуска: " + stopDissolutionTime);
                if ((stopDissolutionTime - criticalSituationStartTime).TotalSeconds > standartStopDissolutionTime + 10) //норма +10 сек без штрафа
                {
                    penaltyScores += (Convert.ToInt32((stopDissolutionTime - criticalSituationStartTime).TotalSeconds) - 
                                      standartRestartDissolutionTime) / 10 * penaltyMultiplicator;
                }
                WriteLine("Штрафные баллы: " + penaltyScores);
            }
            else
            {
                if (previousColor == SemaphoreColor.Red)
                { // TODO: Это и есть рестарт?
                    previousColor = e.ValueColor;
                    restartDissolutionTime = DateTime.Now;
                    WriteLine("---" + Convert.ToString(criticalSituationStartTime - DateTime.Now) + "---");
                    WriteLine("Время рестарта: " + restartDissolutionTime);
                    if ((restartDissolutionTime - criticalSituationStartTime).TotalSeconds > standartRestartDissolutionTime + 10) //норма +10сек без штрафа
                    {
                        penaltyScores += (Convert.ToInt32((restartDissolutionTime - criticalSituationStartTime).TotalSeconds) -
                                          standartRestartDissolutionTime) / 10  * penaltyMultiplicator;
                    }
                    WriteLine("Штрафные баллы: " + penaltyScores);
                    new Program().GetObjectStatesHappenedCall();
                }
            }
        }
        
        /// <summary>
        /// Обработчик события "Получить список замедлителей" (BrakeModesEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void BrakeModesHappened(object sender, BrakeModesEventArgs e)
        {
            if (isStarted)
            {
                int numberOfNotManualBrakes = 0;
                WriteLine("---" + Convert.ToString(criticalSituationStartTime - DateTime.Now) + "---");
                WriteLine("Brakes:");
                foreach (KeyValuePair<Guid, BrakeModeControl> Brake in e.BrakeModes)
                {
                    WriteLine("-" + Brake.Value);
                    if (Brake.Value != BrakeModeControl.Manual)
                    {
                        numberOfNotManualBrakes++;
                    }
                }
                WriteLine("Not manual brakes:" + numberOfNotManualBrakes);
                penaltyScores += numberOfNotManualBrakes * penaltyMultiplicator;
                isBrakeDone = true;
                if (!checkSwitches)
                {
                    // Стрекли проверять не надо, сработало только РУЧНОЕ ТОРМОЖЕНИЕ
                    isStarted = false;
                    checkBrakes = false;
                    checkSwitches = false;
                    WriteLine("TOTAL penalty:" + penaltyScores);
                }
                if (isBrakeDone & isSwitchDone)
                {
                    // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                    isStarted = false;
                    checkBrakes = false;
                    checkSwitches = false;
                    WriteLine("TOTAL penalty:" + penaltyScores);
                }
            }
        }
        
        /// <summary>
        /// Обработчик события "Получить список стрелок" (SwitchModesEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void SwitchModesHappened(object sender, SwitchModesEventArgs e)
        {
            if (checkSwitches) {
                int numberOfNotManualSwitches = 0;
                WriteLine("---" + Convert.ToString(criticalSituationStartTime - DateTime.Now) + "---");
                WriteLine("Switches:");
                foreach (KeyValuePair<Guid, SwitchModeControl> Switch in e.SwitchModes)
                {
                    WriteLine("-"+Switch.Value);
                    if (Switch.Value != SwitchModeControl.Manual)
                    {
                        numberOfNotManualSwitches++;
                    }
                }
                
                penaltyScores += numberOfNotManualSwitches * penaltyMultiplicator;;
                WriteLine("Not manual switches:" + numberOfNotManualSwitches);
        
                isSwitchDone = true;
                if (isBrakeDone & isSwitchDone)
                {
                    // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                    isStarted = false;
                    WriteLine("TOTAL penalty:" + penaltyScores);
                }
            }
        }

        // public class CriticalSituationGacEventArgs : EventArgs
        // { // TODO: Подключить этот класс из внешней программы
        //     /// <summary>
        //     /// Сообщения для отображения в окне АРМ ДСПГ
        //     /// </summary>
        //     public string Message { get; set; }
        //     /// <summary>
        //     /// Тип нештатной ситуации
        //     /// </summary>
        //     public TypeDisrepairGac TypeDisrepair { get; set; }
        // } 
        
        // public enum TypeDisrepairGac
        // { // TODO: Подключить этот класс из внешней программы
        //     /// <summary>
        //     /// ВНИМАНИЕ! ГАЦ МН НЕИСПРАВЕН
        //     /// </summary>
        //     BrokenGAC,
        //     /// <summary>
        //     /// ВНИМАНИЕ! НЕТ СВЯЗИ С ГАЦ МН
        //     /// </summary>
        //     LostConnectionWithGAC,
        //     /// <summary>
        //     /// ВНИМАНИЕ! СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 1 НЕВОЗМОЖЕН
        //     /// </summary>
        //     SwitchesDoNotMatchToTrackForIU1,
        //     /// <summary>
        //     /// ВНИМАНИЕ!  СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 2 НЕВОЗМОЖЕН
        //     /// </summary>
        //     SwitchesDoNotMatchToTrackForIU2,
        //     /// <summary>
        //     /// ВНИМАНИЕ!  СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 3 НЕВОЗМОЖЕН
        //     /// </summary>
        //     SwitchesDoNotMatchToTrackForIU3,
        //     /// <summary>
        //     /// !! ПРОВЕРЬ ПРОГРАММУ РОСПУСКА !! ВНИМАНИЕ! СБОЙ ИУ
        //     /// </summary>
        //     TUFailure,
        //     /// <summary>
        //     /// ВНИМАНИЕ!!  ТОРМОЗИТЬ ВРУЧНУЮ
        //     /// </summary>
        //     ManualBrake,
        //     /// <summary>
        //     /// ВНИМАНИЕ! ВЕСОМЕР ИУ 1 НЕИСПРАВЕН
        //     /// </summary>
        //     BrokenWeightIndicatorOfIU1,
        //     /// <summary>
        //     /// ВНИМАНИЕ! ВЕСОМЕР ИУ 2 НЕИСПРАВЕН
        //     /// </summary>
        //     BrokenWeightIndicatorOfIU2,
        //     /// <summary>
        //     /// ВНИМАНИЕ! ВЕСОМЕР ИУ 3 НЕИСПРАВЕН
        //     /// </summary>
        //     BrokenWeightIndicatorOfIU3,
        //     /// <summary>
        //     /// Нет нештатной ситуации
        //     /// </summary>
        //     None
        // }
        
        // public class StateSemaphoreEventArgs : EventArgs
        // { // TODO: Подключить этот класс из внешней программы
        //     public Guid IdSemaphore; // идентификатор светофора
        //     public Guid IdObj; // идентификатор объекта к которому привязан светофор(путь надвига)
        //     public StageSemaphore ButtonStage; // кнопка нажатая на пульте
        //     public SemaphoreColor ValueColor; // показания светофора
        //     public double Speed; // скорость надвига
        //     public Role Owner; // идентификатор ДСПГ
        // }
        //
        // public enum SemaphoreColor
        // { // TODO: Подключить этот класс из внешней программы
        //     /// <summary>
        //     /// Красный При получении этого значения роспуск считается остановленным
        //     /// </summary>
        //     Red = 0,
        //     /// <summary>
        //     /// Зелёный
        //     /// </summary>
        //     Green = 1,
        //     /// <summary>
        //     /// Желтый(верхний)
        //     /// </summary>
        //     Yellow = 2,
        //     /// <summary>
        //     /// Желтый(верхний) и зеленый
        //     /// </summary>
        //     YellowGreen = 3,
        //     /// <summary>
        //     /// Два желтых
        //     /// </summary>
        //     YellowYellow = 4
        // }
        // public class StageSemaphore
        // { // TODO: Подключить этот класс из внешней программы
        //     // Эмуляция класса
        // }
        // public class Role
        // { // TODO: Подключить этот класс из внешней программы
        //     // Эмуляция класса
        // }
        // public class BrakeModesEventArgs : EventArgs
        // { // TODO: Подключить этот класс из внешней программы
        //     public IDictionary<Guid, BrakeModeControl> BrakeModes { get; set; }
        // }
        //
        // public class SwitchModesEventArgs : EventArgs
        // { // TODO: Подключить этот класс из внешней программы
        //     public IDictionary<Guid, SwitchModeControl> SwitchModes { get; set; }
        // }
        //
        // /// <summary>
        // /// Режим управления стрелкой
        // /// </summary>
        // public enum SwitchModeControl
        // { // TODO: Подключить этот класс из внешней программы
        //     /// <summary>
        //     /// Неизвестный тип управления
        //     /// </summary>
        //     Unknown = 0,
        //     /// <summary>
        //     /// Ручное управление с пульта
        //     /// </summary>
        //     Manual = 1,
        //     /// <summary>
        //     /// Автоматическое управление
        //     /// </summary>
        //     Automatic = 2
        // }
        //
        // /// <summary>
        // /// Режим управления замедлителем
        // /// </summary>
        // public enum BrakeModeControl
        // { // TODO: Подключить этот класс из внешней программы
        //     /// <summary>
        //     /// Неизвестный тип управления
        //     /// </summary>
        //     Unknown = 0,
        //     /// <summary>
        //     /// Ручное управление с пульта
        //     /// </summary>
        //     Manual = 1,
        //     /// <summary>
        //     /// Автоматическое управление
        //     /// </summary>
        //     Automatic = 2,
        //     /// <summary>
        //     /// Отключение замедлителя
        //     /// </summary>
        //     Off = 3,
        //     /// <summary>
        //     /// Включение замедлителя
        //     /// </summary>
        //     On = 4
        // }
    }
}