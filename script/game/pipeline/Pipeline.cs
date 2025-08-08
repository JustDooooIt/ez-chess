using Godot;
using System;
using System.Collections.Generic;

/*
 *
 */
public partial class Pipeline : RefCounted, ILaunchable
{

  private States state;
  private List<ValveGroup> _valves;

  /*
   * 启动管线流程,直至接受到停止信号
   */
  public void Launch()
  {

  }

  public enum States
  {
    IDLE, LAUNCH, STOP
  }
}
