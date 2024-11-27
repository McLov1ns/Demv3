using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demv3
{
    public partial class Form1 : Form
    {
        public static Form1 Instance { get; }= new Form1();
        private SqlConnection connection; 
        public Form1()
        {
            InitializeComponent();
            string connectionString = @"Data Source=MEGAKOMP\MSSQLSERVER06;Initial Catalog=demv3;Integrated Security=True";
            connection = new SqlConnection(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            InitializeHandlers();

        }
        private void InitializeHandlers()
        {
            searchTB.TextChanged += SearchTB_TextChanged;  // Обработчик для TextChanged
            sortBtn.Click += SortBtn_Click;
            sortBtn1.Click += SortBtn1_Click;

            // Заполнение ComboBox колонками
            sortCB.Items.AddRange(new string[] { "ID", "Name", "Brand", "Price" });

            // Заполнение ComboBox для фильтрации (например, по ролям)
            filterCB.Items.AddRange(new string[] { "Все", "Кошки", "Собаки", "Грызуны", "Птицы", "Рыбки","Рептилии" });
            filterCB.SelectedIndexChanged += FilterCB_SelectedIndexChanged;
        }

        private void LoadData(string query = "SELECT * FROM Product$")
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void SortBtn_Click(object sender, EventArgs e)
        {
            string selectedColumn = sortCB.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedColumn))
            {
                string query = $"SELECT * FROM Product$ ORDER BY {selectedColumn} ASC";
                LoadData(query);
            }
            else
            {
                MessageBox.Show("Please select a column to sort.");
            }
        }

        private void SortBtn1_Click(object sender, EventArgs e)
        {
            string selectedColumn = sortCB.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedColumn))
            {
                string query = $"SELECT * FROM Product$ ORDER BY {selectedColumn} DESC";
                LoadData(query);
            }
            else
            {
                MessageBox.Show("Please select a column to sort.");
            }
        }

        // Реализация поиска при изменении текста в поле
        private void SearchTB_TextChanged(object sender, EventArgs e)
        {
            ApplySearchAndFilter();
        }

        // Метод для применения поиска и фильтра
        private void ApplySearchAndFilter()
        {
            string searchText = searchTB.Text.Trim();
            string filterValue = filterCB.SelectedItem?.ToString();

            string query = "SELECT * FROM Product$ WHERE 1 = 1"; // По умолчанию запрос для всех пользователей

            // Если есть текст для поиска, добавляем условие поиска
            if (!string.IsNullOrEmpty(searchText))
            {
                query += $@" AND (
                        ID LIKE '%{searchText}%' OR
                        Name LIKE '%{searchText}%' OR
                        Brand LIKE '%{searchText}%' OR
                        Price LIKE '%{searchText}%'
                    )";
            }

            // Если выбран фильтр (например, роль), добавляем условие фильтрации
            if (!string.IsNullOrEmpty(filterValue) && filterValue != "All")
            {
                query += $" AND Animal = '{filterValue}'";
            }

            LoadData(query);
        }
        private void FilterCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySearchAndFilter(); // Перезапускаем поиск при изменении фильтра
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            EditForm editForm = new EditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // После добавления обновляем данные
                LoadData();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем данные выбранной строки
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(selectedRow.Cells[0].Value);
            string login = selectedRow.Cells[1].Value.ToString();
            string password = selectedRow.Cells[2].Value.ToString();
            string roleId = selectedRow.Cells[3].Value.ToString();

            // Открываем форму для редактирования данных
            EditForm editForm = new EditForm(id, login, password, roleId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // После редактирования обновляем данные
                LoadData();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем данные выбранной строки
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(selectedRow.Cells[0].Value);

            // Подтверждение удаления
            DialogResult dialogResult = MessageBox.Show($"Вы уверены, что хотите удалить пользователя с ID {id}?",
                                                         "Подтверждение удаления",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string connectionString = @"Data Source=MEGAKOMP\MSSQLSERVER06;Initial Catalog=demv3;Integrated Security=True";

                // SQL-запрос для удаления строки
                string query = "DELETE FROM Product$ WHERE ID = @ID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Обновляем данные в DataGridView
                    LoadData();

                    MessageBox.Show("Пользователь успешно удалён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
