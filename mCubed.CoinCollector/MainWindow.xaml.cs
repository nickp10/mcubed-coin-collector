using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using mCubed.CoinCollector.Maps;

namespace mCubed.CoinCollector
{
	public partial class MainWindow : Window, INotifyPropertyChanged, IDisplayable
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private CoinController _controller;
		private readonly ObservableCollection<MapSet> _mapSets = new ObservableCollection<MapSet>();
		private FrameworkElement _prevScreen;
		private BaseMap _selectedMap;
		private MapSet _selectedMapSet;

		#endregion

		#region Properties

		/// <summary>
		/// Get the controller associated with the game play
		/// </summary>
		public CoinController Controller
		{
			get { return _controller; }
			private set { this.SetAndNotify(ref _controller, value, "Controller"); }
		}

		/// <summary>
		/// Get the set of map sets associated with this game
		/// </summary>
		public ObservableCollection<MapSet> MapSets { get { return _mapSets; } }

		/// <summary>
		/// Get the selected map that is currently being played
		/// </summary>
		public BaseMap SelectedMap
		{
			get { return _selectedMap; }
			private set { this.SetAndNotify(ref _selectedMap, value, "SelectedMap"); }
		}

		/// <summary>
		/// Get the selected map set to choose a map from
		/// </summary>
		public MapSet SelectedMapSet
		{
			get { return _selectedMapSet; }
			private set { this.SetAndNotify(ref _selectedMapSet, value, "SelectedMapSet"); }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new coin collector window
		/// </summary>
		public MainWindow()
		{
			// Initialize
			InitializeComponent();
			Controller = new CoinController(this);

			// Load all the mapsets
			LoadMapsets(false);

			// Load the home screen
			ShowScreen("HomeScreen");

			// Setup event handlers
			AddHandler(MainWindow.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
			Closing += delegate { Serialize(); };
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Event that handles when the character has been checked
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnCharacterChecked(object sender, RoutedEventArgs e)
		{
			FrameworkElement element = sender as FrameworkElement;
			Controller.Player.Character = (PlayerCharacter)element.DataContext;
		}

		/// <summary>
		/// Event that handles when a key has been pressed
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (!e.IsRepeat)
			{
				if (e.Key == Key.Space)
				{
					Controller.Jump();
				}
				else if (e.Key == Key.Escape)
				{
					if (Controller.IsStarted)
						OnResumeClicked(null, null);
				}
			}
		}

		/// <summary>
		/// Event that handles when a mapset has been selected
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnMapsetClicked(object sender, MouseButtonEventArgs e)
		{
			// Set the mapset
			FrameworkElement element = sender as FrameworkElement;
			SelectedMapSet = element.DataContext as MapSet;

			// Show the maps screen
			ShowScreen("Maps");
		}

		/// <summary>
		/// Event that handles when a map has been selected
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnMapClicked(object sender, MouseButtonEventArgs e)
		{
			// Set the map
			FrameworkElement element = sender as FrameworkElement;
			SelectedMap = element.DataContext as BaseMap;

			// Show the map
			ShowScreen("GamePlay");

			// Load the map
			Controller.GenerateMap(SelectedMap);
			Controller.Start();
		}

		/// <summary>
		/// Event that handles when the game has been quit
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnQuitClicked(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Event that handles when resume has been clicked
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnResumeClicked(object sender, MouseButtonEventArgs e)
		{
			Controller.Pause();
			SetGameScreenVisibility("Pause", Controller.IsPaused);
		}

		/// <summary>
		/// Event that handles when the current map's retry is clicked
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnRetryClicked(object sender, MouseButtonEventArgs e)
		{
			SetGameScreenVisibility("Pause", false);
			SetGameScreenVisibility("GameOver", false);
			Controller.Stop();
			Controller.GenerateMap(SelectedMap);
			Controller.Start();
		}

		/// <summary>
		/// Event that handles when the next screen should be displayed
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		private void OnShowScreenClicked(object sender, MouseButtonEventArgs e)
		{
			// Get the next screen
			FrameworkElement element = sender as FrameworkElement;
			if (element != null)
			{
				// Show the screen
				ShowScreen(element.Tag as string);
			}
		}

		#endregion

		#region Mapset Members

		/// <summary>
		/// Load all the mapsets for the game
		/// </summary>
		/// <param name="isReload">True if the mapstes are being reloaded, or false otherwise</param>
		private void LoadMapsets(bool isReload)
		{
			// Clear the existing mapsets
			MapSets.Clear();

			// Load all the default mapsets
			string[] maps = new string[] { "Beginner.mps", "Intermediate.mps" };
			foreach (string map in maps)
				MapSets.Add(new MapSet(map, true));

			// Load all the directory mapsets
			if (Directory.Exists("Mapsets"))
			{
				foreach (string filepath in Directory.GetFiles("Mapsets"))
				{
					string filename = filepath.Split('\\').Last();
					if (filename.EndsWith(".mps"))
						MapSets.Add(new MapSet(filename, false));
				}
			}

			// Load the high scores
			Deserialize(!isReload);
		}

		#endregion

		#region Serialization Members

		/// <summary>
		/// Serialize the current state of the game to an XML file
		/// </summary>
		private void Serialize()
		{
			// Create the document
			XDocument doc = new XDocument() { Declaration = new XDeclaration("1.0", "UTF-8", "yes") };
			XElement root = new XElement("CoinCollector");

			// Serialize the settings
			root.Add(new XElement("Settings",
				new XElement("Player",
					new XAttribute("Character", Controller.Player.Character))));

			// Serialize the high scores
			XElement mapsetselement = new XElement("Mapsets");
			foreach (MapSet mapset in MapSets)
			{
				XElement mapsetelement = new XElement("Mapset",
					new XAttribute("Filename", mapset.Filename));
				foreach (BaseMap map in mapset.Maps)
				{
					mapsetelement.Add(new XElement("Map",
						new XAttribute("MapNumber", map.MapNumber),
						new XAttribute("HighScore", map.HighScore)));
				}
				mapsetselement.Add(mapsetelement);
			}
			root.Add(mapsetselement);

			// Save the document
			doc.Add(root);
			doc.Save("CoinCollector.xml");
		}

		/// <summary>
		/// Deserialize the XML file into the game
		/// </summary>
		/// <param name="loadSettings">True to load the settings and high scores, or false for just the high scores</param>
		private void Deserialize(bool loadSettings)
		{
			try
			{
				// Load the document
				XDocument doc = XDocument.Load("CoinCollector.xml");
				XElement root = doc.Root;

				// Load the settings
				if (loadSettings)
				{
					Controller.Player.Character = root.Element("Settings").Element("Player").Parse("Character", PlayerCharacter.Suit);
				}

				// Load the maps' high scores
				foreach (XElement mapset in root.Element("Mapsets").Elements("Mapset"))
				{
					MapSet set = MapSets.FirstOrDefault(m => m.Filename == mapset.Parse<string>("Filename"));
					if (set != null)
					{
						foreach (XElement map in mapset.Elements("Map"))
						{
							BaseMap baseMap = set.Maps.FirstOrDefault(m => m.MapNumber == map.Parse<int>("MapNumber"));
							if (baseMap != null)
							{
								baseMap.HighScore = map.Parse<ulong>("HighScore");
							}
						}
					}
				}
			}
			catch { }
		}

		#endregion

		#region Display Members

		/// <summary>
		/// Show the screen with the given name, hiding the previous screen
		/// </summary>
		/// <param name="name">The name of the element to show</param>
		private void ShowScreen(string name)
		{
			// Hide the previous screen
			if (_prevScreen != null)
			{
				_prevScreen.Visibility = Visibility.Collapsed;
				if (_prevScreen.Name == "Settings")
					Serialize();
			}

			// Find the next screen and show it
			_prevScreen = (FrameworkElement)FindName(name);
			_prevScreen.Visibility = Visibility.Visible;

			// Reset the screen
			if (_prevScreen.Name == "GamePlay")
			{
				SetGameScreenVisibility("Pause", false);
				SetGameScreenVisibility("GameOver", false);
			}
		}

		/// <summary>
		/// Set the visibility of the game screen with the given name
		/// </summary>
		/// <param name="name">The name of the element to show or hide</param>
		/// <param name="isVisible">True to show the element, or false to hide it</param>
		private void SetGameScreenVisibility(string name, bool isVisible)
		{
			// Find the game screen
			FrameworkElement screen = (FrameworkElement)FindName(name);

			// Show or hide
			if (isVisible)
				screen.Visibility = Visibility.Visible;
			else
				screen.Visibility = Visibility.Collapsed;
		}

		#endregion

		#region IDisplayable Members

		/// <summary>
		/// Get the canvas associated with the game area
		/// </summary>
		System.Windows.Controls.Canvas IDisplayable.GameArea { get { return GameArea; } }

		/// <summary>
		/// Display the game over screen
		/// </summary>
		void IDisplayable.GameOver()
		{
			// Serialize the new high score
			if (Controller.Player.Score.IsHighScore)
				Serialize();

			// Show the game over screen
			SetGameScreenVisibility("GameOver", true);
		}

		/// <summary>
		/// Determine whether or not the player should jump again
		/// </summary>
		/// <returns>True to jump again, or false otherwise</returns>
		bool IDisplayable.JumpAgain()
		{
			return Keyboard.IsKeyDown(Key.Space);
		}

		#endregion
	}
}