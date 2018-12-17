using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.ComponentModel;
using System.Security;

namespace UsersGitHubRepositories
{
    class RepositoryModel : IDisposable
    {
        #region Fileds

        private static readonly string NEW_DATABASE = "<New>...";
        private static readonly string DEFAULT_DB = "GitHubRepositories";
        private static readonly string FK_REPO_USERS = "FK__MG_Repositories__MG_Users";
        private static readonly string FK_COLUMN_NAME = "CONSTRAINT_NAME";

        public static readonly string REPOSITORIES = "_MG_Repositories";
        public static readonly string USERS = "_MG_Users";

        private static readonly string TABLE_NAME_COLUMN = "TABLE_NAME";
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
        private DataTable mobjDTVRepositories;
        private DataTable mobjDTVUsers;


        #endregion

        /// <summary>
        /// Repository Model for Managing DataAccess and Objects for the GitHub Repository Data/
        /// </summary>
        public RepositoryModel()

        {
            try
            {

                GetListOfSQLServers();

                InitializeDataBaseObjects();

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed To Initialize RepositoryModel.", Ex);
            }

        }

        /// <summary>
        /// Setting The Credentials for Sql Servre Connection.
        /// </summary>
        /// <param name="strUserName">Sql Username</param>
        /// <param name="objPassword">Sql Password</param>
        /// <param name="blnIsWindowsAuth">Windows Authentication Active</param>
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
        /// <summary>
        /// Initialize Sql Connection Objects.
        /// </summary>
        private void InitializeDataBaseObjects()
        {

            try
            {
                this.mobjSqlConnectionStringBuilder = new SqlConnectionStringBuilder();
                this.mobjSqlConnection = new SqlConnection();
                this.GitHubRepositories = new DataSet("GitHubRepositories");
                this.mobjDTRepositories = new DataTable(REPOSITORIES);
                this.mobjDTUsers = new DataTable(USERS);
                this.mobjDTVRepositories = new DataTable(REPOSITORIES);
                this.mobjDTVUsers = new DataTable(USERS);

                GitHubRepositories.Tables.Add(mobjDTVUsers);
                GitHubRepositories.Tables.Add(mobjDTVRepositories);


            }
            catch (Exception Ex)
            {

                throw new Exception("Failed To Initialize Database Objects.", Ex);
            }

        }
        /// <summary>
        /// Retrive the Sql Servers List on the local network.
        /// </summary>
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
        /// <summary>
        /// Read the server from the table of the Servers info.
        /// </summary>
        /// <param name="objServerData">Server info DataRow</param>
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
        /// <summary>
        /// List the Databses of the Current Sql Server.
        /// </summary>
        /// <param name="objDatabases"></param>
        private void LoadDatabasesList(DataTable objDatabases)
        {
            try
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
            catch (Exception Ex)
            {

                throw new Exception("Failed Load Databases List.", Ex);
            }
        }
        /// <summary>
        /// Creates New Database If it's not Exists on the Server.
        /// </summary>
        /// <param name="strDatabaseName">New Database Name</param>
        public void CreateDataBase(string strDatabaseName)
        {
            try
            {
                if (strDatabaseName.Equals(NEW_DATABASE))
                {
                    CreateNewDataBase();
                }
                else
                {
                    mobjSqlConnectionStringBuilder.InitialCatalog = strDatabaseName;

                    mobjSqlConnection.Credential = mobjCredential;
                }

                CreateRepositoryTables();

                IsDatabaseReady = true;
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Create Database.", Ex);
            }

        }
        /// <summary>
        /// Creates Repository tables if its not Exists.
        /// </summary>
        private void CreateRepositoryTables()
        {
            DataTable objTablesList;
            DataTable objForeignKeys;
            Dictionary<string, string> objTablesToCreate;

            try
            {
                objTablesList = GetDBTablesList();

                objForeignKeys = GetFKeysList();

                objTablesToCreate = CheckRepositoryTablesExists(objTablesList, objForeignKeys);

                CreateTables(objTablesToCreate);

                UpdateTablesCommands(objTablesToCreate);

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed to create repositories Tables.", Ex);
            }

        }
        /// <summary>
        /// Create New Database if It Not Exists.
        /// </summary>
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

                throw new Exception("Failed in Create Database", Ex);
            }
        }
        /// <summary>
        /// Inserts GitHub User to the database.
        /// </summary>
        /// <param name="objNewUserData">User Data To insert to the Database</param>
        public void InsertUserToDB(User objNewUserData)
        {

            try
            {
                mobjDataAdapters[USERS].FillSchema(mobjDTUsers, SchemaType.Source);

                InsertNewUser(objNewUserData);

                mobjDataAdapters[USERS].Update(mobjDTUsers);

                mobjDTUsers.Clear();

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Insert User to the database.", Ex);
            }

        }
        /// <summary>
        /// Inserts GitHub Users to the database.
        /// </summary>
        /// <param name="objUsers">Object List of Users</param>
        public void InsertUsersToDB(GitHubUsers objUsers)
        {
            try
            {

                mobjDataAdapters[USERS].FillSchema(mobjDTUsers, SchemaType.Source);

                mobjDataAdapters[USERS].FillLoadOption = LoadOption.OverwriteChanges;

                mobjDataAdapters[USERS].Fill(mobjDTUsers);

                foreach (User objUser in objUsers.Items)
                {
                    InsertNewUser(objUser);
                }

                mobjDataAdapters[USERS].Update(mobjDTUsers);

                mobjDTUsers.Clear();

                mobjDataAdapters[USERS].Fill(mobjDTUsers);

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Insert Users to the database.", Ex);
            }

        }
        /// <summary>
        /// Inserts GitHub User's Repositories to the Database.
        /// </summary>
        /// <param name="objUser">The User that holds the repositories</param>
        /// <param name="objUserRepositories">The User's Repositories</param>
        public void InsertUserRepositories(User objUser, List<Repository> objUserRepositories)
        {
            int intUserID;

            try
            {


                mobjDataAdapters[USERS].
                   SelectCommand.Parameters["@Node_id"].Value = strNode_id;

                mobjDataAdapters[USERS].Fill(mobjDTUsers);

                if (mobjDTUsers.Rows.Count > 0)
                {
                    intUserID = mobjDTUsers.Rows[0].Field<int>("UserID");

                    mobjDataAdapters[REPOSITORIES].
                        SelectCommand.Parameters["@UserID"].Value = intUserID;

                    mobjDataAdapters[REPOSITORIES].Fill(mobjDTRepositories);

                    foreach (Repository objRepository in objUserRepositories)
                    {
                        InsertNewRepository(intUserID, objRepository);
                    }

                    mobjDataAdapters[REPOSITORIES].Update(mobjDTRepositories);

                    mobjDTRepositories.Clear();

                }
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Insert Repositories to the Database.", Ex);
            }

        }
        /// <summary>
        /// Creates New Row in the Repository Database.
        /// </summary>
        /// <param name="intUserID">User Id of the repository - From DB</param>
        /// <param name="objRepository">Repository Data</param>
        private void InsertNewRepository(int intUserID, Repository objRepository)
        {
            DataRow objDR;
            try
            {
                IEnumerable<DataRow> objFoundRepos =
               (from objDataRow in mobjDTRepositories.AsEnumerable()
                where objDataRow.Field<string>("node_id") == objRepository.Node_id
                select objDataRow);

                if (objFoundRepos.Count() > 0)
                {
                    objDR = objFoundRepos.Single<DataRow>();
                }
                else
                {
                    objDR = mobjDTRepositories.NewRow();

                    objDR["Name"] = objRepository.Name;
                    objDR["node_id"] = objRepository.Node_id;
                    objDR["Language"] = objRepository.Language;
                    objDR["UserID"] = intUserID;

                    mobjDTRepositories.Rows.Add(objDR);
                }

            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Add new Row to Repository.", Ex);
            }

        }
        /// <summary>
        /// Insert New User to the Database.
        /// </summary>
        /// <param name="objUser">The New User Data.</param>
        private void InsertNewUser(User objUser)
        {
            DataRow objDR;

            try
            {

                mobjDataAdapters[USERS].SelectCommand.Parameters["@Node_id"].Value = objUser.Node_id;

                mobjDataAdapters[USERS].Fill(mobjDTUsers);

                if (mobjDTUsers.Rows.Count > 0)
                {
                    objDR = mobjDTUsers.Rows[0];

                }
                else
                {
                    objDR = mobjDTUsers.NewRow();

                    objDR["Username"] = objUser.Login;
                    objDR["FullName"] = objUser.Name;
                    objDR["Followers"] = objUser.Followers;
                    objDR["node_id"] = objUser.Node_id;
                    objDR["Size"] = objUser.Size;

                    mobjDTUsers.Rows.Add(objDR);
                }

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Add new User Row to Database", Ex);
            }

        }
        /// <summary>
        /// Creates tables in the databsae if it's Missing.
        /// </summary>
        /// <param name="objTablesToCreate">List of Missing Tables</param>
        private void CreateTables(Dictionary<string, string> objTablesToCreate)
        {
            SqlCommand objSqlCommand;

            try
            {
                if (objTablesToCreate.Count > 0)
                {
                    mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

                    mobjSqlConnection.Credential = mobjCredential;

                    mobjSqlConnection.Open();

                    objSqlCommand = mobjSqlConnection.CreateCommand();

                    foreach (string strCommandName in objTablesToCreate.Keys)
                    {

                        objSqlCommand.CommandText = objTablesToCreate[strCommandName];

                        objSqlCommand.ExecuteNonQuery();

                    }

                    mobjSqlConnection.Close(); 
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed To create table in the database.", Ex);
            }
        }
        /// <summary>
        /// Get a list of tables from the selected database.
        /// </summary>
        /// <returns></returns>
        private DataTable GetDBTablesList()
        {
            DataTable objTablesList;

            try
            {
                mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

                mobjSqlConnection.Credential = mobjCredential;

                mobjSqlConnection.Open();

                objTablesList = mobjSqlConnection.GetSchema("Tables");

                mobjSqlConnection.Close(); 
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed to get Database tables.", Ex);
            }

            return objTablesList;
        }
        /// <summary>
        /// New Tables Script for Database Tables.
        /// </summary>
        /// <param name="objTablesList">List of Tables from the Sql Server.</param>
        /// <param name="objFKeysList">List of Foreign Keys of the tables</param>
        /// <returns></returns>
        private Dictionary<string, string> CheckRepositoryTablesExists(DataTable objTablesList, DataTable objFKeysList)
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
                {FK_REPO_USERS,
                "ALTER TABLE [dbo].[_MG_Repositories]  " +
                "WITH CHECK ADD  CONSTRAINT [FK__MG_Repositories__MG_Users] " +
                "FOREIGN KEY([UserID])REFERENCES [dbo].[_MG_Users] ([UserID])"
                }
            };


            try
            {
                foreach (DataRow objTableInfo in objTablesList.Rows)
                {
                    if (!(objTableInfo[TABLE_NAME_COLUMN] is System.DBNull))
                    {
                        if (objRepositoryTables.Keys.Contains(objTableInfo[TABLE_NAME_COLUMN].ToString()))
                        {
                            objRepositoryTables.Remove(objTableInfo[TABLE_NAME_COLUMN].ToString());
                        }
                    }
                }
                foreach (DataRow objFKeysInfo in objFKeysList.Rows)
                {
                    if (!(objFKeysInfo[FK_COLUMN_NAME] is System.DBNull))
                    {
                        if (objRepositoryTables.Keys.Contains(objFKeysInfo[FK_COLUMN_NAME].ToString()))
                        {
                            objRepositoryTables.Remove(objFKeysInfo[FK_COLUMN_NAME].ToString());
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Creating Tables List.", Ex);
            }

            return objRepositoryTables;
        }
        /// <summary>
        /// Check Foriegn Keys on Existing Tables .
        /// </summary>
        /// <returns></returns>
        private DataTable GetFKeysList()
        {
            DataTable objFKeysList;

            try
            {
                mobjSqlConnection.ConnectionString = mobjSqlConnectionStringBuilder.ConnectionString;

                mobjSqlConnection.Credential = mobjCredential;

                mobjSqlConnection.Open();

                objFKeysList = mobjSqlConnection.GetSchema("ForeignKeys");

                mobjSqlConnection.Close(); 
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed to get Database tables.", Ex);
            }

            return objFKeysList;
        }
        /// <summary>
        /// Updates the SqlCommands of the Repositories tables SELECT,INSERT,UPDATE,DELETE.
        /// </summary>
        /// <param name="objTablesToCreate">List of tables to create</param>
        private void UpdateTablesCommands(Dictionary<string, string> objTablesToCreate)
        {
            try
            {
                mobjDataAdapters = new Dictionary<string, SqlDataAdapter>()
            {
                { REPOSITORIES,new SqlDataAdapter(
                    "SELECT * FROM _MG_Repositories",
                    mobjSqlConnection) },
                { USERS,new SqlDataAdapter(
                    "SELECT * FROM _MG_Users",
                    mobjSqlConnection) }
            };

                UpdateRepositoriesCommand();

                UpdateUsersCommands();
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Update Table Commands.", Ex);
            }

        }
        /// <summary>
        /// Updates Te Users table commands.
        /// </summary>
        private void UpdateUsersCommands()
        {
            try
            {
                mobjDataAdapters[USERS].SelectCommand =
                       new SqlCommand(
                           "SELECT UserID,UserName,FullName,Followers,node_id,Size " +
                           "FROM _MG_Users " +
                           "WHERE node_id=@Node_id",
                       mobjSqlConnection);

                SetUsersSelectParameters();

                mobjDataAdapters[USERS].UpdateCommand =
                    new SqlCommand(
                    "UPDATE _MG_Users " +
                    "SET [Username]=@Username,[FullName]=@FullName,[Followers]=@Followers,[Size]=@Size " +
                    "WHERE [node_id]=@Node_id",
                    mobjSqlConnection);

                SetUsersUpdateParameters();

                mobjDataAdapters[USERS].InsertCommand =
                    new SqlCommand(
                        "INSERT INTO _MG_Users " +
                        "VALUES (@Username,@FullName,@Followers,@Node_id,@Size)",
                        mobjSqlConnection);

                SetUsersInsertParameters();

                mobjDataAdapters[USERS].DeleteCommand =
                    new SqlCommand("DELETE FROM _MG_Users WHERE [UserID] = @ID",
                    mobjSqlConnection);


            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Set Commands.", Ex);
            }
        }

        private void SetUsersUpdateParameters()
        {
            try
            {
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Username", SqlDbType.NVarChar)
                    {
                        SourceColumn = "UserName",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                new SqlParameter("@Fullname", SqlDbType.NVarChar)
                {
                    SourceColumn = "FullName",
                    SourceVersion = DataRowVersion.Original
                });
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Followers", SqlDbType.Int)
                    {
                        SourceColumn = "Followers",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Node_id", SqlDbType.NVarChar)
                    {
                        SourceColumn = "node_id",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Size", SqlDbType.Int)
                    {
                        SourceColumn = "Size",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].UpdateCommand.Parameters.Add(
                    new SqlParameter("@UserID", SqlDbType.Int)
                    {
                        SourceColumn = "UserID",
                        SourceVersion = DataRowVersion.Original
                    });
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Inser new INSERT Parameters", Ex);
            }
        }
        private void SetUsersSelectParameters()
        {
            try
            {

                mobjDataAdapters[USERS].SelectCommand.Parameters.Add(
                 new SqlParameter("@Node_id", SqlDbType.NVarChar)
                 {
                     SourceColumn = "node_id",
                     SourceVersion = DataRowVersion.Original
                 });
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Insert new SELECT Parameters", Ex);
            }
        }
        /// <summary>
        /// Sets INSERT parameters.
        /// </summary>
        private void SetUsersInsertParameters()
        {
            try
            {
                mobjDataAdapters[USERS].InsertCommand.Parameters.Add(
                    new SqlParameter("@Username", SqlDbType.NVarChar)
                    {
                        SourceColumn = "UserName",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].InsertCommand.Parameters.Add(
                new SqlParameter("@Fullname", SqlDbType.NVarChar)
                {
                    SourceColumn = "FullName",
                    SourceVersion = DataRowVersion.Original
                });
                mobjDataAdapters[USERS].InsertCommand.Parameters.Add(
                    new SqlParameter("@Followers", SqlDbType.Int)
                    {
                        SourceColumn = "Followers",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].InsertCommand.Parameters.Add(
                    new SqlParameter("@Node_id", SqlDbType.NVarChar)
                    {
                        SourceColumn = "node_id",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[USERS].InsertCommand.Parameters.Add(
                    new SqlParameter("@Size", SqlDbType.Int)
                    {
                        SourceColumn = "Size",
                        SourceVersion = DataRowVersion.Original
                    });
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Inser new INSERT Parameters", Ex);
            }
        }
        /// <summary>
        /// Set Repositories Commands
        /// </summary>
        private void UpdateRepositoriesCommand()
        {
            try
            {
                mobjDataAdapters[REPOSITORIES].SelectCommand =
                                new SqlCommand(
                                    "SELECT * FROM _MG_Repositories " +
                                    "WHERE UserID=@UserID",
                                mobjSqlConnection);

                SetRepoSelectParameters();

                mobjDataAdapters[REPOSITORIES].UpdateCommand =
                    new SqlCommand(
                    "UPDATE _MG_Repositories " +
                    "SET [Name]=@Name,[Language]=@Language,[UserID]=@UserID " +
                    "WHERE [node_id]=@Node_id",
                    mobjSqlConnection);

                SetRepoUpdateParameters();

                mobjDataAdapters[REPOSITORIES].InsertCommand =
                    new SqlCommand(
                        "INSERT INTO _MG_Repositories " +
                        "VALUES (@Name,@Node_id,@Language,@UserID)",
                        mobjSqlConnection);

                SetRepoInsertParameters();

                mobjDataAdapters[REPOSITORIES].DeleteCommand =
                    new SqlCommand("DELETE FROM _MG_Repositories WHERE [RepositoryID] = @ID",
                    mobjSqlConnection);
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Create Repositories Commands.", Ex);
            }
        }

        private void SetRepoSelectParameters()
        {
            try
            {

                mobjDataAdapters[REPOSITORIES].SelectCommand.Parameters.Add(
                 new SqlParameter("@UserID", SqlDbType.NVarChar)
                 {
                     SourceColumn = "UserID",
                     SourceVersion = DataRowVersion.Original
                 });
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Insert new SELECT Parameters", Ex);
            }
        }

        private void SetRepoUpdateParameters()
        {
            try
            {

                mobjDataAdapters[REPOSITORIES].UpdateCommand.Parameters.Add(
                 new SqlParameter("@Name", SqlDbType.NVarChar)
                 {
                     SourceColumn = "Name",
                     SourceVersion = DataRowVersion.Original
                 });
                mobjDataAdapters[REPOSITORIES].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Language", SqlDbType.NVarChar)
                    {
                        SourceColumn = "Language",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[REPOSITORIES].UpdateCommand.Parameters.Add(
                    new SqlParameter("@Node_id", SqlDbType.NVarChar)
                    {
                        SourceColumn = "node_id",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[REPOSITORIES].UpdateCommand.Parameters.Add(
                    new SqlParameter("@UserID", SqlDbType.Int)
                    {
                        SourceColumn = "UserID",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[REPOSITORIES].UpdateCommand.Parameters.Add(
                     new SqlParameter("@RepositoryID", SqlDbType.Int)
                     {
                         SourceColumn = "RepositoryID",
                         SourceVersion = DataRowVersion.Original
                     });
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Insert new UPDATE Parameters", Ex);
            }
        }
        /// <summary>
        /// Set Repositories INSERT Parameters.
        /// </summary>
        private void SetRepoInsertParameters()
        {
            try
            {
                mobjDataAdapters[REPOSITORIES].InsertCommand.Parameters.Add(
               new SqlParameter("@Name", SqlDbType.NVarChar)
               {
                   SourceColumn = "Name",
                   SourceVersion = DataRowVersion.Original
               });
                mobjDataAdapters[REPOSITORIES].InsertCommand.Parameters.Add(
                    new SqlParameter("@Language", SqlDbType.NVarChar)
                    {
                        SourceColumn = "Language",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[REPOSITORIES].InsertCommand.Parameters.Add(
                    new SqlParameter("@Node_id", SqlDbType.NVarChar)
                    {
                        SourceColumn = "node_id",
                        SourceVersion = DataRowVersion.Original
                    });
                mobjDataAdapters[REPOSITORIES].InsertCommand.Parameters.Add(
                    new SqlParameter("@UserID", SqlDbType.Int)
                    {
                        SourceColumn = "UserID",
                        SourceVersion = DataRowVersion.Original
                    });
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Insert new INSERT Parameters", Ex);
            }
        }
        /// <summary>
        /// Get Spacific User's Repositories from the Database.
        /// </summary>
        /// <param name="intSelectedUserID">The UserID</param>
        public void GetUserRepository(int intSelectedUserID)
        {
            try
            {
                mobjDTVRepositories.Clear();

                using (SqlDataAdapter objDataAdapter = new SqlDataAdapter
                    (string.Format("SELECT * FROM _MG_Repositories " +
                    "WHERE UserID={0}", intSelectedUserID), 
                    mobjSqlConnectionStringBuilder.ConnectionString))
                {                    
                    
                    objDataAdapter.Fill(mobjDTVRepositories);
                }
                    

            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Get User's Repositories from the Database.", Ex);
            }
        }
        /// <summary>
        /// Upload Users Data.
        /// </summary>
        public void GetAllUsers()
        {

            try
            {
                mobjDTVUsers.Clear();

                using (SqlDataAdapter objDataAdapter = new SqlDataAdapter
                    ("SELECT * FROM _MG_Users",
                    mobjSqlConnectionStringBuilder.ConnectionString))
                {
                    objDataAdapter.Fill(mobjDTVUsers);
                }
            }
            catch (Exception Ex)
            {

                throw new Exception("Failed Insert Users to the database.", Ex);
            }

        }

        public void Dispose()
        {
            mobjPassword.Dispose();
            mobjCredential = null;
            mobjSqlConnectionStringBuilder.Clear();
            mobjSqlConnection.Dispose();
            mobjDataAdapters.Clear();
            mobjDTRepositories.Dispose();
            mobjDTUsers.Dispose();
        }

        #region Props

        public BindingList<Server> ServersList { get; set; }

        public BindingList<string> DatabasesList { get; set; }

        public DataSet GitHubRepositories { get; set; }
        public bool IsDatabaseReady { get; private set; }

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
        string strLogin;
        string strNode_id;
        string strName;

        public int UserID { get; set; }
        public string Login
        {
            get
            {
                return strLogin;
            }
            set => strLogin = value ?? @"N/A";
        }
        public string Name
        {
            get
            {
                return strName;
            }
            set
            {
                strName = value ?? @"N/A";
            }
        }
        public int Followers { get; set; }
        public string Node_id
        {
            get
            {
                return strNode_id;
            }
            set
            {
                strNode_id = value ?? @"N/A";
            }
        }
        public float Size { get; set; }

        internal void Update(User objUserData)
        {
            if (objUserData != null)
            {
                if (Node_id.Equals(objUserData.Node_id) &&
                    Login.Equals(objUserData.Login))
                {
                    if (objUserData.Name != null)
                    {
                        Name = objUserData.Name;
                    }
                    if (objUserData.Followers != 0)
                    {
                        Followers = objUserData.Followers;
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
        string strNode_id;
        string strName;
        string strLanguage;

        public int RepositoryID { get; set; }
        public string Name
        {
            get
            {
                return strName;
            }
            set
            {
                strName = value ?? @"N/A";
            }
        }
        public string Node_id
        {
            get
            {
                return strNode_id;
            }
            set
            {
                strNode_id = value ?? @"N/A";
            }
        }
        public string Language
        {
            get
            {
                return strLanguage;
            }
            set
            {
                strLanguage = value ?? @"N/A";
            }
        }
        public int UserID { get; set; }
    }
}