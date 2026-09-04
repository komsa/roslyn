// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Analyzer.Utilities;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Text.Analyzers.Komsa
{
    /// <summary>
    /// Exposes the CA1704 spell checker so it can be used outside of the analyzer.
    /// </summary>
    public static class SpellingProvider
    {
        private static readonly CodeAnalysisDictionary s_defaultDictionary = GetMainDictionary();

        /// <summary>
        /// Returns whether <paramref name="word"/> is contained in the built-in dictionary.
        /// </summary>
        public static bool IsWordSpelledCorrectly(string word)
            => !s_defaultDictionary.ContainsUnrecognizedWord(word) && s_defaultDictionary.ContainsRecognizedWord(word);

        /// <summary>
        /// Splits <paramref name="symbolName"/> into words and returns the ones that are misspelled.
        /// </summary>
        public static IEnumerable<string> GetMisspelledWords(string symbolName)
        {
            var parser = new WordParser(symbolName, WordParserOptions.SplitCompoundWords);

            string? word;
            while ((word = parser.NextWord()) != null)
            {
                if (!IsWordAcronym(word) && !IsWordNumeric(word) && !IsWordSpelledCorrectly(word))
                {
                    yield return word;
                }
            }
        }

        private static bool IsWordAcronym(string word) => word.All(char.IsUpper);

        private static bool IsWordNumeric(string word) => char.IsDigit(word[0]);

        private static CodeAnalysisDictionary GetMainDictionary()
            => SourceText.From(TextAnalyzersResources.Dictionary).Parse(CodeAnalysisDictionary.CreateFromDic);
    }
}
