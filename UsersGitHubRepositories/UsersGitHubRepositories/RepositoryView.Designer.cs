namespace UsersGitHubRepositories
{
    partial class RepositoryView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbWindowsAuth = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbListOfServers = new System.Windows.Forms.ComboBox();
            this.serversListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.repositoryModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gbQuery = new System.Windows.Forms.GroupBox();
            this.tbQueryPrameter = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnRunQuery = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbDataBases = new System.Windows.Forms.ComboBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.gbUsersList = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.gbRepositories = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serversListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryModelBindingSource)).BeginInit();
            this.gbQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.gbUsersList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.gbRepositories.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainer1.Size = new System.Drawing.Size(812, 299);
            this.splitContainer1.SplitterDistance = 277;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.gbQuery);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainer2.Size = new System.Drawing.Size(277, 299);
            this.splitContainer2.SplitterDistance = 131;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbWindowsAuth);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbUserName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbListOfServers);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(277, 131);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "פרטי התחברות";
            // 
            // cbWindowsAuth
            // 
            this.cbWindowsAuth.AutoSize = true;
            this.cbWindowsAuth.Checked = true;
            this.cbWindowsAuth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWindowsAuth.Location = new System.Drawing.Point(166, 102);
            this.cbWindowsAuth.Name = "cbWindowsAuth";
            this.cbWindowsAuth.Size = new System.Drawing.Size(102, 17);
            this.cbWindowsAuth.TabIndex = 9;
            this.cbWindowsAuth.Text = "משתמש מערכת";
            this.cbWindowsAuth.UseVisualStyleBackColor = true;
            this.cbWindowsAuth.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 98);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "התחבר";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Enabled = false;
            this.tbPassword.Location = new System.Drawing.Point(6, 72);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbPassword.Size = new System.Drawing.Size(150, 20);
            this.tbPassword.TabIndex = 7;
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.Leave += new System.EventHandler(this.TbPassword_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(224, 72);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "סיסמא :";
            // 
            // tbUserName
            // 
            this.tbUserName.Enabled = false;
            this.tbUserName.Location = new System.Drawing.Point(6, 48);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbUserName.Size = new System.Drawing.Size(150, 20);
            this.tbUserName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 51);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "שם משתמש ::";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 22);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "שרת :";
            // 
            // cbListOfServers
            // 
            this.cbListOfServers.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.serversListBindingSource, "ServerName", true));
            this.cbListOfServers.DataSource = this.serversListBindingSource;
            this.cbListOfServers.DisplayMember = "ServerName";
            this.cbListOfServers.FormattingEnabled = true;
            this.cbListOfServers.Location = new System.Drawing.Point(6, 19);
            this.cbListOfServers.Name = "cbListOfServers";
            this.cbListOfServers.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbListOfServers.Size = new System.Drawing.Size(150, 21);
            this.cbListOfServers.TabIndex = 0;
            this.cbListOfServers.ValueMember = "ServerName";
            // 
            // serversListBindingSource
            // 
            this.serversListBindingSource.DataMember = "ServersList";
            this.serversListBindingSource.DataSource = this.repositoryModelBindingSource;
            // 
            // repositoryModelBindingSource
            // 
            this.repositoryModelBindingSource.DataSource = typeof(UsersGitHubRepositories.RepositoryModel);
            // 
            // gbQuery
            // 
            this.gbQuery.Controls.Add(this.tbQueryPrameter);
            this.gbQuery.Controls.Add(this.label5);
            this.gbQuery.Controls.Add(this.btnRunQuery);
            this.gbQuery.Controls.Add(this.label2);
            this.gbQuery.Controls.Add(this.cbDataBases);
            this.gbQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbQuery.Enabled = false;
            this.gbQuery.Location = new System.Drawing.Point(0, 0);
            this.gbQuery.Name = "gbQuery";
            this.gbQuery.Size = new System.Drawing.Size(277, 164);
            this.gbQuery.TabIndex = 0;
            this.gbQuery.TabStop = false;
            this.gbQuery.Text = "חיפוש";
            // 
            // tbQueryPrameter
            // 
            this.tbQueryPrameter.Location = new System.Drawing.Point(6, 72);
            this.tbQueryPrameter.MaxLength = 20;
            this.tbQueryPrameter.Name = "tbQueryPrameter";
            this.tbQueryPrameter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbQueryPrameter.Size = new System.Drawing.Size(150, 20);
            this.tbQueryPrameter.TabIndex = 7;
            this.tbQueryPrameter.TextChanged += new System.EventHandler(this.TbQueryPrameter_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(184, 66);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label5.Size = new System.Drawing.Size(87, 26);
            this.label5.TabIndex = 6;
            this.label5.Text = "מחרוזת לחיפוש\r\nמשתמש";
            // 
            // btnRunQuery
            // 
            this.btnRunQuery.Enabled = false;
            this.btnRunQuery.Location = new System.Drawing.Point(6, 129);
            this.btnRunQuery.Name = "btnRunQuery";
            this.btnRunQuery.Size = new System.Drawing.Size(86, 23);
            this.btnRunQuery.TabIndex = 4;
            this.btnRunQuery.Text = "חפש משתמש";
            this.btnRunQuery.UseVisualStyleBackColor = true;
            this.btnRunQuery.Click += new System.EventHandler(this.BtnRunQuery_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 22);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "בסיס נתונים :";
            // 
            // cbDataBases
            // 
            this.cbDataBases.FormattingEnabled = true;
            this.cbDataBases.Items.AddRange(new object[] {
            "<New>"});
            this.cbDataBases.Location = new System.Drawing.Point(6, 19);
            this.cbDataBases.Name = "cbDataBases";
            this.cbDataBases.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbDataBases.Size = new System.Drawing.Size(150, 21);
            this.cbDataBases.TabIndex = 2;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.gbUsersList);
            this.splitContainer3.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.gbRepositories);
            this.splitContainer3.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitContainer3.Size = new System.Drawing.Size(531, 299);
            this.splitContainer3.SplitterDistance = 131;
            this.splitContainer3.TabIndex = 1;
            // 
            // gbUsersList
            // 
            this.gbUsersList.Controls.Add(this.dataGridView2);
            this.gbUsersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbUsersList.Location = new System.Drawing.Point(0, 0);
            this.gbUsersList.Name = "gbUsersList";
            this.gbUsersList.Size = new System.Drawing.Size(531, 131);
            this.gbUsersList.TabIndex = 1;
            this.gbUsersList.TabStop = false;
            this.gbUsersList.Text = "רשימת משתמשים";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 16);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(525, 112);
            this.dataGridView2.TabIndex = 0;
            // 
            // gbRepositories
            // 
            this.gbRepositories.Controls.Add(this.dataGridView1);
            this.gbRepositories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRepositories.Location = new System.Drawing.Point(0, 0);
            this.gbRepositories.Name = "gbRepositories";
            this.gbRepositories.Size = new System.Drawing.Size(531, 164);
            this.gbRepositories.TabIndex = 2;
            this.gbRepositories.TabStop = false;
            this.gbRepositories.Text = "מאגרי משתמש (Repositories)";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(525, 145);
            this.dataGridView1.TabIndex = 0;
            // 
            // RepositoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 299);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Name = "RepositoryView";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.Text = "תוכן משתמשים";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serversListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryModelBindingSource)).EndInit();
            this.gbQuery.ResumeLayout(false);
            this.gbQuery.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.gbUsersList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.gbRepositories.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbListOfServers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.BindingSource serversListBindingSource;
        private System.Windows.Forms.BindingSource repositoryModelBindingSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbDataBases;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox gbQuery;
        private System.Windows.Forms.CheckBox cbWindowsAuth;
        private System.Windows.Forms.Button btnRunQuery;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox gbUsersList;
        private System.Windows.Forms.GroupBox gbRepositories;
        private System.Windows.Forms.TextBox tbQueryPrameter;
        private System.Windows.Forms.Label label5;
    }
}

