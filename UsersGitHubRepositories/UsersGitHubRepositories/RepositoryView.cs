using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
                        gbQuery.Enabled = true;
                    }
                    else
                    {
                        gbQuery.Enabled = false;
                    }

                    cbDataBases.DataSource = mobjRepositoryModel.DatabasesList;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, "נכשל בהתחברות לשרת הנתונים", "שגיאה", MessageBoxButtons.OK);                
            }
            
        }

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

        private void TbPassword_Leave(object sender, EventArgs e)
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

        private void TbQueryPrameter_TextChanged(object sender, EventArgs e)
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

        private void BtnRunQuery_Click(object sender, EventArgs e)
        {
            List<Repository> objUserRepositories;

            mobjRepositoryModel.CreateDataBase(cbDataBases.Text);

            GitHubClient.OpenClient();

            GitHubUsers objUsersResult = GitHubClient.GetUsersAsync(tbQueryPrameter.Text).GetAwaiter().GetResult();

            objUsersResult = GitHubClient.GetUsersDataAsync(objUsersResult).GetAwaiter().GetResult();

            foreach (User objUser in objUsersResult.Items)
            {
                objUserRepositories = GitHubClient.GetUserRepsitoriesAsync(objUser).GetAwaiter().GetResult();

                mobjRepositoryModel.InsertUserRepositories(objUser,objUserRepositories);
            }

            
        }

       
    }
}
