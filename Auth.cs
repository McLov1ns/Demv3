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
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }
        private void enter_Click(object sender, EventArgs e)
        {
            string login = Login.Text;
            string password = Password.Text;
            string connectionString = @"Data Source=MEGAKOMP\MSSQLSERVER06;Initial Catalog=demv3;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT RoleID FROM Customer$ WHERE Login = @Login AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);

                        var roleCode = command.ExecuteScalar();

                        if (roleCode != null)
                        {
                            Form nextForm = GetNextFormByRole(roleCode.ToString());
                            if (nextForm != null)
                            {
                                nextForm.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private Form GetNextFormByRole(string roleCode)
        {
            switch (roleCode)
            {
                case "3":
                    return Form1.Instance;
                //case "user":
                //    return UserMenu.Instance;
                //case "mod":
                //    return ModMenu.Instance;
                //case "org":
                //    return OrgMenu.Instance;
                default:
                    MessageBox.Show("Неизвестный код роли", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
            }
        }

    }
}
