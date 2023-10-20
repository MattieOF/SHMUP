using Godot;

public partial class FPSCounter : Control
{
	[Export] public Control bg;
	[Export] public RichTextLabel text;

	public override void _Ready()
	{
		Timer timer = new Timer();
		timer.OneShot = false;
		timer.WaitTime = 0.5f;
		timer.Connect(Timer.SignalName.Timeout, Callable.From(() => text.Text = $"[center][b]FPS[/b]   {Engine.GetFramesPerSecond()}"));
		AddChild(timer);
		timer.Start();
	}

	public override void _Input(InputEvent @event)
	{
		return; // For now ignore input
		if (!@event.IsActionPressed("showfps")) return;
		
		bg.Visible = !bg.Visible;
		GetViewport().SetInputAsHandled();
	}
}
