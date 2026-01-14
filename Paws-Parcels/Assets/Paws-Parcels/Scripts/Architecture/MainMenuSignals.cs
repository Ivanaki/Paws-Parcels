using R3;

namespace Game.Root
{
    public class MainMenuSignals
    {
        public readonly Subject<Unit> RestartSignal = new();
        public readonly Subject<Unit> NewGameSignal = new();
        public readonly Subject<Unit> ExitToDesktopSignal = new();
    }
}