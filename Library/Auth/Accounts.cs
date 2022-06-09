using Krympe.Library.Data;
using static Krympe.Library.Data.Global;

namespace Krympe.Library.Auth
{
    public class Accounts
    {
        #region Fields

        public static Accounts Instance = Instance ??= new Accounts();
        private List<User> users;

        #endregion Fields

        #region Protected Constructors

        protected Accounts()
        {
            users = new List<User>();
            Populate();
            if (!users.Any())
            {
                users.Add(new("admin", "krympe", true));
            }
        }

        #endregion Protected Constructors

        #region Public Methods

        public bool AttemptLogin(string username, string password, out User? user)
        {
            user = null;
            foreach (User u in users)
            {
                if (u.Username.Equals(username) && u.Password.Equals(password))
                {
                    user = u;
                    log.Debug($"Successfully Logged in as \"{username}\"");
                    return true;
                }
            }
            log.Warn($"Failed Logged attempt as \"{username}\"");
            return false;
        }

        public void CreateUser(string username, string password)
        {
            log.Info($"Creating User: \"{username}\"");
            users.Add(new(username, password));
        }

        public User? Get(Guid Token) => users.FirstOrDefault(u => u?.ID == Token, null);

        public Guid RegenerateAdminToken()
        {
            Configuration.Instance.Token = Guid.NewGuid();
            return Configuration.Instance.Token;
        }

        #endregion Public Methods

        #region Private Methods

        private void Populate()
        {
            users = new List<User>();

            foreach (string file in Directory.GetFiles(UsersDirectory))
            {
                users.Add(new(file));
            }
        }

        #endregion Private Methods
    }
}