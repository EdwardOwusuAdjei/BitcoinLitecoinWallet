using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using NBitcoin;
using System.Security;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = ""; // I have my keys on the testnet but still not sharing
            QuickWallet wallet = new QuickWallet();
            var walletinfo = wallet.create();
            Console.WriteLine("Enter bitcoin key");
            key = Console.ReadLine();
            var privateKey = new BitcoinSecret(key);
            Console.WriteLine("Public Address is " + privateKey.PubKey.GetAddress(Network.TestNet));
            var unspent = wallet.getUnspent(privateKey.PrivateKey);
            Console.WriteLine(unspent + " left");
            TransactionClass transact = new TransactionClass();
            string address = Console.ReadLine();
            transact.send(privateKey, address);
            Console.ReadKey();
        }
    }
}
