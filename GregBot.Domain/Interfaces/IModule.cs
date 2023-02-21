namespace GregBot.Domain.Interfaces;
public interface IModule
{
    string Name { get; }
    void Load(IGregBot bot);
}
