using System;
using System.Linq;
using System.Windows.Data;

namespace mCubed.CoinCollector {
	public class PlayerConverter : IMultiValueConverter {
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return values.Length == 2 && values.All(v => v!= null) && values[0].ToString() == values[1].ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}