using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.ComponentModel;
using System.Security;

namespace UsersGitHubRepositories
{
    class RepositoryModel
    {
        #region Fileds

        private static readonly string NEW_DATABASE = "<New>...";
        private static readonly string DEFAULT_DB = "GitHubRepositories";
        private static readonly string REPOSITORIES = "_MG_Repositories";
        private static readonly string USERS = "_MG_Users";
        private string mstrUserName;
        private SecureString mobjPassword;
        private bool mblnIsWindowsAuth;
        private SqlCredential mobjCredential;
        private string mobjSelectedDBName;
        private SqlConnectionStringBuilder mobjSqlConnectionStringBuilder;
        private SqlConnection mobjSqlConnection;
        private Dictionary<string, SqlDataAdapter> mobjDataAdapters;
        private DataTable mobjDTRepositories;
        private DataTable mobjDTUsers;
        

        #endregion

        /// <summary>
        /// Repository Model for Managing DataAccess and Objects for the GitHub Repository Data/
        /// </summary>
        public RepositoryModel()
 
        {
          
            GetListOfSQLServers();

            InitializeDataBaseObjects();


        }


        public void SetCridential(
            string strUserName,
            System.Security.SecureString objPassword,
            bool blnIsWindowsAuth)
        {
            this.mstrUserName = strUserName;
            this.mobjPassword = objPassword;
            this.mblnIsWindowsAuth = blnIsWindowsAuth;

            if (!mblnIsWindowsAuth)
            {
                this.mobjCredential = new SqlCredential(mstrUserName, mobjPassword);
            }

            this.mobjSqlConnectionStringBuilder.IntegratedSecurity = mblnIsWindowsAuth;
            
        }

        private void InitializeDataBaseObjects()
        {

            this.mobjSqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            this.mobjSqlConnection = new SqlConnection();
            this.mobjDTRepositories = new DataTable(REPOSITORIES);
            this.mobjDTUsers = new DataTable(USERS);
        }

        private void GetListOfSQLServers()
        {
            DataTable objServerList;

            try
            {
                ServersList = new BindingList<Server>();

                SqlDataSourceEnumerator objDataSourceEnumInstance = SqlDataSourceEnumerator.Instance;
                
                objServerList = objDataSourceEnumInstance.GetDataSources();

                foreach (DataRow objServerData in objServerList.Rows)
                {
                    LoadServerToList(objServerData);
                }


            }
            catch (Exception Ex)
            {
                throw new Exception("Failed In Get the list of Data Source Servers", Ex);
            }
        }

        private void LoadServerToList(DataRow objServerData)
        {
            Server objNewServer = new Server();

            if (!(objServerData["ServerName"] is System.DBNull))
            {
                objNewServer.ServerName = objServerData["ServerName"].ToString();
            }
            if (!(objServerData["InstanceName"] is System.DBNull))
            {
                objNewServer.InstanceName = objServerData["InstanceName"].ToString();
            }
            if (!(objServerData["IsClustered"] is System.DBNull))
            {
                objNewServer.IsClustered = objServerData["IsClustered"].ToString();
            }
            if (!(objServerData["Version"] is System.DBNull))
            {
                objNewServer.Version = objServerData["Version"].ToString();
            }

            ServersList.Add(objNewServer);
        }

        /// <summary>
        /// Query Sql Server for Databases List .
        /// </summary>
        /// <param name="strServer">Sql Server Name</param>
        public void LoadListOfDatabases(string strServer)
        {

            try
            {
                DataTable objDatabases;

                this.mobjSqlConnectionStringBuilder.DataSource = strServer;

                mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;


                mobjSqlConnection.Credential = mobjCredential;

                mobjSqlConnection.Open();
                objDatabases = mobjSqlConnection.GetSchema("databases");

                mobjSqlConnection.Close();

                LoadDatabasesList(objDatabases);

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed In Get the list of DataBases From Server", Ex);
            }
        }

        public void InsertUserRepositories(User objUser, List<Repository> objUserRepositories)
        {
            mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

            mobjSqlConnection.Credential = mobjCredential;

            mobjSqlConnection.Open();



            mobjDataAdapters[REPOSITORIES].FillSchema(mobjDTRepositories, SchemaType.Source);

            mobjDataAdapters[REPOSITORIES].FillSchema(mobjDTUsers, SchemaType.Source);

            InsertNewUser(objUser);

            foreach (Repository objRepository in objUserRepositories)
            {
                InsertNewRepository(objUser, objRepository);
            }

            mobjSqlConnection.Close();

        }

        private void InsertNewRepository(User objUser, Repository objRepository)
        {
            DataRow objNewRepository = mobjDTRepositories.NewRow();

            mobjDTRepositories.Rows.Add(new object[]
                            {
                objRepository.ID,
                    objRepository.Name,
                    objRepository.Node_id,
                    objRepository.Language,
                    objUser.Id
                });

            mobjDTRepositories.AcceptChanges();
        }

        private void InsertNewUser(User objUser)
        {
            DataRow objNewUser = mobjDTUsers.NewRow();

            mobjDTUsers.Rows.Add(new object[]
            {
                objUser.Id,
                objUser.Login,
                objUser.Name,
                objUser.Followers,
                objUser.Node_id,
                objUser.Size
            });

            mobjDTUsers.AcceptChanges();
        }

        private void LoadDatabasesList(DataTable objDatabases)
        {
            DatabasesList = new BindingList<string>
            {
                NEW_DATABASE
            };

            foreach (DataRow objDatabase in objDatabases.Rows)
            {
                if (!(objDatabase["Database_Name"] is System.DBNull))
                {
                    DatabasesList.Add(objDatabase["Database_Name"].ToString());
                }
            }
        }

        public void CreateDataBase(string strDatabaseName)
        {
            if (strDatabaseName.Equals(NEW_DATABASE))
            {
                CreateNewDataBase();
            }
            else
            {
                mobjSqlConnectionStringBuilder.InitialCatalog = strDatabaseName;
            }

            CreateRepositoryTables();

        }

        private void CreateRepositoryTables()
        {
            DataTable objTablesList;
            Dictionary<string,string> objTablesToCreate;

            objTablesList = GetDBTablesList();

            objTablesToCreate = CheckRepositoryTablesExists(objTablesList);

            CreateTables(objTablesToCreate);

            UpdateTablesCommands(objTablesToCreate);
        }

        private void UpdateTablesCommands(Dictionary<string, string> objTablesToCreate)
        {
            mobjDataAdapters = new Dictionary<string, SqlDataAdapter>()
            {
                { REPOSITORIES,new SqlDataAdapter("",mobjSqlConnection) },
                { USERS,new SqlDataAdapter("",mobjSqlConnection) }
            };
            UpdateRepositoriesCommand();

            UpdateUsersCommands();

        }

        private void UpdateUsersCommands()
        {
            mobjDataAdapters[USERS].SelectCommand =
                new SqlCommand("SELECT * FROM _MG_Users");

            mobjDataAdapters[USERS].UpdateCommand =
                new SqlCommand(
                "UPDATE _MG_Users " +
                "SET [Username]=@Username,[FullName]=@FullName,[Followers]=@Followers,[node_id]=@Node_id,[Size]=@Size" +
                "WHERE [UserID] = @ID");

            mobjDataAdapters[USERS].InsertCommand =
                new SqlCommand(
                    "INSERT INTO _MG_Users " +
                    "VALUES (@ID,@Username,@FullName,@Followers,@Node_id,@Size)");

            mobjDataAdapters[USERS].DeleteCommand =
                new SqlCommand("DELETE FROM _MG_Users WHERE [UserID] = @ID");
        }

        private void UpdateRepositoriesCommand()
        {
            mobjDataAdapters[REPOSITORIES].SelectCommand =
                            new SqlCommand("SELECT * FROM _MG_Repositories");

            mobjDataAdapters[REPOSITORIES].UpdateCommand =
                new SqlCommand(
                "UPDATE _MG_Repositories " +
                "SET [Name]=@Name,[node_id]=@Node_id,[Language]=@Language,[UserID]=@UserID" +
                "WHERE [RepositoryID] = @ID");

            mobjDataAdapters[REPOSITORIES].InsertCommand =
                new SqlCommand(
                    "INSERT INTO _MG_Repositories " +
                    "VALUES (@ID,@Name,@Node_id,@Language,@UserID)");

            mobjDataAdapters[REPOSITORIES].DeleteCommand =
                new SqlCommand("DELETE FROM _MG_Repositories WHERE [RepositoryID] = @ID");
        }

        private void CreateTables(Dictionary<string,string> objTablesToCreate)
        {
            SqlCommand objSqlCommand;

            if (objTablesToCreate.Count > 0)
            {
                mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

                mobjSqlConnection.Credential = mobjCredential;

                mobjSqlConnection.Open();

                objSqlCommand = mobjSqlConnection.CreateCommand();

                foreach (string strSqlCommand in objTablesToCreate.Values)
                {
                    objSqlCommand.CommandText = strSqlCommand;

                    objSqlCommand.ExecuteNonQuery();
                }

                mobjSqlConnection.Close();
            }
        }

        private DataTable GetDBTablesList()
        {
            DataTable objTablesList;

            mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

            mobjSqlConnection.Credential = mobjCredential;

            mobjSqlConnection.Open();

            objTablesList = mobjSqlConnection.GetSchema("Tables");

            mobjSqlConnection.Close();

            return objTablesList;
        }

        private Dictionary<string,string> CheckRepositoryTablesExists(DataTable objTablesList)
        {
            Dictionary<string, string> objRepositoryTables = new Dictionary<string, string>
            {
                { REPOSITORIES ,
                    "CREATE TABLE [dbo].[_MG_Repositories](	" +
                    "[RepositoryID] [int] IDENTITY(1,1) NOT NULL,	" +
                    "[Name] [nvarchar](254) NOT NULL,	" +
                    "[node_id] [nvarchar](254) NOT NULL,	" +
                    "[Language] [nvarchar](254) NOT NULL,	" +
                    "[UserID] [int] NOT NULL, " +
                    "CONSTRAINT [PK__MG_Repositories] PRIMARY KEY CLUSTERED (" +
                    "[RepositoryID] ASC)" +
                    "WITH (" +
                    "PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, " +
                    "ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) " +
                    "ON [PRIMARY], " +
                    "CONSTRAINT [IX__MG_Repositories] UNIQUE NONCLUSTERED (	" +
                    "[UserID] ASC,	" +
                    "[Name] ASC)" +
                    "WITH (" +
                    "PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, " +
                    "ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) " +
                    "ON [PRIMARY]) " +
                    "ON [PRIMARY]"

                },
                { USERS,
                    "CREATE TABLE [dbo].[_MG_Users]( " +
                    "[UserID] [int] IDENTITY(1,1) NOT NULL,	" +
                    "[Username] [nvarchar](254) NOT NULL,	" +
                    "[FullName] [nvarchar](254) NOT NULL,	" +
                    "[Followers] [int] NOT NULL,	" +
                    "[node_id] [nvarchar](254) NOT NULL,	" +
                    "[Size] [float] NOT NULL, " +
                    "CONSTRAINT [PK__MG_Users] PRIMARY KEY CLUSTERED ([UserID] ASC)" +
                    "WITH (" +
                    "PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, " +
                    "ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) " +
                    "ON [PRIMARY], " +
                    "CONSTRAINT [IX__MG_Users] UNIQUE NONCLUSTERED (" +
                    "[UserID] ASC)" +
                    "WITH (" +
                    "PAD_INDEX = OFF, " +
                    "STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, " +
                    "ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON) " +
                    "ON [PRIMARY]) " +
                    "ON [PRIMARY]"
                },
                {"Relation1",
                "ALTER TABLE [dbo].[_MG_Repositories]  " +
                "WITH CHECK ADD  CONSTRAINT [FK__MG_Repositories__MG_Users] " +
                "FOREIGN KEY([UserID])REFERENCES [dbo].[_MG_Users] ([UserID])"
                },
                {"Relation2",
                "ALTER TABLE [dbo].[_MG_Repositories] " +
                "CHECK CONSTRAINT [FK__MG_Repositories__MG_Users]"
                }
            };
            

            foreach (DataRow objTableInfo in objTablesList.Rows)
            {
                if (!(objTableInfo[0] is System.DBNull))
                {
                    if (objRepositoryTables.Keys.Contains(objTableInfo[0].ToString()))
                    {
                        objRepositoryTables.Remove(objTableInfo[0].ToString());
                    }
                }
            }

            return objRepositoryTables;
        }

        private void CreateNewDataBase()
        {
            SqlCommand objSqlCreate;

            mobjSelectedDBName = DEFAULT_DB;

            try
            {
                if (!DatabasesList.Contains(DEFAULT_DB))
                {
                    mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;


                    mobjSqlConnection.Credential = mobjCredential;

                    mobjSqlConnection.Open();

                    objSqlCreate = mobjSqlConnection.CreateCommand();

                    objSqlCreate.CommandText = string.Format("CREATE DATABASE {0}", DEFAULT_DB);

                    objSqlCreate.ExecuteNonQuery();

                    mobjSqlConnection.ChangeDatabase(DEFAULT_DB);

                    mobjSqlConnectionStringBuilder.InitialCatalog = DEFAULT_DB;

                    mobjSqlConnection.Close();

                }
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed in Create Database",Ex);
            }
        }
        #region Props

        public BindingList<Server> ServersList { get; set; }

        public BindingList<string> DatabasesList { get; set; }


        #endregion

    }
    /// <summary>
    /// Represents an Entity of A Server in the local network .     
    /// </summary>
    public class Server
    {
        public string ServerName { get; set; }
        public string InstanceName { get; set; }
        public string IsClustered { get; set; }
        public string Version { get; set; }
    }

    public class GitHubUsers
    {
        public int Total_Count { get; set; }
        public bool Incomplete_results { get; set; }
        public List<User> Items { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public int Followers { get; set; }
        public string Node_id { get; set; }
        public float Size { get; set; }

        internal void Update(User objUserData)
        {
            if (objUserData != null)
            {
                if (Id.Equals(objUserData.Id) && 
                    Login.Equals(objUserData.Login))
                {
                    if (objUserData.Name != null)
                    {
                        Name = objUserData.Name;
                    }
                    if(objUserData.Followers != 0)
                    {
                        Followers = objUserData.Followers;
                    }
                    if (objUserData.Node_id != null)
                    {
                        Node_id = objUserData.Node_id;
                    }
                    if (objUserData.Size != 0)
                    {
                        Size = objUserData.Size;
                    }
                }
            }
        }
    }

    public class Repository
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Node_id { get; set; }
        public string Language { get; set; }
        public User Owner { get; set; }
    }
}