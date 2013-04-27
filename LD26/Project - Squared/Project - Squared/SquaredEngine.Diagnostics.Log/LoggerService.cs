using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredEngine.Diagnostics.Log {
	public static class LoggerService {
		public enum MessageType {
			DebugInformation,
			Information,
			Warning,
			Error,
			FatalError
		}


		#region Write methods

		#region Other messages types

		#region Information messages

		public static void WriteInformation(Type type, String text, String id = null) {
			LoggerService.WriteInformation(type.ToString(), text, id);
		}
		public static void WriteInformation(String category, String text, String id = null) {
			LoggerService.WriteInformation(category, id, text, new object[0]);
		}
		public static void WriteInformation(Type type, String id, String textWithFormat, params object[] objs) {
			LoggerService.WriteInformation(type.ToString(), id, textWithFormat, objs);
		}
		public static void WriteInformation(String category, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(MessageType.Information, category, id, textWithFormat, objs);
		}

		#endregion

		#region DebugInformation messages

		public static void WriteDebugInformation(Type type, String text, String id = null) {
			LoggerService.WriteDebugInformation(type.ToString(), text, id);
		}
		public static void WriteDebugInformation(String category, String text, String id = null) {
			LoggerService.WriteDebugInformation(category, id, text, new object[0]);
		}
		public static void WriteDebugInformation(Type type, String id, String textWithFormat, params object[] objs) {
			LoggerService.WriteDebugInformation(type.ToString(), id, textWithFormat, objs);
		}
		public static void WriteDebugInformation(String category, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(MessageType.DebugInformation, category, id, textWithFormat, objs);
		}

		#endregion

		#region Warning messages

		public static void WriteWarning(Type type, String text, String id = null) {
			LoggerService.WriteWarning(type.ToString(), text, id);
		}
		public static void WriteWarning(String category, String text, String id = null) {
			LoggerService.WriteWarning(category, id, text, new object[0]);
		}
		public static void WriteWarning(Type type, String id, String textWithFormat, params object[] objs) {
			LoggerService.WriteWarning(type.ToString(), id, textWithFormat, objs);
		}
		public static void WriteWarning(String category, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(MessageType.Warning, category, id, textWithFormat, objs);
		}

		#endregion

		#region Error messages

		public static void WriteError(Type type, String text, String id = null) {
			LoggerService.WriteError(type.ToString(), text, id);
		}
		public static void WriteError(String category, String text, String id = null) {
			LoggerService.WriteError(category, id, text, new object[0]);
		}
		public static void WriteError(Type type, String id, String textWithFormat, params object[] objs) {
			LoggerService.WriteError(type.ToString(), id, textWithFormat, objs);
		}
		public static void WriteError(String category, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(MessageType.Error, category, id, textWithFormat, objs);
		}

		#endregion

		#region FatalError messages

		public static void WriteFatalError(Type type, String text, String id = null) {
			LoggerService.WriteFatalError(type.ToString(), text, id);
		}
		public static void WriteFatalError(String category, String text, String id = null) {
			LoggerService.WriteFatalError(category, id, text, new object[0]);
		}
		public static void WriteFatalError(Type type, String id, String textWithFormat, params object[] objs) {
			LoggerService.WriteFatalError(type.ToString(), id, textWithFormat, objs);
		}
		public static void WriteFatalError(String category, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(MessageType.FatalError, category, id, textWithFormat, objs);
		}

		#endregion

		#endregion

		#region Generic messages

		public static void Write(MessageType messageType, Type type, String text, String id = null) {
			LoggerService.Write(messageType, type.ToString(), id, text, new object[0]);
		}
		public static void Write(MessageType type, String category, String text, String id = null) {
			LoggerService.Write(type, category, id, text, new object[0]);
		}
		public static void Write(MessageType messageType, Type objectType, String id, String textWithFormat, params object[] objs) {
			LoggerService.Write(messageType, objectType.ToString(), id, textWithFormat, objs);
		}
		public static void Write(MessageType type, String category, String id, String textWithFormat, params object[] objs) {
			Console.WriteLine("[{0}] {1} - {2} ({3}): {4}",
				type.ToString(),
				DateTime.Now.ToLocalTime(),
				category,
				id ?? "0",
				String.Format(textWithFormat, objs));
		}

		#endregion

		#endregion
	}
}
