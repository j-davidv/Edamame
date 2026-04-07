using Microsoft.Extensions.DependencyInjection;
using Edamam.Application.Services;
using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;
using System.Text;

namespace Edamam
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MealService _mealService;
        private readonly IDailyMealAggregator _dailyAggregator;
        private readonly IGeminiChatService? _chatService;
        private List<Meal> _allMeals = new();
        private Panel _currentContentPanel;

        public Form1(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mealService = _serviceProvider.GetRequiredService<MealService>();
            _dailyAggregator = _serviceProvider.GetRequiredService<IDailyMealAggregator>();
            _chatService = _serviceProvider.GetService<IGeminiChatService>();
            _currentContentPanel = null!;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                BtnCreateMeal.Click += BtnCreateMeal_Click;
                BtnNavDashboard.Click += BtnNavDashboard_Click;
                BtnNavMyMeals.Click += BtnNavMyMeals_Click;
                BtnNavDailyLog.Click += BtnNavDailyLog_Click;
                BtnSendMessage.Click += BtnSendMessage_Click;
                BtnClearChat.Click += BtnClearChat_Click;
                TextBoxChatInput.KeyDown += TextBoxChatInput_KeyDown;

                // Disable chat if service is not available
                if (_chatService == null)
                {
                    BtnSendMessage.Enabled = false;
                    TextBoxChatInput.Enabled = false;
                    TextBoxChatHistory.Text = "Gemini Chat Service not available.\nPlease ensure GOOGLE_API_KEY is set.";
                }

                // Load initial data
                await RefreshMealsAsync();

                // Show dashboard by default
                ShowDashboardPanel();
            }
            catch (Exception ex)
            {
                ShowError("Error Loading Form", ex.Message);
            }
        }

        private async Task RefreshMealsAsync()
        {
            try
            {
                // Get all meals from the repository
                var mealRepository = _serviceProvider.GetRequiredService<IRepository<Meal>>();
                var allMeals = await mealRepository.GetAllAsync();
                _allMeals = allMeals.ToList();
                UpdateStatus($"Loaded {_allMeals.Count} meals");
            }
            catch (Exception ex)
            {
                ShowError("Error Loading Meals", ex.Message);
            }
        }

        private void ShowDashboardPanel()
        {
            ClearContentPanel();
            _currentContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            ContentPanel.Controls.Add(_currentContentPanel);

            var titleLabel = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };

            var summaryLabel = new Label
            {
                Text = $"Total Meals Recorded: {_allMeals.Count}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };

            var totalCalories = _allMeals.Sum(m => m.Nutritionals?.Calories ?? 0);
            var caloriesLabel = new Label
            {
                Text = $"Total Calories: {totalCalories:F0} kcal",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(76, 175, 80),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };

            var totalProtein = _allMeals.Sum(m => m.Nutritionals?.Protein ?? 0);
            var proteinLabel = new Label
            {
                Text = $"Total Protein: {totalProtein:F1} g",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(33, 150, 243),
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };

            flowPanel.Controls.Add(titleLabel);
            flowPanel.Controls.Add(summaryLabel);
            flowPanel.Controls.Add(caloriesLabel);
            flowPanel.Controls.Add(proteinLabel);

            _currentContentPanel.Controls.Add(flowPanel);
        }

        private void ShowMyMealsPanel()
        {
            ClearContentPanel();
            _currentContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            ContentPanel.Controls.Add(_currentContentPanel);

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

            foreach (var meal in _allMeals)
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
                    var meal = _allMeals[e.RowIndex];

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
                        var result = MessageBox.Show($"Are you sure you want to delete '{meal.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                var mealRepository = _serviceProvider.GetRequiredService<IRepository<Meal>>();
                                await mealRepository.DeleteAsync(meal.Id.ToString());
                                await RefreshMealsAsync();
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

            // Search functionality
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
            ClearContentPanel();
            _currentContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
            ContentPanel.Controls.Add(_currentContentPanel);

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
            ClearContentPanel();
            _currentContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
            ContentPanel.Controls.Add(_currentContentPanel);

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

            // Build recipes text for editing
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
                    meal.Name = mealNameBox.Text;
                    meal.Type = (MealType)mealTypeCombo.SelectedIndex;
                    meal.MealDate = mealDatePicker.Value;

                    // Parse updated recipes from text
                    var recipes = new List<Recipe>();
                    var ingredients = new List<Ingredient>();

                    var lines = recipesBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrEmpty(trimmedLine)) continue;

                        var parts = trimmedLine.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 3 && decimal.TryParse(parts[0], out var quantity))
                        {
                            var unit = parts[1];
                            var name = parts[2];
                            ingredients.Add(new Ingredient { Quantity = quantity, Unit = unit, Name = name });
                        }
                        else if (parts.Length >= 2 && decimal.TryParse(parts[0], out var singleQty))
                        {
                            var name = string.Join(" ", parts.Skip(1));
                            ingredients.Add(new Ingredient { Quantity = singleQty, Unit = "serving", Name = name });
                        }
                    }

                    if (ingredients.Count > 0)
                    {
                        var mainRecipe = new Recipe
                        {
                            Name = "Main Recipe",
                            Ingredients = ingredients,
                            CreatedDate = DateTime.UtcNow
                        };
                        recipes.Add(mainRecipe);
                        meal.Recipes = recipes;
                    }

                    var mealRepository = _serviceProvider.GetRequiredService<IRepository<Meal>>();
                    await mealRepository.UpdateAsync(meal.Id.ToString(), meal);
                    await RefreshMealsAsync();
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
            ClearContentPanel();
            _currentContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
            ContentPanel.Controls.Add(_currentContentPanel);

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

            var mealsForToday = _allMeals.Where(m => m.MealDate.Date == DateTime.Today).ToList();
            var totalCaloriesToday = mealsForToday.Sum(m => m.Nutritionals?.Calories ?? 0);
            var totalProteinToday = mealsForToday.Sum(m => m.Nutritionals?.Protein ?? 0);
            var totalCarbsToday = mealsForToday.Sum(m => m.Nutritionals?.Carbohydrates ?? 0);
            var totalFatToday = mealsForToday.Sum(m => m.Nutritionals?.Fat ?? 0);
            var totalSodiumToday = mealsForToday.Sum(m => m.Nutritionals?.Sodium ?? 0);
            var totalSugarToday = mealsForToday.Sum(m => m.Nutritionals?.Sugar ?? 0);
            var totalSaturatedFatToday = mealsForToday.Sum(m => m.Nutritionals?.SaturatedFat ?? 0);

            // Metrics Section
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

            // Create text-based metric labels with improved spacing
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

        private void BtnNavDashboard_Click(object? sender, EventArgs e) => ShowDashboardPanel();
        private void BtnNavMyMeals_Click(object? sender, EventArgs e) => ShowMyMealsPanel();
        private void BtnNavDailyLog_Click(object? sender, EventArgs e) => ShowDailyLogPanel();

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

                var recipes = new List<Recipe>();
                var ingredients = new List<Ingredient>();

                var lines = recipesText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;

                    var parts = trimmedLine.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 3 && decimal.TryParse(parts[0], out var quantity))
                    {
                        var unit = parts[1];
                        var name = parts[2];
                        ingredients.Add(new Ingredient { Quantity = quantity, Unit = unit, Name = name });
                    }
                    else if (parts.Length >= 2 && decimal.TryParse(parts[0], out var singleQty))
                    {
                        var name = string.Join(" ", parts.Skip(1));
                        ingredients.Add(new Ingredient { Quantity = singleQty, Unit = "serving", Name = name });
                    }
                }

                if (ingredients.Count == 0)
                {
                    ShowError("Input Error", "Please enter at least one ingredient, for example: 1 kilogram chicken cut ups");
                    return;
                }

                var mainRecipe = new Recipe
                {
                    Name = "Main Recipe",
                    Ingredients = ingredients,
                    CreatedDate = DateTime.UtcNow
                };
                recipes.Insert(0, mainRecipe);

                var meal = new Meal
                {
                    Name = mealName,
                    Type = mealType,
                    MealDate = mealDate,
                    Recipes = recipes
                };

                UpdateStatus("Analyzing meal nutrition...");
                var created = await _mealService.CreateAndAnalyzeMealAsync(meal);

                UpdateStatus($"✓ Meal '{mealName}' created with {created.Nutritionals?.Calories.ToString("F0") ?? "0"} calories");
                TextBoxMealName.Clear();
                TextBoxRecipes.Clear();
                ComboBoxMealType.SelectedIndex = 0;

                await RefreshMealsAsync();
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
            if (_chatService == null)
            {
                ShowError("Chat Not Available", "Gemini Chat Service is not configured.");
                return;
            }

            var userMessage = TextBoxChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
                return;

            try
            {
                BtnSendMessage.Enabled = false;
                TextBoxChatInput.ReadOnly = true;

                TextBoxChatHistory.AppendText($"You: {userMessage}\n\n");
                TextBoxChatHistory.ScrollToCaret();
                TextBoxChatInput.Clear();

                UpdateStatus("Analyzing...");

                var response = await _chatService.ChatAsync(userMessage);

                TextBoxChatHistory.AppendText($"Coach: {response}\n\n");
                TextBoxChatHistory.ScrollToCaret();

                UpdateStatus("✓ Response received");
            }
            catch (Exception ex)
            {
                ShowError("Chat Error", ex.Message);
                UpdateStatus($"✗ Error: {ex.Message}");
            }
            finally
            {
                BtnSendMessage.Enabled = true;
                TextBoxChatInput.ReadOnly = false;
                TextBoxChatInput.Focus();
            }
        }

        private async void BtnClearChat_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_chatService != null)
                {
                    await _chatService.ClearHistoryAsync();
                }
                TextBoxChatHistory.Clear();
                UpdateStatus("Chat history cleared");
            }
            catch (Exception ex)
            {
                ShowError("Clear Chat Error", ex.Message);
            }
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
