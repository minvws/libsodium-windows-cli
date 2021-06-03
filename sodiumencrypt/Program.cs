using System;
using System.IO;
using System.Text;
using Sodium;

namespace SodiumEncrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                CreateKeyPair();
                Console.WriteLine("Generated a keypair..");

                return;
            }

            // Parse args
            var publicKeyFile = args[1];
            var privateKeyFile = args[2];
            
            // Init keypair
            byte[] publicKey = File.ReadAllBytes(publicKeyFile);
            byte[] privateKey = File.ReadAllBytes(privateKeyFile);
            var keyPair = new KeyPair(publicKey, privateKey);

            // Encrypt the UTF-8 text in the first arg + output
            if (args.Length == 3)
            {
                // Parse args
                var data = args[0];
                var dataBytes = Encoding.UTF8.GetBytes(data);
                
                // Encrypt
                var cypherBytes = Encrypt(keyPair, dataBytes);
                
                // Dump the cypher bytes to stdout
                var stdOut = Console.OpenStandardOutput();
                stdOut.Write(Encrypt(keyPair, cypherBytes));

                // Print the b64 encoding of the same
                var cypherData = Convert.ToBase64String(cypherBytes);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(cypherData);
            }

            // Decrypt the b64 encoded bytes in the last arg + print to console
            if (args.Length == 4)
            {
                // Parse args
                var data = args[3];

                // Encrypt
                var plainBytes = Decrypt(keyPair, Convert.FromBase64String(data));
                var plainText = Encoding.UTF8.GetString(plainBytes);
                
                // Print the plaintext
                Console.WriteLine(plainText);
            }
            
            Console.Read();
        }

        private static byte[] Encrypt(KeyPair keyPair, byte[] plainBytes)
        {
            return SealedPublicKeyBox.Create(plainBytes, keyPair);
        }

        private static byte[] Decrypt(KeyPair keyPair, byte[] cypherBytes)
        {
            return SealedPublicKeyBox.Open(cypherBytes, keyPair);
        }

        private static void CreateKeyPair()
        {
            var keyPair = PublicKeyBox.GenerateKeyPair();
            File.WriteAllBytes("public.key", keyPair.PublicKey);
            File.WriteAllBytes("private.key", keyPair.PrivateKey);
        }
    }
}

