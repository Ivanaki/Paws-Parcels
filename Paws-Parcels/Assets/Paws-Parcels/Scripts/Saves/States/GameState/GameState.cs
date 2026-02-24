using System;
using System.Collections.Generic;
using Items;
using Paws_Parcels.Scripts.Quests;
using UnityEngine;
using Zenject;


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

    /*[Serializable]
    public class HandsItemSaveData
    {
        public bool isNull = false;
        public string ItemTypeId = "";
        public string MarkId = "";

        public HandsItemSaveData()
        {
        }
        
        public HandsItemSaveData(HandsItemSaveData origin)
        {
            ItemTypeId = origin.ItemTypeId;
            MarkId = origin.MarkId;
            isNull = origin.isNull;
        }

        public HandsItemSaveData(ItemProxy itemProxy)
        {
            isNull = false;
            ItemTypeId = itemProxy.ItemTypeId;
            MarkId = itemProxy.MarkId;
        }
    }*/
    
    /*[Serializable]
    public struct CarSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        // и ещё вращение камеры
    }*/

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
        public string SelectedQuest;
        public List<string> SucceedQuests = new();
        public List<string> CurrentActiveDestinations = new();
        
        //Player
        public PlayerSaveData PlayerData;
        
        //Items
        public List<ItemData> ItemsInWorld = new();
        //public HandsItemSaveData ItemInHands;
        public bool IsItemInHandsNull = true;
        public string InHandsItemTypeId = "";
        public string InHandsMarkId = "";
        
        
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