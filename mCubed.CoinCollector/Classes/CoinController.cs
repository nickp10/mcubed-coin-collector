using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using mCubed.CoinCollector.Maps;
using mCubed.CoinCollector.Tiles;

namespace mCubed.CoinCollector {
	public class CoinController : INotifyPropertyChanged {
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private readonly IDisplayable _display;
		private bool _isPaused;
		private BaseMap _map;
		private readonly Player _player;
		private readonly DispatcherTimer _timer;

		#endregion

		#region Properties

		/// <summary>
		/// Get whether or not the game has started
		/// </summary>
		public bool IsStarted { get { return _timer.IsEnabled || IsPaused; } }

		/// <summary>
		/// Get whether or not the game is currently paused
		/// </summary>
		public bool IsPaused {
			get { return _isPaused; }
			private set { this.SetAndNotify(ref _isPaused, value, "IsPaused", "IsStarted"); }
		}

		/// <summary>
		/// Get the map that is currently associated with the controller
		/// </summary>
		public BaseMap Map {
			get { return _map; }
			private set { this.SetAndNotify(ref _map, value, "Map"); }
		}

		/// <summary>
		/// Get the player associated with the game
		/// </summary>
		public Player Player { get { return _player; } }

		/// <summary>
		/// Get the list of tiles currently on the canvas, excluding the player
		/// </summary>
		public List<BaseTile> Tiles { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new coin controller
		/// </summary>
		/// <param name="display">The display responsible for drawing the game area</param>
		public CoinController(IDisplayable display) {
			// Initialize
			_display = display;
			_player = new Player();
			_timer = new DispatcherTimer();
			Tiles = new List<BaseTile>();

			// Setup
			_timer.Interval = TimeSpan.FromMilliseconds(1000d / MagicNumbers.FRAMESPERSECOND);
			_timer.Tick += new EventHandler(OnTimerTicked);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Event that handles every tick of the timer
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnTimerTicked(object sender, EventArgs e) {
			NextFrame();
		}

		#endregion

		#region Map Generation Members

		/// <summary>
		/// Place the given tile on the canvas or remove it from the canvas
		/// </summary>
		/// <param name="tile">The tile to add or remove from the canvas</param>
		/// <param name="addTile">True to add the tile or false to remove the tile</param>
		public void PlaceTile(BaseTile tile, bool addTile) {
			if (addTile) {
				Tiles.Add(tile);
				_display.GameArea.Children.Add(tile.Representation);
			} else {
				Tiles.Remove(tile);
				_display.GameArea.Children.Remove(tile.Representation);
			}
		}

		/// <summary>
		/// Generate the map to play within
		/// </summary>
		/// <param name="map">The map to generate the game from</param>
		public void GenerateMap(BaseMap map) {
			// Clear the canvas
			_display.GameArea.Children.Clear();
			Tiles.Clear();
			IsPaused = false;

			// Reset the player
			Player.Score = new ScoreArgs();
			Player.Ground();
			_display.GameArea.Children.Add(Player.Representation);

			// Generate the map
			Map = map;
			Map.GenerateMap(this);

			// Fill the players score
			Player.Score.CoinsAvailable = Tiles.Count(t => t is Coin);
			Player.Score.MegaCoinsAvailable = Tiles.Count(t => t is MegaCoin);
		}

		#endregion

		#region Collision/Frame Members

		/// <summary>
		/// Move to the next frame in the game
		/// </summary>
		public void NextFrame() {
			// Move all the tiles
			foreach (BaseTile tile in Tiles)
				tile.Left -= MagicNumbers.TILEVELOCITY;

			// Perform the gravity
			Player.PerformGravity();
			foreach (GravityTile tile in Tiles.OfType<GravityTile>())
				tile.PerformGravity();

			// Remove all invisible tiles
			foreach (BaseTile tile in Tiles.Where(t => t.Right < 0).ToArray())
				PlaceTile(tile, false);

			// Perform collision check
			PerformCollisionCheck();
		}

		/// <summary>
		/// Perform the collision checking
		/// </summary>
		private void PerformCollisionCheck() {
			// Collide with the tiles
			List<CollisionEventArgs> args = new List<CollisionEventArgs>();
			foreach (BaseTile tile in Tiles) {
				if (tile.IsCollided(Player) || tile is Finish) {
					CollisionSide side = tile.GetCollisionSide(Player);
					if (!tile.CheckOpenSide || IsSideOpen(tile, side)) {
						CollisionEventArgs arg = tile.CollideWith(Player, side);
						PerformCollisionEvent(args, arg);
					}
				}
			}

			// Perform the remaining collisions
			PerformRemainingCollisions(args);
		}

		/// <summary>
		/// Perform the collision event for the given event args
		/// </summary>
		/// <param name="args">The list to add the collision event to if it should be handled later</param>
		/// <param name="arg">The collision event to handle</param>
		private void PerformCollisionEvent(List<CollisionEventArgs> args, CollisionEventArgs arg) {
			if (arg != null) {
				// Check if halting
				if (arg.Command.StartsWith("halt")) {
					foreach (BaseTile tile in Tiles)
						tile.Left += (double)arg.Argument;
				}

				// Check that we aren't only halting
				if (arg.Command != "halt")
					args.Add(arg);
			}
		}

		/// <summary>
		/// Perform the collision event for the remaining event args
		/// </summary>
		/// <param name="args">The list of args to perform</param>
		private void PerformRemainingCollisions(List<CollisionEventArgs> args) {
			// Setup
			bool isDead = false;
			bool isWon = false;
			bool unground = false;
			double ground = double.NaN;

			// Collision events
			foreach (CollisionEventArgs arg in args) {
				if (arg.Command == "coin") {
					Player.Score.CoinsCollected++;
					PlaceTile(arg.Sender, false);
				} else if (arg.Command == "megacoin") {
					Player.Score.MegaCoinsCollected++;
					PlaceTile(arg.Sender, false);
				} else if (arg.Command == "died") {
					isDead = true;
				} else if (arg.Command == "haltandwin") {
					isWon = true;
				} else if (arg.Command == "ground") {
					ground = (double)arg.Argument;
				} else if (arg.Command == "unground") {
					unground = true;
				}
			}

			// Unground the player
			if (unground) {
				Player.Ground();
				Player.Unground(MagicNumbers.GRAVITY);
			}

			// Ground the player
			if (Double.IsNaN(ground)) {
				Player.Unground();
			} else {
				bool wasGround = Player.Gravity == null;
				Player.Ground();
				Player.Bottom = ground;
				if (!wasGround && _display.JumpAgain())
					Player.Jump();
			}

			// Player reached the bottom
			if (Player.Top >= Map.MapHeight)
				isDead = true;

			// Check if the game is over
			if (isDead || isWon)
				GameOver(isDead);
		}

		/// <summary>
		/// Determine whether or not the collision side is open to collisions
		/// </summary>
		/// <param name="tile">The tile to check against</param>
		/// <param name="side">The side that the collision occurred</param>
		/// <returns>True if the side is open or false otherwise</returns>
		private bool IsSideOpen(BaseTile tile, CollisionSide side) {
			// Setup
			int xOffset = side == CollisionSide.Left ? -1 : (side == CollisionSide.Right ? 1 : 0);
			int yOffset = side == CollisionSide.Top ? -1 : (side == CollisionSide.Bottom ? 1 : 0);
			
			// Calculate the moved rectangle
			Rect rect = tile.Rect;
			rect.X = rect.X + (xOffset * MagicNumbers.TILESIZE) + 2;
			rect.Y = rect.Y + (yOffset * MagicNumbers.TILESIZE) + 2;
			rect.Width -= 4;
			rect.Height -= 4;

			return side == CollisionSide.None || !Tiles.Any(t => t != tile && t.IsCollided(rect));
		}

		#endregion

		#region Start/Stop/Pause Members

		/// <summary>
		/// Start the game
		/// </summary>
		public void Start() {
			if (!_timer.IsEnabled) {
				_timer.Start();
				this.OnPropertyChanged("IsStarted");
			}
		}

		/// <summary>
		/// Stop the game
		/// </summary>
		public void Stop() {
			if (_timer.IsEnabled) {
				_timer.Stop();
				this.OnPropertyChanged("IsStarted");
			}
		}

		/// <summary>
		/// Pause or unpause the game
		/// </summary>
		public void Pause() {
			// Pause or unpause the game
			if (IsPaused)
				Start();
			else
				Stop();

			// Update the pause state
			IsPaused = !IsPaused;
		}

		/// <summary>
		/// Perform the game over logic after a player either dies or finishes the map
		/// </summary>
		/// <param name="playerDied">True if the player died, or false if the player finished themap</param>
		private void GameOver(bool playerDied) {
			// Perform the player winning logic
			if (!playerDied)
				Player.Score.IsCompletedBonus = true;

			// Check the high score
			if (Player.Score.TotalScore > Map.HighScore) {
				Map.HighScore = Player.Score.TotalScore;
				Player.Score.IsHighScore = true;
			}

			// Stop the game and display the game over message
			Stop();
			_display.GameOver();
		}

		#endregion

		#region Members
		
		/// <summary>
		/// Causes the player to beginning jumping at the next frame
		/// </summary>
		public void Jump() {
			if (IsStarted && Player != null)
				Player.Jump();
		}

		#endregion
	}
}