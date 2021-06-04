// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using CommandLine;
using NaCLI.Options;
using Sodium;

namespace NaCLI
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal class Program
    {
        static void Main(string[] args)
        {
            var types = LoadVerbs();

            Parser.Default.ParseArguments(args, types)
                .WithParsed(Run)
                .WithNotParsed(HandleErrors);
        }

        private static Type[] LoadVerbs()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();
        }

        private static void Run(object options)
        {
            switch (options)
            {
                case EncryptOptions e:
                    HandleEncrypt(e);
                    break;
                case DecryptOptions d:
                    HandleDecrypt(d);
                    break;
                case GenerateKeyPairOptions g:
                    HandleCreateKeyPair(g);
                    break;
                default:
                    Console.WriteLine("Invalid or unknown options type");
                    Environment.Exit(-1);
                    break;
            }
        }

        private static void HandleErrors(IEnumerable<Error> errors)
        {
            Console.WriteLine("Error parsing input, please check your call and try again.");

            foreach (var e in errors) Console.WriteLine(e.Tag);

            Environment.Exit(-1);
        }

        private static void HandleEncrypt(EncryptOptions options)
        {
            byte[] publicKey = null;
            byte[] privateKey = null;

            try
            {
                publicKey = File.ReadAllBytes(options.PublicKeyFile);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "public key");
                Environment.Exit(-1);
            }

            try
            {
                privateKey = File.ReadAllBytes(options.PrivateKeyFile);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "private key");
                Environment.Exit(-1);
            }

            var keyPair = new KeyPair(publicKey, privateKey);

            // Get the message to encode
            var messageBytes = Encoding.UTF8.GetBytes(options.Message);

            // Encrypt
            byte[] cipherBytes;
            byte[] nonceBytes = null;
            if (options.SealedBox)
            {
                cipherBytes = SealedPublicKeyBox.Create(messageBytes, keyPair);
            }
            else
            { 
                nonceBytes = PublicKeyBox.GenerateNonce();
                cipherBytes = PublicKeyBox.Create(messageBytes, nonceBytes, keyPair.PrivateKey, keyPair.PublicKey);
            }

            // Encode the cipher text output
            var encodedCipherBytes = options.OutputEncoding switch
            {
                OutputEncodingType.b64 => Encoding.UTF8.GetBytes(Convert.ToBase64String(cipherBytes)),
                OutputEncodingType.hex => Encoding.UTF8.GetBytes(Convert.ToHexString(cipherBytes)),
                _ => Array.Empty<byte>()
            };

            Output(encodedCipherBytes, options.OutputFile, "Data");

            if (nonceBytes == null) Environment.Exit(0);

            // Encode the output
            var encodedNonceBytes = options.OutputEncoding switch
            {
                OutputEncodingType.b64 => Encoding.UTF8.GetBytes(Convert.ToBase64String(nonceBytes)),
                OutputEncodingType.hex => Encoding.UTF8.GetBytes(Convert.ToHexString(nonceBytes)),
                _ => Array.Empty<byte>()
            };

            Output(encodedNonceBytes, options.OutputFileNonce, "Nonce");
        }

        private static void Output(byte[] bytes, string outputFile, string contents)
        {
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                try
                {
                    File.WriteAllBytes(outputFile, bytes);
                    Console.WriteLine($"{contents} has been written to: { outputFile }");
                }
                catch (Exception e)
                {
                    HandleFileErrors(e, outputFile);
                    Environment.Exit(-1);
                }
            }
            else
            {
                Console.WriteLine($"{contents}: { Encoding.UTF8.GetString(bytes) }");
            }
        }

        private static void HandleDecrypt(DecryptOptions options)
        {
            byte[] publicKey = null;
            byte[] privateKey = null;

            try
            {
                publicKey = File.ReadAllBytes(options.PublicKeyFile);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "public key");
                Environment.Exit(-1);
            }

            try
            {
                privateKey = File.ReadAllBytes(options.PrivateKeyFile);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "private key");
                Environment.Exit(-1);
            }

            // If the length is greater than 32, it is probably base64 encoded.
            if(publicKey.Length > 32)
            {
                publicKey = Convert.FromBase64String(Encoding.UTF8.GetString(publicKey));
            }
            if(privateKey.Length > 32)
            {
                privateKey = Convert.FromBase64String(Encoding.UTF8.GetString(privateKey));
            }

            var keyPair = new KeyPair(publicKey, privateKey);

            // Get the message to decode
            byte[] messageBytes = null;
            switch (options.InputEncoding)
            {
                case InputEncodingType.b64:
                    messageBytes = Convert.FromBase64String(options.Message);
                    break;
                case InputEncodingType.hex:
                    messageBytes = Convert.FromHexString(options.Message);
                    break;
                default:
                    Console.WriteLine($"Unexpected error! Encoding {options.InputEncoding} was unexpected.");
                    Environment.Exit(-1);
                    break;
            }

            byte[] outputBytes;

            if (string.IsNullOrWhiteSpace(options.Nonce))
            {
                outputBytes = SealedPublicKeyBox.Open(messageBytes, keyPair);
            }
            else
            {
                byte[] nonceBytes = null;

                switch (options.InputEncoding)
                {
                    case InputEncodingType.b64:
                       nonceBytes = Convert.FromBase64String(options.Nonce);
                       break;
                    case InputEncodingType.hex:
                        nonceBytes = Convert.FromHexString(options.Nonce);
                        break;
                    default:
                        Console.WriteLine($"Unexpected error! Encoding {options.InputEncoding} was unexpected.");
                        Environment.Exit(-1);
                        break;
                }

                outputBytes = PublicKeyBox.Open(messageBytes, nonceBytes, keyPair.PrivateKey, keyPair.PublicKey);
            }

            // Save 
            using var stdOut = Console.OpenStandardOutput();
            stdOut.Write(outputBytes);
        }


        private static void HandleCreateKeyPair(GenerateKeyPairOptions options)
        {
            var keyPair = PublicKeyBox.GenerateKeyPair();

            try
            {
                File.WriteAllBytes(options.PublicKeyPath, keyPair.PublicKey);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "public key");
                Environment.Exit(-1);
            }

            try
            {
                File.WriteAllBytes(options.PrivateKeyPath, keyPair.PrivateKey);
            }
            catch (Exception e)
            {
                HandleFileErrors(e, "private key");
                Environment.Exit(-1);
            }

            Console.WriteLine("KeyPair generated successfully!");
            Console.WriteLine($"Public key is saved to: {options.PublicKeyPath}");
            Console.WriteLine($"Private key is saved to: {options.PublicKeyPath}");
        }

        private static void HandleFileErrors(Exception e, string fileName)
        {
            var error = "";

            switch (e)
            {
                case PathTooLongException:
                    error = "the path name is too long";
                    break;
                case DirectoryNotFoundException:
                    error = "the directory was not found";
                    break;
                case IOException:
                    error = "a general IO error occurred";
                    break;
                case UnauthorizedAccessException:
                case SecurityException:
                    error = "you have insufficient rights to save the file";
                    break;
                default:
                    Console.WriteLine("a unexpected error occurred");
                    break;
            }

            Console.WriteLine($"Error saving {fileName}: {error}.");
        }
    }
}
