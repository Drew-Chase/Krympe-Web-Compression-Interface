using Newtonsoft.Json.Linq;
using System.Text;

namespace Krympe.Library.Objects
{
    public class MediaExtensions
    {
        #region Fields

        private List<string> ext;

        #endregion Fields

        #region Protected Constructors

        protected MediaExtensions(List<string> ext)
        {
            this.ext = ext;
        }

        #endregion Protected Constructors

        #region Properties

        public JArray AsJson
        {
            get
            {
                return new JArray(ext);
            }
        }

        #endregion Properties

        #region Public Methods

        public static MediaExtensions Make(JArray extList)
        {
            List<string> ext = new();
            foreach (var item in extList)
            {
                ext.Add(item.ToString());
            }
            return new(ext);
        }

        public static MediaExtensions Make(string[] extList)
        {
            return new(extList.ToList());
        }

        public void Add(string extension)
        {
            ext.Add(extension);
        }

        public string[] Get()
        {
            return ext.ToArray();
        }

        public override string ToString()
        {
            return AsJson.ToString();
        }

        #endregion Public Methods
    }
}