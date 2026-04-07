namespace TEST
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Main Layout with TableLayoutPanel
            var mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(0)
            };
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));  // Side Navigation
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));  // Input Form
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // Main Content
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));  // Chat

            // =============== SIDE NAVIGATION PANEL ===============
            var sideNavPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15, 20, 15, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Navigation header
            var navHeader = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20),
                Dock = DockStyle.Top
            };
            sideNavPanel.Controls.Add(navHeader);

            var navFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = false,
                WrapContents = false,
                Margin = new Padding(0),
                Width = 130,
                Height = 220
            };

            BtnNavDashboard = CreateNavButton("📊 Dashboard", 0);
            BtnNavMyMeals = CreateNavButton("🍽️ My Meals", 1);
            BtnNavDailyLog = CreateNavButton("📅 Daily Log", 2);

            navFlowPanel.Controls.Add(BtnNavDashboard);
            navFlowPanel.Controls.Add(BtnNavMyMeals);
            navFlowPanel.Controls.Add(BtnNavDailyLog);

            sideNavPanel.Controls.Add(navFlowPanel);

            // =============== LEFT PANEL - INPUT FORM ===============
            var inputTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 10,
                AutoSize = false,
                BackColor = Color.White,
                Padding = new Padding(15)
            };
            inputTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Meal Name Label
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Meal Name Input
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Meal Type Label
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Meal Type Input
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Date Label
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Date Input
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Recipes Label
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Recipes Input
            inputTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Button

            InputPanel = inputTableLayout;

            var headerLabel = new Label
            {
                Text = "Add Meal",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Dock = DockStyle.Top
            };

            // Meal Name
            var lblMealName = CreateLabel("Meal Name");
            TextBoxMealName = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 28,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top,
                PlaceholderText = "Enter meal name..."
            };

            // Meal Type
            var lblMealType = CreateLabel("Meal Type");
            ComboBoxMealType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                Margin = new Padding(0, 0, 0, 10),
                Height = 28,
                Dock = DockStyle.Top
            };
            ComboBoxMealType.Items.AddRange(new[] { "Breakfast", "Brunch", "Lunch", "Snack", "Dinner", "Supper" });
            ComboBoxMealType.SelectedIndex = 0;

            // Meal Date
            var lblMealDate = CreateLabel("Date");
            DateTimePickerMeal = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 0, 10),
                Value = DateTime.Today,
                Height = 28,
                Dock = DockStyle.Top
            };

            // Recipes/Ingredients
            var lblRecipes = CreateLabel("Recipes & Ingredients");
            TextBoxRecipes = new TextBox
            {
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Fill,
                PlaceholderText = "List recipes and ingredients (one per line)..."
            };

            // Button
            BtnCreateMeal = CreateBlackButton("Create Meal");
            BtnCreateMeal.Dock = DockStyle.Top;
            BtnCreateMeal.Height = 32;

            inputTableLayout.Controls.Add(headerLabel, 0, 0);
            inputTableLayout.Controls.Add(lblMealName, 0, 1);
            inputTableLayout.Controls.Add(TextBoxMealName, 0, 2);
            inputTableLayout.Controls.Add(lblMealType, 0, 3);
            inputTableLayout.Controls.Add(ComboBoxMealType, 0, 4);
            inputTableLayout.Controls.Add(lblMealDate, 0, 5);
            inputTableLayout.Controls.Add(DateTimePickerMeal, 0, 6);
            inputTableLayout.Controls.Add(lblRecipes, 0, 7);
            inputTableLayout.Controls.Add(TextBoxRecipes, 0, 8);
            inputTableLayout.Controls.Add(BtnCreateMeal, 0, 9);

            // =============== CENTER PANEL - MAIN CONTENT ===============
            ContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // =============== RIGHT PANEL - GEMINI CHAT ===============
            var chatTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White,
                Padding = new Padding(15),
                AutoSize = false
            };
            chatTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            chatTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
            chatTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Chat history
            chatTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Input
            chatTableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            var chatHeaderLabel = new Label
            {
                Text = "AI Nutrition Coach",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            TextBoxChatHistory = new RichTextBox
            {
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                Font = new Font("Segoe UI", 9),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10),
                WordWrap = true
            };

            TextBoxChatInput = new TextBox
            {
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 60,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 0, 0, 10),
                Dock = DockStyle.Top,
                PlaceholderText = "Ask about nutrition..."
            };

            var chatButtonsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Margin = new Padding(0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Top
            };

            BtnSendMessage = CreateBlackButton("Send");
            BtnSendMessage.Width = 80;
            BtnSendMessage.Margin = new Padding(0, 0, 5, 0);
            BtnClearChat = CreateBlackButton("Clear");
            BtnClearChat.Width = 70;

            chatButtonsPanel.Controls.Add(BtnSendMessage);
            chatButtonsPanel.Controls.Add(BtnClearChat);

            chatTableLayout.Controls.Add(chatHeaderLabel, 0, 0);
            chatTableLayout.Controls.Add(TextBoxChatHistory, 0, 1);
            chatTableLayout.Controls.Add(TextBoxChatInput, 0, 2);
            chatTableLayout.Controls.Add(chatButtonsPanel, 0, 3);

            ChatPanel = chatTableLayout;

            // Add panels to main layout
            mainTableLayout.Controls.Add(sideNavPanel, 0, 0);
            mainTableLayout.Controls.Add(InputPanel, 1, 0);
            mainTableLayout.Controls.Add(ContentPanel, 2, 0);
            mainTableLayout.Controls.Add(ChatPanel, 3, 0);

            // Status Bar
            StatusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 9)
            };

            StatusLabel = new ToolStripStatusLabel
            {
                Text = "Ready",
                Spring = true,
                Alignment = ToolStripItemAlignment.Left
            };
            StatusStrip.Items.Add(StatusLabel);

            // Form Setup
            Controls.Add(mainTableLayout);
            Controls.Add(StatusStrip);
            ClientSize = new Size(1600, 700);
            Text = "Edadmam - Meal Planner";
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.FromArgb(245, 245, 245);
            Load += Form1_Load;
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 3)
            };
        }

        private Button CreateBlackButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 32,
                Margin = new Padding(0),
                AutoSize = true
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 100, 100);
            return btn;
        }

        private Button CreateNavButton(string text, int index)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Width = 130,
                Height = 50,
                Margin = new Padding(0, 0, 0, 5),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 100, 100);
            return btn;
        }

        // Designer Controls
        public Panel InputPanel;
        public Panel ContentPanel;
        public Panel ChatPanel;
        public TextBox TextBoxMealName;
        public ComboBox ComboBoxMealType;
        public DateTimePicker DateTimePickerMeal;
        public TextBox TextBoxRecipes;
        public Button BtnCreateMeal;
        public Button BtnNavDashboard;
        public Button BtnNavMyMeals;
        public Button BtnNavDailyLog;
        public RichTextBox TextBoxChatHistory;
        public TextBox TextBoxChatInput;
        public Button BtnSendMessage;
        public Button BtnClearChat;
        public StatusStrip StatusStrip;
        public ToolStripStatusLabel StatusLabel;
    }
}
