using MyLibrary;
using MyLibrary.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                // Создаем репозитории (только для чтения)
                var productRepo = new MySqlProductRepository();
                var componentRepo = new MySqlComponentRepository();
                var productComponentRepo = new MySqlProductComponentRepository();

                // Создаем сервисы
                var productService = new ProductService(productRepo, productComponentRepo, componentRepo);
                var componentService = new ComponentService(componentRepo, productComponentRepo);

                // Создаем главную форму (презентер создастся внутри формы)
                var mainForm = new MainForm(productService, componentService);
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
