using R3;

namespace Game.Root
{
    public class CoreSignals
    {
        public readonly Subject<Unit> RestartSignal = new();
        public readonly Subject<Unit> NewGameSignal = new();
        public readonly Subject<Unit> MainMenuSignal = new();
    }
}
