using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace RegistroDeHorasDevOpsAzure
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static TarefaController TarefaController = new TarefaController();
        static string responsavel = TarefaController.BuscarUsuarioLogado();

        public Timer timer = new Timer((state) =>
        {
            DataTable dataTable = TarefaController.GetTarefaAndamento(responsavel);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (dataTable.Rows.Count == 0)
                {
                    Notificacao notificacao = new Notificacao();
                    notificacao.InitializeComponent();
                    notificacao.Show();
                }
            });
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }
}