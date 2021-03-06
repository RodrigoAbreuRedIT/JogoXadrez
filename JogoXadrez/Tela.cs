using System;
using System.Collections.Generic;
using tabuleiro;
using xadrez;

namespace JogoXadrez {
    class Tela {

        public static void imprimirPartida(PartidaDeXadrez partida) {
            Tela.imprimirTabuleiro(partida.tab);
            imprimirPecasApturadas(partida);
            Console.WriteLine($"Turno: {partida.turno}");

            if (!partida.terminada) {
                Console.WriteLine($"Aguardando Jogada: {partida.jogadorAtual}");
                if (partida.xeque)
                    Console.WriteLine("XEQUE!!");
            }
            else {
                Console.WriteLine("XEQUEMATE!");
                Console.WriteLine($"Vencedor: {partida.jogadorAtual}");
                Console.ReadLine();
            }
        }

        public static void imprimirPecasApturadas(PartidaDeXadrez partida) {
            Console.WriteLine("\n\nPeças Capturadas:");
            Console.Write("Brancas: ");
            imprimirConjunto(partida.pecasCapturadas(Cor.Branca));
            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Pretas: ");
            imprimirConjunto(partida.pecasCapturadas(Cor.Preta));
            Console.ForegroundColor = aux;
        }

        public static void imprimirConjunto(HashSet<Peca> conjunto) {
            Console.Write("[");
            foreach (Peca p in conjunto)
                Console.Write(p + " ");
            Console.Write("]\n");
        }

        public static void imprimirTabuleiro(Tabuleiro tab) {
            for (int i = 0; i < tab.linhas; i++) {
                Tela.selectColor(8 - i + " ", ConsoleColor.DarkMagenta);
                for (int j = 0; j < tab.colunas; j++) {
                    Tela.imprimirPeca(tab.peca(i, j));
                }
                Console.WriteLine();
            }
            Tela.selectColor("  a b c d e f g h", ConsoleColor.DarkMagenta);
        }

        public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoesPossiveis) {

            ConsoleColor fundoOriginal = Console.BackgroundColor;
            ConsoleColor fundoAlterado = ConsoleColor.DarkGray;

            for (int i = 0; i < tab.linhas; i++) {
                Tela.selectColor(8 - i + " ", ConsoleColor.DarkMagenta);
                for (int j = 0; j < tab.colunas; j++) {
                    if (posicoesPossiveis[i, j])
                        Console.BackgroundColor = fundoAlterado;
                    else
                        Console.BackgroundColor = fundoOriginal;
                    Tela.imprimirPeca(tab.peca(i, j));
                    Console.BackgroundColor = fundoOriginal;
                }
                Console.WriteLine();
            }
            Tela.selectColor("  a b c d e f g h", ConsoleColor.DarkMagenta);
            Console.BackgroundColor = fundoOriginal;
        }

        internal static PosicaoXadrez lerPosicaoXadrez() {
            string s = Console.ReadLine();
            char coluna = s[0];
            int linha = int.Parse(s[1] + "");

            return new PosicaoXadrez(coluna, linha);
        }

        public static void imprimirPeca(Peca peca) {
            if (peca == null)
                Console.Write("- ");
            else {
                if (peca.cor == Cor.Branca)
                    Console.Write(peca);
                else {
                    ConsoleColor aux = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(peca);
                    Console.ForegroundColor = aux;
                }
                Console.Write(" ");
            }
        }

        public static void selectColor(string textWrite, ConsoleColor color) {
            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = color;
            //Console.ForegroundColor = ConsoleColor.DarkRed;
            //Console.WriteLine("  a b c d e f g h");
            Console.Write(textWrite);
            Console.ForegroundColor = aux;
        }
    }
}