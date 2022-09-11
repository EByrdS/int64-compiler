/*
  INT64 compiler - This class performs the lexical analysis, 
  (a.k.a. scanning).
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
                     Emmanuel Byrd, ITESM CEM
                     Carlos Reyes, ITESM CEM
                     Diego Rodríguez, ITESM CEM
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Int64Proy {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"                             
               (?<And>        [&][&]    )
              | (?<Comment>    ([/][/].*)|([/][*](.|\n)*?[*][/]) )
              | (?<Character>  '((\\[nrt\\'""])|(\\u [a-fA-F0-9]{6})|[^\n\r\t\'\\""])'  )
              | (?<String>     ""(\\[nrt""\\']|(\\u[a-fA-F0-9]{6})|([^\n\r\t\""\\']))*?""  )
              | (?<Or>    	   [|][|]    )
              | (?<Equal>      [=][=]    )
              | (?<NotEqual>   [!][=]    ) # faltó este
              | (?<GreaterEqual>  [>][=]  ) # faltó este
              | (?<LessEqual>     [<][=]  ) # faltó este
              | (?<Not>    	   [!]       ) 
			  | (?<SemiColon>  [;]   	 )
			  | (?<Colon>      [:]   	 ) # faltó este
			  | (?<Condition>  [\?]      ) # faltó este, hay que ver si funciona
			  | (?<Comma>      [,]   	 )
              | (?<Assign>     [=]       )
              | (?<Identifier> [a-zA-Z]([_]|[a-zA-Z]|[0-9])*)
              | (?<False>      false     )
              | (?<Minus>        [-]     )
              | (?<Newline>    \n        )
              | (?<ParLeft>    [(]       )
              | (?<ParRight>   [)]       )
              | (?<BracLeft>   [{]       )
              | (?<BracRight>  [}]       )
              | (?<Plus>       [+]       )
              | (?<Pow>        [*][*]    )
              | (?<BitUnShiftRight>  [>][>][>]   )
              | (?<BitNot>     [~]       )
              | (?<BitAND>     [&]       )
              | (?<BitOR>      [|]       )
              | (?<BitXOR>     \^       )
              | (?<BitShiftLeft> [<][<]     )
              | (?<BitShiftRight>  [>][>]   )
              | (?<IntBin>     0[Bb][01]+\b         ) 
              | (?<IntOct>     0[Oo][0-7]+\b        ) 
              | (?<IntHex>     0[Xx][0-9a-fA-F]+\b  ) 
              | (?<IntDec>     \d+\b                ) 
              | (?<IntLiteral> \d+       ) #orden mal
              | (?<Less>       [<]       )
              | (?<Greater>     [>]       ) #Faltó este
              | (?<True>       true      ) 
              | (?<Mul>        [*]       )
              | (?<Div>        [/]       )
              | (?<Mod>        [%]       )
              | (?<WhiteSpace> \s        )     # Must go anywhere after Newline.
              | (?<Other>      .         )     # Must be last: match any other character.
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"case", TokenCategory.CASE},
                {"continue", TokenCategory.CONTINUE},
                {"default", TokenCategory.DEFAULT},
                {"do", TokenCategory.DO},
                {"else", TokenCategory.ELSE},
                {"false", TokenCategory.FALSE},
                {"for", TokenCategory.FOR},
                {"if", TokenCategory.IF},
                {"in", TokenCategory.IN},
                {"return", TokenCategory.RETURN},
                {"switch", TokenCategory.SWITCH},
                {"true", TokenCategory.TRUE},
                {"var", TokenCategory.VAR},
                {"while", TokenCategory.WHILE}
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"And", TokenCategory.AND},
                {"Assign", TokenCategory.ASSIGN},
                {"BitAND", TokenCategory.BITWISE_AND},
                {"BitNot", TokenCategory.BITWISE_NOT},
                {"BitOR", TokenCategory.BITWISE_OR},
                {"BitShiftLeft", TokenCategory.BITWISE_SHIFT_LEFT},
                {"BitShiftRight", TokenCategory.BITWISE_SHIFT_RIGHT},
                {"BitUnShiftRight", TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT},
                {"BitXOR", TokenCategory.BITWISE_XOR},
                {"BracLeft", TokenCategory.BRACKET_OPEN},
                {"BracRight", TokenCategory.BRACKET_CLOSE},
                {"Character", TokenCategory.CHARACTER},  
                {"Colon", TokenCategory.COLON}, //Emmanuel
                {"Comma", TokenCategory.COMMA},
                {"Condition", TokenCategory.CONDITION}, //Emmanuel
                {"Div", TokenCategory.DIVISION},
                {"Equal", TokenCategory.EQUAL},
                {"Greater", TokenCategory.GREATER}, //Emmanuel
                {"GreaterEqual", TokenCategory.GREATEREQUAL}, //Emmanuel
                {"IntBin", TokenCategory.INTBIN},
                {"IntDec", TokenCategory.INTDEC},   
                {"IntHex", TokenCategory.INTHEX},
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"IntOct", TokenCategory.INTOCT},
                {"Less", TokenCategory.LESS},
                {"LessEqual", TokenCategory.LESSEQUAL}, //Emmanuel
                {"Minus", TokenCategory.MINUS},
                {"Mod", TokenCategory.REMAINDER},
                {"Mul", TokenCategory.MUL},
                {"Not", TokenCategory.NEG},
                {"NotEqual", TokenCategory.NOTEQUAL}, //Emmanuel
                {"Or", TokenCategory.OR},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"Plus", TokenCategory.PLUS},
                {"Pow", TokenCategory.POWER},
                {"SemiColon", TokenCategory.SEMICOLON},
                {"String", TokenCategory.STRING},
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Length > 0) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Length > 0 
                    || m.Groups["Comment"].Length > 0) {

                    foreach (var c in m.Value){ //Metodo Propio
                        if (c == '\n'){
                            row++;
                        }
                    }
                    // Skip white space and comments.

                } else if (m.Groups["Identifier"].Length > 0) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a INT64 keyword.
                        yield return newTok(m, keywords[m.Value]);                                               

                    } else { 

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["Other"].Length > 0) {

                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);

                } else {

                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Length > 0) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, 
                                   TokenCategory.EOF, 
                                   row, 
                                   input.Length - columnStart + 1);
        }
    }
}
