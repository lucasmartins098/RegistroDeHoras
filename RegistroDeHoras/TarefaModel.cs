using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegistroDeHorasDevOpsAzure
{
   public class TarefaModel
    {
        public int IdTarefa { get; set; }
        public int IdTarefaBanco { get; set; }
        protected int NaturezaTrabalho { get; set; }
        protected string Tipo { get; set; }
        protected string AssignedTo { get; set; }
        public string Titulo { get; set; }
        protected string OrdemServico { get; set; }
        protected string Estado { get; set; }
        protected string FaseOS { get; set; }
        protected string StateFase { get; set; }
        protected string Situacao { get; set; }
        //Varável será usada para lançamento da quantidade de horas gastas em uma tarefa, de forma manual.
        protected DateTime QtdHoras { get; set; }
        //Varável será usada para lançamento do dia de realização de uma tarefa, de forma manual.
        protected DateTime Data { get; set; }
        protected DateTime DataInicioTarefa { get; set; }
        protected DateTime DataFimTarefa { get; set; }
        public TarefaRepositorio tarefaRepositorio { get; set; }
        public TarefaModel()
        {
            this.tarefaRepositorio = new TarefaRepositorio();
        }
    }
}
