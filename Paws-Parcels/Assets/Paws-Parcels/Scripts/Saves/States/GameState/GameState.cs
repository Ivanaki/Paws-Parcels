using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Quests;


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
        
        //Quests
        public List<QuestData> ActiveQuests = new();
        public string SelectedQuest;
        public List<string> SucceedQuests = new();
        public List<string> CurrentActiveDestinations = new();
        
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