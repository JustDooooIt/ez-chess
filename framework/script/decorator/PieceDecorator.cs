using System;
using System.Collections.Generic;
using Godot;

public abstract partial class PieceDecorator : RefCounted, IPiece
{
	private IPiece _wrapped;

	protected PieceDecorator(IPiece wrapped)
	{
		_wrapped = wrapped;
		(this as IPiece).SetWrapper(wrapped);
	}

	public GodotObject Origin => _wrapped.Origin;
	public IPiece Wrapped => _wrapped;
	public IInterfaceQueryable Proxy => (this as IPiece).GetProxy();
	public IInterfaceQueryable Wrapper { get; set; }
	public PlayerPipeline PipelineAdapter { get => _wrapped.PipelineAdapter; set => _wrapped.PipelineAdapter = value; }
	public PiecesManager PiecesManager { get => _wrapped.PiecesManager; set => _wrapped.PiecesManager = value; }
	public int Faction { get => _wrapped.Faction; set => _wrapped.Faction = value; }
	public int PieceType { get => _wrapped.PieceType; set => _wrapped.PieceType = value; }

	public virtual V As<V>() where V : class
	{
		if (this is V selfImpl) return selfImpl;
		if (_wrapped is V wrappedImpl) return wrappedImpl;
		if (_wrapped is IInterfaceQueryable queryable) return queryable.As<V>();
		return null;
	}

	protected V GetDeepWrapped<V>()
	{
		if (_wrapped is V w) return w;
		else if (_wrapped is PieceDecorator d) return d.GetDeepWrapped<V>();
		return default;
	}

	protected ulong GetPieceInstanceId() => PieceAdapter.GetInstanceFromState(Origin.GetInstanceId());

	protected ulong GetPieceId()
	{
		return (InstanceFromId(GetPieceInstanceId()) as Node).GetParent().GetInstanceId();
	}
}