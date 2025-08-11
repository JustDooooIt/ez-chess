using System;
using Godot;

public abstract class PieceDecorator<T>(IPiece wrapped) : IPiece, IInterfaceQueryable, IPieceProvider<T>
{
	public event Action<int> ActionCompleted;

	protected IPiece _wrapped = wrapped;

	public T Piece => (T)_wrapped;

	public virtual V As<V>() where V : class
	{
		if (this is V selfImpl) return selfImpl;

		if (_wrapped is V wrappedImpl) return wrappedImpl;

		if (_wrapped is IInterfaceQueryable queryable)
			return queryable.As<V>();

		return null;
	}

	private V GetOrigin<V>()
	{
		if (_wrapped is PieceInstanceDecorator decorator1)
		{
			return decorator1.GetOrigin<V>();
		}
		else if (_wrapped is PieceStateDecorator decorator2)
		{
			return decorator2.GetOrigin<V>();
		}
		else if (_wrapped is V w)
		{
			return w;
		}
		return default;
	}

	public GodotObject GetOrigin()
	{
		return GetOrigin<GodotObject>();
	}

	public ulong GetInstanceId()
	{
		return GetOrigin().GetInstanceId();
	}

}
