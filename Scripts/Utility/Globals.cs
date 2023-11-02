using Godot;

public partial class Globals : Node
{
	public static Globals Instance;
	
	[Export] public LoopingAudioStreamPlayer music;
	[Export] public FPSCounter fpsCounter;

	[ExportCategory("Global Asset Instances")]
	[Export] public PackedScene ResourceNotif;
	
	private Tween _musicTween;

	public float LookAheadDistance = 70;
	public float LookAheadSpeed    = 150;

	public override void _Ready()
	{
		Instance = this;
	}

	public void StopMusic()
	{
		if (_musicTween is not null && _musicTween.IsRunning())
			_musicTween.Stop();

		_musicTween = music.CreateTween();
		_musicTween.TweenProperty(music, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 0.1, 0.5f);
	}

	public void StartMusic()
	{
		if (_musicTween is not null && _musicTween.IsRunning())
			_musicTween.Stop();

		_musicTween = music.CreateTween();
		_musicTween.TweenProperty(music, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 1, 0.5f);
	}
}
