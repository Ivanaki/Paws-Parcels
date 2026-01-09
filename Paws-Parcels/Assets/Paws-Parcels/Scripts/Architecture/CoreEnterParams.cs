
using System;
using Game.Root;

namespace Game.Params
{
    public class CoreEnterParams : BaseEnterParams
    {
        public string DataCore { get; }
        public LoadType LoadType { get; }
        public Guid SaveNumber { get; }
        
        public CoreEnterParams(string lastScene, string dataCore, LoadType loadType, Guid saveNumber) : base(lastScene)
        {
            DataCore = dataCore;
            LoadType = loadType;
            SaveNumber = saveNumber;
        }
    }
}
