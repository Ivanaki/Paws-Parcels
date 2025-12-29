
namespace Game.Params
{
    public class CoreExitParams
    {
        public string SceneTo { get;}
        public BaseEnterParams BaseEnterParams { get;}

        public CoreExitParams(string sceneTo, BaseEnterParams baseEnterParams)
        {
            BaseEnterParams = baseEnterParams;
            SceneTo = sceneTo;
        }
    }
}
