using System.Threading.Channels;
using System.Threading.Tasks;

public interface ILaunchable
{
  void Launch();
}