using System.Threading.Channels;

public interface ILaunchableProvider<T>
{
    Channel<T> LaunchableList { get; }
}