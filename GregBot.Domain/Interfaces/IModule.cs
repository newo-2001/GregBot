namespace GregBot.Domain.Interfaces;
public interface IModule
{
    public string Name { get; }
    public void Activate(GregBot bot);
}
