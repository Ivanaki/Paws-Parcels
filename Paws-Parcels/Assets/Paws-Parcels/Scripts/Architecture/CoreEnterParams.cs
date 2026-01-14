
using System;
using Game.Root;

namespace Game.Params
{
    public class CoreEnterParams : BaseEnterParams
    {
        public LoadType LoadType { get; }
        public Guid SaveNumber { get; }
        
        
        public CoreEnterParams(string lastScene, LoadType loadType, Guid saveNumber) : base(lastScene)
        {
            LoadType = loadType;
            SaveNumber = saveNumber;
        }
    }
}
