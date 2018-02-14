using System;
using System.Windows.Controls;

namespace mCubed.CoinCollector.Tiles {
	public class Player : GravityTile {
		#region Data Store

		private PlayerCharacter _character = PlayerCharacter.Suit;
		private ScoreArgs _score;

		#endregion

		#region Properties

		/// <summary>
		/// Get/set the character associated with the player
		/// </summary>
		public PlayerCharacter Character {
			get { return _character; }
			set { this.SetAndNotify(ref _character, value, null, OnCharacterChanged, "Character"); }
		}

		/// <summary>
		/// Get whether or not the side must be open when checking collision
		/// </summary>
		public override bool CheckOpenSide { get { return false; } }

		/// <summary>
		/// Get/set the score for the player
		/// </summary>
		public ScoreArgs Score {
			get { return _score; }
			set { this.SetAndNotify(ref _score, value, "Score"); }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new player for the game with the suit character
		/// </summary>
		public Player() : this(PlayerCharacter.Suit) { }

		/// <summary>
		/// Create a new player for the game with the given character
		/// </summary>
		/// <param name="character">The character to create the player with</param>
		public Player(PlayerCharacter character) : base(new Image
			{
				Source = GenerateImage("Player" + character.ToString() + ".gif")
			}) {
			ZIndex = int.MaxValue;
			Character = character;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Event that handles when the player's character has changed
		/// </summary>
		private void OnCharacterChanged() {
			Image image = Representation as Image;
			if (image != null)
				image.Source = GenerateImage("Player" + Character.ToString() + ".gif");
		}

		#endregion

		#region Members

		/// <summary>
		/// Begins the jump process for the player tile
		/// </summary>
		public void Jump() {
			Unground(0 - MagicNumbers.PLAYERVELOCITY);
		}

		#endregion

		#region BaseTile Members

		/// <summary>
		/// Perform the logic when the given tile collides with this tile
		/// </summary>
		/// <param name="tile">The tile that collided with this tile</param>
		/// <param name="side">The prominent side of the collision</param>
		/// <returns>The event arguments for the collision</returns>
		public override CollisionEventArgs CollideWith(Player tile, CollisionSide side) {
			throw new Exception("Cannot collide with another player tile.");
		}

		#endregion
	}
}