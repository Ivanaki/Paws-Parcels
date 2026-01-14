
namespace Game.Params
{
    public class MainMenuEnterParams : BaseEnterParams
    {
        public string MessageToMainMenu { get; }
        public bool IsWasErrorWhileLoadCoreScene { get; }
        public string MessageToUser { get; }
        
        public MainMenuEnterParams(string lastScene, string messageToMainMenu,bool isWasErrorWhileLoadCoreScene, 
            string messageToUser) : base(lastScene)
        {
            MessageToMainMenu = messageToMainMenu;
            IsWasErrorWhileLoadCoreScene = isWasErrorWhileLoadCoreScene;
            MessageToUser = messageToUser;
        }
    }
}
