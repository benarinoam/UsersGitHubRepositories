using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;

namespace UsersGitHubRepositories
{
    public partial class RepositoryView : Form
    {
        private RepositoryModel mobjRepositoryModel;
        private SecureString mobjSP;

        public RepositoryView()
        {

            try
            {
                this.mobjRepositoryModel = new RepositoryModel();
                this.mobjSP = new SecureString();

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
            }
            else
            {
                btnRunQuery.Enabled = false;
            }
        }
        /// <summary>
        /// Running a search on Github Users , 
        /// Get GitHub User's Repositories and Load the data to the DataBase 
        /// and Views.
        /// </summary>
        private void BtnRunQuery_Click(object sender, EventArgs e)
        {
            List<Repository> objUserRepositories;
            GitHubUsers objUsersResult;

            try
            {
                mobjRepositoryModel.CreateDataBase(cbDataBases.Text);

                GitHubClient.OpenClient(string.Format("{0}:{1}", tbGitLogin.Text, tbGitPassword.Text));

                objUsersResult = GitHubClient.GetUsersAsync(tbQueryPrameter.Text).GetAwaiter().GetResult();

                objUsersResult = GitHubClient.GetUsersDataAsync(objUsersResult).GetAwaiter().GetResult();

                mobjRepositoryModel.InsertUsersToDB(objUsersResult);

                foreach (User objUser in objUsersResult.Items)
                {
                    objUserRepositories = GitHubClient.GetUserRepsitoriesAsync(objUser).GetAwaiter().GetResult();

                    mobjRepositoryModel.InsertUserRepositories(objUser, objUserRepositories);
                }

                UpdateDataView();
            }
            catch (Exception Ex)
            {
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

                intSelectedUserID = (int)dgvUsers.SelectedRows[0].Cells["UserID"].Value;

                mobjRepositoryModel.GetUserRepository(intSelectedUserID);

            }
            catch (Exception Ex)
            {
                MessageBox.Show(
                   this,
                   "נכשל לשלוף את רשימת המאגרים של המשתמש",
                   "אירעה שגיאה",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                   MessageBoxOptions.RtlReading);
            }
        }
    }
}
