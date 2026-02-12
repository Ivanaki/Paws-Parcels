namespace Quests
{
    public interface IMessageHandler
    {
        void OnMessage(MessageArgs messageArgs);
    }
}