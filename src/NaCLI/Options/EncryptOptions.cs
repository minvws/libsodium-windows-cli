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

        [Option('f', "output-encoding", Required = false, HelpText = "Specifies the encoding used for the output: raw, hex or base64.", Default = OutputEncodingType.raw)]
        public OutputEncodingType OutputEncoding { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum OutputEncodingType
    {
        raw,
        hex,
        b64
    }
}