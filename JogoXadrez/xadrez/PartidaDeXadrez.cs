using System.Collections.Generic;
using tabuleiro;

namespace xadrez {
    class PartidaDeXadrez {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        public bool xeque { get; private set; }

        public Peca vulneravelEnPassant { get; private set; }

        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public PartidaDeXadrez() {
            this.tab = new Tabuleiro(8, 8);
            this.pecas = new HashSet<Peca>();
            this.capturadas = new HashSet<Peca>();
            this.turno = 1;
            this.jogadorAtual = Cor.Branca;
            this.terminada = false;
            this.xeque = false;
            this.vulneravelEnPassant = null;
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino) {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pCapturada != null)
                capturadas.Add(pCapturada);

            // #jogadaEspecial Roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }
            // #jogadaEspecial Roque Grande
            if (p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial en passant
            if (p is Peao) {
                if (origem.coluna != destino.coluna && pCapturada == null) {
                    Posicao posP;
                    if (p.cor == Cor.Branca) {
                        posP = new Posicao(destino.linha + 1, destino.coluna);
                    }
                    else {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pCapturada = tab.retirarPeca(posP);
                    capturadas.Add(pCapturada);
                }
            }

            return pCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pCapturada) {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if (pCapturada != null) {
                tab.colocarPeca(pCapturada, destino);
                capturadas.Remove(pCapturada);
            }
            tab.colocarPeca(p, origem);

            // #jogadaEspecial Roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }
            // #jogadaEspecial Roque Grande
            if (p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial en passant
            if (p is Peao) {
                if (origem.coluna != destino.coluna && pCapturada == vulneravelEnPassant) {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posP;
                    if (p.cor == Cor.Branca) {
                        posP = new Posicao(3, destino.coluna);
                    }
                    else {
                        posP = new Posicao(4, destino.coluna);
                    }
                    tab.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Posicao origem, Posicao destino) {
            Peca pCapturada = executaMovimento(origem, destino);
            if (estaEmXeque(jogadorAtual)) {
                desfazMovimento(origem, destino, pCapturada);
                throw new TabuleiroException("Você não se pode colocar em Xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
                xeque = true;
            else
                xeque = false;

            if (testeXequeMate(adversaria(jogadorAtual)))
                terminada = true;
            else {
                turno++;
                mudaJogador();
            }

            Peca p = tab.peca(destino);

            // #jogadaEspecial en passant
            if (p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2))
                this.vulneravelEnPassant = p;
            else
                this.vulneravelEnPassant = null;
        }

        public void validarPosicaoOrigem(Posicao pos) {
            if (tab.peca(pos) == null)
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            if (jogadorAtual != tab.peca(pos).cor)
                throw new TabuleiroException("A peça escolhida não é sua!");
            if (!tab.peca(pos).existeMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida!");
        }

        public void validarPosicaoDestino(Posicao origem, Posicao destino) {
            if (!tab.peca(origem).MovimentoPossivel(destino))
                throw new TabuleiroException("Posicao de destino inválida!");
        }

        private void mudaJogador() {
            this.jogadorAtual = jogadorAtual == Cor.Branca ? Cor.Preta : Cor.Branca;
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca p in capturadas) {
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

        private Cor adversaria(Cor cor) {
            return cor == Cor.Branca ? Cor.Preta : Cor.Branca;
        }

        private Peca rei(Cor cor) {
            foreach (Peca p in pecasEmJogao(cor)) {
                if (p is Rei) {
                    return p;
                }
            }

            return null;
        }

        public bool estaEmXeque(Cor cor) {
            Peca pRei = rei(cor);
            if (pRei == null)
                throw new TabuleiroException($"Não tem rei da cor {cor} no tabuleiro");

            foreach (Peca p in pecasEmJogao(adversaria(cor))) {
                bool[,] mat = p.movimentosPossiveis();
                if (mat[pRei.posicao.linha, pRei.posicao.coluna]) {
                    return true;
                }
            }

            return false;
        }

        public bool testeXequeMate(Cor cor) {
            if (!estaEmXeque(cor))
                return false;
            foreach (Peca p in pecasEmJogao(cor)) {
                bool[,] mat = p.movimentosPossiveis();

                for (int i = 0; i < tab.linhas; i++) {
                    for (int j = 0; j < tab.colunas; j++) {
                        if (mat[i, j]) {
                            Posicao origem = p.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pCapturada);
                            if (!testeXeque)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas() {
            // pecas cor Branca
            colocarNovaPeca('a', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branca, this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('a', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.Branca, this));

            // pecas cor Preta
            colocarNovaPeca('a', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preta, this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('a', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.Preta, this));
        }
    }
}