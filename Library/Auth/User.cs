using ChaseLabs.CLConfiguration.List;
using Krympe.Library.Data;
using static Krympe.Library.Data.Global;

namespace Krympe.Library.Auth
{
    public class User
    {
        #region Fields

        private ConfigManager manager;

        #endregion Fields

        #region Public Constructors

        public User(string username, string password, bool isAdmin = false)
        {
            ID = Guid.NewGuid();
            manager = Configuration.Make(Path.Combine(UsersDirectory, ID.ToString()));
            IsAdmin = isAdmin;
            Username = username;
            Password = password;
            manager.Add("username", Username);
            manager.Add("password", Password);
            manager.Add("admin", isAdmin);

            if (isAdmin)
            {
                Configuration.Instance.Token = ID;
            }
        }

        public User(string file)
        {
            manager = Configuration.Make(file);
            if (manager.GetConfigByKey("username") == null || manager.GetConfigByKey("password") == null)
            {
                log.Error($"User doesn't exist");
                return;
            }
            ID = Guid.Parse(new FileInfo(file).Name);
            Username = manager.GetConfigByKey("username").Value;
            Password = manager.GetConfigByKey("password").Value;
            IsAdmin = manager.GetConfigByKey("admin").Value;
        }

        #endregion Public Constructors

        #region Properties

        public Guid ID { get; init; }

        public bool IsAdmin { get; private set; }
        public string Password { get; private set; }

        public string Username { get; private set; }

        #endregion Properties

        #region Public Methods

        public void ChangePassword(string password)
        {
            Password = password;
            manager.GetConfigByKey("password").Value = password;
        }

        public void ChangeUsername(string username)
        {
            Username = username;
            manager.GetConfigByKey("username").Value = username;
        }

        #endregion Public Methods
    }
}