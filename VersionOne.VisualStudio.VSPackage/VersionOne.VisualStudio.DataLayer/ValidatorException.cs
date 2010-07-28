using System;

namespace VersionOne.VisualStudio.DataLayer {
	public class ValidatorException : DataLayerException {
        public ValidatorException(string message) : base(message) { }
		public ValidatorException(string message, Exception exception) : base(message, exception) { }
	}
}