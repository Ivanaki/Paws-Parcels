
namespace Game.Params
{
    public class MainMenuExitParams
    {
        public string SceneTo { get;}
        public BaseEnterParams BaseEnterParams { get;}

        public MainMenuExitParams(string sceneTo, BaseEnterParams baseEnterParams)
        {
            BaseEnterParams = baseEnterParams;
            SceneTo = sceneTo;
        }
    }
}
