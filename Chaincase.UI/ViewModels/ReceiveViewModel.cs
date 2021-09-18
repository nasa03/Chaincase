using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chaincase.Common;
using Chaincase.Common.Services;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Keys;
using WalletWasabi.Logging;

namespace Chaincase.UI.ViewModels
{
    public class ReceiveViewModel : ReactiveObject
    {
        private readonly ChaincaseWalletManager _walletManager;
        private readonly Config _config;
        private P2EPServer _P2EPServer;
        private string _P2EPAddress;
        private Network _network => _config.Network;
        private bool _isBusy;
        private string _receiveType = "pj";
        private string _p2epAddress;

        public Money ProposedAmount { get; private set; }
        private string _proposedLabel;
        private bool[,] _qrCode;
        private string _requestAmount;

        public ReceiveViewModel(ChaincaseWalletManager walletManager, Config config, P2EPServer P2EPServer)
        {
            _walletManager = walletManager;
            _config = config;
            _P2EPServer = P2EPServer;
        }

        public void InitNextReceiveKey()
        {
            ReceivePubKey = _walletManager.CurrentWallet.KeyManager.GetNextReceiveKey(ProposedLabel, out bool minGapLimitIncreased);
            ProposedLabel = "";
        }

        public string AppliedLabel => ReceivePubKey.Label ?? "";
        public string Address => ReceivePubKey.GetP2wpkhAddress(_config.Network).ToString();
        public string Pubkey => ReceivePubKey.PubKey.ToString();
        public string KeyPath => ReceivePubKey.FullKeyPath.ToString();

        public HdPubKey ReceivePubKey { get; set; }

        public string BitcoinUri => $"bitcoin:{Address}";
        public string P2EPUri => $"bitcoin:{Address}?pj={_P2EPServer.PaymentEndpoint}";

        public void GenerateP2EP(string password)
        {
            if (!_P2EPServer.HiddenServiceIsOn)
            {
                StartPayjoin(password);
            }
            P2EPAddress = _P2EPServer.PaymentEndpoint;
        }

        public void StartPayjoin(string password)
        {
            var cts = new CancellationToken();
            _P2EPServer.StartAsync(cts);
            _P2EPServer.Password = password;
            Logger.LogInfo($"P2EP Server listening created: {_P2EPServer.PaymentEndpoint}");
        }

        public async Task TryStartPayjoin(string password)
        {
            IsBusy = true;
            string walletFilePath = Path.Combine(_walletManager.WalletDirectories.WalletsDir, $"{_network}.json");
            try
            {
                await Task.Run(() => KeyManager.FromFile(walletFilePath).GetMasterExtKey(password ?? ""));
                GenerateP2EP(password);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public string ProposedLabel
        {
            get => _proposedLabel;
            set => this.RaiseAndSetIfChanged(ref _proposedLabel, value);
        }

        public bool[,] QrCode
        {
            get => _qrCode;
            set => this.RaiseAndSetIfChanged(ref _qrCode, value);
        }

        public string RequestAmount
        {
            get => _requestAmount;
            set => this.RaiseAndSetIfChanged(ref _requestAmount, value);
        }

        public bool IsPayjoinLive => _P2EPServer?.HiddenServiceIsOn ?? false;

        public string P2EPAddress
        {
            get => _p2epAddress;
            set => this.RaiseAndSetIfChanged(ref _p2epAddress, value);
        }

        public string ReceiveType
        {
            get => _receiveType;
            set => this.RaiseAndSetIfChanged(ref _receiveType, value);
        }
    }
}
