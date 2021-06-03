using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NaCLI.Options
{
    [Verb("keygen")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class GenerateKeyPairOptions
    {
        [Option('O', "output-public-key", Required = true, HelpText = "Path/file name to save the public key")]
        public string PublicKeyPath { get; set; }

        [Option('o', "output-private-key", Required = true, HelpText = "Path/file name to save the public key")]
        public string PrivateKeyPath { get; set; }
    }
}