namespace Messages
{
    public interface IMessageHandler
    {
        void OnMessage(MessageArgs messageArgs);
    }
}