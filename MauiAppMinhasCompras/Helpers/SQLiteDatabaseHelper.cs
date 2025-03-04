using MauiAppMinhasCompras.Models;
using SQLite;


namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        //criando conexao com o banco de dados SQLite
        readonly SQLiteAsyncConnection _conn;


        //construtor para conexao com SQLite e criação da tabela Produto
        public SQLiteDatabaseHelper(string path) 
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }
        // Classe para cadastrar novo produto
        public Task<int> Insert(Produto p) 
        {
            return _conn.InsertAsync(p);
        }
        // Classe para atualizar produto cadastrado
        public Task<List<Produto>> Update(Produto p) 
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=? WHERE iD=?";
            
            return _conn.QueryAsync<Produto>(sql, p.Descricao, p.Quantidade, p.Preco, p.Id);
        }
        // Classe para deletar produtos cadastrados
        public Task<int> Delete(int id) 
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }
        // Classe onde se exibe todos os produtos cadastrados
        public Task<List<Produto>> GetAll() 
        {
            return _conn.Table<Produto>().ToListAsync();
        }
        // Classe criada para pesquisa dos produtos cadastrados
        public Task<List<Produto>> Search(string q) 
        {
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%'";

            return _conn.QueryAsync<Produto>(sql);
        }



    }
}
