using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredEngine.Diagnostics.Log {
	public class LoggerInstance {
		public String ID { get; private set; }

		public String ComponentID { get; protected set; }
		public String Category { get; protected set; }


		public LoggerInstance(Type type, String id = null)
			: this(type.ToString(), id) { }
		public LoggerInstance(String category, String id = null) {
			Category = category;
			ComponentID = id ?? "0";

			ID = Guid.NewGuid().ToString();

			WriteDebugInformation("New LoggerInstance created...\n{0}", this.ToString());
		}


		public void WriteInformation(String text) {
			WriteInformation(text, new object[0]);
		}
		public void WriteInformation(String textWithFormat, params object[] objs) {
			LoggerService.WriteInformation(Category, ComponentID, textWithFormat, objs);
		}

		public void WriteDebugInformation(String text) {
			WriteDebugInformation(text, new object[0]);
		}
		public void WriteDebugInformation(String textWithFormat, params object[] objs) {
			LoggerService.WriteDebugInformation(Category, ComponentID, textWithFormat, objs);
		}

		public void WriteWarning(String text) {
			WriteWarning(text, new object[0]);
		}
		public void WriteWarning(String textWithFormat, params object[] objs) {
			LoggerService.WriteWarning(Category, ComponentID, textWithFormat, objs);
		}

		public void WriteError(String text) {
			WriteError(text, new object[0]);
		}
		public void WriteError(String textWithFormat, params object[] objs) {
			LoggerService.WriteError(Category, ComponentID, textWithFormat, objs);
		}

		public void WriteFatalError(String text) {
			WriteFatalError(text, new object[0]);
		}
		public void WriteFatalError(String textWithFormat, params object[] objs) {
			LoggerService.WriteFatalError(Category, ComponentID, textWithFormat, objs);
		}

		public void Write(LoggerService.MessageType type, String text) {
			Write(type, text, new object[0]);
		}
		public void Write(LoggerService.MessageType type, String textWithFormat, params object[] objs) {
			LoggerService.Write(type, Category, ComponentID, textWithFormat, objs);
		}


		public override string ToString() {
			return String.Format("{0} ({1}) - {2} ({3})", base.ToString(), ID, Category, ComponentID);
		}
	}
}
