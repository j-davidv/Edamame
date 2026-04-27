using Edamam.Presentation.Interfaces;
using Edamam.Presentation.Models;
using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;
using System.Text;
using Edamam.Application.Interfaces;

namespace Edamam
{
    public partial class Form1 : Form
    {
        private readonly IFormController _controller;
        private Panel _currentContentPanel;
        private double _dailyCalorieGoal = 2000;

        // filter properties
        private string _currentFilterType = "Daily";
        private DateTime _selectedFilterDate = DateTime.Today;

        public Form1(IFormController controller)
        {
            InitializeComponent();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _currentContentPanel = null!;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // UI event handlers
                BtnCreateMeal.Click += BtnCreateMeal_Click;
                BtnClearForm.Click += BtnClearForm_Click;
                BtnNavDashboard.Click += BtnNavDashboard_Click;
                BtnNavMyMeals.Click += BtnNavMyMeals_Click;
                BtnNavDailyLog.Click += BtnNavDailyLog_Click;
                BtnNavBmi.Click += BtnNavBmi_Click;
                BtnSendMessage.Click += BtnSendMessage_Click;
                TextBoxChatInput.KeyDown += TextBoxChatInput_KeyDown;

                // load saved calorie goal from controller
                _dailyCalorieGoal = _controller.LoadDailyCalorieGoal();

                // Add welcome message to chat
                AppendChatBubble("👋 Hi! I'm your AI Nutrition Coach.\n\n" +
                    "💡 You can ask me about:\n" +
                    "• Macronutrient breakdowns\n" +
                    "• Nutrition goals\n" +
                    "• Meal suggestions\n" +
                    "• Health tips\n\n" +
                    "Let's make your meals healthier!", isUser: false);

                // Check chat availability through controller
                if (!_controller.IsChatAvailable)
                {
                    BtnSendMessage.Enabled = false;
                    TextBoxChatInput.Enabled = false;
                    AppendChatBubble(_controller.GetChatUnavailableMessage(), isUser: false);
                }

                // initialize controller and load meals
                await _controller.InitializeAsync();
                UpdateMealLists();

                SetActiveNavButton(BtnNavDashboard);
                ShowDashboardPanel();

                UpdateStatus($"Loaded {_controller.GetAllMeals().Count} meals");
            }
            catch (Exception ex)
            {
                ShowError("Error Loading Form", ex.Message);
            }
        }

        /// get updated meal lists from controller
        private void UpdateMealLists()
        {
            var filteredMeals = _controller.GetFilteredMeals();
            UpdateMealList(filteredMeals);
            UpdateDashboard();
        }

        /// update meal by filter
        private void UpdateMealList(List<Meal> meals)
        {
        }
        private void UpdateDashboard()
        {
            var dashboardData = _controller.GetDashboardData();
        }

        private void ShowDashboardPanel()
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(15);
            _currentContentPanel.AutoScroll = true;

            // mainc container panel
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Add filter controls at the top
            var filterPanel = CreateMacroFilterPanel();
            mainContainer.Controls.Add(filterPanel);

            // get filtered results
            var filteredMeals = GetFilteredMeals();
            var totalMeals = filteredMeals.Count;
            var totalCalories = filteredMeals.Sum(m => m.Nutritionals?.Calories ?? 0);
            var totalProtein = filteredMeals.Sum(m => m.Nutritionals?.Protein ?? 0);
            var totalCarbs = filteredMeals.Sum(m => m.Nutritionals?.Carbohydrates ?? 0);
            var totalFat = filteredMeals.Sum(m => m.Nutritionals?.Fat ?? 0);
            var totalSodium = filteredMeals.Sum(m => m.Nutritionals?.Sodium ?? 0);
            var totalSugar = filteredMeals.Sum(m => m.Nutritionals?.Sugar ?? 0);
            var totalSaturatedFat = filteredMeals.Sum(m => m.Nutritionals?.SaturatedFat ?? 0);

            // Create main container with improved 3-column grid
            var mainStatsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                RowCount = 3,
                AutoSize = false,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Height = 700,
                Margin = new Padding(0, 20, 0, 0)
            };

            // Set equal column widths (33% each)
            for (int i = 0; i < 3; i++)
            {
                mainStatsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            }

            // Set row heights - balanced distribution
            mainStatsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35));  // Row 0 - Main stats
            mainStatsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35));  // Row 1 - Macros visualization
            mainStatsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));  // Row 2 - Secondary metrics

            // **Row 0: Key metrics** (Meals, Calories, Protein)
            var mealsCard = CreateStatCard("Total Meals", totalMeals.ToString(), Color.FromArgb(52, 168, 83), "", size: StatCardSize.Large);
            var caloriesCard = CreateStatCard("Total Calories", $"{totalCalories:F0}", Color.FromArgb(244, 120, 80), "kcal", hasProgress: true, totalCalories: (double)totalCalories, size: StatCardSize.Large);
            var proteinCard = CreateStatCard("Total Protein", $"{totalProtein:F1}", Color.FromArgb(51, 150, 243), "g", size: StatCardSize.Large);

            mainStatsPanel.Controls.Add(mealsCard, 0, 0);
            mainStatsPanel.Controls.Add(caloriesCard, 1, 0);
            mainStatsPanel.Controls.Add(proteinCard, 2, 0);

            // **Row 1: Macronutrient breakdown with stacked progress visualization**
            var macroCard = CreateMacroBreakdownCard((double)totalCarbs, (double)totalFat, (double)totalProtein);
            mainStatsPanel.Controls.Add(macroCard, 0, 1);
            mainStatsPanel.SetColumnSpan(macroCard, 3);

            // **Row 2: Secondary metrics (Carbs, Fat, Sodium)**
            var carbsCard = CreateStatCard("Carbohydrates", $"{totalCarbs:F1}", Color.FromArgb(255, 152, 0), "g", size: StatCardSize.Medium);
            var fatCard = CreateStatCard("Total Fat", $"{totalFat:F1}", Color.FromArgb(156, 39, 176), "g", size: StatCardSize.Medium);
            var sodiumCard = CreateStatCard("Sodium", $"{totalSodium:F0}", Color.FromArgb(76, 175, 80), "mg", size: StatCardSize.Medium);

            mainStatsPanel.Controls.Add(carbsCard, 0, 2);
            mainStatsPanel.Controls.Add(fatCard, 1, 2);
            mainStatsPanel.Controls.Add(sodiumCard, 2, 2);

            mainContainer.Controls.Add(mainStatsPanel);
            _currentContentPanel.Controls.Add(mainContainer);
        }

        /// macro filters
        private Panel CreateMacroFilterPanel()
        {
            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = "Filter Macros By:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(15, 12),
                AutoSize = true
            };

            // buttons
            var radioDailyPanel = new Panel { Location = new Point(150, 10), Size = new Size(130, 25) };
            var radioDailyBtn = new RadioButton
            {
                Text = "Daily",
                Checked = _currentFilterType == "Daily",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            radioDailyBtn.CheckedChanged += (s, e) =>
            {
                if (radioDailyBtn.Checked)
                {
                    _currentFilterType = "Daily";
                    _selectedFilterDate = DateTime.Today;
                    ShowDashboardPanel();
                }
            };
            radioDailyPanel.Controls.Add(radioDailyBtn);

            var radioWeeklyPanel = new Panel { Location = new Point(280, 10), Size = new Size(130, 25) };
            var radioWeeklyBtn = new RadioButton
            {
                Text = "Weekly",
                Checked = _currentFilterType == "Weekly",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            radioWeeklyBtn.CheckedChanged += (s, e) =>
            {
                if (radioWeeklyBtn.Checked)
                {
                    _currentFilterType = "Weekly";
                    ShowDashboardPanel();
                }
            };
            radioWeeklyPanel.Controls.Add(radioWeeklyBtn);

            var radioMonthlyPanel = new Panel { Location = new Point(410, 10), Size = new Size(130, 25) };
            var radioMonthlyBtn = new RadioButton
            {
                Text = "Monthly",
                Checked = _currentFilterType == "Monthly",
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            radioMonthlyBtn.CheckedChanged += (s, e) =>
            {
                if (radioMonthlyBtn.Checked)
                {
                    _currentFilterType = "Monthly";
                    ShowDashboardPanel();
                }
            };
            radioMonthlyPanel.Controls.Add(radioMonthlyBtn);

            // date nav
            var dateLabel = new Label
            {
                Text = "Date:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(66, 66, 66),
                Location = new Point(15, 45),
                AutoSize = true
            };

            var datePickerControl = new DateTimePicker
            {
                Value = _selectedFilterDate,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(60, 43),
                Width = 120
            };
            datePickerControl.ValueChanged += (s, e) =>
            {
                _selectedFilterDate = datePickerControl.Value;
                ShowDashboardPanel();
            };

            // prev/next buttons
            var btnPrevious = new Button
            {
                Text = "← Previous",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.FromArgb(33, 33, 33),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(190, 43),
                Width = 80,
                Height = 28
            };
            btnPrevious.FlatAppearance.BorderSize = 1;
            btnPrevious.Click += (s, e) =>
            {
                if (_currentFilterType == "Daily")
                    _selectedFilterDate = _selectedFilterDate.AddDays(-1);
                else if (_currentFilterType == "Weekly")
                    _selectedFilterDate = _selectedFilterDate.AddDays(-7);
                else
                    _selectedFilterDate = _selectedFilterDate.AddMonths(-1);

                datePickerControl.Value = _selectedFilterDate;
                ShowDashboardPanel();
            };

            var btnNext = new Button
            {
                Text = "Next →",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.FromArgb(33, 33, 33),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(275, 43),
                Width = 80,
                Height = 28
            };
            btnNext.FlatAppearance.BorderSize = 1;
            btnNext.Click += (s, e) =>
            {
                if (_currentFilterType == "Daily")
                    _selectedFilterDate = _selectedFilterDate.AddDays(1);
                else if (_currentFilterType == "Weekly")
                    _selectedFilterDate = _selectedFilterDate.AddDays(7);
                else
                    _selectedFilterDate = _selectedFilterDate.AddMonths(1);

                datePickerControl.Value = _selectedFilterDate;
                ShowDashboardPanel();
            };

            var btnToday = new Button
            {
                Text = "Today",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(52, 168, 83),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(360, 43),
                Width = 70,
                Height = 28
            };
            btnToday.FlatAppearance.BorderSize = 0;
            btnToday.Click += (s, e) =>
            {
                _selectedFilterDate = DateTime.Today;
                datePickerControl.Value = _selectedFilterDate;
                ShowDashboardPanel();
            };

            filterPanel.Controls.Add(label);
            filterPanel.Controls.Add(radioDailyPanel);
            filterPanel.Controls.Add(radioWeeklyPanel);
            filterPanel.Controls.Add(radioMonthlyPanel);
            filterPanel.Controls.Add(dateLabel);
            filterPanel.Controls.Add(datePickerControl);
            filterPanel.Controls.Add(btnPrevious);
            filterPanel.Controls.Add(btnNext);
            filterPanel.Controls.Add(btnToday);

            return filterPanel;
        }

        /// filters
        private List<Meal> GetFilteredMeals()
        {
            // Get filter type enum
            var filterType = _currentFilterType switch
            {
                "Weekly" => MealFilterParameters.FilterType.Weekly,
                "Monthly" => MealFilterParameters.FilterType.Monthly,
                _ => MealFilterParameters.FilterType.Daily
            };

            // Use controller to filter meals
            _controller.SetFilterParameters(filterType, _selectedFilterDate);
            return _controller.GetFilteredMeals();
        }

        private enum StatCardSize
        {
            Small,
            Medium,
            Large
        }

        private void ShowCalorieGoalDialog()
        {
            var form = new Form
            {
                Text = "Set Daily Calorie Goal",
                Width = 350,
                Height = 200,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(243, 244, 246)
            };

            var labelGoal = new Label
            {
                Text = "Daily Calorie Goal (kcal):",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(20, 30, 20, 10),
                Location = new Point(20, 30)
            };

            var inputGoal = new TextBox
            {
                Text = _dailyCalorieGoal.ToString("F0"),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 33, 33),
                BorderStyle = BorderStyle.FixedSingle,
                Width = 150,
                Height = 32,
                Location = new Point(20, 60)
            };

            var btnSave = new Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 168, 83),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Location = new Point(20, 110)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 145, 71);
            btnSave.Click += (s, e) =>
            {
                if (double.TryParse(inputGoal.Text, out var goal) && goal > 0)
                {
                    _dailyCalorieGoal = goal;
                    SaveCalorieGoal(goal);
                    UpdateStatus($"Daily calorie goal set to {goal:F0} kcal");
                    form.Close();
                    ShowDashboardPanel(); // Refresh dashboard with new goal
                }
                else
                {
                    ShowError("Invalid Input", "Please enter a valid number greater than 0");
                }
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.FromArgb(33, 33, 33),
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Location = new Point(130, 110)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => form.Close();

            form.Controls.Add(labelGoal);
            form.Controls.Add(inputGoal);
            form.Controls.Add(btnSave);
            form.Controls.Add(btnCancel);

            form.ShowDialog(this);
        }

        private void SaveCalorieGoal(double goal)
        {
            try
            {
                var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
                File.WriteAllText(settingsPath, $"DailyCalorieGoal={goal:F0}");
            }
            catch (Exception ex)
            {
                ShowError("Save Error", $"Could not save settings: {ex.Message}");
            }
        }

        private void LoadCalorieGoal()
        {
            try
            {
                var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
                if (File.Exists(settingsPath))
                {
                    var content = File.ReadAllText(settingsPath);
                    if (content.StartsWith("DailyCalorieGoal="))
                    {
                        var goalStr = content.Replace("DailyCalorieGoal=", "");
                        if (double.TryParse(goalStr, out var goal) && goal > 0)
                        {
                            _dailyCalorieGoal = goal;
                        }
                    }
                }
            }
            catch
            {
                // Use default if loading fails
            }
        }

        private void CalculateBMI()
        {
            if (double.TryParse(TxtBmiHeight.Text, out var height) && 
                double.TryParse(TxtBmiWeight.Text, out var weight) && 
                height > 0 && weight > 0)
            {
                // BMI = weight (kg) / (height (m))^2
                var heightInMeters = height / 100.0;
                var bmi = weight / (heightInMeters * heightInMeters);
                LblBmiResult.Text = $"BMI: {bmi:F1}";

                // Change color and category based on BMI range
                if (bmi < 18.5)
                {
                    LblBmiResult.ForeColor = Color.FromArgb(51, 150, 243); // Blue - Underweight
                    LblBmiCategory.Text = "Category: Underweight";
                    LblBmiCategory.ForeColor = Color.FromArgb(51, 150, 243);
                }
                else if (bmi < 25)
                {
                    LblBmiResult.ForeColor = Color.FromArgb(52, 168, 83); // Green - Normal
                    LblBmiCategory.Text = "Category: Normal Weight";
                    LblBmiCategory.ForeColor = Color.FromArgb(52, 168, 83);
                }
                else if (bmi < 30)
                {
                    LblBmiResult.ForeColor = Color.FromArgb(244, 120, 80); // Orange - Overweight
                    LblBmiCategory.Text = "Category: Overweight";
                    LblBmiCategory.ForeColor = Color.FromArgb(244, 120, 80);
                }
                else
                {
                    LblBmiResult.ForeColor = Color.FromArgb(244, 67, 54); // Red - Obese
                    LblBmiCategory.Text = "Category: Obese";
                    LblBmiCategory.ForeColor = Color.FromArgb(244, 67, 54);
                }
            }
            else
            {
                LblBmiResult.Text = "BMI: -";
                LblBmiCategory.Text = "Category: -";
                LblBmiResult.ForeColor = Color.FromArgb(52, 168, 83);
                LblBmiCategory.ForeColor = Color.FromArgb(100, 100, 100);
            }
        }

        private Panel CreateStatCard(string label, string value, Color accentColor, string unit = "", bool hasProgress = false, double totalCalories = 0, StatCardSize size = StatCardSize.Medium)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(15)
            };

            // Add rounded corners and border effect
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillRoundedRectangle(brush, 0, 0, card.Width, card.Height, 12);
                    e.Graphics.DrawRoundedRectangle(pen, 0, 0, card.Width - 1, card.Height - 1, 12);
                }
            };

            // Accent bar at top
            var accentBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = accentColor
            };

            // Set font sizes based on card size
            int labelFontSize = size switch
            {
                StatCardSize.Large => 13,
                StatCardSize.Medium => 11,
                StatCardSize.Small => 10,
                _ => 11
            };

            int valueFontSize = size switch
            {
                StatCardSize.Large => 48,
                StatCardSize.Medium => 36,
                StatCardSize.Small => 28,
                _ => 36
            };

            // Label
            var labelControl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", labelFontSize),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Margin = new Padding(15, 15, 15, 0),
                Dock = DockStyle.Top
            };

            // Value (large and bold)
            var valueControl = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", valueFontSize, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Margin = new Padding(15, 10, 15, 0),
                Dock = DockStyle.Top
            };

            // Unit
            var unitControl = new Label
            {
                Text = unit,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Margin = new Padding(15, 5, 15, 0),
                Dock = DockStyle.Top
            };

            card.Controls.Add(accentBar);

            if (hasProgress)
            {
                // Daily goal progress bar - using the user's set goal
                var dailyGoal = _dailyCalorieGoal;
                var progressPercent = Math.Min(totalCalories / dailyGoal * 100, 100);

                var progressLabel = new Label
                {
                    Text = $"Daily Goal: {totalCalories:F0} / {dailyGoal:F0}",
                    Font = new Font("Segoe UI", 8),
                    ForeColor = Color.FromArgb(120, 120, 120),
                    AutoSize = true,
                    Margin = new Padding(15, 8, 15, 2),
                    Dock = DockStyle.Top
                };

                var progressBar = new ProgressBar
                {
                    Value = (int)progressPercent,
                    Maximum = 100,
                    Dock = DockStyle.Top,
                    Height = 6,
                    Margin = new Padding(15, 0, 15, 8)
                };
                progressBar.ForeColor = accentColor;

                card.Controls.Add(progressLabel);
                card.Controls.Add(progressBar);
            }

            card.Controls.Add(unitControl);
            card.Controls.Add(valueControl);
            card.Controls.Add(labelControl);

            return card;
        }

        private Panel CreateMacroBreakdownCard(double carbs, double fat, double protein)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(15)
            };

            // Add rounded corners and border
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillRoundedRectangle(brush, 0, 0, card.Width, card.Height, 12);
                    e.Graphics.DrawRoundedRectangle(pen, 0, 0, card.Width - 1, card.Height - 1, 12);
                }
            };

            // Accent bar at top
            var accentBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = Color.FromArgb(76, 175, 80)
            };
            card.Controls.Add(accentBar);

            // Title
            var titleLabel = new Label
            {
                Text = "Macronutrient Breakdown",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 71, 55),
                AutoSize = true,
                Margin = new Padding(15, 15, 0, 12),
                Dock = DockStyle.Top
            };
            card.Controls.Add(titleLabel);

            // Calculate total macros
            var totalMacros = carbs + fat + protein;
            var carbsPercent = totalMacros > 0 ? (carbs / totalMacros) * 100 : 0;
            var fatPercent = totalMacros > 0 ? (fat / totalMacros) * 100 : 0;
            var proteinPercent = totalMacros > 0 ? (protein / totalMacros) * 100 : 0;

            // Stacked progress bar container
            var progressContainer = new Panel
            {
                Height = 35,
                Dock = DockStyle.Top,
                Margin = new Padding(15, 5, 15, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Use a custom paint to draw stacked bars
            progressContainer.Paint += (s, e) =>
            {
                var width = progressContainer.Width - 2;
                var height = progressContainer.Height - 2;
                var x = 1;

                // Carbs bar (orange)
                var carbsWidth = (int)(width * carbsPercent / 100);
                if (carbsWidth > 0)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(255, 152, 0)))
                    {
                        e.Graphics.FillRectangle(brush, x, 1, carbsWidth, height);
                    }
                    x += carbsWidth;
                }

                // Fat bar (purple)
                var fatWidth = (int)(width * fatPercent / 100);
                if (fatWidth > 0)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(156, 39, 176)))
                    {
                        e.Graphics.FillRectangle(brush, x, 1, fatWidth, height);
                    }
                    x += fatWidth;
                }

                // Protein bar (blue)
                var proteinWidth = width - (carbsWidth + fatWidth);
                if (proteinWidth > 0)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(51, 150, 243)))
                    {
                        e.Graphics.FillRectangle(brush, x, 1, proteinWidth, height);
                    }
                }
            };

            card.Controls.Add(progressContainer);

            // Legend/Details panel
            var legendPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Margin = new Padding(15, 0, 15, 15),
                BackColor = Color.Transparent
            };

            // Carbs
            var carbsLegend = CreateMacroLegendItem("     Carbs", $"{carbs:F1}g", Color.FromArgb(255, 152, 0), carbsPercent);
            carbsLegend.Margin = new Padding(0, 0, 15, 5);
            legendPanel.Controls.Add(carbsLegend);

            // Fat
            var fatLegend = CreateMacroLegendItem("     Fat", $"{fat:F1}g", Color.FromArgb(156, 39, 176), fatPercent);
            fatLegend.Margin = new Padding(0, 0, 15, 5);
            legendPanel.Controls.Add(fatLegend);

            // Protein
            var proteinLegend = CreateMacroLegendItem("     Protein", $"{protein:F1}g", Color.FromArgb(51, 150, 243), proteinPercent);
            proteinLegend.Margin = new Padding(0, 0, 0, 5);
            legendPanel.Controls.Add(proteinLegend);

            card.Controls.Add(legendPanel);

            return card;
        }

        private Panel CreateMacroLegendItem(string label, string value, Color color, double percent)
        {
            var panel = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Color indicator
            var colorBox = new Panel
            {
                Width = 14,
                Height = 14,
                BackColor = color,
                Margin = new Padding(0, 2, 6, 0),
                Dock = DockStyle.Left
            };
            panel.Controls.Add(colorBox);

            // Text
            var textLabel = new Label
            {
                Text = $"{label} {value} ({percent:F0}%)",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(textLabel);

            return panel;
        }

        private void ShowMyMealsPanel()
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(20);
            _currentContentPanel.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "My Meals",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Dock = DockStyle.Top
            };

            var searchPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 15),
                WrapContents = false
            };

            var searchLabel = new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 0)
            };

            var searchBox = new TextBox
            {
                Width = 200,
                Height = 28,
                PlaceholderText = "Search meals...",
                Margin = new Padding(0, 0, 10, 0)
            };

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchBox);

            var mealsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };

            mealsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            mealsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 33, 33);
            mealsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            mealsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            mealsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Meal Name", Width = 150 });
            mealsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", Width = 80 });
            mealsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", Width = 100 });
            mealsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Calories", HeaderText = "Calories", Width = 80 });
            mealsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Protein", HeaderText = "Protein(g)", Width = 80 });
            mealsGrid.Columns.Add(new DataGridViewLinkColumn { Name = "ViewRecipe", HeaderText = "View Recipe", Width = 80, Text = "View" });
            mealsGrid.Columns.Add(new DataGridViewLinkColumn { Name = "EditMeal", HeaderText = "Edit", Width = 50, Text = "Edit" });
            mealsGrid.Columns.Add(new DataGridViewLinkColumn { Name = "DeleteMeal", HeaderText = "Delete", Width = 60, Text = "Delete" });

            var allMeals = _controller.GetAllMeals();
            foreach (var meal in allMeals)
            {
                mealsGrid.Rows.Add(
                    meal.Name,
                    meal.Type,
                    meal.MealDate.ToString("MMM dd, yyyy"),
                    meal.Nutritionals?.Calories.ToString("F0") ?? "N/A",
                    meal.Nutritionals?.Protein.ToString("F1") ?? "N/A",
                    "View",
                    "Edit",
                    "Delete"
                );
            }

            mealsGrid.CellContentClick += async (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var meal = allMeals[e.RowIndex];

                    if (e.ColumnIndex == 5) // View Recipe
                    {
                        ShowRecipeDetailsPanel(meal);
                    }
                    else if (e.ColumnIndex == 6) // Edit
                    {
                        ShowEditMealPanel(meal, mealsGrid);
                    }
                    else if (e.ColumnIndex == 7) // Delete
                    {
                        using var confirm = new DeleteConfirmationForm("Are you sure you want to delete this meal?");
                        var dr = confirm.ShowDialog(this);
                        if (dr == DialogResult.OK)
                        {
                            try
                            {
                                await _controller.DeleteMealAsync(meal.Id.ToString());
                                ShowMyMealsPanel();
                                UpdateStatus($"✓ Meal '{meal.Name}' deleted successfully");
                            }
                            catch (Exception ex)
                            {
                                ShowError("Delete Error", ex.Message);
                            }
                        }
                    }
                }
            };

            // search function
            searchBox.TextChanged += (s, e) =>
            {
                string searchTerm = searchBox.Text.ToLower();

                foreach (DataGridViewRow row in mealsGrid.Rows)
                {
                    if (row.Cells["Name"].Value != null)
                    {
                        string mealName = row.Cells["Name"].Value.ToString()!.ToLower();
                        string mealType = row.Cells["Type"].Value?.ToString()?.ToLower() ?? "";

                        bool matches = mealName.Contains(searchTerm) || mealType.Contains(searchTerm);
                        row.Visible = matches;
                    }
                }
            };

            _currentContentPanel.Controls.Add(mealsGrid);
            _currentContentPanel.Controls.Add(searchPanel);
            _currentContentPanel.Controls.Add(titleLabel);
        }

        private void ShowRecipeDetailsPanel(Meal meal)
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(20);
            _currentContentPanel.AutoScroll = true;

            var titleLabel = new Label
            {
                Text = $"Recipe: {meal.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Dock = DockStyle.Top
            };

            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };

            var mealTypeLabel = new Label
            {
                Text = $"Type: {meal.Type}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            };

            var dateLabel = new Label
            {
                Text = $"Date: {meal.MealDate:MMM dd, yyyy}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 15)
            };

            flowPanel.Controls.Add(mealTypeLabel);
            flowPanel.Controls.Add(dateLabel);

            var ingredientsLabel = new Label
            {
                Text = "Ingredients:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 8)
            };

            flowPanel.Controls.Add(ingredientsLabel);

            if (meal.Recipes != null)
            {
                foreach (var recipe in meal.Recipes)
                {
                    var recipeLabel = new Label
                    {
                        Text = $"🔸 {recipe.Name}",
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = Color.FromArgb(76, 175, 80),
                        AutoSize = true,
                        Margin = new Padding(0, 5, 0, 5)
                    };
                    flowPanel.Controls.Add(recipeLabel);

                    foreach (var ingredient in recipe.Ingredients)
                    {
                        var ingredientLabel = new Label
                        {
                            Text = $"   • {ingredient.Quantity} {ingredient.Unit} {ingredient.Name}",
                            Font = new Font("Segoe UI", 10),
                            ForeColor = Color.FromArgb(100, 100, 100),
                            AutoSize = true,
                            Margin = new Padding(0, 3, 0, 3)
                        };
                        flowPanel.Controls.Add(ingredientLabel);
                    }
                }
            }

            var nutritionLabel = new Label
            {
                Text = "Nutrition Info:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 10)
            };
            flowPanel.Controls.Add(nutritionLabel);

            if (meal.Nutritionals != null)
            {
                var caloriesLabel = new Label
                {
                    Text = $"Calories: {meal.Nutritionals.Calories:F0} kcal",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 5)
                };

                var proteinLabel = new Label
                {
                    Text = $"Protein: {meal.Nutritionals.Protein:F1} g",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 5)
                };

                var carbsLabel = new Label
                {
                    Text = $"Carbohydrates: {meal.Nutritionals.Carbohydrates:F1} g",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 5)
                };

                var fatLabel = new Label
                {
                    Text = $"Fat: {meal.Nutritionals.Fat:F1} g",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 5)
                };

                flowPanel.Controls.Add(caloriesLabel);
                flowPanel.Controls.Add(proteinLabel);
                flowPanel.Controls.Add(carbsLabel);
                flowPanel.Controls.Add(fatLabel);
            }

            _currentContentPanel.Controls.Add(flowPanel);
            _currentContentPanel.Controls.Add(titleLabel);
        }

        private void ShowEditMealPanel(Meal meal, DataGridView mealsGrid)
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(20);
            _currentContentPanel.AutoScroll = true;

            var titleLabel = new Label
            {
                Text = $"Edit Meal: {meal.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Dock = DockStyle.Top
            };

            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };

            var mealNameLabel = new Label { Text = "Meal Name:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 10, 0, 3) };
            var mealNameBox = new TextBox { Text = meal.Name, Width = 300, Height = 28, Margin = new Padding(0, 0, 0, 15) };

            var mealTypeLabel = new Label { Text = "Meal Type:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 3) };
            var mealTypeCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300,
                Height = 28,
                Margin = new Padding(0, 0, 0, 15)
            };
            mealTypeCombo.Items.AddRange(new[] { "Breakfast", "Brunch", "Lunch", "Snack", "Dinner", "Supper" });
            mealTypeCombo.SelectedIndex = (int)meal.Type;

            var mealDateLabel = new Label { Text = "Date:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 3) };
            var mealDatePicker = new DateTimePicker { Value = meal.MealDate, Width = 300, Height = 28, Format = DateTimePickerFormat.Short, Margin = new Padding(0, 0, 0, 15) };

            var recipesLabel = new Label { Text = "Recipes & Ingredients:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 10, 0, 3) };

            // build recipes text for editing
            var recipesText = new StringBuilder();
            if (meal.Recipes != null)
            {
                foreach (var recipe in meal.Recipes)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        recipesText.AppendLine($"{ingredient.Quantity} {ingredient.Unit} {ingredient.Name}");
                    }
                }
            }

            var recipesBox = new TextBox
            {
                Text = recipesText.ToString(),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 400,
                Height = 150,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(33, 33, 33),
                Margin = new Padding(0, 0, 0, 15)
            };

            var buttonsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 15, 0, 0)
            };

            var saveBtn = new Button
            {
                Text = "Save Changes",
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Width = 120,
                Height = 32,
                Margin = new Padding(0, 0, 10, 0)
            };
            saveBtn.FlatAppearance.BorderSize = 0;

            var cancelBtn = new Button
            {
                Text = "Cancel",
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Width = 100,
                Height = 32,
                Margin = new Padding(0)
            };
            cancelBtn.FlatAppearance.BorderSize = 0;

            saveBtn.Click += async (s, e) =>
            {
                try
                {
                    await _controller.UpdateMealAsync(meal, mealNameBox.Text, (MealType)mealTypeCombo.SelectedIndex, mealDatePicker.Value, recipesBox.Text);
                    ShowMyMealsPanel();
                    UpdateStatus($"✓ Meal '{meal.Name}' updated successfully");
                }
                catch (Exception ex)
                {
                    ShowError("Update Error", ex.Message);
                }
            };

            cancelBtn.Click += (s, e) => ShowMyMealsPanel();

            buttonsPanel.Controls.Add(saveBtn);
            buttonsPanel.Controls.Add(cancelBtn);

            flowPanel.Controls.Add(mealNameLabel);
            flowPanel.Controls.Add(mealNameBox);
            flowPanel.Controls.Add(mealTypeLabel);
            flowPanel.Controls.Add(mealTypeCombo);
            flowPanel.Controls.Add(mealDateLabel);
            flowPanel.Controls.Add(mealDatePicker);
            flowPanel.Controls.Add(recipesLabel);
            flowPanel.Controls.Add(recipesBox);
            flowPanel.Controls.Add(buttonsPanel);

            _currentContentPanel.Controls.Add(flowPanel);
            _currentContentPanel.Controls.Add(titleLabel);
        }

        private void ShowDailyLogPanel()
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(20);
            _currentContentPanel.AutoScroll = true;

            var titleLabel = new Label
            {
                Text = "Daily Meal Log",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5),
                Dock = DockStyle.Top
            };

            var dateLabel = new Label
            {
                Text = DateTime.Today.ToString("dddd, MMMM dd, yyyy"),
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 25),
                Dock = DockStyle.Top
            };

            var allMeals = _controller.GetAllMeals();
            var mealsForToday = allMeals.Where(m => m.MealDate.Date == DateTime.Today).ToList();
            var totalCaloriesToday = mealsForToday.Sum(m => m.Nutritionals?.Calories ?? 0);
            var totalProteinToday = mealsForToday.Sum(m => m.Nutritionals?.Protein ?? 0);
            var totalCarbsToday = mealsForToday.Sum(m => m.Nutritionals?.Carbohydrates ?? 0);
            var totalFatToday = mealsForToday.Sum(m => m.Nutritionals?.Fat ?? 0);
            var totalSodiumToday = mealsForToday.Sum(m => m.Nutritionals?.Sodium ?? 0);
            var totalSugarToday = mealsForToday.Sum(m => m.Nutritionals?.Sugar ?? 0);
            var totalSaturatedFatToday = mealsForToday.Sum(m => m.Nutritionals?.SaturatedFat ?? 0);

            // metrics Section
            var metricsHeaderLabel = new Label
            {
                Text = "Daily Totals",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 12),
                Dock = DockStyle.Top
            };

            var statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 25),
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(12, 12, 12, 12),
                BorderStyle = BorderStyle.FixedSingle
            };

            // text-based metric labels
            var caloriesLabel = new Label
            {
                Text = $"• Total Calories: {totalCaloriesToday:F0} kcal",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var proteinLabel = new Label
            {
                Text = $"• Total Protein: {totalProteinToday:F1} g",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var carbsLabel = new Label
            {
                Text = $"• Total Carbohydrates: {totalCarbsToday:F1} g",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var fatLabel = new Label
            {
                Text = $"• Total Fat: {totalFatToday:F1} g",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var sodiumLabel = new Label
            {
                Text = $"• Total Sodium: {totalSodiumToday:F0} mg",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var sugarLabel = new Label
            {
                Text = $"• Total Sugar: {totalSugarToday:F1} g",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            var saturatedFatLabel = new Label
            {
                Text = $"• Total Saturated Fat: {totalSaturatedFatToday:F1} g",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4)
            };

            statsPanel.Controls.Add(caloriesLabel);
            statsPanel.Controls.Add(proteinLabel);
            statsPanel.Controls.Add(carbsLabel);
            statsPanel.Controls.Add(fatLabel);
            statsPanel.Controls.Add(sodiumLabel);
            statsPanel.Controls.Add(sugarLabel);
            statsPanel.Controls.Add(saturatedFatLabel);

            var mealsLabel = new Label
            {
                Text = "Today's Meals",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 12),
                Dock = DockStyle.Top
            };

            var mealsFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };

            if (mealsForToday.Count == 0)
            {
                var noMealsLabel = new Label
                {
                    Text = "No meals recorded for today",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    AutoSize = true,
                    Margin = new Padding(0, 15, 0, 0)
                };
                mealsFlowPanel.Controls.Add(noMealsLabel);
            }
            else
            {
                foreach (var meal in mealsForToday)
                {
                    var mealCard = new Panel
                    {
                        Width = 700,
                        AutoSize = true,
                        BackColor = Color.FromArgb(250, 250, 250),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 12),
                        Padding = new Padding(15, 12, 15, 12)
                    };

                    var mealNameLabel = new Label
                    {
                        Text = $"{meal.Name} ({meal.Type})",
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = Color.FromArgb(33, 33, 33),
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, 8)
                    };

                    var mealNutriLabel = new Label
                    {
                        Text = $"{meal.Nutritionals?.Calories:F0} kcal  |  {meal.Nutritionals?.Protein:F1}g protein  |  {meal.Nutritionals?.Carbohydrates:F1}g carbs  |  {meal.Nutritionals?.Fat:F1}g fat",
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, 0)
                    };

                    mealCard.Controls.Add(mealNameLabel);
                    mealCard.Controls.Add(mealNutriLabel);
                    mealsFlowPanel.Controls.Add(mealCard);
                }
            }

            _currentContentPanel.Controls.Add(mealsFlowPanel);
            _currentContentPanel.Controls.Add(mealsLabel);
            _currentContentPanel.Controls.Add(statsPanel);
            _currentContentPanel.Controls.Add(metricsHeaderLabel);
            _currentContentPanel.Controls.Add(dateLabel);
            _currentContentPanel.Controls.Add(titleLabel);
        }

        private void ClearContentPanel()
        {
            ContentPanel.Controls.Clear();
        }

        private void BtnNavDashboard_Click(object? sender, EventArgs e) 
        { 
            SetActiveNavButton(BtnNavDashboard);
            ShowDashboardPanel(); 
        }
        private void BtnNavMyMeals_Click(object? sender, EventArgs e) 
        { 
            SetActiveNavButton(BtnNavMyMeals);
            ShowMyMealsPanel(); 
        }
        private void BtnNavDailyLog_Click(object? sender, EventArgs e) 
        { 
            SetActiveNavButton(BtnNavDailyLog);
            ShowDailyLogPanel(); 
        }

        private void BtnNavBmi_Click(object? sender, EventArgs e)
        {
            SetActiveNavButton(BtnNavBmi);
            ShowBmiCalculatorPanel();
        }

        private void ShowBmiCalculatorPanel()
        {
            ContentInnerPanel.Controls.Clear();
            _currentContentPanel = ContentInnerPanel;
            _currentContentPanel.Padding = new Padding(15);
            _currentContentPanel.AutoScroll = true;

            if (BmiCardPanel.Parent != null)
            {
                BmiCardPanel.Parent.Controls.Remove(BmiCardPanel);
            }

            BmiCardPanel.Dock = DockStyle.Top;
            BmiCardPanel.Margin = new Padding(0);
            _currentContentPanel.Controls.Add(BmiCardPanel);
        }

        private void SetActiveNavButton(Button activeButton)
        {
            // Reset all nav buttons to normal state
            BtnNavDashboard.BackColor = Color.FromArgb(31, 71, 55); // Dark forest green
            BtnNavDashboard.ForeColor = Color.FromArgb(200, 220, 210); // Light text

            BtnNavMyMeals.BackColor = Color.FromArgb(31, 71, 55);
            BtnNavMyMeals.ForeColor = Color.FromArgb(200, 220, 210);

            BtnNavDailyLog.BackColor = Color.FromArgb(31, 71, 55);
            BtnNavDailyLog.ForeColor = Color.FromArgb(200, 220, 210);

            BtnNavBmi.BackColor = Color.FromArgb(31, 71, 55);
            BtnNavBmi.ForeColor = Color.FromArgb(200, 220, 210);

            // Highlight the active button with a lighter background and brighter text
            activeButton.BackColor = Color.FromArgb(52, 104, 86); // Lighter green for active state
            activeButton.ForeColor = Color.White; // Bright white text for active button
        }

        private void BtnClearForm_Click(object? sender, EventArgs e)
        {
            TextBoxMealName.Clear();
            ComboBoxMealType.SelectedIndex = 0;
            DateTimePickerMeal.Value = DateTime.Today;
            TextBoxRecipes.Clear();
            UpdateStatus("Form cleared");
        }

        private async void BtnCreateMeal_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextBoxMealName.Text))
                {
                    ShowError("Validation Error", "Please enter a meal name");
                    return;
                }

                var mealName = TextBoxMealName.Text.Trim();
                var mealType = (MealType)ComboBoxMealType.SelectedIndex;
                var mealDate = DateTimePickerMeal.Value;
                var recipesText = TextBoxRecipes.Text.Trim();

                if (string.IsNullOrEmpty(recipesText))
                {
                    ShowError("Validation Error", "Please enter recipes and ingredients");
                    return;
                }

                UpdateStatus("Analyzing meal nutrition...");
                var created = await _controller.CreateMealAsync(mealName, mealType, mealDate, recipesText);

                UpdateStatus($"✓ Meal '{mealName}' created with {created.Nutritionals?.Calories.ToString("F0") ?? "0"} calories");
                TextBoxMealName.Clear();
                TextBoxRecipes.Clear();
                ComboBoxMealType.SelectedIndex = 0;

                UpdateMealLists();
            }
            catch (Exception ex)
            {
                ShowError("Error Creating Meal", ex.Message);
            }
        }

        private async void BtnSendMessage_Click(object? sender, EventArgs e)
        {
            await SendChatMessageAsync();
        }

        private async void TextBoxChatInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                e.Handled = true;
                await SendChatMessageAsync();
            }
        }

        private async Task SendChatMessageAsync()
        {
            if (!_controller.IsChatAvailable)
            {
                ShowError("Chat Not Available", _controller.GetChatUnavailableMessage());
                return;
            }

            var userMessage = TextBoxChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
                return;

            try
            {
                BtnSendMessage.Enabled = false;
                TextBoxChatInput.ReadOnly = true;

                // Add user message as a chat bubble
                AppendChatBubble(userMessage, isUser: true);
                TextBoxChatInput.Clear();

                UpdateStatus("Analyzing...");

                string? response = null;
                int maxAttempts = 3;
                int delayMs = 800;

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        response = await _controller.SendChatMessageAsync(userMessage);
                        break;
                    }
                    catch (Exception ex) when (attempt < maxAttempts && IsTransientError(ex))
                    {
                        UpdateStatus($"Transient chat error, retrying ({attempt}/{maxAttempts})...");
                        await Task.Delay(delayMs + new Random().Next(0, 200));
                        delayMs *= 2;
                        continue;
                    }
                }

                if (response == null)
                {
                    response = await _controller.SendChatMessageAsync(userMessage);
                }

                AppendChatBubble(response ?? "", isUser: false);
                UpdateStatus("✓ Response received");
            }
            catch (Exception ex)
            {
                var msg = ex.Message ?? "An error occurred while calling the chat service.";
                var errorTitle = _controller.GetChatErrorTitle(ex);
                var errorMsg = _controller.GetChatErrorMessage(ex);
                ShowError(errorTitle, errorMsg);
                UpdateStatus($"✗ Error: {msg}");
            }
            finally
            {
                BtnSendMessage.Enabled = true;
                TextBoxChatInput.ReadOnly = false;
                TextBoxChatInput.Focus();
            }
        }

        private bool IsTransientError(Exception ex)
        {
            if (ex is System.Net.Http.HttpRequestException) return true;
            var m = ex.Message?.ToLowerInvariant() ?? string.Empty;
            return m.Contains("high demand") || m.Contains("rate limit") || m.Contains("429") || m.Contains("503") || m.Contains("timeout");
        }

        private void AppendChatBubble(string message, bool isUser)
        {
            if (isUser)
            {
                // User message bubble - vibrant green with white text
                // Add spacing before bubble
                TextBoxChatHistory.AppendText("\n");

                // Add the label "You" in smaller font and different style
                TextBoxChatHistory.AppendText("You");
                var youLabelStart = TextBoxChatHistory.TextLength - 3;
                TextBoxChatHistory.Select(youLabelStart, 3);
                TextBoxChatHistory.SelectionFont = new Font("Segoe UI", 8, FontStyle.Bold);
                TextBoxChatHistory.SelectionColor = Color.FromArgb(52, 168, 83);

                // New line before bubble content
                TextBoxChatHistory.AppendText("\n");
                var bubbleStart = TextBoxChatHistory.TextLength;

                // Add message with padding effect (spaces at start and end)
                TextBoxChatHistory.AppendText("  " + message + "  ");
                var bubbleEnd = TextBoxChatHistory.TextLength;

                // Format the entire bubble content with green background
                TextBoxChatHistory.Select(bubbleStart, bubbleEnd - bubbleStart);
                TextBoxChatHistory.SelectionColor = Color.White;
                TextBoxChatHistory.SelectionBackColor = Color.FromArgb(52, 168, 83); // Vibrant green
                TextBoxChatHistory.SelectionFont = new Font("Segoe UI", 9);

                // Add spacing after
                TextBoxChatHistory.AppendText("\n");
                TextBoxChatHistory.Select(TextBoxChatHistory.TextLength, 0);
            }
            else
            {
                // AI message bubble - light sage green with dark text
                // Add spacing before bubble
                TextBoxChatHistory.AppendText("\n");

                // Add the label "Coach" in smaller font and different style
                TextBoxChatHistory.AppendText("Coach");
                var coachLabelStart = TextBoxChatHistory.TextLength - 5;
                TextBoxChatHistory.Select(coachLabelStart, 5);
                TextBoxChatHistory.SelectionFont = new Font("Segoe UI", 8, FontStyle.Bold);
                TextBoxChatHistory.SelectionColor = Color.FromArgb(31, 71, 55);

                // New line before bubble content
                TextBoxChatHistory.AppendText("\n");
                var bubbleStart = TextBoxChatHistory.TextLength;

                // Add message with padding effect (spaces at start and end)
                TextBoxChatHistory.AppendText("  " + message + "  ");
                var bubbleEnd = TextBoxChatHistory.TextLength;

                // Format the entire bubble content with light sage background
                TextBoxChatHistory.Select(bubbleStart, bubbleEnd - bubbleStart);
                TextBoxChatHistory.SelectionColor = Color.FromArgb(33, 33, 33);
                TextBoxChatHistory.SelectionBackColor = Color.FromArgb(220, 240, 230); // Light sage green
                TextBoxChatHistory.SelectionFont = new Font("Segoe UI", 9);

                // Add spacing after
                TextBoxChatHistory.AppendText("\n");
                TextBoxChatHistory.Select(TextBoxChatHistory.TextLength, 0);
            }

            TextBoxChatHistory.ScrollToCaret();
        }

        private void UpdateStatus(string message)
        {
            StatusLabel.Text = message;
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateStatus("Error: " + title);
        }

        private void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

// Graphics extension methods for rounded rectangles
namespace System.Drawing
{
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
        {
            using (var path = CreateRoundedRectanglePath(x, y, width, height, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, float x, float y, float width, float height, float radius)
        {
            using (var path = CreateRoundedRectanglePath(x, y, width, height, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath CreateRoundedRectanglePath(float x, float y, float width, float height, float radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            var d = radius * 2;

            path.AddArc(x, y, d, d, 180, 90);
            path.AddArc(x + width - d, y, d, d, 270, 90);
            path.AddArc(x + width - d, y + height - d, d, d, 0, 90);
            path.AddArc(x, y + height - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
