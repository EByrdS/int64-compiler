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

namespace Int64Proy{
    
    class Parser {
        
        static readonly ISet<TokenCategory> opComp =
            new HashSet<TokenCategory>() {
                TokenCategory.EQUAL,
                TokenCategory.NOTEQUAL
            };
            
        static readonly ISet<TokenCategory> opRel =
            new HashSet<TokenCategory>() {
                TokenCategory.LESS,
                TokenCategory.LESSEQUAL,
                TokenCategory.GREATER,
                TokenCategory.GREATEREQUAL
            }; 
            
        static readonly ISet<TokenCategory> opBitOr =
            new HashSet<TokenCategory>() {
                TokenCategory.BITWISE_OR,
                TokenCategory.BITWISE_XOR
            };    
            
        static readonly ISet<TokenCategory> opBitShift =
            new HashSet<TokenCategory>() {
                TokenCategory.BITWISE_SHIFT_LEFT,
                TokenCategory.BITWISE_SHIFT_RIGHT,
                TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT
            };   
            
        static readonly ISet<TokenCategory> opAdd =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS
            };   
            
        static readonly ISet<TokenCategory> opMul =
            new HashSet<TokenCategory>() {
                TokenCategory.MUL,
                TokenCategory.DIVISION,
                TokenCategory.REMAINDER
            }; 
            
        static readonly ISet<TokenCategory> opUnary =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS,
                TokenCategory.NEG,
                TokenCategory.BITWISE_NOT
            };   
            
        static readonly ISet<TokenCategory> firstOfStmt =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.SWITCH,
                TokenCategory.WHILE,
                TokenCategory.DO,
                TokenCategory.FOR,
                TokenCategory.BREAK,
                TokenCategory.CONTINUE,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON
            };     
        
        static readonly ISet<TokenCategory> firstOfExpr =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS,
                TokenCategory.NOT,
                TokenCategory.BITWISE_NOT,
                TokenCategory.IDENTIFIER,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.INT_LITERAL,
                TokenCategory.CHARACTER,
                TokenCategory.STRING,
                TokenCategory.BRACKET_OPEN,
                TokenCategory.PARENTHESIS_OPEN
            };
        
        static readonly ISet<TokenCategory> litSimple =
            new HashSet<TokenCategory>() {
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.INT_LITERAL,
                TokenCategory.CHARACTER,
            }; 
            
        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError(category, tokenStream.Current);                
            }
        }    
        // FUNCIONES DE APOYO
        
        public void Expr_Block(){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
        }
        
        public void Stmt_Block(){
            Expect(TokenCategory.BRACKET_OPEN);
            Stmt_List();
            Expect(TokenCategory.BRACKET_CLOSE);
        }
        // FIN DE FUNCIONES DE APOYO
        
        public void Program(){
            Def_List();
        }
        
        public void Def_List(){
            while (CurrentToken == TokenCategory.VAR || CurrentToken == TokenCategory.IDENTIFIER){
                if (CurrentToken == TokenCategory.VAR){
                    Var_Def();
                }else if (CurrentToken == TokenCategory.IDENTIFIER){
                    Fun_Def();
                }
            }
        }
        
        public void Var_Def(){
            Expect(TokenCategory.VAR);
            Id_List();
        }
        
        public void Id_List(){
            Expect(TokenCategory.IDENTIFIER);
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
            }
        }
        
        public void Fun_Def(){
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Param_List();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.BRACKET_OPEN);
            Var_Def_List();
            Stmt_List();
            Expect(TokenCategory.BRACKET_CLOSE);
        }
        
        public void Param_List(){
            if (CurrentToken == TokenCategory.IDENTIFIER){
                Id_List();
            }
        }
        
        public void Var_Def_List(){
            if (CurrentToken == TokenCategory.VAR){
                Var_Def();
            }
        }
        
        public void Stmt_List(){
            while (firstOfStmt.Contains(CurrentToken)){
                Stmt();
            }
        }
        
        public void Stmt(){
            switch (CurrentToken){
                
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.ASSIGN){
                        Expect(TokenCategory.ASSIGN);
                        Expr();
                        Expect(TokenCategory.SEMICOLON);
                    }else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        Expr_List();
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                    }
                    break;
                    
                case TokenCategory.IF:
                    Stmt_If();
                    break;
                
                case TokenCategory.SWITCH:
                    Stmt_Switch();
                    break;
                
                case TokenCategory.WHILE:
                    Stmt_While();
                    break;
                
                case TokenCategory.DO:
                    Stmt_Do_While();
                    break;
                
                case TokenCategory.FOR:
                    Stmt_For();
                    break;
                    
                case TokenCategory.BREAK:
                    Expect(TokenCategory.BREAK);
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.CONTINUE:
                    Expect(TokenCategory.CONTINUE);
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.RETURN:
                    Expect(TokenCategory.RETURN);
                    Expr();
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.SEMICOLON:
                    Expect(TokenCategory.SEMICOLON);
                    break;
            }
        }
        
        public void Expr_List(){
            if (firstOfExpr.Contains(CurrentToken)){
                Expr();
                while (CurrentToken == TokenCategory.COMMA){
                    Expect(TokenCategory.COMMA);
                    Expr();
                }
            }
        }
        
        public void Stmt_If(){
            Expect(TokenCategory.IF);
            Expr_Block();
            Stmt_Block();
            while (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                if (CurrentToken == TokenCategory.BRACKET_OPEN){
                    Stmt_Block();
                    break;
                }
                Expect(TokenCategory.IF);
                Expr_Block();
                Stmt_Block();
            }
        }
        
        public void Stmt_Switch(){
            Expect(TokenCategory.SWITCH);
            Expr_Block();
            Expect(TokenCategory.BRACKET_OPEN);
            Case_List();
            Default();
            Expect(TokenCategory.BRACKET_CLOSE);
        }
        
        public void Case_List(){
            while (CurrentToken == TokenCategory.CASE){
                Expect(TokenCategory.CASE);
                Lit_List();
                Expect(TokenCategory.COLON);
                Stmt_List();
            }
        }
        
        public void Lit_List(){
            Lit_Simple();
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                Lit_Simple();
            }
        }
        
        public void Lit_Simple(){
            switch (CurrentToken){
                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;
                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;
                case TokenCategory.INT_LITERAL:
                    Expect(TokenCategory.INT_LITERAL);
                    break;
                case TokenCategory.CHARACTER:
                    Expect(TokenCategory.CHARACTER);
                    break;
            }
        }
        
        public void Default(){
            if (CurrentToken == TokenCategory.DEFAULT){
                Expect(TokenCategory.DEFAULT);
                Expect(TokenCategory.COLON);
                Stmt_List();
            }
        }
        
        public void Stmt_While(){
            Expect(TokenCategory.WHILE);
            Expr_Block();
            Stmt_Block();
        }
        
        public void Stmt_Do_While(){
            Expect(TokenCategory.DO);
            Stmt_Block();
            Expect(TokenCategory.WHILE);
            Expr_Block();
            Expect(TokenCategory.SEMICOLON);
        }
        
        public void Stmt_For(){
            Expect(TokenCategory.FOR);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.IN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Stmt_Block();
        }
        
        public void Expr(){
            Expr_Or();
            if (CurrentToken == TokenCategory.CONDITION){
                Expect(TokenCategory.CONDITION);
                Expr();
                Expect(TokenCategory.COLON);
                Expr();
            }
        }
        
        public void Expr_Or(){
            Expr_And();
            while (CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
                Expr_And();
            }
        }
        
        public void Expr_And(){
            Expr_Comp();
            while (CurrentToken == TokenCategory.AND){
                Expect(TokenCategory.AND);
                Expr_Comp();
            }
        }
        
        public void Expr_Comp(){
            Expr_Rel();
            while (opComp.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Rel();
            }
        }
        
        public void Expr_Rel(){
            Expr_Bit_Or();
            while (opRel.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Bit_Or();
            }
        }
        
        public void Expr_Bit_Or(){
            Expr_Bit_And();
            while (opBitOr.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Bit_And();
            }
        }
        
        public void Expr_Bit_And(){
            Expr_Bit_Shift();
            while (CurrentToken == TokenCategory.BITWISE_AND){
                Expect(CurrentToken);
                Expr_Bit_Shift();
            }
        }
        
        public void Expr_Bit_Shift(){
            Expr_Add();
            while (opBitShift.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Add();
            }
        }
        
        public void Expr_Add(){
            Expr_Mul();
            while (opAdd.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Mul();
            }
        }
        
        public void Expr_Mul(){
            Expr_Pow();
            while (opMul.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Pow();
            }
        }
        
        public void Expr_Pow(){
            Expr_Unary();
            while (CurrentToken == TokenCategory.POWER){
                Expect(CurrentToken);
                Expr_Pow();
            }
        }
        
        public void Expr_Unary(){
            if (opUnary.Contains(CurrentToken)){
                Expect(CurrentToken);
                Expr_Unary();
            }else{
                Expr_Primary();
            }
        }
        
        public void Expr_Primary(){
            if (CurrentToken == TokenCategory.IDENTIFIER){
                Expect(TokenCategory.IDENTIFIER);
                if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    Expr_List();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                }
            }else if (litSimple.Contains(CurrentToken) || CurrentToken == TokenCategory.STRING){
                Expect(CurrentToken);
            }else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                Expr_Block();
            }else if (CurrentToken == TokenCategory.BRACKET_OPEN){
                Expect(TokenCategory.BRACKET_OPEN);
                if (litSimple.Contains(CurrentToken)){
                    Lit_List();
                }
                Expect(TokenCategory.BRACKET_CLOSE);
            }
            else {
                throw new SyntaxError(litSimple,tokenStream.Current);
                //throw new System.ArgumentException("Error in expresion", "original");
                //Console.WriteLine("Error");                
            }
        }
    }
}