using System;
using System.Collections;
using System.Collections.Generic;

namespace TestingEnvironment
{
    public class CriticalSituationGacEventArgs : EventArgs
    {
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
    {
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
    {
        public Guid IdSemaphore; // идентификатор светофора
        public Guid IdObj; // идентификатор объекта к которому привязан светофор(путь надвига)
        // TODO: Лексическая ошибка в ТЗ StageSemafor -> StageSemaphore
        public StageSemaphore ButtonStage; // кнопка нажатая на пульте
        public SemaphoreColor ValueColor; // показания светофора
        public double Speed; // скорость надвига
        public Role Owner; // идентификатор ДСПГ
    }

    public enum SemaphoreColor
    {
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

    public class GetObjectStatesEventArgs : EventArgs
    {
        public IEnumerable<ObjectType> ObjectTypes { get; set; } 
    }

    public enum ObjectType
    {
        Retarder,
        Switch
    }

    class BrakeModesEventArgs : EventArgs
    {
        public IDictionary<Guid, BrakeModeControl> BrakeModes { get; set; }
    }

    class SwitchModesEventArgs : EventArgs
    {
        public IDictionary<Guid, SwitchModeControl> SwitchModes { get; set; }
    }
    
    /// <summary>
    /// Режим управления стрелкой
    /// </summary>
    public enum SwitchModeControl
    {
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
    {
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

    public class StageSemaphore
    {
        // Эмуляция класса
    }

    public class Role
    {
        // Эмуляция класса
    }
}