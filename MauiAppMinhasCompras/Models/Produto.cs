using SQLite;
using System.Numerics;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        internal object Nome;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao { get; set; }
        public double Quantidade { get; set; }
        public double Preco {  get; set; }

        public double Total { get => Quantidade * Preco; }


    }
}
