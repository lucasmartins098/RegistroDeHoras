using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace RegistroDeHorasDevOpsAzure
{
    public class TarefaController : TarefaModel
    {
        //Mudar de acordo com a URL da Infosis no DeVOps Azure; Observasão: Sempre usar "HTTPS".
        internal const string azureDevOpsOrganizationUrl = "https://dev.azure.com/infosis-fsw";
        public List<TarefaModel> tarefasModel = new List<TarefaModel>();
        //Solicita o login do usuário no DevOpsAzure
        VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new VssClientCredentials());

        public DataTable GetTarefaAndamento(string responsavel)
        {
            return this.tarefaRepositorio.GetTarefaAndamento(responsavel);
        }

        public int InsereDados(int idTarefa, string tipoAcao, DateTime? data, DateTime? qtdHoras)
        {
            BuscarDetalhesTarefas(idTarefa);
            int idTarefaBanco = 0;

            //futuramente terá a opção de inserir manualmente, por isso foi colocado esse if
            if (tipoAcao == "InicioTarefa" || tipoAcao == "TerminarIniciarTarefa")
            {
                this.DataInicioTarefa = DateTime.Now;
                idTarefaBanco = this.tarefaRepositorio.InsereDados(this.Tipo, this.Titulo, this.OrdemServico, this.FaseOS, null, this.AssignedTo, null, this.StateFase, idTarefa, null, null,
                                                   this.DataInicioTarefa, null, null);
            }
            return idTarefaBanco;
        }

        public int AtualizaDados(int idTarefaBanco, string tipoAcao, int idTarefa)
        {
            this.DataFimTarefa = DateTime.Now;
            this.tarefaRepositorio.AtualizaDados(this.DataFimTarefa, idTarefaBanco);
            if (tipoAcao == "TerminarIniciarTarefa")
            {
                idTarefaBanco = InsereDados(idTarefa, tipoAcao, null, null);
            }
            return idTarefaBanco;
        }

        public string BuscarUsuarioLogado()
        {
            string email = connection.AuthorizedIdentity.Descriptor.Identifier;
            // Preenche a variável email somente como o email do usuário logado.
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(email);
            foreach (Match emailMatch in emailMatches)
            {
                email = emailMatch.Value.ToString();
            }
            return email;
        }

        public DataTable GetDataTableTarefas(DateTime dataInicial, DateTime dataFinal, int tipo)
        {
            DataTable dataTable;
            string naturezaTrabalho = "";
            if (tipo == 2)
            {
                naturezaTrabalho = " AND NaturezaTrabalho = 2";
            }
            else if (tipo == 1)
            {
                naturezaTrabalho = " AND NaturezaTrabalho = 1";
            }

            dataTable = tarefaRepositorio.GetDataTableTarefas(dataInicial, dataFinal, naturezaTrabalho);
            return dataTable;
        }

        public List<TarefaModel> DadosDevOps()
        {
            //cria http client e query para resultados
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            Wiql query = new Wiql() { Query = "SELECT [Id] FROM workitems WHERE [Assigned To] = @Me AND [System.State] = 'Active'" };
            //[System.State] = 'Active' ou [State]
            // Activity Type

            // Retorna os Ids dos workitems ativos do usuário
            WorkItemQueryResult queryResults = witClient.QueryByWiqlAsync(query).Result;
            if (queryResults == null || queryResults.WorkItems.Count() == 0)
            {
             //   MessageBox.Show("Você não tem tarefa ativa cadastrada no DevOps");
            }
            else
            {
                //Pega o id de cada workItems para fazer a busca dos detalhes do workitem especifico
                foreach (var item in queryResults.WorkItems)
                {
                    BuscarDetalhesTarefas(item.Id);
                    tarefasModel.Add(new TarefaModel { IdTarefa = item.Id, Titulo = this.Titulo });
                }

            }
            return tarefasModel;
        }
        public void BuscarDetalhesTarefas(int workItemId)
        {
            // Pega uma instância do work item tracking client
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Pega work item especifico
            WorkItem workitem = witClient.GetWorkItemAsync(workItemId).Result;

            foreach (var field in workitem.Fields)
            {
                if (field.Key == "System.Title")
                {
                    this.Titulo = field.Value.ToString();
                }
                else if (field.Key == "System.AssignedTo")
                {
                    // Essa parte do código preenche a variavel AssignedTo somente como o email do usuário.
                    Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
                    MatchCollection emailMatches = emailRegex.Matches(field.Value.ToString());
                    StringBuilder sb = new StringBuilder();
                    foreach (Match emailMatch in emailMatches)
                    {
                        this.AssignedTo = sb.AppendLine(emailMatch.Value).ToString();
                    }
                }
                else if (field.Key == "Custom.ActivityType")
                {
                    this.Tipo = field.Value.ToString();
                }
            }
            Wiql query = new Wiql() { Query = "SELECT [Id] FROM WorkItemLinks where [Target].[System.Id] = " + workItemId };
            WorkItemQueryResult queryResults2 = witClient.QueryByWiqlAsync(query).Result;

            foreach (var wir in queryResults2.WorkItemRelations)
            {
                //Target é a tarefa base e Source é o parent dessa tarefa
                //Caso o Target for uma OS, ele não terá um Source
                if (wir.Target.Id == workItemId && wir.Source?.Id != null)
                {
                    WorkItem workItemRelacionado = witClient.GetWorkItemAsync(wir.Source.Id).Result;
                    foreach (var fieldParent in workItemRelacionado.Fields)
                    {
                        if (fieldParent.Key == "System.Title")
                        {
                            this.FaseOS = fieldParent.Value.ToString();
                        }
                        else if (fieldParent.Key == "Custom.ServiceOrder")
                        {
                            this.OrdemServico = fieldParent.Value.ToString();
                        }
                        else if (fieldParent.Key == "System.State")
                        {
                            this.StateFase = fieldParent.Value.ToString();
                        }
                    }
                }
            }
        }
    }
}
