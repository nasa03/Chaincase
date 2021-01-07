using System.Threading.Tasks;
using Chaincase.Common;
using Chaincase.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using QRCoder;

namespace Chaincase.UI.Services
{
	public static class Extensions
	{
		public static IServiceCollection AddUIServices(this IServiceCollection services)
		{
			services.AddSingleton<UIStateService>();

			services.AddSingleton<Global>();
			services.AddSingleton<StackService>();
			services.AddScoped<ThemeSwitcher>();
			services.AddSingleton<QRCodeGenerator>();
			services.AddSingleton<IndexViewModel>();
			services.AddSingleton<ReceiveViewModel>();
			services.AddSingleton<SendViewModel>();
			services.AddSingleton<StatusViewModel>();
			services.AddTransient<LoadWalletViewModel>();
			services.AddTransient<WalletInfoViewModel>();
			services.AddTransient<NewPasswordViewModel>();
			services.AddTransient<CoinJoinViewModel>();
			services.AddSingleton<SelectCoinsViewModel>();

			return services;
		}

		public static async Task NavigateBack(this NavigationManager navigationManager, IJSRuntime runtime, string route)
		{
			navigationManager.NavigateTo(route);
		}
	}
}
