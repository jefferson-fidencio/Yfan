using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VotacaoEstampas.Model;

namespace VotacaoEstampas.Repository
{
	public class VotacaoRepository {

        public ObservableCollection<Votacao> Votacoes
        {
            get { return votacoes; }
        }
        private ObservableCollection<Votacao> votacoes;

		public VotacaoRepository() {
			GenerateVotacoes();            
		}     

		void GenerateVotacoes()
        { 
            votacoes = new ObservableCollection<Votacao> ();
            foreach (var votacao in App.UltimaColecao.Votacoes)
            {
                votacoes.Add(votacao);
            }
        }
	}
}

