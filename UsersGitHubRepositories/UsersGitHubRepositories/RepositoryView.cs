using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Windows.Forms;

namespace UsersGitHubRepositories
{
    public partial class RepositoryView : Form
    {
        private RepositoryModel mobjRepositoryModel;
        private SecureString mobjSP;
        private string mstrQueryParam;
        private string mstrSelectedDB;
        private string mstrGitLogin;
        private string mstrGitPassword;

        public RepositoryView()
        {

            try
            {
                this.mobjRepositoryModel = new RepositoryModel();
                this.mobjSP = new SecureString();
                this.mstrQueryParam = string.Empty;
                this.mstrSelectedDB = string.Empty;
                this.mstrGitLogin = string.Empty;
                this.mstrGitPassword = string.Empty;

                InitializeComponent();

                this.cbListOfServers.DataSource = mobjRepositoryModel.ServersList;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "שגיאה", MessageBoxButtons.AbortRetryIgnore);
            }
        }
        
        /// <summary>
        /// Event when User push the Connect Button to connect to the Sql Server.
        /// </summary>
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cbListOfServers.Text))
                {
                    this.mobjRepositoryModel.SetCridential(
                        tbUserName.Text,
                        mobjSP,
                        cbWindowsAuth.Checked);

                    this.mobjRepositoryModel.LoadListOfDatabases(
                        cbListOfServers.Text);

                    if (this.mobjRepositoryModel.DatabasesList.Count > 0)
                    {
                        gbGIT.Enabled = true;
                    }
                    else
                    {
                        gbGIT.Enabled = false;
                    }

                    cbDataBases.DataSource = mobjRepositoryModel.DatabasesList;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                MessageBox.Show(this, "נכשל בהתחברות לשרת הנתונים", "שגיאה", MessageBoxButtons.OK);                
            }
            
        }
        /// <summary>
        /// Event Shows the Username and password 
        /// when Windows Authentication is not selected.
        /// </summary>
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWindowsAuth.Checked)
            {
                tbUserName.Enabled = false;
                tbPassword.Enabled = false;
            }
            else
            {
                tbUserName.Enabled = true;
                tbPassword.Enabled = true;
            }
        }
        /// <summary>
        /// Event to clear user SQLServer pasword when leave the textbox.
        /// </summary>
        private void Password_Leave(object sender, EventArgs e)
        {
            try
            {
                InitializeSP();

                SPClone();

                mobjSP.MakeReadOnly();
            }
            catch (Exception Ex) 
            {
                throw new Exception("Failed Reading Password",Ex);
            }
            
        }
        /// <summary>
        /// Save the Password and Clear the text on the Text box.
        /// </summary>
        private void SPClone()
        {
            foreach (char objChar in tbPassword.Text)
            {
                mobjSP.AppendChar(objChar);
            }

            tbPassword.Text = string.Empty;
        }
        private void InitializeSP()
        {
            mobjSP.Dispose();
            mobjSP = new SecureString();
            
        }
        /// <summary>
        /// Event enables the search button when the user enter 
        /// the Query Parameter - Name of GitHub User
        /// </summary>
        private void QueryPrameter_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbQueryPrameter.Text))
            {
                btnRunQuery.Enabled = true;
                btnRefresh.Enabled = true;
            }
            else
            {
                btnRunQuery.Enabled = false;
                btnRefresh.Enabled = false;
            }
        }
        /// <summary>
        /// Running a search on Github Users , 
        /// Get GitHub User's Repositories and Load the data to the DataBase 
        /// and Views.
        /// </summary>
        private void RunQuery_Click(object sender, EventArgs e)
        {
            
            try
            {
                UpdateDataView();

                if (!bwGetUsersRepo.IsBusy)
                {
                    mstrQueryParam = tbQueryPrameter.Text;
                    mstrSelectedDB = cbDataBases.Text;
                    mstrGitLogin= tbGitLogin.Text;
                    mstrGitPassword = tbGitPassword.Text;

                    bwGetUsersRepo.RunWorkerAsync(mobjRepositoryModel);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                MessageBox.Show(
                    this,
                    "החיפוש במאגר נכשל", 
                    "אירעה שגיאה", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading);
            }
        }

 
 
        /// <summary>
        /// Update the View that lists the Users and Repositories.
        /// </summary>
        private void UpdateDataView()
        {
            try
            {
                bsRepositories.DataSource = mobjRepositoryModel.GitHubRepositories;
                bsRepositories.DataMember = RepositoryModel.REPOSITORIES;

                bsUsers.DataSource = mobjRepositoryModel.GitHubRepositories;
                bsUsers.DataMember = RepositoryModel.USERS;

                bnUsersNavigator.BindingSource = bsUsers;
                bnRepositoryNavigator.BindingSource = bsRepositories;

                this.dgvRepositories.AutoGenerateColumns = true;
                this.dgvUsers.AutoGenerateColumns = true;
                this.dgvRepositories.AutoGenerateColumns = true;

                dgvUsers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dgvRepositories.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception Ex)
            {
                throw new Exception("Failed Load Data To View : UpdateDataView",Ex);
            }
        }

        /// <summary>
        /// Event Unable the GroupBox of the search 
        /// when the user enters username and password.
        /// </summary>
        private void GitLogin_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbGitLogin.Text) &&
                !string.IsNullOrEmpty(tbGitPassword.Text))
            {
                gbQuery.Enabled = true;
            }
            else
            {
                gbQuery.Enabled = false;
            }
            
        }
        /// <summary>
        /// Occurs when the Changing User At the DataGridView , 
        /// It Fills the Repository DataGridView with the selected User's Repository.
        /// </summary>
        private void Users_SelectionChanged(object sender, EventArgs e)
        {
            int intSelectedUserID;

            try
            {
                
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    intSelectedUserID = (int)dgvUsers.SelectedRows[0].Cells["UserID"].Value;

                    mobjRepositoryModel.GetUserRepository(intSelectedUserID);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                MessageBox.Show(
                   this,
                   "נכשל לשלוף את רשימת המאגרים של המשתמש",
                   "אירעה שגיאה",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                   MessageBoxOptions.RtlReading);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (mobjRepositoryModel == null ||
                !mobjRepositoryModel.IsDatabaseReady)
            {
                mobjRepositoryModel.CreateDataBase(cbDataBases.Text);
            }

            mobjRepositoryModel.GetAllUsers();

            //UpdateDataView();
        }

        private void bwGetUsersRepo_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            RepositoryModel objRepoModelInWork;

            objRepoModelInWork = (RepositoryModel)e.Argument;

            try
            {
                mobjRepositoryModel.CreateDataBase(mstrSelectedDB);

                GitHubClient.OpenClient(string.Format("{0}:{1}", mstrGitLogin, mstrGitPassword));

                GetUsersFromGitHub();

            }
            catch (Exception Ex)
            {
                throw new Exception("Failed To Complete DoWork", Ex);
            }
        }

        private void bwGetUsersRepo_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            pbGetRepoStatus.Value = e.ProgressPercentage;            
            
        }

        private void bwGetUsersRepo_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(
                   this,
                   "נכשל לשלוף את רשימת המאגרים של המשתמש",
                   "אירעה שגיאה",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                   MessageBoxOptions.RtlReading);
            }
            else
            {
                mobjRepositoryModel.GetAllUsers();
                pbGetRepoStatus.Value = 0;
            }
        }

        private void GetUsersFromGitHub()
        {
            GitHubUsers objGitHubUsers;
            int intNumberOfPages = 10;

            for (int intPageNumber = 1; intPageNumber <= intNumberOfPages; intPageNumber++)
            {

                if (bwGetUsersRepo.CancellationPending)
                {
                    break;
                }

                objGitHubUsers = GitHubClient.GetUsersAsync(mstrQueryParam, intPageNumber).GetAwaiter().GetResult();

                UpdateUsersData(objGitHubUsers, intPageNumber);

                if (objGitHubUsers.Total_Count < 1000)
                {
                    intNumberOfPages = (objGitHubUsers.Total_Count / 100) + 1;
                }                
            }
        }

        private void UpdateUsersData(GitHubUsers objGitHubUsers,int intPageNumber)
        {
            int intUserCount;
            User objNewUserData;
            List<Repository> objUserRepositories;

            intUserCount = 1;

            foreach (User objUser in objGitHubUsers.Items)
            {
                try
                {
                    if (bwGetUsersRepo.CancellationPending)
                    {
                        break;
                    }
                    objNewUserData = GitHubClient.GetUsersDataAsync(objUser).GetAwaiter().GetResult();

                    mobjRepositoryModel.InsertUserToDB(objNewUserData);

                    objUserRepositories = GitHubClient.GetUserRepsitoriesAsync(objUser).GetAwaiter().GetResult();

                    mobjRepositoryModel.InsertUserRepositories(objUser, objUserRepositories);

                    bwGetUsersRepo.ReportProgress(((intUserCount++ + (100 * intPageNumber)) * 100) / objGitHubUsers.Total_Count);
                }
                catch (HttpRequestException HttpEx)
                {
                    Console.WriteLine(HttpEx.Message);
                    Console.WriteLine(string.Format("Not all User Data Updated for user: {0}", objUser.Login));
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (bwGetUsersRepo.IsBusy)
            {
                bwGetUsersRepo.CancelAsync();
            }
        }
    }

}
