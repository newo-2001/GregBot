namespace GregBot.Domain.Interfaces;
public interface IModule
{
    string Name { get; }
    void Activate(IGregBot bot);
}
