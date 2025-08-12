using System;
using Godot;

public abstract class PieceDecorator<T>(IPiece wrapped) : IPiece, IPieceProvider<T>, IInterfaceQueryable
{
	public event Action<int> ActionCompleted;

	protected IPiece _wrapped = wrapped;

	public GodotObject Origin => GetDeepWrapped<GodotObject>();
	public T Wrapped => GetDeepWrapped<T>();
	public PipelineAdapter PipelineAdapter { get => ((IPiece)Origin).PipelineAdapter; set => ((IPiece)Origin).PipelineAdapter = value; }

	public virtual V As<V>() where V : class
	{
		if (this is V selfImpl) return selfImpl;

		if (_wrapped is V wrappedImpl) return wrappedImpl;

		if (_wrapped is IInterfaceQueryable queryable)
			return queryable.As<V>();

		return null;
	}

	private V GetDeepWrapped<V>()
	{
		if (_wrapped is PieceDecorator<IPieceState> decorator1)
		{
			return decorator1.GetDeepWrapped<V>();
		}
		if (_wrapped is PieceDecorator<IPieceInstance> decorator2)
		{
			return decorator2.GetDeepWrapped<V>();
		}
		if (_wrapped is PieceDecorator<IPiece> decorator3)
		{
			return decorator3.GetDeepWrapped<V>();
		}
		else if (_wrapped is V w)
		{
			return w;
		}
		return default;
	}

	public ulong GetInstanceId()
	{
		return Origin.GetInstanceId();
	}

}
