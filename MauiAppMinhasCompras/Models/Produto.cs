using SQLite;
using System.Numerics;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string _descricao;

        internal object Nome;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao { 
            get => _descricao; 
            set
            {
                if(value == null)
                {
                    throw new Exception("Por favor, preencha a descrição");
                }
                _descricao = value;
            }
        
        }
        public double Quantidade { get; set; }
        public double Preco {  get; set; }

        public double Total { get => Quantidade * Preco; }


    }
}
