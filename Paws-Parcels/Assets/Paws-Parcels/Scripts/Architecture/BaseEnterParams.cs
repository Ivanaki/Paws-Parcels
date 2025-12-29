namespace Game.Params
{
    public abstract class BaseEnterParams
    {
        public string LastScene { get; }

        public BaseEnterParams(string lastScene)
        {
            LastScene = lastScene;
        }
        
        public T As<T>() where T : BaseEnterParams
        {
            return (T)this;
        }
    }
}