using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Globalization;



namespace GestorTareas3
{
    public partial class Form1 : Form

    {
        private string connectionSqlite = "Data Source=mibasedatos.db;Version=3;";

        public Form1()
        {
            InitializeComponent();
            CrearBaseDeDatos();
        }

        private String OctenerHora()
        {
            DateTime HoraActual = DateTime.Now;
            return HoraActual.ToString();
        }



        private void ListarTareas()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionSqlite))
                {
                    connection.Open();

                    string query = "SELECT id, nombre, fecha FROM tareas";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Configurar el DataGridView
                            dataGridView1.Columns.Clear();
                            dataGridView1.Rows.Clear();

                            // Agregar columnas al DataGridView
                            dataGridView1.Columns.Add("ColumnaId", "ID");
                            dataGridView1.Columns.Add("ColumnaNombre", "Nombre");
                            dataGridView1.Columns.Add("ColumnaFecha", "Fecha");

                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string nombre = reader.GetString(1);
                                string fecha = reader.GetString(2);

                                // Agregar filas al DataGridView
                                dataGridView1.Rows.Add(id, nombre, fecha);
                            }
                            dataGridView1.Columns["ColumnaNombre"].Width = 400;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Error al cargar tareas: " + ex.Message);
            }
        }






        private void GuardarTarea()
        {

            string nombre = caja_nombre.Text;

            foreach (char c in nombre)
            {
                System.Diagnostics.Debug.WriteLine($"Carácter: {c}, Valor hexadecimal: {Convert.ToInt32(c):X}");
            }

            if (nombre != string.Empty)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionSqlite))
                    {
                        Console.WriteLine("Abriendo la conexión a la base de datos");
                        connection.Open();
                        Console.WriteLine("Cerrando la conexión a la base de datos");

                        string insertQuery = "INSERT INTO tareas (nombre,fecha) VALUES(@nombre,@fecha)";

                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@fecha", OctenerHora());
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Datos guardados correctamente.");
                            }
                            else
                            {
                                MessageBox.Show("Error al guardar los datos. No se afectaron filas.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Por favor, inserte un dato antes de guardarlo." +" "+ nombre);
            }

            ListarTareas();
        }

        private void EliminarTarea()
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Obtener el ID de la fila seleccionada
                    int idSeleccionado = (int)dataGridView1.SelectedRows[0].Cells["ColumnaId"].Value;

                    // Conectar a la base de datos
                    using (SQLiteConnection connection = new SQLiteConnection(connectionSqlite))
                    {
                        connection.Open();

                        // Consulta para eliminar la tarea
                        string deleteQuery = "DELETE FROM tareas WHERE id = @id";

                        using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@id", idSeleccionado);

                            // Ejecutar la consulta de eliminación
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tarea eliminada correctamente.");

                                // Actualizar la visualización del DataGridView
                                ListarTareas();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo eliminar la tarea. No se afectaron filas.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una fila para eliminar.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Error al intentar eliminar la tarea: " + ex.Message);
            }
        }


        private void CrearBaseDeDatos()
        {
            try
            {
                // Verificar si la base de datos existe
                if (!System.IO.File.Exists("mibasedatos.db"))
                {
                    // Si no existe, crear la base de datos
                    SQLiteConnection.CreateFile("mibasedatos.db");
                    MessageBox.Show("La base de datos 'mibasedatos.db' ha sido creada correctamente.");
                }
                else
                {
                    MessageBox.Show("La base de datos 'mibasedatos.db' ya existe.");
                }

                // Conectar a la base de datos
                using (SQLiteConnection connection = new SQLiteConnection(connectionSqlite))
                {
                    connection.Open();

                    // Consulta para verificar si la tabla "tareas" existe
                    string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='tareas'";

                    using (SQLiteCommand checkTableCommand = new SQLiteCommand(checkTableQuery, connection))
                    {
                        object result = checkTableCommand.ExecuteScalar();

                        // Si la tabla no existe, la creamos
                        if (result == null || result == DBNull.Value)
                        {
                            // Consulta para crear la tabla "tareas"
                            string createTableQuery = "CREATE TABLE tareas (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, nombre INTEGER NOT NULL, fecha INTEGER NOT NULL)";

                            using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                            {
                                createTableCommand.ExecuteNonQuery();
                                MessageBox.Show("La tabla 'tareas' ha sido creada correctamente.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("La tabla 'tareas' ya existe.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("Error al verificar/crear la base de datos y la tabla: " + ex.Message);
            }
        }



        private void bt_guardar_Click(object sender, EventArgs e)
        {
            GuardarTarea();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListarTareas();

        }

        private void bt_eliminar_Click(object sender, EventArgs e)
        {
            EliminarTarea();
        }
    }
}
