using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;

namespace Paws_Parcels.Scripts.Saves
{
    [System.Serializable]
    public class CoreSaveMain
    {
        public string Id; //Guid
        public Dictionary<string, object> Saves;
    }
    
    public class CoreSaveManager
    {
        private const string Path = "GameSaves";
        
        public void Save()
        {
            //собрать все данные
            //создать папку сохранения
            //сохранить туда мету
            //сделать и сохранить скриншот
            //сохранить основное сохранение
        }

        public void Load(Guid guid)
        {
            //загрузить нужное сохранение
            //отдать данные всем подписчикам
        }
        
        public void Delete(Guid guid)
        {
            //удалить сохранение по ключу
        }

        public GameMeta[] GetAllSavesMeta()
        {
            return null;
        }

        //считывает все метаданные сейвов из папки и записывает их в словарь
        private void UpdateSavesList()
        {
            
        }
    }
}