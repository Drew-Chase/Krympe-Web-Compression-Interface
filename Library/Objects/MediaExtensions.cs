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

        #region Public Methods

        public static MediaExtensions Make(string extList)
        {
            return new(extList.Split(";").ToList());
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
            StringBuilder builder = new();
            foreach (string s in ext)
            {
                builder.Append(s);
            }
            for (int i = 0; i < ext.Count; i++)
            {
                builder.Append(ext[i]);
                if (i < ext.Count - 1)
                {
                    builder.Append(";");
                }
            }
            return builder.ToString().Trim();
        }

        #endregion Public Methods
    }
}