using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace mCubed.CoinCollector.Maps {
	public class MapSet : INotifyPropertyChanged {
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private string _filename;
		private readonly ObservableCollection<BaseMap> _maps;
		private string _name;

		#endregion

		#region Propeties

		/// <summary>
		/// Get/set the filename for this map set
		/// </summary>
		public string Filename {
			get { return _filename; }
			set { this.SetAndNotify(ref _filename, value, "Filename"); }
		}

		/// <summary>
		/// Get the set of maps associated with this map set
		/// </summary>
		public ObservableCollection<BaseMap> Maps { get { return _maps; } }

		/// <summary>
		/// Get/set the name for this map set
		/// </summary>
		public string Name {
			get { return _name; }
			set { this.SetAndNotify(ref _name, value, "Name"); }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new set of maps
		/// </summary>
		/// <param name="filename">The filename associated with this map set</param>
		/// <param name="isResource">True if the file is a resource, or false otherwise</param>
		public MapSet(string filename, bool isResource) {
			// Initialize
			_maps = new ObservableCollection<BaseMap>();
			Filename = isResource ? "/" + filename : filename;

			// Read the file
			ParseFile();
		}

		#endregion

		#region I/O Members

		/// <summary>
		/// Read the file associated with this map set
		/// </summary>
		/// <returns>A stream reader to read the map set file</returns>
		private StreamReader ReadFile() {
			StreamReader reader = null;
			if (Filename.StartsWith("/")) {
				Uri uri = new Uri("/Mapsets" + Filename, UriKind.Relative);
				var stream = Application.GetResourceStream(uri).Stream;
				reader = new StreamReader(stream);
			} else if (File.Exists("Mapsets/" + Filename)) {
				reader = new StreamReader("Mapsets/" + Filename);
			}
			return reader;
		}

		/// <summary>
		/// Parse the file associated with this map set
		/// </summary>
		private void ParseFile() {
			// Read the file
			StreamReader reader = ReadFile();
			if (reader == null)
				return;

			// Read the map set name
			Name = reader.ReadLine();
			
			// Read each line one at a time
			string line = null;
			List<string> lines = new List<string>();
			while ((line = reader.ReadLine()) != null || lines.Count > 0) {
				// Build the map
				if (line == null || line == "-----") {
					if (lines.Count > 0)
						Maps.Add(new StreamMap(lines.ToArray()));
					lines.Clear();
				}

				// Read the map
				else {
					lines.Add(line);
				}
			}

			// Close the reader
			reader.Close();
		}

		#endregion
	}
}