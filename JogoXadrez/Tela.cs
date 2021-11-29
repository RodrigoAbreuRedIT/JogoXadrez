using System;
using tabuleiro;
using xadrez;

namespace JogoXadrez {
    class Tela {
        public static void imprimirTabuleiro(Tabuleiro tab) {
            for(int i=0; i < tab.linhas; i++) {
                Tela.selectColor(8 - i + " ", ConsoleColor.DarkMagenta);
                for (int j=0; j < tab.colunas; j++) {
                    if (tab.peca(i, j) == null)
                        Console.Write("- ");
                    else {
                        Tela.imprimirPeca(tab.peca(i, j));
                        Console.Write($" ");
                    }
                }
                Console.WriteLine();
            }
            Tela.selectColor("  a b c d e f g h", ConsoleColor.DarkMagenta);
        }

        internal static PosicaoXadrez lerPosicaoXadrez() {
            string s = Console.ReadLine();
            char coluna = s[0];
            int linha = int.Parse(s[1] + "");

            return new PosicaoXadrez(coluna, linha);
        }

        public static void imprimirPeca(Peca peca) {
            if (peca.cor == Cor.Branca)
                Console.Write(peca);
            else {
                ConsoleColor aux = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(peca);
                Console.ForegroundColor = aux;
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