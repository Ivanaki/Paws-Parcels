
namespace Game.Params
{
    public class MainMenuEnterParams : BaseEnterParams
    {
        public string MessageToMainMenu { get; }
        
        public MainMenuEnterParams(string lastScene, string messageToMainMenu) : base(lastScene)
        {
            MessageToMainMenu = messageToMainMenu;
        }
    }
}
