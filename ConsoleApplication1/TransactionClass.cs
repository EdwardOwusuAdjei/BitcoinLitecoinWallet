using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class TransactionClass
    {
        public bool send(BitcoinSecret bitcoinPrivateKey, String sendtoAddress)
        {
            var network = bitcoinPrivateKey.Network;

            var address = bitcoinPrivateKey.GetAddress();

            var client = new QBitNinjaClient(network);

            Console.WriteLine(client.BaseAddress);
            QuickWallet quickwallet = new QuickWallet();
            decimal balance = quickwallet.getUnspent(bitcoinPrivateKey.PrivateKey);
            HashSet<ICoin> coin = quickwallet.ugocoin(bitcoinPrivateKey.PrivateKey);

            var received = new Transaction();
            received.Outputs.Add(new TxOut(Money.Coins(balance), bitcoinPrivateKey.ScriptPubKey));
           
            var transaction = new Transaction();

            var sendAddress = BitcoinPubKeyAddress.Create(sendtoAddress);

            TransactionBuilder builder = new TransactionBuilder();

            Transaction signed = builder
                .AddCoins(coin)
                .AddKeys(bitcoinPrivateKey.PrivateKey)
                .Send(sendAddress.ScriptPubKey.GetDestinationAddress(Network.TestNet), Money.Coins(1.0m))
                .SendFees(Money.Coins(0.0005m))
                .SetChange(bitcoinPrivateKey.PubKey.GetAddress(Network.TestNet))
                .BuildTransaction(true);//signed


            Console.WriteLine(builder.Verify(signed)); // Tells me if everything went on right :)

            GetTransactionResponse getTxResp = new GetTransactionResponse();
            do
            {
                if (builder.Verify(signed) == true)
                {
                    BroadcastResponse broadcastResponse = client.Broadcast(signed).Result;
                    Console.WriteLine(signed.GetHash());
                    Console.WriteLine("Hex : " + signed.ToHex());
                    getTxResp = client.GetTransaction(signed.GetHash()).Result;
                    if (getTxResp == null)
                    {
                        Console.WriteLine("Trying again");
                        Thread.Sleep(3000);
                        continue;
                    }
                    else
                    {
                        //  success = true;
                        Console.WriteLine("Broadcasted");
                        break;
                    }
                    if (!broadcastResponse.Success)
                    {
                        Console.WriteLine(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
                        Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
                    }
                    else
                    {
                        Console.WriteLine("Success! You can check out the hash of the tranx in any block explorer:");
                        Console.WriteLine(transaction.GetHash());
                    }
                }
            } while (getTxResp == null);
            return true;

        }
    }
}
