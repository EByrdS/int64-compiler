/*
  INT64 compiler - Token categories for the scanner.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
                     Emmanuek Byrd, ITESM CEM
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

namespace Int64Proy {

    public enum TokenCategory {
        //KEYWORDS
        BREAK,
        CASE,
        CONTINUE,
        DEFAULT,
        DO,
        ELSE,
        FALSE,
        FOR,
        IF,
        IN,
        RETURN,
        SWITCH,
        TRUE,
        VAR,
        WHILE,

        //NON KEYWORDS
        AND,
        ASSIGN,
        BITWISE_AND,
        BITWISE_NOT,
        BITWISE_OR,
        BITWISE_SHIFT_LEFT,
        BITWISE_SHIFT_RIGHT,
        BITWISE_UNSIGNED_SHIFT_RIGHT,
        BITWISE_XOR,
        BRACKET_OPEN,
        BRACKET_CLOSE,
        CHARACTER,
        COLON,
        COMMA,
        CONDITION,
        DIVISION,
        EQUAL,
        GREATER,
        GREATEREQUAL,
        INTBIN,
        INTDEC,
        INTHEX,
        INT_LITERAL,
        INTOCT,
        LESS,
        LESSEQUAL,
        MINUS,
        REMAINDER,
        MUL,
        NEG,
        NOTEQUAL,
        OR,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        PLUS,
        POWER,
        SEMICOLON,
        STRING,
        
        //OTHER
        EOF,
        IDENTIFIER,
        ILLEGAL_CHAR,
    }
}

