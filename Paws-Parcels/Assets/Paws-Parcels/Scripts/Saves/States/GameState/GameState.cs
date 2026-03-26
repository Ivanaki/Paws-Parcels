using System;
using System.Collections.Generic;
using Items;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using UnityEngine;
using Zenject;


namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public struct PlayerSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        
        public float Speed;
        public float Verticalvelocity;
        public float TargetRotation;
        public float Yaw;
        public float Pitch;
        public float AnimationBlend;
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
    
    //не забывать обновлять GameStateCollecter
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
        public string SelectedQuest = "";
        public List<string> SucceedQuests = new();
        public List<string> CurrentActiveDestinations = new();
        
        //Player
        public PlayerSaveData PlayerData;
        
        //Car
        public CarSaveData CarData;
        
        //Items in world
        public List<ItemData> ItemsInWorld = new();
        
        //Items in hands
        public bool IsItemInHandsNull = true;
        public string InHandsItemTypeId = "";
        public string InHandsMarkId = "";
        
        //Items in car
        public List<ItemInsideCar> ItemsInsideCar = new();
        
        //car&player
        public bool IsPlayerInCar;
        public bool IsCarDoorOpen;
        
        //Resources
        public List<ResourceData> Resources = new();
        
        
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