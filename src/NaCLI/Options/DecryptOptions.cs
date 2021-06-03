// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NaCLI.Options
{
    [Verb("decrypt")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class DecryptOptions
    {
        [Option('e', "encoding", Required = false, HelpText = "Specifies the encoding used for the data: HEX or Base64.", Default = InputEncodingType.hex)]
        public InputEncodingType InputEncoding { get; set; }

        [Option('o', "output", Required = false, HelpText = "Optional output file; if set then the output will be directed towards this file instead of std:out")]
        public string OutputFile { get; set; }

        [Option('d', "data", Required = true, HelpText = "Data to encrypt encoded as a base64 string")]
        public string Message { get; set; }
        
        [Option('K', "public-key", Required = true, HelpText = "Path/file name to the public key")]
        public string PublicKeyFile { get; set; }

        [Option('k', "public-key", Required = true, HelpText = "Path/file name to the private key")]
        public string PrivateKeyFile { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum InputEncodingType
    {
        hex,
        b64
    }
}