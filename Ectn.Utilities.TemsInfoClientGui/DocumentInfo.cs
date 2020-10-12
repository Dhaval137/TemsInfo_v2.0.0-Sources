using System;
using System.IO;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfoClientGui {

    public class DocumentInfo : PropertyChangedBase {

        #region Props

        public string Title {
            get;
            private set;
        }

        public XDocument Document {
            get;
            private set;
        }

        #endregion Props

        #region Fields

        private string temporaryFile;

        #endregion Fields

        public DocumentInfo(string title, XDocument document) {
            this.Title = title;
            this.Document = document;
            temporaryFile = string.Empty;
        }

        #region Public Access

        public string GetTemporaryFile() {
            if (string.IsNullOrWhiteSpace(temporaryFile)) {
                temporaryFile = GetTempFile(string.Format("{0}_", Title.Replace(" ", ".")), ".xml");
                Document.Save(temporaryFile);
            }
            return temporaryFile;
        }

        public void DeleteTemporaryFile() {
            if (!string.IsNullOrWhiteSpace(temporaryFile)) {
                File.Delete(temporaryFile);
                temporaryFile = string.Empty;
            }
        }

        #endregion Public Access

        #region Utils

        private string GetTempFile(string filePrefix, string extension) {
            return System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format("{0}{1}{2}",
                filePrefix,
                Guid.NewGuid().ToString(),
                extension));
        }

        #endregion Utils
    }
}