﻿using System.IO;
using Chaincase.Common;
using Chaincase.Common.Contracts;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Helpers;
using WalletWasabi.Wallets;

namespace Chaincase.UI.ViewModels
{
	public class NewPasswordViewModel : ReactiveObject
	{
		private readonly WalletManager _walletManager;
		private readonly Config _config;
		private readonly UiConfig _uiConfig;
		private readonly IHsmStorage _hsm;

		public NewPasswordViewModel(WalletManager walletManager, Config config, UiConfig uiConfig, IHsmStorage hsmStorage)
		{
			_walletManager = walletManager;
			_config = config;
			_uiConfig = uiConfig;
			_hsm = hsmStorage;
		}

		public void SetPassword(string password)
		{
			// Here we are not letting anything that will be autocorrected later.
			// Generate wallet with password exactly as entered for compatibility.
			// Todo what do we do if PasswordHelper.Guard fails?
			PasswordHelper.Guard(password);

			string walletFilePath = Path.Combine(_walletManager.WalletDirectories.WalletsDir, $"{_config.Network}.json");
			KeyManager.CreateNew(out Mnemonic seedWords, password, walletFilePath);
			// MUST prompt permissions
			_hsm.SetAsync($"{_config.Network}-seedWords", seedWords.ToString());

			_uiConfig.HasSeed = true;
			_uiConfig.ToFile();
		}
	}
}
