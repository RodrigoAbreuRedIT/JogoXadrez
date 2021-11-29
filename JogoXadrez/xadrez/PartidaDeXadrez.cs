using System.Collections.Generic;
using tabuleiro;

namespace xadrez {
    class PartidaDeXadrez {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }

        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public PartidaDeXadrez() {
            this.tab = new Tabuleiro(8, 8);
            this.pecas = new HashSet<Peca>();
            this.capturadas = new HashSet<Peca>();
            this.turno = 1;
            this.jogadorAtual = Cor.Branca;
            this.terminada = false;
            colocarPecas();
        }

        public void executaMovimento(Posicao origem, Posicao destino) {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pCapturada != null)
                capturadas.Add(pCapturada);
        }

        public void realizaJogada(Posicao origem, Posicao destino) {
            executaMovimento(origem, destino);
            turno++;
            mudaJogador();
        }

        public void validarPosicaoOrigem(Posicao pos) {
            if (tab.peca(pos) == null)
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            if(jogadorAtual != tab.peca(pos).cor)
                throw new TabuleiroException("A peça escolhida não é sua!");
            if(!tab.peca(pos).existeMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida!");
        }

        public void validarPosicaoDestino(Posicao origem, Posicao destino) {
            if(!tab.peca(origem).podeMoverPara(destino))
                throw new TabuleiroException("Posicao de destino inválida!");
        }

        private void mudaJogador() {
            this.jogadorAtual = jogadorAtual == Cor.Branca ? Cor.Preta : Cor.Branca;
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach(Peca p in capturadas) {
                if (p.cor == cor)
                    aux.Add(p);
            }

            return aux;
        }

        public HashSet<Peca> pecasEmJogao(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca p in pecas) {
                if (p.cor == cor)
                    aux.Add(p);
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas() {
            // Colocar peças brancas
            colocarNovaPeca('c', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('c', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('d', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Rei(tab, Cor.Branca));

            // Colocar peças pretas
            colocarNovaPeca('c', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('d', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Rei(tab, Cor.Preta));
        }
    }
}