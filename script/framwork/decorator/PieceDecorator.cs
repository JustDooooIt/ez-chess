using System;
using Godot;

public abstract class PieceDecorator<T>(IPiece wrapped) : IPiece, IPieceProvider<T>, IInterfaceQueryable
{
	public event Action<int> ActionCompleted;

	protected IPiece _wrapped = wrapped;

	public T OriginPiece => (T)_wrapped;

	public Pipeline StatePipeline { get => _wrapped.StatePipeline; set => _wrapped.StatePipeline = value; }
	public Pipeline RenderPipeline { get => _wrapped.RenderPipeline; set => _wrapped.RenderPipeline = value; }

	public virtual V As<V>() where V : class
	{
		if (this is V selfImpl) return selfImpl;

		if (_wrapped is V wrappedImpl) return wrappedImpl;

		if (_wrapped is IInterfaceQueryable queryable)
			return queryable.As<V>();

		return null;
	}

	private V GetWrapped<V>()
	{
		if (_wrapped is PieceInstanceDecorator decorator1)
		{
			return decorator1.GetWrapped<V>();
		}
		else if (_wrapped is PieceStateDecorator decorator2)
		{
			return decorator2.GetWrapped<V>();
		}
		else if (_wrapped is V w)
		{
			return w;
		}
		return default;
	}

	public GodotObject GetWrapped()
	{
		return GetWrapped<GodotObject>();
	}

	public ulong GetInstanceId()
	{
		return GetWrapped().GetInstanceId();
	}

}
