namespace CqrsBlog.Models
{
    public interface IHandles<in T>
    {
        void Handle(T @event);
    }
}