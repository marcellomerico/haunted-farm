using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 80f;
	private AnimatedSprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = direction * Speed;
		MoveAndSlide();

		if (direction != Vector2.Zero)
		{

			if (direction.X > 0)
			{
				_sprite.Play("walk_right");

			}

			else if (direction.X < 0)
			{
				_sprite.Play("walk_left");

			}

			else if (direction.Y > 0)
			{
				_sprite.Play("walk_down");

			}
			else if (direction.Y < 0)
			{
				_sprite.Play("walk_up");
			}
		}
		else
		{
			_sprite.Stop();
		}


	}
}
