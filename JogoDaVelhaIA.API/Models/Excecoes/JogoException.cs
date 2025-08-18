using System;

namespace JogoDaVelhIA.Models.Excecoes
{
    public class JogoException : Exception
    {
        public JogoException(string mensagem) : base(mensagem)
        {
        }

        public JogoException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}