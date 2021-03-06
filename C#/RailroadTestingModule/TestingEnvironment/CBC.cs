using System;
using System.Collections.Generic;
using static System.Console;

namespace TestingEnvironment
{ // Необходимо подтянуть все классы из остальной части программы
    // А именно, BrakeModeControl, SwitchModeControl, SwitchModesEventArgs, BrakeModesEventArgs, Role, StageSemaphore,
    // SemaphoreColor, StateSemaphoreEventArgs, TypeDisrepairGac, CriticalSituationGacEventArgs, GetObjectStatesEventArgs
    public class CorrectBehaviorCheck
    {
        public int standartStopDissolutionTime { get; private set; } // время на остановку роспуска
        public int standartRestartDissolutionTime { get; private set; } // время на рестарт роспуска
        public DateTime criticalSituationStartTime { get; private set; } // время срабатывания экстренной ситуации
        public DateTime stopDissolutionTime { get; private set; } // время остановки роспуска
        public DateTime restartDissolutionTime { get; private set; } // время рестарта роспуска
        private TypeDisrepairGac TypeDisrepair; // тип экстренной ситуации
        private SemaphoreColor previousColor; // предыдущее значение семафора
        public bool isStarted; // сработала ли экстренная ситуация
        public bool isBrakeDone { get; private set; } // получена ли информация о тормозах
        public bool isSwitchDone { get; private set; } // получена ли информация о стрелках
        public bool checkBrakes { get; private set; } // нужно ли проверять тормоза
        public bool checkSwitches { get; private set; } // нужно ли проверять стрелки
        public int penaltyScores { get; private set; } // начисленные штрафные очки
        public int penaltyMultiplicator { get; private set; } // множитель штрафа. по умолчания = 100

        public CorrectBehaviorCheck(int multiplicator = 100, int stopDissolutionTime = 5,
            int restartDissolutionTime = 30) // стандартный конструктор, задаёт множитель, время для остановки
        {
            // роспуска, время для рестарта роспуска
            penaltyMultiplicator = multiplicator;
            standartStopDissolutionTime = stopDissolutionTime;
            standartRestartDissolutionTime = restartDissolutionTime;
        }

        public void reset()
        { // Метод используется только при тестировании, обнуляет все параметры для чистого старта
            isStarted = false;
            criticalSituationStartTime = DateTime.MinValue;
            stopDissolutionTime = DateTime.MinValue;
            restartDissolutionTime = DateTime.MinValue;
            isBrakeDone = false;
            isSwitchDone = false;
            previousColor = SemaphoreColor.Green;
            checkBrakes = false;
            checkSwitches = false;
            penaltyScores = 0;
        }



        /// <summary>
        /// Обработчик события "критическая ситуация" (CriticalSituationGacEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void CriticalSituationHappened(object sender, CriticalSituationGacEventArgs e)
        {
            if (e.TypeDisrepair != TypeDisrepairGac.None)
            {  // Если сработала ситуация, которая несёт какой-то смысл в себе
                if (isStarted) // Если уже началась какая-то ситация, то нужно проверить не нарушил ли оператор время на её реагирование.
                    // Ситуация 3.2.3 -> Вторая ситуация спустя минуту после первой. Оператор не успел зарестартить роспуск во время первой.
                {
                    DateTime now = DateTime.Now; // Запоминаем текущее время, 
                    if ((now - criticalSituationStartTime).TotalSeconds > standartRestartDissolutionTime + 10)
                    { // Если время между текущей сработавшей новой ситуацией и прошлой ситуацией превышает время на рестарта роспуска больше, чем на 10сек
                        // То значит что оператор не успел предпринят все необходимые действия, получает штраф за 10сек
                        penaltyScores += (Convert.ToInt32((now - criticalSituationStartTime).TotalSeconds) - 
                                        standartRestartDissolutionTime) / 10 * penaltyMultiplicator;
                    }
                }
                criticalSituationStartTime = DateTime.Now; // Перезаписываем или впервые определяем время срабатывания ситуации
                TypeDisrepair = e.TypeDisrepair; // Перезаписываем или впервые определяем тип ситуации
                checkBrakes = true; // Ставим галочку что нужно проверять тормоза
                if (e.TypeDisrepair != TypeDisrepairGac.ManualBrake)
                { // Если тип ситуации НЕ "тормозить вручную", то ставим галочку что нужно проверять стрелки
                    checkSwitches = true;
                }
                isStarted = true; // Ставим галочку что обработка ситуации начата
            }
        }
        
        /// <summary>
        /// Обработчик события "смена сигнала семафора" (StateSemaphoreEventArgs)
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        public void StateSemaphoreHappened(object sender, StateSemaphoreEventArgs e)
        {
            if (e.ValueColor == SemaphoreColor.Red & isStarted)
            { // Если ситуация начата и получен красный сигнал светофора, то это остановка роспуска
                previousColor = e.ValueColor; // Запоминаем сигнал светофора
                stopDissolutionTime = DateTime.Now; // Запоминаем время остановки роспуска
                if ((stopDissolutionTime - criticalSituationStartTime).TotalSeconds > standartStopDissolutionTime + 10) //норма +10 сек без штрафа
                { // Если с момента срабатывания последней критической ситуации прошло больше, чем стандартное время +10сек
                    // то значит, что оператор не успел во время среагировать, начисляем штраф за каждые 10сек
                    penaltyScores += (Convert.ToInt32((stopDissolutionTime - criticalSituationStartTime).TotalSeconds) - 
                                      standartRestartDissolutionTime) / 10 * penaltyMultiplicator;
                }
            }
            else
            {
                if (previousColor == SemaphoreColor.Red & isStarted)
                { // Если ситуация начата и получен НЕ красный светофор, причём предыдущий был красный. Значит произошёл рестарт роспуска
                    previousColor = e.ValueColor; // Запоминаем новый цвет
                    restartDissolutionTime = DateTime.Now; // Запоминаем время рестарта роспуска
                    if ((restartDissolutionTime - criticalSituationStartTime).TotalSeconds > standartRestartDissolutionTime + 10) //норма +10сек без штрафа
                    { // Если с момента срабатывания критической ситуации прошло больше, чем на 10сек больше времени, чем задано на реакцию,
                        // то оператор получает штраф за каждые 10сек сверх нормы
                        penaltyScores += (Convert.ToInt32((restartDissolutionTime - criticalSituationStartTime).TotalSeconds) -
                                          standartRestartDissolutionTime) / 10  * penaltyMultiplicator;
                    }
                    // TODO: Вызов GetObjectStatesEventArgs
                    // Здесь должен быть вызов GetObjectStatesEventArgs, в ответ на который
                    // произойдут BrakeModesHappened и SwitchModesHappened
                    // TODO: Вызов GetObjectStatesEventArgs
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
            { // Если ситуация начата
                int numberOfNotManualBrakes = 0; // Подсчитываем количество неправильных 
                foreach (KeyValuePair<Guid, BrakeModeControl> Brake in e.BrakeModes)
                { // Перебираем список полученных тормозов
                    if (Brake.Value != BrakeModeControl.Manual)
                    { // Если тормоз не в ручном положении, увеличиваем счётчик на 1
                        numberOfNotManualBrakes++; 
                    }
                }
                penaltyScores += numberOfNotManualBrakes * penaltyMultiplicator; // Начисляем штраф за каждый неправильный тормоз
                isBrakeDone = true; // Ставим галочку что тормоза были првоерены
                if (!checkSwitches)
                {
                    // Стрелки проверять не надо, сработало только РУЧНОЕ ТОРМОЖЕНИЕ
                    isStarted = false;
                    checkBrakes = false;
                    checkSwitches = false;
                }
                if (isBrakeDone & isSwitchDone)
                {
                    // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                    isStarted = false;
                    checkBrakes = false;
                    checkSwitches = false;
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
            if (checkSwitches) { // Если нужно проверять стрелки
                int numberOfNotManualSwitches = 0; // СОздаём счётчик неправильных стрелок 
                foreach (KeyValuePair<Guid, SwitchModeControl> Switch in e.SwitchModes)
                { // В цикле перебираем полученные стрелки
                    if (Switch.Value != SwitchModeControl.Manual)
                    {  // Если стрелка не в ручнмо режиме, то увеличиваем счётчик на 1
                        numberOfNotManualSwitches++;
                    }
                }
                
                penaltyScores += numberOfNotManualSwitches * penaltyMultiplicator; // Начисляем штраф за неправильные стрелки
        
                isSwitchDone = true; // СТавим галочку что стрелки были проверены
                if (isBrakeDone & isSwitchDone)
                {
                    // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                    isStarted = false;
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