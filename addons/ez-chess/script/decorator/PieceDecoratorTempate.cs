public abstract partial class PieceDecorator<TEvent> : PieceDecorator, IAction<TEvent>
	where TEvent : PieceEvent
{
	protected bool IsAutoSubmit { get; set; } = true;

	protected PieceDecorator(IPiece wrapped) : base(wrapped) { }

	protected abstract void _ReciveEvent(TEvent @event);

	protected virtual void _SaveOperation(TEvent @event) { }

	protected virtual Operation _ToOperation(TEvent @event)
	{
		return null;
	}

	protected virtual void _SaveOperation(Operation op)
	{
		GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, op);
	}

	public void ReciveEvent(TEvent @event)
	{
		_ReciveEvent(@event);
		// 判断是否为恢复棋盘阶段,以及判断当前操作者是否为玩家
		if (IsAutoSubmit)
		{
			if (!@event.recovered && PipelineAdapter is not OtherPipeline)
			{
				Operation op;
				if ((op = _ToOperation(@event)) != null)
				{
					_SaveOperation(op);
				}
			}
		}
		else
		{
			_SaveOperation(@event);
		}
	}
}
