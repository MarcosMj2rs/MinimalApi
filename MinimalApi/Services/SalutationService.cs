namespace MinimalApi.Model
{
	public class SalutationService
	{
		public string Welcome(string nome)
		{
			return $"Bem-Vindo {nome}";
		}
	}
}
