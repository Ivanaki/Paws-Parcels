using R3;

namespace Game.Root
{
    public class CoreScenesChanger
    {
        public readonly Subject<Unit> RestartSignal = new();
        public readonly Subject<Unit> MainMenuSignal = new();
    }
}