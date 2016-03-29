namespace AgoraIDChecker
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
        	this.components = new System.ComponentModel.Container();
        	this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        	this.oraBase = new System.Windows.Forms.TextBox();
        	this.oraLogin = new System.Windows.Forms.TextBox();
        	this.oraPass = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.label5 = new System.Windows.Forms.Label();
        	this.startDate = new System.Windows.Forms.DateTimePicker();
        	this.endDate = new System.Windows.Forms.DateTimePicker();
        	this.tabControl = new System.Windows.Forms.TabControl();
        	this.tabPage1 = new System.Windows.Forms.TabPage();
        	this.listView1 = new System.Windows.Forms.ListView();
        	this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
        	this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        	this.SameNumbersFixButton = new System.Windows.Forms.Button();
        	this.SameNumbersSearchButton = new System.Windows.Forms.Button();
        	this.tabPage2 = new System.Windows.Forms.TabPage();
        	this.listView2 = new System.Windows.Forms.ListView();
        	this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
        	this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
        	this.DifferentJudgesFixButton = new System.Windows.Forms.Button();
        	this.DifferentJudgesSearchButton = new System.Windows.Forms.Button();
        	this.tabPage3 = new System.Windows.Forms.TabPage();
        	this.judgeFix2List = new System.Windows.Forms.ListView();
        	this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
        	this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
        	this.DifferentJudges2FixButton = new System.Windows.Forms.Button();
        	this.DifferentJudges2SearchButton = new System.Windows.Forms.Button();
        	this.tabPage4 = new System.Windows.Forms.TabPage();
        	this.ExclusionsList = new System.Windows.Forms.ListView();
        	this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.groupBox4 = new System.Windows.Forms.GroupBox();
        	this.ShowAllExclButton = new System.Windows.Forms.RadioButton();
        	this.HideCheckedExclButton = new System.Windows.Forms.RadioButton();
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.CheckServiceBDExclButton = new System.Windows.Forms.Button();
        	this.CheckBSRExclButton = new System.Windows.Forms.Button();
        	this.UncheckAllExclButton = new System.Windows.Forms.Button();
        	this.CheckAllExclButton = new System.Windows.Forms.Button();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
        	this.exclBase = new System.Windows.Forms.TextBox();
        	this.exclUser = new System.Windows.Forms.TextBox();
        	this.exclPass = new System.Windows.Forms.TextBox();
        	this.label7 = new System.Windows.Forms.Label();
        	this.label8 = new System.Windows.Forms.Label();
        	this.label9 = new System.Windows.Forms.Label();
        	this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
        	this.ExclusionSyncFixButton = new System.Windows.Forms.Button();
        	this.ExclusionSyncSearchButton = new System.Windows.Forms.Button();
        	this.SearchWorker = new System.ComponentModel.BackgroundWorker();
        	this.FixWorker = new System.ComponentModel.BackgroundWorker();
        	this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        	this.JudgesSearchWorker = new System.ComponentModel.BackgroundWorker();
        	this.JudgesFixWorker = new System.ComponentModel.BackgroundWorker();
        	this.JudgesFix2Worker = new System.ComponentModel.BackgroundWorker();
        	this.JudgesSearch2Worker = new System.ComponentModel.BackgroundWorker();
        	this.ExclusionsSearchWorker = new System.ComponentModel.BackgroundWorker();
        	this.ExclusionsSyncWorker = new System.ComponentModel.BackgroundWorker();
        	this.inProgressPanel = new System.Windows.Forms.Panel();
        	this.cancelButton = new System.Windows.Forms.Button();
        	this.label6 = new System.Windows.Forms.Label();
        	this.pictureBox1 = new System.Windows.Forms.PictureBox();
        	this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
        	this.tableLayoutPanel1.SuspendLayout();
        	this.groupBox1.SuspendLayout();
        	this.tableLayoutPanel2.SuspendLayout();
        	this.tabControl.SuspendLayout();
        	this.tabPage1.SuspendLayout();
        	this.flowLayoutPanel1.SuspendLayout();
        	this.tabPage2.SuspendLayout();
        	this.flowLayoutPanel2.SuspendLayout();
        	this.tabPage3.SuspendLayout();
        	this.flowLayoutPanel3.SuspendLayout();
        	this.tabPage4.SuspendLayout();
        	this.panel1.SuspendLayout();
        	this.groupBox4.SuspendLayout();
        	this.groupBox3.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	this.tableLayoutPanel3.SuspendLayout();
        	this.flowLayoutPanel4.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        	this.inProgressPanel.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// tableLayoutPanel1
        	// 
        	this.tableLayoutPanel1.ColumnCount = 1;
        	this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        	this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
        	this.tableLayoutPanel1.Controls.Add(this.tabControl, 0, 1);
        	this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        	this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        	this.tableLayoutPanel1.RowCount = 2;
        	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        	this.tableLayoutPanel1.Size = new System.Drawing.Size(672, 465);
        	this.tableLayoutPanel1.TabIndex = 0;
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.AutoSize = true;
        	this.groupBox1.Controls.Add(this.tableLayoutPanel2);
        	this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
        	this.groupBox1.Location = new System.Drawing.Point(3, 3);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(474, 97);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Подключение к БСР";
        	// 
        	// tableLayoutPanel2
        	// 
        	this.tableLayoutPanel2.AutoSize = true;
        	this.tableLayoutPanel2.ColumnCount = 3;
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
        	this.tableLayoutPanel2.Controls.Add(this.oraBase, 0, 1);
        	this.tableLayoutPanel2.Controls.Add(this.oraLogin, 1, 1);
        	this.tableLayoutPanel2.Controls.Add(this.oraPass, 2, 1);
        	this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
        	this.tableLayoutPanel2.Controls.Add(this.label2, 1, 0);
        	this.tableLayoutPanel2.Controls.Add(this.label3, 2, 0);
        	this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
        	this.tableLayoutPanel2.Controls.Add(this.label5, 1, 2);
        	this.tableLayoutPanel2.Controls.Add(this.startDate, 0, 3);
        	this.tableLayoutPanel2.Controls.Add(this.endDate, 1, 3);
        	this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
        	this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        	this.tableLayoutPanel2.RowCount = 4;
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel2.Size = new System.Drawing.Size(468, 78);
        	this.tableLayoutPanel2.TabIndex = 0;
        	// 
        	// oraBase
        	// 
        	this.oraBase.Dock = System.Windows.Forms.DockStyle.Top;
        	this.oraBase.Location = new System.Drawing.Point(3, 16);
        	this.oraBase.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.oraBase.Name = "oraBase";
        	this.oraBase.Size = new System.Drawing.Size(137, 20);
        	this.oraBase.TabIndex = 0;
        	this.oraBase.Text = "GAS";
        	// 
        	// oraLogin
        	// 
        	this.oraLogin.Dock = System.Windows.Forms.DockStyle.Top;
        	this.oraLogin.Location = new System.Drawing.Point(159, 16);
        	this.oraLogin.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.oraLogin.Name = "oraLogin";
        	this.oraLogin.Size = new System.Drawing.Size(137, 20);
        	this.oraLogin.TabIndex = 1;
        	this.oraLogin.Text = "BSR";
        	// 
        	// oraPass
        	// 
        	this.oraPass.Dock = System.Windows.Forms.DockStyle.Top;
        	this.oraPass.Location = new System.Drawing.Point(315, 16);
        	this.oraPass.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.oraPass.Name = "oraPass";
        	this.oraPass.PasswordChar = '*';
        	this.oraPass.Size = new System.Drawing.Size(137, 20);
        	this.oraPass.TabIndex = 2;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(3, 0);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(32, 13);
        	this.label1.TabIndex = 3;
        	this.label1.Text = "База";
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(159, 0);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(38, 13);
        	this.label2.TabIndex = 4;
        	this.label2.Text = "Логин";
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(315, 0);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(45, 13);
        	this.label3.TabIndex = 5;
        	this.label3.Text = "Пароль";
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(3, 39);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(89, 13);
        	this.label4.TabIndex = 6;
        	this.label4.Text = "Начало периода";
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(159, 39);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(83, 13);
        	this.label5.TabIndex = 7;
        	this.label5.Text = "Конец периода";
        	// 
        	// startDate
        	// 
        	this.startDate.Location = new System.Drawing.Point(3, 55);
        	this.startDate.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.startDate.Name = "startDate";
        	this.startDate.Size = new System.Drawing.Size(137, 20);
        	this.startDate.TabIndex = 8;
        	// 
        	// endDate
        	// 
        	this.endDate.Location = new System.Drawing.Point(159, 55);
        	this.endDate.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.endDate.Name = "endDate";
        	this.endDate.Size = new System.Drawing.Size(137, 20);
        	this.endDate.TabIndex = 9;
        	// 
        	// tabControl
        	// 
        	this.tabControl.Controls.Add(this.tabPage1);
        	this.tabControl.Controls.Add(this.tabPage2);
        	this.tabControl.Controls.Add(this.tabPage3);
        	this.tabControl.Controls.Add(this.tabPage4);
        	this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tabControl.Location = new System.Drawing.Point(3, 106);
        	this.tabControl.Name = "tabControl";
        	this.tabControl.SelectedIndex = 0;
        	this.tabControl.Size = new System.Drawing.Size(666, 356);
        	this.tabControl.TabIndex = 2;
        	// 
        	// tabPage1
        	// 
        	this.tabPage1.Controls.Add(this.listView1);
        	this.tabPage1.Controls.Add(this.flowLayoutPanel1);
        	this.tabPage1.Location = new System.Drawing.Point(4, 22);
        	this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
        	this.tabPage1.Name = "tabPage1";
        	this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
        	this.tabPage1.Size = new System.Drawing.Size(658, 330);
        	this.tabPage1.TabIndex = 0;
        	this.tabPage1.Text = "1. Одинаковые номера. Разные ID";
        	this.tabPage1.UseVisualStyleBackColor = true;
        	// 
        	// listView1
        	// 
        	this.listView1.CheckBoxes = true;
        	this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader1,
        	        	        	this.columnHeader2,
        	        	        	this.columnHeader3,
        	        	        	this.columnHeader4});
        	this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.listView1.FullRowSelect = true;
        	this.listView1.GridLines = true;
        	this.listView1.Location = new System.Drawing.Point(3, 3);
        	this.listView1.Name = "listView1";
        	this.listView1.Size = new System.Drawing.Size(652, 295);
        	this.listView1.TabIndex = 3;
        	this.listView1.UseCompatibleStateImageBehavior = false;
        	this.listView1.View = System.Windows.Forms.View.Details;
        	this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
        	// 
        	// columnHeader1
        	// 
        	this.columnHeader1.Text = "Номер дела";
        	this.columnHeader1.Width = 90;
        	// 
        	// columnHeader2
        	// 
        	this.columnHeader2.Text = "ID СДП";
        	this.columnHeader2.Width = 86;
        	// 
        	// columnHeader3
        	// 
        	this.columnHeader3.Text = "ID_AGORA БСР";
        	this.columnHeader3.Width = 103;
        	// 
        	// columnHeader4
        	// 
        	this.columnHeader4.Text = "Номер дела БСР с ID СДП";
        	this.columnHeader4.Width = 153;
        	// 
        	// flowLayoutPanel1
        	// 
        	this.flowLayoutPanel1.AutoSize = true;
        	this.flowLayoutPanel1.Controls.Add(this.SameNumbersFixButton);
        	this.flowLayoutPanel1.Controls.Add(this.SameNumbersSearchButton);
        	this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        	this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 298);
        	this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
        	this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        	this.flowLayoutPanel1.Size = new System.Drawing.Size(652, 29);
        	this.flowLayoutPanel1.TabIndex = 4;
        	// 
        	// SameNumbersFixButton
        	// 
        	this.SameNumbersFixButton.AutoSize = true;
        	this.SameNumbersFixButton.Enabled = false;
        	this.SameNumbersFixButton.Location = new System.Drawing.Point(516, 3);
        	this.SameNumbersFixButton.Name = "SameNumbersFixButton";
        	this.SameNumbersFixButton.Size = new System.Drawing.Size(133, 23);
        	this.SameNumbersFixButton.TabIndex = 0;
        	this.SameNumbersFixButton.Text = "Исправить выбранные";
        	this.SameNumbersFixButton.UseVisualStyleBackColor = true;
        	this.SameNumbersFixButton.Click += new System.EventHandler(this.SameNumbersFixButton_Click);
        	// 
        	// SameNumbersSearchButton
        	// 
        	this.SameNumbersSearchButton.Location = new System.Drawing.Point(435, 3);
        	this.SameNumbersSearchButton.Name = "SameNumbersSearchButton";
        	this.SameNumbersSearchButton.Size = new System.Drawing.Size(75, 23);
        	this.SameNumbersSearchButton.TabIndex = 1;
        	this.SameNumbersSearchButton.Text = "Искать";
        	this.SameNumbersSearchButton.UseVisualStyleBackColor = true;
        	this.SameNumbersSearchButton.Click += new System.EventHandler(this.SameNumbersSearchButton_Click);
        	// 
        	// tabPage2
        	// 
        	this.tabPage2.Controls.Add(this.listView2);
        	this.tabPage2.Controls.Add(this.flowLayoutPanel2);
        	this.tabPage2.Location = new System.Drawing.Point(4, 22);
        	this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
        	this.tabPage2.Name = "tabPage2";
        	this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
        	this.tabPage2.Size = new System.Drawing.Size(658, 330);
        	this.tabPage2.TabIndex = 1;
        	this.tabPage2.Text = "2. Несовпадающие группы судей (1.5.0)";
        	this.tabPage2.UseVisualStyleBackColor = true;
        	// 
        	// listView2
        	// 
        	this.listView2.CheckBoxes = true;
        	this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader5,
        	        	        	this.columnHeader6,
        	        	        	this.columnHeader7});
        	this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.listView2.FullRowSelect = true;
        	this.listView2.GridLines = true;
        	this.listView2.Location = new System.Drawing.Point(3, 3);
        	this.listView2.Margin = new System.Windows.Forms.Padding(6);
        	this.listView2.Name = "listView2";
        	this.listView2.Size = new System.Drawing.Size(652, 295);
        	this.listView2.TabIndex = 3;
        	this.listView2.UseCompatibleStateImageBehavior = false;
        	this.listView2.View = System.Windows.Forms.View.Details;
        	this.listView2.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView2_ItemChecked);
        	// 
        	// columnHeader5
        	// 
        	this.columnHeader5.Text = "Номер дела";
        	this.columnHeader5.Width = 90;
        	// 
        	// columnHeader6
        	// 
        	this.columnHeader6.Text = "Судья в СДП";
        	this.columnHeader6.Width = 150;
        	// 
        	// columnHeader7
        	// 
        	this.columnHeader7.Text = "Судья БСР";
        	this.columnHeader7.Width = 150;
        	// 
        	// flowLayoutPanel2
        	// 
        	this.flowLayoutPanel2.AutoSize = true;
        	this.flowLayoutPanel2.Controls.Add(this.DifferentJudgesFixButton);
        	this.flowLayoutPanel2.Controls.Add(this.DifferentJudgesSearchButton);
        	this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        	this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 298);
        	this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
        	this.flowLayoutPanel2.Name = "flowLayoutPanel2";
        	this.flowLayoutPanel2.Size = new System.Drawing.Size(652, 29);
        	this.flowLayoutPanel2.TabIndex = 4;
        	// 
        	// DifferentJudgesFixButton
        	// 
        	this.DifferentJudgesFixButton.AutoSize = true;
        	this.DifferentJudgesFixButton.Enabled = false;
        	this.DifferentJudgesFixButton.Location = new System.Drawing.Point(516, 3);
        	this.DifferentJudgesFixButton.Name = "DifferentJudgesFixButton";
        	this.DifferentJudgesFixButton.Size = new System.Drawing.Size(133, 23);
        	this.DifferentJudgesFixButton.TabIndex = 0;
        	this.DifferentJudgesFixButton.Text = "Исправить выбранные";
        	this.DifferentJudgesFixButton.UseVisualStyleBackColor = true;
        	this.DifferentJudgesFixButton.Click += new System.EventHandler(this.DifferentJudgesFixButton_Click);
        	// 
        	// DifferentJudgesSearchButton
        	// 
        	this.DifferentJudgesSearchButton.Location = new System.Drawing.Point(435, 3);
        	this.DifferentJudgesSearchButton.Name = "DifferentJudgesSearchButton";
        	this.DifferentJudgesSearchButton.Size = new System.Drawing.Size(75, 23);
        	this.DifferentJudgesSearchButton.TabIndex = 1;
        	this.DifferentJudgesSearchButton.Text = "Искать";
        	this.DifferentJudgesSearchButton.UseVisualStyleBackColor = true;
        	this.DifferentJudgesSearchButton.Click += new System.EventHandler(this.DifferentJudgesSearchButton_Click);
        	// 
        	// tabPage3
        	// 
        	this.tabPage3.Controls.Add(this.judgeFix2List);
        	this.tabPage3.Controls.Add(this.flowLayoutPanel3);
        	this.tabPage3.Location = new System.Drawing.Point(4, 22);
        	this.tabPage3.Name = "tabPage3";
        	this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
        	this.tabPage3.Size = new System.Drawing.Size(658, 330);
        	this.tabPage3.TabIndex = 2;
        	this.tabPage3.Text = "3. Несовпадающие группы судей (1.5.1)";
        	this.tabPage3.UseVisualStyleBackColor = true;
        	// 
        	// judgeFix2List
        	// 
        	this.judgeFix2List.CheckBoxes = true;
        	this.judgeFix2List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader8,
        	        	        	this.columnHeader9,
        	        	        	this.columnHeader10});
        	this.judgeFix2List.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.judgeFix2List.FullRowSelect = true;
        	this.judgeFix2List.GridLines = true;
        	this.judgeFix2List.Location = new System.Drawing.Point(3, 3);
        	this.judgeFix2List.Margin = new System.Windows.Forms.Padding(6);
        	this.judgeFix2List.Name = "judgeFix2List";
        	this.judgeFix2List.Size = new System.Drawing.Size(652, 295);
        	this.judgeFix2List.TabIndex = 5;
        	this.judgeFix2List.UseCompatibleStateImageBehavior = false;
        	this.judgeFix2List.View = System.Windows.Forms.View.Details;
        	this.judgeFix2List.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.JudgeFix2ListItemChecked);
        	// 
        	// columnHeader8
        	// 
        	this.columnHeader8.Text = "Номер дела";
        	this.columnHeader8.Width = 90;
        	// 
        	// columnHeader9
        	// 
        	this.columnHeader9.Text = "Судья в СДП";
        	this.columnHeader9.Width = 150;
        	// 
        	// columnHeader10
        	// 
        	this.columnHeader10.Text = "Судья БСР";
        	this.columnHeader10.Width = 150;
        	// 
        	// flowLayoutPanel3
        	// 
        	this.flowLayoutPanel3.AutoSize = true;
        	this.flowLayoutPanel3.Controls.Add(this.DifferentJudges2FixButton);
        	this.flowLayoutPanel3.Controls.Add(this.DifferentJudges2SearchButton);
        	this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        	this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 298);
        	this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
        	this.flowLayoutPanel3.Name = "flowLayoutPanel3";
        	this.flowLayoutPanel3.Size = new System.Drawing.Size(652, 29);
        	this.flowLayoutPanel3.TabIndex = 6;
        	// 
        	// DifferentJudges2FixButton
        	// 
        	this.DifferentJudges2FixButton.AutoSize = true;
        	this.DifferentJudges2FixButton.Enabled = false;
        	this.DifferentJudges2FixButton.Location = new System.Drawing.Point(516, 3);
        	this.DifferentJudges2FixButton.Name = "DifferentJudges2FixButton";
        	this.DifferentJudges2FixButton.Size = new System.Drawing.Size(133, 23);
        	this.DifferentJudges2FixButton.TabIndex = 0;
        	this.DifferentJudges2FixButton.Text = "Исправить выбранные";
        	this.DifferentJudges2FixButton.UseVisualStyleBackColor = true;
        	this.DifferentJudges2FixButton.Click += new System.EventHandler(this.DifferentJudges2FixButtonClick);
        	// 
        	// DifferentJudges2SearchButton
        	// 
        	this.DifferentJudges2SearchButton.Location = new System.Drawing.Point(435, 3);
        	this.DifferentJudges2SearchButton.Name = "DifferentJudges2SearchButton";
        	this.DifferentJudges2SearchButton.Size = new System.Drawing.Size(75, 23);
        	this.DifferentJudges2SearchButton.TabIndex = 1;
        	this.DifferentJudges2SearchButton.Text = "Искать";
        	this.DifferentJudges2SearchButton.UseVisualStyleBackColor = true;
        	this.DifferentJudges2SearchButton.Click += new System.EventHandler(this.DifferentJudges2SearchButtonClick);
        	// 
        	// tabPage4
        	// 
        	this.tabPage4.Controls.Add(this.ExclusionsList);
        	this.tabPage4.Controls.Add(this.panel1);
        	this.tabPage4.Controls.Add(this.groupBox2);
        	this.tabPage4.Controls.Add(this.flowLayoutPanel4);
        	this.tabPage4.Location = new System.Drawing.Point(4, 22);
        	this.tabPage4.Name = "tabPage4";
        	this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
        	this.tabPage4.Size = new System.Drawing.Size(658, 330);
        	this.tabPage4.TabIndex = 3;
        	this.tabPage4.Text = "4. Исключения (Служебная БД -> БД БСР ) (БСР 1.5.3)";
        	this.tabPage4.UseVisualStyleBackColor = true;
        	// 
        	// ExclusionsList
        	// 
        	this.ExclusionsList.CheckBoxes = true;
        	this.ExclusionsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader11,
        	        	        	this.columnHeader15,
        	        	        	this.columnHeader12,
        	        	        	this.columnHeader13,
        	        	        	this.columnHeader14});
        	this.ExclusionsList.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.ExclusionsList.FullRowSelect = true;
        	this.ExclusionsList.GridLines = true;
        	this.ExclusionsList.Location = new System.Drawing.Point(3, 119);
        	this.ExclusionsList.Margin = new System.Windows.Forms.Padding(6);
        	this.ExclusionsList.Name = "ExclusionsList";
        	this.ExclusionsList.Size = new System.Drawing.Size(652, 179);
        	this.ExclusionsList.TabIndex = 13;
        	this.ExclusionsList.UseCompatibleStateImageBehavior = false;
        	this.ExclusionsList.View = System.Windows.Forms.View.Details;
        	this.ExclusionsList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ExclusionsListColumnClick);
        	this.ExclusionsList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ExclusionsListItemChecked);
        	// 
        	// columnHeader11
        	// 
        	this.columnHeader11.Text = "Номер дела";
        	this.columnHeader11.Width = 90;
        	// 
        	// columnHeader12
        	// 
        	this.columnHeader12.Text = "Причина исключения в БД БСР";
        	this.columnHeader12.Width = 183;
        	// 
        	// columnHeader13
        	// 
        	this.columnHeader13.Text = "Причина исключения в служебной БД";
        	this.columnHeader13.Width = 217;
        	// 
        	// columnHeader14
        	// 
        	this.columnHeader14.Text = "Исключил";
        	this.columnHeader14.Width = 80;
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.groupBox4);
        	this.panel1.Controls.Add(this.groupBox3);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
        	this.panel1.Location = new System.Drawing.Point(3, 66);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(652, 53);
        	this.panel1.TabIndex = 12;
        	// 
        	// groupBox4
        	// 
        	this.groupBox4.Controls.Add(this.ShowAllExclButton);
        	this.groupBox4.Controls.Add(this.HideCheckedExclButton);
        	this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.groupBox4.Location = new System.Drawing.Point(356, 0);
        	this.groupBox4.Name = "groupBox4";
        	this.groupBox4.Size = new System.Drawing.Size(296, 53);
        	this.groupBox4.TabIndex = 12;
        	this.groupBox4.TabStop = false;
        	this.groupBox4.Text = "Скрыть/Показать";
        	// 
        	// ShowAllExclButton
        	// 
        	this.ShowAllExclButton.Appearance = System.Windows.Forms.Appearance.Button;
        	this.ShowAllExclButton.Checked = true;
        	this.ShowAllExclButton.Location = new System.Drawing.Point(6, 20);
        	this.ShowAllExclButton.Name = "ShowAllExclButton";
        	this.ShowAllExclButton.Size = new System.Drawing.Size(134, 23);
        	this.ShowAllExclButton.TabIndex = 1;
        	this.ShowAllExclButton.TabStop = true;
        	this.ShowAllExclButton.Text = "Показать все";
        	this.ShowAllExclButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.ShowAllExclButton.UseVisualStyleBackColor = true;
        	this.ShowAllExclButton.CheckedChanged += new System.EventHandler(this.ShowAllExclButtonCheckedChanged);
        	// 
        	// HideCheckedExclButton
        	// 
        	this.HideCheckedExclButton.Appearance = System.Windows.Forms.Appearance.Button;
        	this.HideCheckedExclButton.Location = new System.Drawing.Point(146, 19);
        	this.HideCheckedExclButton.Name = "HideCheckedExclButton";
        	this.HideCheckedExclButton.Size = new System.Drawing.Size(139, 24);
        	this.HideCheckedExclButton.TabIndex = 0;
        	this.HideCheckedExclButton.Text = "Скрыть невыбранные";
        	this.HideCheckedExclButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.HideCheckedExclButton.UseVisualStyleBackColor = true;
        	this.HideCheckedExclButton.CheckedChanged += new System.EventHandler(this.HideCheckedExclButtonCheckedChanged);
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Controls.Add(this.CheckServiceBDExclButton);
        	this.groupBox3.Controls.Add(this.CheckBSRExclButton);
        	this.groupBox3.Controls.Add(this.UncheckAllExclButton);
        	this.groupBox3.Controls.Add(this.CheckAllExclButton);
        	this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
        	this.groupBox3.Location = new System.Drawing.Point(0, 0);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(356, 53);
        	this.groupBox3.TabIndex = 11;
        	this.groupBox3.TabStop = false;
        	this.groupBox3.Text = "Выбрать";
        	// 
        	// CheckServiceBDExclButton
        	// 
        	this.CheckServiceBDExclButton.Location = new System.Drawing.Point(250, 20);
        	this.CheckServiceBDExclButton.Name = "CheckServiceBDExclButton";
        	this.CheckServiceBDExclButton.Size = new System.Drawing.Size(92, 23);
        	this.CheckServiceBDExclButton.TabIndex = 3;
        	this.CheckServiceBDExclButton.Text = "В служебной";
        	this.CheckServiceBDExclButton.UseVisualStyleBackColor = true;
        	this.CheckServiceBDExclButton.Click += new System.EventHandler(this.CheckServiceBDExclButtonClick);
        	// 
        	// CheckBSRExclButton
        	// 
        	this.CheckBSRExclButton.Location = new System.Drawing.Point(169, 20);
        	this.CheckBSRExclButton.Name = "CheckBSRExclButton";
        	this.CheckBSRExclButton.Size = new System.Drawing.Size(75, 23);
        	this.CheckBSRExclButton.TabIndex = 2;
        	this.CheckBSRExclButton.Text = "В БСР";
        	this.CheckBSRExclButton.UseVisualStyleBackColor = true;
        	this.CheckBSRExclButton.Click += new System.EventHandler(this.CheckBSRExclButtonClick);
        	// 
        	// UncheckAllExclButton
        	// 
        	this.UncheckAllExclButton.Location = new System.Drawing.Point(88, 20);
        	this.UncheckAllExclButton.Name = "UncheckAllExclButton";
        	this.UncheckAllExclButton.Size = new System.Drawing.Size(75, 23);
        	this.UncheckAllExclButton.TabIndex = 1;
        	this.UncheckAllExclButton.Text = "Снять";
        	this.UncheckAllExclButton.UseVisualStyleBackColor = true;
        	this.UncheckAllExclButton.Click += new System.EventHandler(this.UncheckAllExclButtonClick);
        	// 
        	// CheckAllExclButton
        	// 
        	this.CheckAllExclButton.Location = new System.Drawing.Point(7, 20);
        	this.CheckAllExclButton.Name = "CheckAllExclButton";
        	this.CheckAllExclButton.Size = new System.Drawing.Size(75, 23);
        	this.CheckAllExclButton.TabIndex = 0;
        	this.CheckAllExclButton.Text = "Все";
        	this.CheckAllExclButton.UseVisualStyleBackColor = true;
        	this.CheckAllExclButton.Click += new System.EventHandler(this.CheckAllExclButtonClick);
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.tableLayoutPanel3);
        	this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
        	this.groupBox2.Location = new System.Drawing.Point(3, 3);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(652, 63);
        	this.groupBox2.TabIndex = 8;
        	this.groupBox2.TabStop = false;
        	this.groupBox2.Text = "Подключение к служебной БД";
        	// 
        	// tableLayoutPanel3
        	// 
        	this.tableLayoutPanel3.AutoSize = true;
        	this.tableLayoutPanel3.ColumnCount = 3;
        	this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        	this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        	this.tableLayoutPanel3.Controls.Add(this.exclBase, 0, 1);
        	this.tableLayoutPanel3.Controls.Add(this.exclUser, 1, 1);
        	this.tableLayoutPanel3.Controls.Add(this.exclPass, 2, 1);
        	this.tableLayoutPanel3.Controls.Add(this.label7, 0, 0);
        	this.tableLayoutPanel3.Controls.Add(this.label8, 1, 0);
        	this.tableLayoutPanel3.Controls.Add(this.label9, 2, 0);
        	this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
        	this.tableLayoutPanel3.Name = "tableLayoutPanel3";
        	this.tableLayoutPanel3.RowCount = 2;
        	this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
        	this.tableLayoutPanel3.Size = new System.Drawing.Size(646, 44);
        	this.tableLayoutPanel3.TabIndex = 1;
        	// 
        	// exclBase
        	// 
        	this.exclBase.Dock = System.Windows.Forms.DockStyle.Top;
        	this.exclBase.Location = new System.Drawing.Point(3, 16);
        	this.exclBase.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.exclBase.Name = "exclBase";
        	this.exclBase.Size = new System.Drawing.Size(304, 20);
        	this.exclBase.TabIndex = 0;
        	// 
        	// exclUser
        	// 
        	this.exclUser.Dock = System.Windows.Forms.DockStyle.Top;
        	this.exclUser.Location = new System.Drawing.Point(326, 16);
        	this.exclUser.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.exclUser.Name = "exclUser";
        	this.exclUser.Size = new System.Drawing.Size(142, 20);
        	this.exclUser.TabIndex = 1;
        	this.exclUser.Text = "SYSDBA";
        	// 
        	// exclPass
        	// 
        	this.exclPass.Dock = System.Windows.Forms.DockStyle.Top;
        	this.exclPass.Location = new System.Drawing.Point(487, 16);
        	this.exclPass.Margin = new System.Windows.Forms.Padding(3, 3, 16, 3);
        	this.exclPass.Name = "exclPass";
        	this.exclPass.PasswordChar = '*';
        	this.exclPass.Size = new System.Drawing.Size(143, 20);
        	this.exclPass.TabIndex = 2;
        	// 
        	// label7
        	// 
        	this.label7.AutoSize = true;
        	this.label7.Location = new System.Drawing.Point(3, 0);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(32, 13);
        	this.label7.TabIndex = 3;
        	this.label7.Text = "База";
        	// 
        	// label8
        	// 
        	this.label8.AutoSize = true;
        	this.label8.Location = new System.Drawing.Point(326, 0);
        	this.label8.Name = "label8";
        	this.label8.Size = new System.Drawing.Size(38, 13);
        	this.label8.TabIndex = 4;
        	this.label8.Text = "Логин";
        	// 
        	// label9
        	// 
        	this.label9.AutoSize = true;
        	this.label9.Location = new System.Drawing.Point(487, 0);
        	this.label9.Name = "label9";
        	this.label9.Size = new System.Drawing.Size(45, 13);
        	this.label9.TabIndex = 5;
        	this.label9.Text = "Пароль";
        	// 
        	// flowLayoutPanel4
        	// 
        	this.flowLayoutPanel4.AutoSize = true;
        	this.flowLayoutPanel4.Controls.Add(this.ExclusionSyncFixButton);
        	this.flowLayoutPanel4.Controls.Add(this.ExclusionSyncSearchButton);
        	this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        	this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 298);
        	this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
        	this.flowLayoutPanel4.Name = "flowLayoutPanel4";
        	this.flowLayoutPanel4.Size = new System.Drawing.Size(652, 29);
        	this.flowLayoutPanel4.TabIndex = 7;
        	// 
        	// ExclusionSyncFixButton
        	// 
        	this.ExclusionSyncFixButton.AutoSize = true;
        	this.ExclusionSyncFixButton.Enabled = false;
        	this.ExclusionSyncFixButton.Location = new System.Drawing.Point(476, 3);
        	this.ExclusionSyncFixButton.Name = "ExclusionSyncFixButton";
        	this.ExclusionSyncFixButton.Size = new System.Drawing.Size(173, 23);
        	this.ExclusionSyncFixButton.TabIndex = 0;
        	this.ExclusionSyncFixButton.Text = "Синхронизировать выбранные";
        	this.ExclusionSyncFixButton.UseVisualStyleBackColor = true;
        	this.ExclusionSyncFixButton.Click += new System.EventHandler(this.ExclusionSyncFixButtonClick);
        	// 
        	// ExclusionSyncSearchButton
        	// 
        	this.ExclusionSyncSearchButton.Location = new System.Drawing.Point(395, 3);
        	this.ExclusionSyncSearchButton.Name = "ExclusionSyncSearchButton";
        	this.ExclusionSyncSearchButton.Size = new System.Drawing.Size(75, 23);
        	this.ExclusionSyncSearchButton.TabIndex = 1;
        	this.ExclusionSyncSearchButton.Text = "Искать";
        	this.ExclusionSyncSearchButton.UseVisualStyleBackColor = true;
        	this.ExclusionSyncSearchButton.Click += new System.EventHandler(this.ExclusionSyncSearchButtonClick);
        	// 
        	// SearchWorker
        	// 
        	this.SearchWorker.WorkerSupportsCancellation = true;
        	this.SearchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SearchWorker_DoWork);
        	this.SearchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SearchWorker_RunWorkerCompleted);
        	// 
        	// FixWorker
        	// 
        	this.FixWorker.WorkerSupportsCancellation = true;
        	this.FixWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FixWorker_DoWork);
        	this.FixWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FixWorker_RunWorkerCompleted);
        	// 
        	// errorProvider1
        	// 
        	this.errorProvider1.ContainerControl = this;
        	// 
        	// JudgesSearchWorker
        	// 
        	this.JudgesSearchWorker.WorkerSupportsCancellation = true;
        	this.JudgesSearchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.JudgesSearchWorker_DoWork);
        	this.JudgesSearchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.JudgesSearchWorker_RunWorkerCompleted);
        	// 
        	// JudgesFixWorker
        	// 
        	this.JudgesFixWorker.WorkerSupportsCancellation = true;
        	this.JudgesFixWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.JudgesFixWorker_DoWork);
        	this.JudgesFixWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.JudgesFixWorker_RunWorkerCompleted);
        	// 
        	// JudgesFix2Worker
        	// 
        	this.JudgesFix2Worker.WorkerSupportsCancellation = true;
        	this.JudgesFix2Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.JudgesFix2WorkerDoWork);
        	this.JudgesFix2Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.JudgesFix2WorkerRunWorkerCompleted);
        	// 
        	// JudgesSearch2Worker
        	// 
        	this.JudgesSearch2Worker.WorkerSupportsCancellation = true;
        	this.JudgesSearch2Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.JudgesSearch2WorkerDoWork);
        	this.JudgesSearch2Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.JudgesSearch2WorkerRunWorkerCompleted);
        	// 
        	// ExclusionsSearchWorker
        	// 
        	this.ExclusionsSearchWorker.WorkerSupportsCancellation = true;
        	this.ExclusionsSearchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ExclusionsSearchWorkerDoWork);
        	this.ExclusionsSearchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ExclusionsSearchWorkerRunWorkerCompleted);
        	// 
        	// ExclusionsSyncWorker
        	// 
        	this.ExclusionsSyncWorker.WorkerSupportsCancellation = true;
        	this.ExclusionsSyncWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ExclusionsSyncWorkerDoWork);
        	this.ExclusionsSyncWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ExclusionsSyncWorkerRunWorkerCompleted);
        	// 
        	// inProgressPanel
        	// 
        	this.inProgressPanel.BackColor = System.Drawing.Color.White;
        	this.inProgressPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.inProgressPanel.Controls.Add(this.cancelButton);
        	this.inProgressPanel.Controls.Add(this.label6);
        	this.inProgressPanel.Controls.Add(this.pictureBox1);
        	this.inProgressPanel.Location = new System.Drawing.Point(181, 210);
        	this.inProgressPanel.Name = "inProgressPanel";
        	this.inProgressPanel.Size = new System.Drawing.Size(310, 45);
        	this.inProgressPanel.TabIndex = 2;
        	this.inProgressPanel.Visible = false;
        	// 
        	// cancelButton
        	// 
        	this.cancelButton.Location = new System.Drawing.Point(234, 3);
        	this.cancelButton.Name = "cancelButton";
        	this.cancelButton.Size = new System.Drawing.Size(71, 23);
        	this.cancelButton.TabIndex = 2;
        	this.cancelButton.Text = "Отмена";
        	this.cancelButton.UseVisualStyleBackColor = true;
        	// 
        	// label6
        	// 
        	this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
        	this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        	this.label6.Location = new System.Drawing.Point(42, 0);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(265, 43);
        	this.label6.TabIndex = 1;
        	this.label6.Text = "Выполняется операция. Пожалуйста подождите";
        	// 
        	// pictureBox1
        	// 
        	this.pictureBox1.Image = global::AgoraIDChecker.Properties.Resources.loader;
        	this.pictureBox1.Location = new System.Drawing.Point(4, 4);
        	this.pictureBox1.Name = "pictureBox1";
        	this.pictureBox1.Size = new System.Drawing.Size(32, 32);
        	this.pictureBox1.TabIndex = 0;
        	this.pictureBox1.TabStop = false;
        	// 
        	// columnHeader15
        	// 
        	this.columnHeader15.Text = "Дата рассмотрения";
        	this.columnHeader15.Width = 114;
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(672, 465);
        	this.Controls.Add(this.inProgressPanel);
        	this.Controls.Add(this.tableLayoutPanel1);
        	this.MinimumSize = new System.Drawing.Size(498, 350);
        	this.Name = "Form1";
        	this.Text = "Синхронизация идентификаторов";
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
        	this.Load += new System.EventHandler(this.Form1_Load);
        	this.Resize += new System.EventHandler(this.Form1_Resize);
        	this.tableLayoutPanel1.ResumeLayout(false);
        	this.tableLayoutPanel1.PerformLayout();
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	this.tableLayoutPanel2.ResumeLayout(false);
        	this.tableLayoutPanel2.PerformLayout();
        	this.tabControl.ResumeLayout(false);
        	this.tabPage1.ResumeLayout(false);
        	this.tabPage1.PerformLayout();
        	this.flowLayoutPanel1.ResumeLayout(false);
        	this.flowLayoutPanel1.PerformLayout();
        	this.tabPage2.ResumeLayout(false);
        	this.tabPage2.PerformLayout();
        	this.flowLayoutPanel2.ResumeLayout(false);
        	this.flowLayoutPanel2.PerformLayout();
        	this.tabPage3.ResumeLayout(false);
        	this.tabPage3.PerformLayout();
        	this.flowLayoutPanel3.ResumeLayout(false);
        	this.flowLayoutPanel3.PerformLayout();
        	this.tabPage4.ResumeLayout(false);
        	this.tabPage4.PerformLayout();
        	this.panel1.ResumeLayout(false);
        	this.groupBox4.ResumeLayout(false);
        	this.groupBox3.ResumeLayout(false);
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	this.tableLayoutPanel3.ResumeLayout(false);
        	this.tableLayoutPanel3.PerformLayout();
        	this.flowLayoutPanel4.ResumeLayout(false);
        	this.flowLayoutPanel4.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        	this.inProgressPanel.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.ComponentModel.BackgroundWorker ExclusionsSyncWorker;
        private System.Windows.Forms.RadioButton HideCheckedExclButton;
        private System.Windows.Forms.RadioButton ShowAllExclButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CheckAllExclButton;
        private System.Windows.Forms.Button UncheckAllExclButton;
        private System.Windows.Forms.Button CheckBSRExclButton;
        private System.Windows.Forms.Button CheckServiceBDExclButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.ComponentModel.BackgroundWorker ExclusionsSearchWorker;
        private System.Windows.Forms.Button ExclusionSyncSearchButton;
        private System.Windows.Forms.Button ExclusionSyncFixButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox exclPass;
        private System.Windows.Forms.TextBox exclUser;
        private System.Windows.Forms.TextBox exclBase;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ListView ExclusionsList;
        private System.Windows.Forms.TabPage tabPage4;
        private System.ComponentModel.BackgroundWorker JudgesSearch2Worker;
        private System.ComponentModel.BackgroundWorker JudgesFix2Worker;
        private System.Windows.Forms.Button DifferentJudges2SearchButton;
        private System.Windows.Forms.Button DifferentJudges2FixButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ListView judgeFix2List;
        private System.Windows.Forms.TabPage tabPage3;

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox oraBase;
        private System.Windows.Forms.TextBox oraLogin;
        private System.Windows.Forms.TextBox oraPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.ComponentModel.BackgroundWorker SearchWorker;
        private System.ComponentModel.BackgroundWorker FixWorker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.Panel inProgressPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button DifferentJudgesFixButton;
        private System.Windows.Forms.Button DifferentJudgesSearchButton;
        private System.ComponentModel.BackgroundWorker JudgesSearchWorker;
        private System.ComponentModel.BackgroundWorker JudgesFixWorker;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button SameNumbersFixButton;
        private System.Windows.Forms.Button SameNumbersSearchButton;
    }
}

