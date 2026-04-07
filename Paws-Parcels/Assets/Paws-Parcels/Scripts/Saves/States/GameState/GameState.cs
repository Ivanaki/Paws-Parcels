using System;
using System.Collections.Generic;
using Items;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using R3;
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

    [Serializable]
    public class ItemDestinationData
    {
        public List<ItemClearData> Items = new();
    }
    
    public class ItemDestinationProxy
    {
        public readonly ItemDestinationData Origin;
        
        public ObservableList<ItemClearProxy> Items { get; } = new();

        public ItemDestinationProxy(ItemDestinationData origin)
        {
            Origin = origin;
            
            origin.Items.ForEach(item => Items.Add(new ItemClearProxy(item)));

            Items.ObserveAdd().Subscribe(e => 
                origin.Items.Add(e.Value.Origin));

            Items.ObserveRemove().Subscribe(e => 
                origin.Items.Remove(e.Value.Origin));
            
            Items.ObserveClear().Subscribe(e => 
                origin.Items.Clear());
        }
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
        public string InHandsDestinationId = "";
        
        //Items in car
        public List<ItemClearData> ItemsInsideCar = new();
        
        //car&player
        public bool IsPlayerInCar;
        public bool IsCarDoorOpen;
        
        //Resources
        public List<ResourceData> Resources = new();
        
        //ItemDestinations
        public Dictionary<string, ItemDestinationData> ItemsDestinations = new();
        
        //cat
        public bool IsCatInCar;
        
        //stopCar
        public bool StopCar;
        
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