using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class QuickWallet
    {
        public Dictionary<string, string> create()
        {
            Key privateKey = new Key(); 
            BitcoinAddress address = privateKey.PubKey.GetAddress(Network.TestNet); //Get the public key, and derive the address on the Main network
            PubKey publicKey = privateKey.PubKey;
            var pubaddress = publicKey.GetAddress(Network.TestNet);
            var privatekeyOnNetwork = privateKey.GetBitcoinSecret(Network.TestNet);
            var map = new Dictionary<string, string>();
            map.Add(privatekeyOnNetwork.ToString(),pubaddress.ToString());
            return map;
        }
        
        public decimal getUnspent(Key privateKey)
        {           
            BitcoinAddress address = privateKey.PubKey.GetAddress(Network.TestNet);
            PubKey publicKey = privateKey.PubKey;
            var pubaddress = publicKey.GetAddress(Network.TestNet);
            var client = new QBitNinjaClient(Network.TestNet);
            var balanceModel = client.GetBalance(pubaddress, unspentOnly: true).Result;
           
            if (balanceModel.Operations.Count == 0)
            {
                throw new Exception("No coins to spend");
            }
            var unspentCoins = new List<Coin>();
            
            foreach (var operation in balanceModel.Operations)
            {
                
                unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
            }

            var balance = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            //   Console.WriteLine(balance.ToString());
            return balance;
            //var transactionId = uint256.Parse("f3fd12ad61a43a188229b68d2c92e687e34151fcbbe67d170e8bf0cd8f64e82d"); //	Query	the	transaction
            //GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            //foreach (var b in transactionResponse.ReceivedCoins)
            //{
            //    Console.WriteLine(b.Amount.ToString());
            //}

        }
        public string getSpent(Key privateKey)
        {
            BitcoinAddress address = privateKey.PubKey.GetAddress(Network.TestNet);
            PubKey publicKey = privateKey.PubKey;
            var pubaddress = publicKey.GetAddress(Network.TestNet);
            var client = new QBitNinjaClient(Network.TestNet);
            Console.ReadKey();
            
            var balanceModel = client.GetBalance(pubaddress, unspentOnly: true).Result;

            if (balanceModel.Operations.Count == 0)
            {
                throw new Exception("No coins to spend");
            }
            var spentCoins = new List<Coin>();
            foreach (var operation in balanceModel.Operations)
            {
                spentCoins.AddRange(operation.SpentCoins.Select(coin => coin as Coin));
            }

            var spent = spentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            return spent.ToString();
        }
        public string encrypt(string key,string password)
        {
           
            var privateKey = new BitcoinSecret(key);
            var encryptedkey = privateKey.Encrypt(password);

            return encryptedkey.ToString();
        }
        public string decrypt(string key,string password)
        {
            var privateKey = new BitcoinSecret(key);
           // BitcoinEncryptedSecret
        //    var encryptedkeyvar = privateKey.PrivateKey
            try
            {
             //   encryptedkey.GetKey(password).ToString(Network.TestNet);
            }
            catch (SecurityException e)
            {
                //Wrong Password or Network
            }
            return null;
        }
        public HashSet<ICoin> ugocoin(Key privateKey)
        {
            BitcoinAddress address = privateKey.PubKey.GetAddress(Network.TestNet);
            PubKey publicKey = privateKey.PubKey;
            var pubaddress = publicKey.GetAddress(Network.TestNet);
            var client = new QBitNinjaClient(Network.TestNet);
            var balanceModel = client.GetBalance(pubaddress, unspentOnly: true).Result;
            var ugoCoins = new HashSet<ICoin>();

            foreach (var operation in balanceModel.Operations)
            {
                foreach (var coin in operation.ReceivedCoins)
                {
                    if (coin.TxOut.ScriptPubKey.GetDestinationAddress(Network.TestNet) == address)
                    {
                        ugoCoins.Add(coin);
                    }
                }
            }
            return ugoCoins;
        }
    }

}
