using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public abstract partial class PipelineImpl<T> : Pipeline, ILaunchableProvider<T> where T : class, ILaunchable
{
  protected CancellationTokenSource readCts;
  public Channel<T> LaunchableList { get; } = Channel.CreateUnbounded<T>();
  public States State { get; protected set; } = States.IDLED;

  public override async void Launch()
  {
    if (State == States.LAUNCHING) 
      return;

    readCts = new CancellationTokenSource();
    State = States.LAUNCHING;
    while (await LaunchableList.Reader.WaitToReadAsync(readCts.Token))
    {
      if (LaunchableList.Reader.TryRead(out var launchable))
      {
        launchable.Launch();
      }
    }
    State = States.STOPED;
  }

  public override void Stop()
  {
    if (State != States.LAUNCHING)
      return;
    readCts.CancelAsync();
    State = States.STOPED;
  }

  public override void AddValve(Valve valve)
  {
    LaunchableList.Writer.WriteAsync(valve as T).AsTask().Wait();
    OnValveAdded(valve);
  }
}
