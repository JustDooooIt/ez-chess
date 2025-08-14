using System;
using Godot;

public abstract class PieceDecorator<T> : IPiece, IPieceProvider<T>, IInterfaceQueryable
{
	public event Action<int> ActionCompleted;

	protected IPiece _wrapped;

	public PieceDecorator(IPiece wrapped)
	{
		_wrapped = wrapped;
		(this as IPiece)?.SetWrapper(wrapped);
	}

	public GodotObject Origin { get => GetDeepWrapped<GodotObject>(); }
	public IInterfaceQueryable Proxy { get => (this as IPiece).GetProxy(); }
	public IInterfaceQueryable Wrapper { get; set; }
	public T Wrapped => GetDeepWrapped<T>();
	public PipelineAdapter PipelineAdapter { get => _wrapped.PipelineAdapter; set => _wrapped.PipelineAdapter = value; }
	public PiecesManager PiecesManager { get => _wrapped.PiecesManager; set => _wrapped.PiecesManager = value; }

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

	public void RemoveSelf()
	{
		_wrapped.SetWrapper((IPiece)Wrapper);
	}
}
