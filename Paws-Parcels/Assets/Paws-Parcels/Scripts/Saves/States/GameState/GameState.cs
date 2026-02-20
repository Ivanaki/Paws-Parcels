using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Quests;
using UnityEngine;


namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public struct PlayerSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Yaw;
        public float Pitch;
    }
    
    [Serializable]
    public struct CarSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        // и ещё вращение камеры
    }
    
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
        
        //Player
        public PlayerSaveData PlayerData;
        
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