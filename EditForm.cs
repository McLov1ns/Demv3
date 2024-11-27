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
    public partial class EditForm : Form
    {
        private int? userId; // ID пользователя (null для добавления)
        public EditForm()
        {
            InitializeComponent();
            LoadRoles();
        }

        public EditForm(int id, string login, string password, string role) : this()
        {
            userId = id; // Сохраняем ID для редактирования
            textBoxLogin.Text = login;
            textBoxPassword.Text = password;
            comboBoxRole.SelectedItem = role;
        }

        private void LoadRoles()
        {
            // Предполагаем, что роли фиксированы
            comboBoxRole.Items.Add("Кошка");
            comboBoxRole.Items.Add("Собака");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка ввода
            if (string.IsNullOrWhiteSpace(textBoxLogin.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text) || comboBoxRole.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //switch (comboBoxRole.SelectedItem.ToString())
            //{
            //    case "Admin":
            //        roleId = 3;
            //        break;
            //    case "User":
            //        roleId = 1;
            //        break;
            //    case "Guest":
            //        roleId = 0;
            //        break;
            //    default:
            //        MessageBox.Show("Некорректная роль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //}

            string connectionString = @"Data Source=MEGAKOMP\MSSQLSERVER06;Initial Catalog=demv3;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (userId == null)
                    {
                        // Получаем максимальное значение ID
                        string getMaxIdQuery = "SELECT ISNULL(MAX(ID), 0) + 1 FROM Product$";
                        SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, connection);
                        int newId = Convert.ToInt32(getMaxIdCommand.ExecuteScalar());

                        // Добавление нового пользователя
                        string query = "INSERT INTO Product$ (ID, Name, Brand, Animal) VALUES (@ID, @Name, @Brand, @Animal)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", newId);
                        command.Parameters.AddWithValue("@Name", textBoxLogin.Text.Trim());
                        command.Parameters.AddWithValue("@Brand", textBoxPassword.Text.Trim());
                        command.Parameters.AddWithValue("@Animal", comboBoxRole.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // Редактирование существующего пользователя
                        string query = "UPDATE Product$ SET Name = @Name, Brand = @Brand, Animal = @Animal WHERE ID = @ID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", userId); // ID передаётся только как параметр WHERE
                        command.Parameters.AddWithValue("@Name", textBoxLogin.Text.Trim());
                        command.Parameters.AddWithValue("@Brand", textBoxPassword.Text.Trim());
                        command.Parameters.AddWithValue("@Animal", comboBoxRole.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                    }
                }

                // Закрываем форму и возвращаем результат
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }

}
