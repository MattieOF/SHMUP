using Godot;

public partial class GameOver : CanvasLayer
{
	public void StartNewRun() => GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	public void ReturnToMenu() => GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
	public void QuitToDesktop() => GetTree().Quit();
}
