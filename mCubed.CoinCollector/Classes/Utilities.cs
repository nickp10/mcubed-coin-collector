using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace mCubed.CoinCollector {
	public static class Utilities {
		#region Extension Methods: INotifyPropertyChanged

		/// <summary>
		/// Sets a field value only if it has changed, and the result is returned
		/// </summary>
		/// <typeparam name="T">The type the field and the value are</typeparam>
		/// <param name="sender">The object that the field is being set on</param>
		/// <param name="field">The field that will be changing</param>
		/// <param name="value">The new value for the field</param>
		/// <param name="before">The action that should be performed before the value is set</param>
		/// <param name="after">The action that should be performed after the value is set</param>
		/// <returns>True if the field's value changed, or false otherwise</returns>
		public static bool Set<T>(this object sender, ref T field, T value, Action before, Action after) {
			// Check if the value is changing
			if (Object.Equals(field, value))
				return false;

			// Invoke the before action
			if (before != null)
				before();

			// Set the value
			field = value;

			// Invoke the after action
			if (after != null)
				after();
			return true;
		}

		/// <summary>
		/// Set a property value on an object and notify that it changed only if it changed
		/// </summary>
		/// <typeparam name="T">The type the field and the value are</typeparam>
		/// <param name="sender">The object that the field is being set on</param>
		/// <param name="field">The field that will be changing</param>
		/// <param name="value">The new value for the field</param>
		/// <param name="properties">The property names of the properties that have changed</param>
		public static void SetAndNotify<T>(this INotifyPropertyChanged sender, ref T field, T value, params string[] properties) {
			SetAndNotify(sender, ref field, value, null, null, properties);
		}

		/// <summary>
		/// Set a property value on an object and notify that it changed only if it changed
		/// </summary>
		/// <typeparam name="T">The type the field and the value are</typeparam>
		/// <param name="sender">The object that the field is being set on</param>
		/// <param name="field">The field that will be changing</param>
		/// <param name="value">The new value for the field</param>
		/// <param name="before">The action that should be performed before the value is set</param>
		/// <param name="after">The action that should be performed after the value is set</param>
		/// <param name="properties">The property names of the properties that have changed</param>
		public static void SetAndNotify<T>(this INotifyPropertyChanged sender, ref T field, T value, Action before, Action after, params string[] properties) {
			if (sender != null && sender.Set(ref field, value, before, after))
				sender.OnPropertyChanged(properties);
		}

		/// <summary>
		/// Notify other objects that a property or properties have changed on the sender
		/// </summary>
		/// <param name="sender">The object that the properties have changed on</param>
		/// <param name="properties">The property names of the properties that have changed</param>
		public static void OnPropertyChanged(this INotifyPropertyChanged sender, params string[] properties) {
			// Check the sender and get the event information
			if (sender == null)
				return;
			Type eventType = sender.GetType();
			FieldInfo eventField = null;
			while (eventType != null && (eventField = eventType.GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)) == null)
				eventType = eventType.BaseType;

			// Check the event and get the invocation information
			if (eventType == null || eventField == null)
				return;
			var eventDelegate = eventField.GetValue(sender) as MulticastDelegate;
			var invocationList = (eventDelegate == null) ? null : eventDelegate.GetInvocationList();

			// Invoke each property changed on each event listener
			if (invocationList != null && invocationList.Length > 0) {
				foreach (var property in properties.Select(p => new System.ComponentModel.PropertyChangedEventArgs(p)))
					foreach (var handler in invocationList)
						handler.Method.Invoke(handler.Target, new object[] { sender, property });
			}
		}

		#endregion

		#region Extension Methods: Parsing

		/// <summary>
		/// Try to parse the given XML element attribute value into the specified type
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="element">The element that contains the attribute</param>
		/// <param name="attributeName">The name of the attribute witin the element</param>
		/// <returns>The value within the attribute of the element, or the default for the given type</returns>
		public static T Parse<T>(this XElement element, string attributeName) {
			return element.Parse(attributeName, default(T));
		}

		/// <summary>
		/// Try to parse the given XML element attribute value into the specified type
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="element">The element that contains the attribute</param>
		/// <param name="attributeName">The name of the attribute witin the element</param>
		/// <param name="defaultValue">The value to default to when the attribute does not exist</param>
		/// <returns>The value within the attribute of the element, or the default for the given type</returns>
		public static T Parse<T>(this XElement element, string attributeName, T defaultValue) {
			T retValue = defaultValue;
			if (element != null && element.Attribute(attributeName) != null) {
				retValue = element.Attribute(attributeName).Value.Parse<T>();
			}
			return retValue;
		}

		/// <summary>
		/// Parse a given string value into the specified type
		/// </summary>
		/// <typeparam name="T">The type to convert the string into</typeparam>
		/// <param name="str">The string value to convert</param>
		/// <returns>The string converted into the type, or the default value for the type</returns>
		public static T Parse<T>(this string str) {
			return str.Parse(default(T));
		}

		/// <summary>
		/// Parse a given string value into the specified type
		/// </summary>
		/// <typeparam name="T">The type to convert the string into</typeparam>
		/// <param name="str">The string value to convert</param>
		/// <param name="defaultValue">The default or fallback value if the conversion failed</param>
		/// <returns>The string converted into the type, or the given default value</returns>
		public static T Parse<T>(this string str, T defaultValue) {
			// Check the input
			if (str == null)
				return defaultValue;

			// Attempt to convert
			try {
				// Attempt to keep it the same
				if (str is T)
					return (T)(object)str;

				// Attempt to get the parse method
				var parse = typeof(T).GetMethod("Parse", new[] { typeof(string) });
				if (parse != null && parse.IsStatic)
					return (T)(object)parse.Invoke(null, new[] { str });

				// Attempt to parse it into an enum
				if (typeof(T).IsEnum)
					return (T)(object)Enum.Parse(typeof(T), str);
			} catch { }
			return defaultValue;
		}

		#endregion
	}
}