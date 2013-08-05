using System;

namespace VersionOne.VisualStudio.DataLayer {
    public class DataLayerException : ApplicationException {
        public DataLayerException(string message) : base(message) { }

        public DataLayerException(string message, Exception exception) : base(message, exception) { }
    }
}