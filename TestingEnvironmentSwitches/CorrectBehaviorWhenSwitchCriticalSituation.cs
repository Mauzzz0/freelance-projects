using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace TestingEnvironmentSwitches
{
    public class CorrectBehaviorWhenSwitchCriticalSituation
    {
        
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