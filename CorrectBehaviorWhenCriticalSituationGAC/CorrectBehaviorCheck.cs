using System;
using System.Collections.Generic;


namespace CorrectBehaviorWhenCriticalSituationGAC
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
                criticalSituationStartTime = DateTime.Now;
                TypeDisrepair = e.TypeDisrepair;
                Console.WriteLine("РАСБОТАЛА НЕШТАТНАЯ СИТУАЦИЯ");
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
            if (e.ValueColor == SemaphoreColor.Red & isStarted)
            { // Если семафор стал красный и ситуация произошла => остановка роспуска
                previousColor = e.ValueColor;
                stopDissolutionTime = DateTime.Now;
                if ((stopDissolutionTime - criticalSituationStartTime).Seconds > standartStopDissolutionTime + 10) //норма +10 сек без штрафа
                {
                    penaltyScores += ((stopDissolutionTime - criticalSituationStartTime).Seconds - 
                                      standartRestartDissolutionTime) * penaltyMultiplicator;
                }
            }
            else
            { // Если предыдущий сигнал был красным и ситуация произошла => роспуск уже был оставлен, предприняты меры, рестарт
                if (previousColor == SemaphoreColor.Red & isStarted)
                {
                    previousColor = e.ValueColor;
                    restartDissolutionTime = DateTime.Now;
                    if ((restartDissolutionTime - criticalSituationStartTime).Seconds > standartRestartDissolutionTime + 10) //норма +10сек без штрафа
                    {
                        penaltyScores += ((restartDissolutionTime - criticalSituationStartTime).Seconds -
                                          standartRestartDissolutionTime) * penaltyMultiplicator;
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
            {
                int NumberOfNotManualBrakes = 0;
                foreach (KeyValuePair<Guid, BrakeModeControl> Brake in e.BrakeModes)
                {
                    if (Brake.Value != BrakeModeControl.Manual)
                    {
                        NumberOfNotManualBrakes++;
                    }
                }
            
                penaltyScores += NumberOfNotManualBrakes * penaltyMultiplicator;
                isBrakeDone = true;
                if (isBrakeDone & isSwitchDone)
                {
                    // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                    isStarted = false;
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
            if (TypeDisrepair != TypeDisrepairGac.ManualBrake & isStarted)
            {
                int NumberOfNotManualSwitches = 0;
                foreach (KeyValuePair<Guid, SwitchModeControl> Switch in e.SwitchModes)
                {
                    if (Switch.Value != SwitchModeControl.Manual)
                    {
                        NumberOfNotManualSwitches++;
                    }
                }

                penaltyScores = NumberOfNotManualSwitches * penaltyMultiplicator;
            }
            isSwitchDone = true;
            if (isBrakeDone & isSwitchDone)
            {
                // Тормоза и стрелки проверены, штрафы начислены. Ситацию можно закрывать.
                isStarted = false;
            }
        }
        
        public class CriticalSituationGacEventArgs : EventArgs
        { // TODO: Подключить этот класс из внешней программы
            /// <summary>
            /// Сообщения для отображения в окне АРМ ДСПГ
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Тип нештатной ситуации
            /// </summary>
            public TypeDisrepairGac TypeDisrepair { get; set; }
        } 
        
        public enum TypeDisrepairGac
        { // TODO: Подключить этот класс из внешней программы
            /// <summary>
            /// ВНИМАНИЕ! ГАЦ МН НЕИСПРАВЕН
            /// </summary>
            BrokenGAC,
            /// <summary>
            /// ВНИМАНИЕ! НЕТ СВЯЗИ С ГАЦ МН
            /// </summary>
            LostConnectionWithGAC,
            /// <summary>
            /// ВНИМАНИЕ! СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 1 НЕВОЗМОЖЕН
            /// </summary>
            SwitchesDoNotMatchToTrackForIU1,
            /// <summary>
            /// ВНИМАНИЕ!  СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 2 НЕВОЗМОЖЕН
            /// </summary>
            SwitchesDoNotMatchToTrackForIU2,
            /// <summary>
            /// ВНИМАНИЕ!  СТРЕЛКИ СТОЯТ НЕ ПО МАРШРУТУ! РОСПУСК ПО ИУ 3 НЕВОЗМОЖЕН
            /// </summary>
            SwitchesDoNotMatchToTrackForIU3,
            /// <summary>
            /// !! ПРОВЕРЬ ПРОГРАММУ РОСПУСКА !! ВНИМАНИЕ! СБОЙ ИУ
            /// </summary>
            TUFailure,
            /// <summary>
            /// ВНИМАНИЕ!!  ТОРМОЗИТЬ ВРУЧНУЮ
            /// </summary>
            ManualBrake,
            /// <summary>
            /// ВНИМАНИЕ! ВЕСОМЕР ИУ 1 НЕИСПРАВЕН
            /// </summary>
            BrokenWeightIndicatorOfIU1,
            /// <summary>
            /// ВНИМАНИЕ! ВЕСОМЕР ИУ 2 НЕИСПРАВЕН
            /// </summary>
            BrokenWeightIndicatorOfIU2,
            /// <summary>
            /// ВНИМАНИЕ! ВЕСОМЕР ИУ 3 НЕИСПРАВЕН
            /// </summary>
            BrokenWeightIndicatorOfIU3,
            /// <summary>
            /// Нет нештатной ситуации
            /// </summary>
            None
        }
        
        public class StateSemaphoreEventArgs : EventArgs
        { // TODO: Подключить этот класс из внешней программы
            public Guid IdSemaphore; // идентификатор светофора
            public Guid IdObj; // идентификатор объекта к которому привязан светофор(путь надвига)
            public StageSemaphore ButtonStage; // кнопка нажатая на пульте
            public SemaphoreColor ValueColor; // показания светофора
            public double Speed; // скорость надвига
            public Role Owner; // идентификатор ДСПГ
        }

        public enum SemaphoreColor
        { // TODO: Подключить этот класс из внешней программы
            /// <summary>
            /// Красный При получении этого значения роспуск считается остановленным
            /// </summary>
            Red = 0,
            /// <summary>
            /// Зелёный
            /// </summary>
            Green = 1,
            /// <summary>
            /// Желтый(верхний)
            /// </summary>
            Yellow = 2,
            /// <summary>
            /// Желтый(верхний) и зеленый
            /// </summary>
            YellowGreen = 3,
            /// <summary>
            /// Два желтых
            /// </summary>
            YellowYellow = 4
        }
        public class StageSemaphore
        { // TODO: Подключить этот класс из внешней программы
            // Эмуляция класса
        }
        public class Role
        { // TODO: Подключить этот класс из внешней программы
            // Эмуляция класса
        }
        public class BrakeModesEventArgs : EventArgs
        { // TODO: Подключить этот класс из внешней программы
            public IDictionary<Guid, BrakeModeControl> BrakeModes { get; set; }
        }

        public class SwitchModesEventArgs : EventArgs
        { // TODO: Подключить этот класс из внешней программы
            public IDictionary<Guid, SwitchModeControl> SwitchModes { get; set; }
        }
    
        /// <summary>
        /// Режим управления стрелкой
        /// </summary>
        public enum SwitchModeControl
        { // TODO: Подключить этот класс из внешней программы
            /// <summary>
            /// Неизвестный тип управления
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Ручное управление с пульта
            /// </summary>
            Manual = 1,
            /// <summary>
            /// Автоматическое управление
            /// </summary>
            Automatic = 2
        }
    
        /// <summary>
        /// Режим управления замедлителем
        /// </summary>
        public enum BrakeModeControl
        { // TODO: Подключить этот класс из внешней программы
            /// <summary>
            /// Неизвестный тип управления
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Ручное управление с пульта
            /// </summary>
            Manual = 1,
            /// <summary>
            /// Автоматическое управление
            /// </summary>
            Automatic = 2,
            /// <summary>
            /// Отключение замедлителя
            /// </summary>
            Off = 3,
            /// <summary>
            /// Включение замедлителя
            /// </summary>
            On = 4
        }
    }
}