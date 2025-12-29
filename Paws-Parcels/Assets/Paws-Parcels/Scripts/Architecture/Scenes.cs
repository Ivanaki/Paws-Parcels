namespace Game.Root
{
    public static class Scenes
    {
        public const string BOOT = "Boot";
        public const string MAINMENU = "MainMenu";
        public const string CORE = "Core";
        //public const string LEVEL2 = "Level2";

        public static readonly string[] GameplayScenes = {"Core"};
    }

    public enum GameplayScenes
    {
        Core,
        //Level2
    }
}