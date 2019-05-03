using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RegistroDeHorasDevOpsAzure
{
    /// <summary>
    /// Lógica interna para RegistroDeHorasManual.xaml
    /// </summary>
    public partial class RegistroDeHorasManual : Window
    {
        public TarefaController TarefaController = new TarefaController();

        public RegistroDeHorasManual()
        {
            InitializeComponent();
            this.CarregarGrid();
        }

        public void CarregarGrid()
        {
            int tipo = 0;
            DateTime dateInicial;
            DateTime dateFinal;
            dateInicial = dataInicial.SelectedDate == null ? DateTime.Now.AddDays(-15) : (DateTime)dataInicial.SelectedDate;
            dateFinal = dataFinal.SelectedDate == null ? DateTime.Now : (DateTime)dataFinal.SelectedDate.Value.AddDays(1);

            if (dateFinal < dateInicial || dateInicial < DateTime.Now.AddDays(-15))
            {
                MessageBox.Show("Cláusulas para preenchimento dos filtros: \n" +
                    "- Data Final não pode ser inferior a data inicial. \n" +
                    "- Data inicial deve ser de até duas semanas atrás.");
            }
            else
            {
                if (Operacional.IsChecked == true)
                {
                    tipo = 1;
                }
                else if (NaoFaturado.IsChecked == true)
                {
                    tipo = 2;
                }
                DataTable dataTable = TarefaController.GetDataTableTarefas(dateInicial, dateFinal, tipo);
                Tarefas.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.CarregarGrid();
        }
    }
}
