using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves;
using R3;

namespace Paws_Parcels.Scripts
{
    public class LoadService
    {
        public Observable<Guid> LoadCoreSceneWithSaveId => _loadCoreSceneWithSaveIdSubject;
        
        private readonly Subject<Guid> _loadCoreSceneWithSaveIdSubject = new();

        public LoadService()
        {
            
        }

        public void LoadGame(Guid idSave)
        {
            
        }

        public void DeleteSave(Guid idSave)
        {
            
        }

        public IEnumerable<CoreSaveMeta> GetAllSavesMeta()
        {
            return null;
        }
    }
}