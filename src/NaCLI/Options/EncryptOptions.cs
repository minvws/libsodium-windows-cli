// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NaCLI.Options
{
    [Verb("encrypt")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class EncryptOptions
    {
        [Option('d', "data", Required = true, HelpText = "Data to encrypt encoded as a base64 string")]
        public string Message { get; set; }

        [Option('K', "public-key", Required = true, HelpText = "Path/file name to the public key")]
        public string PublicKeyFile { get; set; }

        [Option('k', "public-key", Required = true, HelpText = "Path/file name to the private key")]
        public string PrivateKeyFile { get; set; }

        [Option('f', "output-encoding", Required = false, HelpText = "Specifies the encoding used for the output: hex or base64.", Default = OutputEncodingType.b64)]
        public OutputEncodingType OutputEncoding { get; set; }

        [Option('O', "output-data", Required = false, HelpText = "Path/file to the data output file; the encrypted data will be output to this file if provided")]
        public string OutputFile { get; set; }

        [Option('o', "output-nonce", Required = false, HelpText = "Path/file to the nonce output file; the nonce data will be output to this file if provided")]
        public string OutputFileNonce { get; set; }

        [Option('S', "sealed-box", Required = false, HelpText = "Encrypt the message using a sealed box so that the receiver cannot identify the sender.")]
        public bool SealedBox { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum OutputEncodingType
    {
        hex,
        b64
    }
}