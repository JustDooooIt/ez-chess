using Godot;
using System;

public partial class Camera : Camera2D
{
	// --- EXPORTED VARIABLES ---
	[ExportGroup("Toggles")]
	[Export] public bool IsActive { get; set; } = true;
	[Export] public bool DragPanEnabled { get; set; } = true;
	[Export] public bool ScrollZoomEnabled { get; set; } = true;

	[ExportGroup("Pan Settings")]
	[Export] public MouseButton PanButton { get; set; } = MouseButton.Left;
	[Export(PropertyHint.Range, "0.1, 5.0")] public float DragSpeed { get; set; } = 1.0f;

	[ExportGroup("Zoom Settings")]
	[Export(PropertyHint.Range, "1.01, 2.0")] public float ZoomSpeed { get; set; } = 1.1f;
	[Export(PropertyHint.Range, "0.1, 1.0")] public float MinZoom { get; set; } = 0.2f;
	[Export(PropertyHint.Range, "1.0, 10.0")] public float MaxZoom { get; set; } = 5.0f;
	[Export] public bool InvertScrollDirection { get; set; } = true;

	// --- PRIVATE VARIABLES ---
	private bool isDragging = false;

	public override void _Input(InputEvent @event)
	{
		if (!IsActive)
		{
			return;
		}
		if (DragPanEnabled)
		{
			HandleDragPan(@event);
		}
		if (ScrollZoomEnabled)
		{
			HandleScrollZoom(@event);
		}
	}

	private void HandleDragPan(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == PanButton)
		{
			isDragging = mouseButtonEvent.Pressed;
		}

		if (@event is InputEventMouseMotion mouseMotionEvent && isDragging)
		{
			// ===================================================================
			// THE FIX IS HERE: Changed from multiplication (*) to division (/)
			// This correctly scales the screen-space mouse motion to world-space camera motion.
			Position -= mouseMotionEvent.Relative * DragSpeed / Zoom;
			// ===================================================================
		}
	}

	private void HandleScrollZoom(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseWheelEvent && mouseWheelEvent.Pressed)
		{
			float zoomAmount = 0;
			float zoomInFactor = 1 / ZoomSpeed;
			float zoomOutFactor = ZoomSpeed;

			if (mouseWheelEvent.ButtonIndex == MouseButton.WheelUp)
			{
				zoomAmount = InvertScrollDirection ? zoomOutFactor : zoomInFactor;
			}
			else if (mouseWheelEvent.ButtonIndex == MouseButton.WheelDown)
			{
				zoomAmount = InvertScrollDirection ? zoomInFactor : zoomOutFactor;
			}

			if (zoomAmount != 0)
			{
				Vector2 worldPosBeforeZoom = GetGlobalMousePosition();
				Vector2 newZoom = Zoom * zoomAmount;
				newZoom.X = Mathf.Clamp(newZoom.X, MinZoom, MaxZoom);
				newZoom.Y = Mathf.Clamp(newZoom.Y, MinZoom, MaxZoom);
				Zoom = newZoom;
				Vector2 worldPosAfterZoom = GetGlobalMousePosition();
				Position += worldPosBeforeZoom - worldPosAfterZoom;
			}
		}
	}
}
