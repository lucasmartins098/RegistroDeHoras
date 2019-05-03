using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Data;


namespace RegistroDeHorasDevOpsAzure
{
    /// <summary>
    /// Interaction logic for TaskBarRegistroDeHoras.xaml
    /// </summary>
    public partial class TaskBarRegistroDeHoras
    {
        TarefaController TarefaController = new TarefaController();
        public List<TarefaModel> listAtivo = new List<TarefaModel>();
        public List<TarefaModel> listAndamento = new List<TarefaModel>();
        int IdTarefaBanco;
        static TarefaController TarefaControllerTimer = new TarefaController();
        static string responsavel = TarefaControllerTimer.BuscarUsuarioLogado();

        public TaskBarRegistroDeHoras()
        {
            InitializeComponent();
            atualizarGrid();
        }
        public void atualizarGrid()
        {
            listAtivo.Clear();
            listAndamento.Clear();
            listAtivo = TarefaController.DadosDevOps();
            string responsavel = TarefaController.BuscarUsuarioLogado();
            DataTable dataTable = TarefaController.GetTarefaAndamento(responsavel);
            if (dataTable.Rows.Count == 0)
            {
                listAndamento.Add(new TarefaModel { Titulo = "Não há tarefa em andamento" });
            }
            else
            {
                string titulo = dataTable.Rows[0]["Descricao"].ToString();
                listAndamento.Add(new TarefaModel { Titulo = titulo });
            }
            this.CarregarGrids();
        }
        public void IniciarValoresGrid()
        {
            listAtivo.Clear();
            listAndamento.Clear();
            listAtivo = TarefaController.DadosDevOps();
            listAndamento.Add(new TarefaModel { Titulo = "Não há tarefa em andamento" });
            this.CarregarGrids();
        }

        public void CarregarGrids()
        {
            TarefaAndamento.CanUserAddRows = false;
            TarefaAndamento.ItemsSource = listAndamento;

            TarefasDataGrid.CanUserAddRows = false;
            TarefasDataGrid.ItemsSource = listAtivo;

            CollectionViewSource.GetDefaultView(TarefaAndamento.ItemsSource).Refresh();
            CollectionViewSource.GetDefaultView(TarefasDataGrid.ItemsSource).Refresh();
        }

        private void TarefaAndamento_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string responsavel = TarefaController.BuscarUsuarioLogado();
            DataTable dataTable = TarefaController.GetTarefaAndamento(responsavel);
            int auxiliar = 0;
            string titulo = "";
            int idTarefa = 0;
            int idTarefaBanco = 0;
            if (dataTable.Rows.Count == 0)
            {
                listAndamento.Add(new TarefaModel { Titulo = "Não há tarefa em andamento" });
            }
            else
            {
                auxiliar = 1;
                titulo = dataTable.Rows[0]["Descricao"].ToString();
                idTarefa = (int)dataTable.Rows[0]["IdTarefa"];
                idTarefaBanco = (int)dataTable.Rows[0]["IdRegistro_Horas"];
                listAndamento.Add(new TarefaModel { Titulo = titulo });
            }

            if (auxiliar != 0)
            {
                TarefaController.AtualizaDados(idTarefaBanco, "TerminoTarefa", idTarefa);
                listAtivo.Clear();
                listAndamento.Clear();
                IniciarValoresGrid();
            }

        }

        private void TarefasDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string responsavel = TarefaController.BuscarUsuarioLogado();
            DataTable dataTable = TarefaController.GetTarefaAndamento(responsavel);
            //variável auxiliar para verificar se existe existe alguma tarefa em andamento
            bool auxiliar = false;
            string titulo = "";
            int idTarefa = 0;
            int idTarefaBanco = 0;
            if (dataTable.Rows.Count == 0)
            {
                listAndamento.Add(new TarefaModel { Titulo = "Não há tarefa em andamento" });
            }
            else
            {
                auxiliar = true;
                titulo = dataTable.Rows[0]["Descricao"].ToString();
                idTarefa = (int)dataTable.Rows[0]["IdTarefa"];
                idTarefaBanco = (int)dataTable.Rows[0]["IdRegistro_Horas"];
                listAndamento.Add(new TarefaModel { Titulo = titulo });
            }

            //Início de uma tarefa, sem ter tarefa em andamento
            if (auxiliar != true)
            {
                var andamento = TarefasDataGrid.SelectedItem;
                string tituloAndamento = (string)andamento.GetType().GetProperty("Titulo").GetValue(andamento);
                int idTarefaAndamento = (int)andamento.GetType().GetProperty("IdTarefa").GetValue(andamento);
                listAtivo.Clear();
                listAndamento.Clear();
                listAtivo = TarefaController.DadosDevOps();
                IdTarefaBanco = TarefaController.InsereDados(idTarefaAndamento, "InicioTarefa", null, null);
                listAndamento.Add(new TarefaModel { IdTarefaBanco = IdTarefaBanco, Titulo = tituloAndamento });
                this.CarregarGrids();
            }
            //Término da tarefa em andamento
            else if ((int)TarefasDataGrid.SelectedItem.GetType().GetProperty("IdTarefa").GetValue(TarefasDataGrid.SelectedItem) == idTarefa)
            {
                TarefaController.AtualizaDados(idTarefaBanco, "TerminoTarefa", idTarefa);
                listAtivo.Clear();
                listAndamento.Clear();
                IniciarValoresGrid();
            }
            //Término da tarefa em andamento einício de uma nova tarefa
            else if (auxiliar != false && (int)TarefasDataGrid.SelectedItem.GetType().GetProperty("IdTarefa").GetValue(TarefasDataGrid.SelectedItem) != idTarefa)
            {
                IdTarefaBanco = TarefaController.AtualizaDados(idTarefaBanco, "TerminarIniciarTarefa", (int)TarefasDataGrid.SelectedItem.GetType().GetProperty("IdTarefa").GetValue(TarefasDataGrid.SelectedItem));
                listAtivo.Clear();
                listAndamento.Clear();
                string tituloAndamento = (string)TarefasDataGrid.SelectedItem.GetType().GetProperty("Titulo").GetValue(TarefasDataGrid.SelectedItem);
                listAtivo = TarefaController.DadosDevOps();
                listAndamento.Add(new TarefaModel { IdTarefaBanco = IdTarefaBanco, Titulo = tituloAndamento });
                CarregarGrids();
            }
        }

        private void botaoAtualizar(object sender, RoutedEventArgs e)
        {
            this.atualizarGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegistroDeHorasManual registroDeHorasManual = new RegistroDeHorasManual();
            registroDeHorasManual.InitializeComponent();
            registroDeHorasManual.Show();
        }
    }
}