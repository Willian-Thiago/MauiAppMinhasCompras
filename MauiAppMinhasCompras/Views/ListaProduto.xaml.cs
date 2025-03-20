using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{

	// Lista observável completa usada como base de dados (não muda durante a busca)
	private List<Produto> ListaOriginal = new();

	// Lista observável usada para exibição da UI (atualizada dinamicamente)
	public ObservableCollection<Produto> ListaFiltrada {  get; set; } = new();

    // Token para controle de debounce (evita buscas excessivas ao digitar rápido)
    private CancellationTokenSource _cts = new();

    public ListaProduto()
	{
		InitializeComponent();

		// Associa a lista filtrada a interface gráfica (ListView, CollectionView, etc)
		lst_produtos.ItemsSource = ListaFiltrada;
	}

	// Método chamado automaticamente quando a página aparece
    protected async override void OnAppearing()
    {
		base.OnAppearing();
        await CarregarDadosAssync();

    }

    // Melhoria 1: Carregar dados sem travar a UI (uso do Task.Run)
    private async Task CarregarDadosAssync()
    {
        var tmp = await Task.Run(() => App.Db.GetAll()); //Busca os produtos no banco de dados de forma assíncrona
        if (tmp != null)
        {
            ListaOriginal = new List<Produto>(tmp); // Mantém uma cópia fixa dos dados
            AtualizarLista(ListaOriginal); //Atualiza a exibição com todos os produtos
        }
    }
    
    // Melhoria 2: Implementação de debounce na busca para evitar múltiplas chamadas seguidas
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cts?.Cancel(); //Cancela a última busca pendente, evitando sobrecarga no banco de dados
        _cts = new CancellationTokenSource();
        await Task.Delay(300, _cts.Token); //Aguarda 300 ms antes de buscar (caso o usuario continue digitando)

        if(_cts.Token.IsCancellationRequested)
            return; //Se foi cancelado, sai do método sem executar a busca

        string query = e.NewTextValue?.ToLower() ?? string.Empty;

        if(string.IsNullOrWhiteSpace(query))
        {
            AtualizarLista(ListaOriginal); //Se a busca estiver vazia, mostra todos os produtos novamente
            return;
        }

        // Melhoria 3: Busca otimizada usando Task.Run para evitar travamento da UI
        var tmp = await Task.Run(() => App.Db.Search(query));

        AtualizarLista(tmp);

    }

    //Melhoria 4: Atualização da UI na thread principal para evitar exceções
    private void AtualizarLista(List<Produto> produtos)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ListaFiltrada.Clear();
            foreach (var item in produtos)
                ListaFiltrada.Add(item);
        });
    }

    //Melhoria 5: Evita que a UI trave ao abrir a tela de novo produto
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // Melhoria 6: Calcula o total de forma eficiente apena com os itens filtrados
    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		double soma = ListaFiltrada.Sum(i => i.Total); //Soma apenas os itens visiveis na tela

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "OK");
    }

    // Menu para exclusão do produto das listas
    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem certeza?", $"Remover {p.Descricao} ?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                ListaOriginal.Remove(p);
                ListaFiltrada.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message , "OK");
        }
    }
}