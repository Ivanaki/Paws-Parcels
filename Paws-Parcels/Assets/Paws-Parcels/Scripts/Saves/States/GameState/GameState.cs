using System;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [System.Serializable]
    public class GameState
    {
        public Guid SaveId; //Guid
        public int NextSavableEntityID;
        public int NextNonSavableEntityID;
        
        public int TrackedMissionID;
        public bool IsCompassActive;
        
        public int GetNewSavableEntityID()
        {
            return NextSavableEntityID++;
        }
        
        public int GetNewNonSavableEntityID()
        {
            return NextNonSavableEntityID++;
        }
    }
}