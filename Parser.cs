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
using System.Text.RegularExpressions;

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
                TokenCategory.NEG,
                TokenCategory.BITWISE_NOT,
                TokenCategory.IDENTIFIER,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.INT_LITERAL,
                TokenCategory.INTBIN,
                TokenCategory.INTDEC,
                TokenCategory.INTHEX,
                TokenCategory.INTOCT,
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
                TokenCategory.INTBIN,
                TokenCategory.INTDEC,
                TokenCategory.INTHEX,
                TokenCategory.INTOCT,
                TokenCategory.CHARACTER,
            }; 
            
        static readonly ISet<TokenCategory> firstOfExprPrim =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.BRACKET_OPEN,
                TokenCategory.STRING,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.INT_LITERAL,
                TokenCategory.INTBIN,
                TokenCategory.INTDEC,
                TokenCategory.INTHEX,
                TokenCategory.INTOCT,
                TokenCategory.CHARACTER
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
        
        public Node Expr_Block(){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Node r = Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            return r;
        }
        
        public Node Stmt_Block(){
            Expect(TokenCategory.BRACKET_OPEN);
            Node r = Stmt_List();
            Expect(TokenCategory.BRACKET_CLOSE);
            return r;
        }
        // FIN DE FUNCIONES DE APOYO
        
        public Node Program(){
            Node r = Def_List();
            return r;
        }
        
        public Node Def_List(){
            Def_List_Node r = new Def_List_Node();
            while (CurrentToken == TokenCategory.VAR || CurrentToken == TokenCategory.IDENTIFIER){
                if (CurrentToken == TokenCategory.VAR){
                    r.Add(Var_Def());
                }else if (CurrentToken == TokenCategory.IDENTIFIER){
                    r.Add(Fun_Def());
                }
            }
            return r;
        }
        
        public Node Var_Def(){
            Var_Def_Node r = new Var_Def_Node(){
                AnchorToken = Expect(TokenCategory.VAR)
            };
            r.Add(Id_List());
            Expect(TokenCategory.SEMICOLON);
            return r;
        }
        
        public Node Id_List(){
            Id_List_Node r = new Id_List_Node();
            Id_Node i = new Id_Node(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            r.Add(i);
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                i = new Id_Node(){
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                };
                r.Add(i);
            }
            return r;
        }
        
        public Node Fun_Def(){
            Fun_Def_Node r = new Fun_Def_Node(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            
            Expect(TokenCategory.PARENTHESIS_OPEN);
            r.Add(Param_List());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            
            Expect(TokenCategory.BRACKET_OPEN);
            r.Add(Var_Def_List());
            r.Add(Stmt_List());
            Expect(TokenCategory.BRACKET_CLOSE);
            
            return r;
        }
        
        public Node Param_List(){
            Param_List_Node r = new Param_List_Node();
            if (CurrentToken == TokenCategory.IDENTIFIER){
                r.Add(Id_List());
            }else{
                r.Add(new Empty_Node());
            };
            return r;
        }
        
        public Node Var_Def_List(){
            Var_Def_List_Node r = new Var_Def_List_Node();
            while (CurrentToken == TokenCategory.VAR){
                r.Add(Var_Def());
            };
            return r;
        }
        
        public Node Stmt_List(){
            Stmt_List_Node r = new Stmt_List_Node();
            while (firstOfStmt.Contains(CurrentToken)){
                r.Add(Stmt());
            };
            return r;
        }
        
        public Node Stmt(){
            Node r = new Empty_Node();
            switch (CurrentToken){
                
                case TokenCategory.IDENTIFIER:
                    Token k = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.ASSIGN){
                        Expect(TokenCategory.ASSIGN);
                        r = new Assign_Node();
                        r.Add(new Id_Node{AnchorToken = k});
                        r.Add(Expr());
                        Expect(TokenCategory.SEMICOLON);
                    }else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                        Expect(TokenCategory.PARENTHESIS_OPEN);
                        r = new Fun_Call_Node(){
                            AnchorToken = k
                        };
                        r.Add(Expr_List());
                        Expect(TokenCategory.PARENTHESIS_CLOSE);
                    }
                    break;
                    
                case TokenCategory.IF:
                    r = Stmt_If();
                    break;
                
                case TokenCategory.SWITCH:
                    r = Stmt_Switch();
                    break;
                
                case TokenCategory.WHILE:
                    r = Stmt_While();
                    break;
                
                case TokenCategory.DO:
                    r = Stmt_Do_While();
                    break;
                
                case TokenCategory.FOR:
                    r = Stmt_For();
                    break;
                    
                case TokenCategory.BREAK:
                    r = new Break_Node(){
                        AnchorToken = Expect(TokenCategory.BREAK)
                    };
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.CONTINUE:
                    r = new Continue_Node(){
                        AnchorToken = Expect(TokenCategory.CONTINUE)
                    };
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.RETURN:
                    r = new Return_Node(){
                        AnchorToken = Expect(TokenCategory.RETURN)
                    };
                    r.Add(Expr());
                    Expect(TokenCategory.SEMICOLON);
                    break;
                    
                case TokenCategory.SEMICOLON:
                    r = new Empty_Node();
                    Expect(TokenCategory.SEMICOLON);
                    break;
                
                default:
                    throw new SyntaxError(firstOfStmt,tokenStream.Current);               
            }
            return r;
        }
        
        public Node Expr_List(){
            Node r = new Empty_Node();
            if (firstOfExpr.Contains(CurrentToken)){
                r = new Expr_List_Node();
                r.Add(Expr());
                while (CurrentToken == TokenCategory.COMMA){
                    Expect(TokenCategory.COMMA);
                    r.Add(Expr());
                }
            }
            return r;
        }
        
        public Node Stmt_If(){
            Stmt_If_Node r = new Stmt_If_Node(){
                AnchorToken = Expect(TokenCategory.IF)
            };
            r.Add(Expr_Block());
            r.Add(Stmt_Block());
            while (CurrentToken == TokenCategory.ELSE){
                Else_Node e = new Else_Node(){
                    AnchorToken = Expect(TokenCategory.ELSE)
                };
                if (CurrentToken == TokenCategory.BRACKET_OPEN){
                    e.Add(Stmt_Block());
                    r.Add(e);
                    break;
                }
                /*
                Stmt_If_Node ei = new Stmt_If_Node(){
                    AnchorToken = Expect(TokenCategory.IF)
                };
                ei.Add(Expr_Block());
                ei.Add(Stmt_Block());
                e.Add(ei);
                */
                e.Add(Stmt_If()); // añadido 6 mayo para poder tener else-if infinitos
                r.Add(e);
            }
            return r;
        }
        
        public Node Stmt_Switch(){
            Stmt_Switch_Node r = new Stmt_Switch_Node(){
                AnchorToken = Expect(TokenCategory.SWITCH)
            };
            r.Add(Expr_Block());
            Expect(TokenCategory.BRACKET_OPEN);
            r.Add(Case_List());
            r.Add(Default());
            Expect(TokenCategory.BRACKET_CLOSE);
            return r;
        }
        
        public Node Case_List(){
            Case_List_Node r = new Case_List_Node();
            while (CurrentToken == TokenCategory.CASE){
                Case_Node c = new Case_Node(){
                    AnchorToken = Expect(TokenCategory.CASE)
                };
                c.Add(Lit_List());
                Expect(TokenCategory.COLON);
                c.Add(Stmt_List());
                r.Add(c);
            }
            return r;
        }
        
        public Node Lit_List(){
            Node r = new Lit_List_Node();
            r.Add(Lit_Simple());
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                r.Add(Lit_Simple());
            }
            return r;
        }
        
        public Node Lit_Simple(){
            Node r;
            switch (CurrentToken){
                case TokenCategory.TRUE:
                    r = new True_Node();
                    r.AnchorToken = Expect(TokenCategory.TRUE);
                    break;
                case TokenCategory.FALSE:
                    r = new False_Node();
                    r.AnchorToken = Expect(TokenCategory.FALSE);
                    break;
                case TokenCategory.INT_LITERAL:
                    r = new Int_Lit_Node();
                    r.AnchorToken = Expect(TokenCategory.INT_LITERAL);
                    break;
                case TokenCategory.INTBIN:
                    r = new Int_Bin_Node();
                    r.AnchorToken = Expect(TokenCategory.INTBIN);
                    break;
                case TokenCategory.INTDEC:
                    r = new Int_Dec_Node();
                    r.AnchorToken = Expect(TokenCategory.INTDEC);
                    break;
                case TokenCategory.INTHEX:
                    r = new Int_Hex_Node();
                    r.AnchorToken = Expect(TokenCategory.INTHEX);
                    break;
                case TokenCategory.INTOCT:
                    r = new Int_Oct_Node();
                    r.AnchorToken = Expect(TokenCategory.INTOCT);
                    break;
                case TokenCategory.CHARACTER:
                    r = new Character_Node();
                    r.AnchorToken = Expect(TokenCategory.CHARACTER);
                    break;
                case TokenCategory.MINUS:
                    r = new Add_Node(){
                        AnchorToken = Expect(CurrentToken)
                    };
                    r.Add(
                        new Int_Lit_Node(){
                            AnchorToken = new Token("0", TokenCategory.INT_LITERAL, r.AnchorToken.Row, r.AnchorToken.Column)
                        }
                    );
                    r.Add(Lit_Simple());
                    break;
                default:
                    throw new SyntaxError(litSimple,tokenStream.Current);                
            }
            if (litSimple.Contains(r.AnchorToken.Category)){
                        
                        r.AnchorToken = new Token(Convert(r.AnchorToken.Lexeme, r.AnchorToken.Category),
                                                    r.AnchorToken.Category,
                                                    r.AnchorToken.Row,
                                                    r.AnchorToken.Column);
                        
                    }
            return r;
        }
        
        public Node Default(){
            Node r = new Empty_Node();
            if (CurrentToken == TokenCategory.DEFAULT){
                r = new Default_Node(){
                    AnchorToken = Expect(TokenCategory.DEFAULT)
                };
                Expect(TokenCategory.COLON);
                r.Add(Stmt_List());
            }
            return r;
        }
        
        public Node Stmt_While(){
            Stmt_While_Node r = new Stmt_While_Node(){
                AnchorToken = Expect(TokenCategory.WHILE)
            };
            r.Add(Expr_Block());
            r.Add(Stmt_Block());
            return r;
        }
        
        public Node Stmt_Do_While(){
            Stmt_Do_While_Node r = new Stmt_Do_While_Node(){
                AnchorToken = Expect(TokenCategory.DO)
            };
            r.Add(Stmt_Block());
            Expect(TokenCategory.WHILE);
            r.Add(Expr_Block());
            Expect(TokenCategory.SEMICOLON);
            return r;
        }
        
        public Node Stmt_For(){
            Stmt_For_Node r = new Stmt_For_Node(){
                AnchorToken = Expect(TokenCategory.FOR)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Id_Node i = new Id_Node(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            In_Node n = new In_Node(){
                AnchorToken = Expect(TokenCategory.IN)
            };
            n.Add(i);
            n.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            r.Add(n);
            r.Add(Stmt_Block());
            return r;
        }
        
        public Node Expr(){
            Node r = Expr_Or();
            if (CurrentToken == TokenCategory.CONDITION){
                Stmt_If_Node c = new Stmt_If_Node(){
                    AnchorToken = Expect(TokenCategory.CONDITION)
                };
                c.Add(r);
                c.Add(Expr());
                Else_Node e = new Else_Node(){
                    AnchorToken = Expect(TokenCategory.COLON)
                };
                e.Add(Expr());
                c.Add(e);
                r = c;
            }
            return r;
        }
        
        public Node Expr_Or(){
            Node r = Expr_And();
            while (CurrentToken == TokenCategory.OR){
                Or_Node c = new Or_Node(){
                    AnchorToken = Expect(TokenCategory.OR)
                };
                c.Add(r);
                c.Add(Expr_And());
                r = c;
            }
            return r;
        }
        
        public Node Expr_And(){
            Node r = Expr_Comp();
            while (CurrentToken == TokenCategory.AND){
                And_Node a = new And_Node(){
                    AnchorToken = Expect(TokenCategory.AND)
                };
                a.Add(r);
                a.Add(Expr_Comp());
                r = a;
            }
            return r;
        }
        
        public Node Expr_Comp(){
            Node r = Expr_Rel();
            while (opComp.Contains(CurrentToken)){
                Comp_Node c = new Comp_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                c.Add(r);
                c.Add(Expr_Rel());
                r = c;
            }
            return r;
        }
        
        public Node Expr_Rel(){
            Node r = Expr_Bit_Or();
            while (opRel.Contains(CurrentToken)){
                Rel_Node c = new Rel_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                c.Add(r);
                c.Add(Expr_Bit_Or());
                r = c;
            }
            return r;
        }
        
        public Node Expr_Bit_Or(){
            Node r = Expr_Bit_And();
            while (opBitOr.Contains(CurrentToken)){
                BitOr_Node b = new BitOr_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                b.Add(r);
                b.Add(Expr_Bit_And());
                r = b;
            }
            return r;
        }
        
        public Node Expr_Bit_And(){
            Node r = Expr_Bit_Shift();
            while (CurrentToken == TokenCategory.BITWISE_AND){
                BitAnd_Node a = new BitAnd_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                a.Add(r);
                a.Add(Expr_Bit_Shift());
                r = a;
            }
            return r;
        }
        
        public Node Expr_Bit_Shift(){
            Node r = Expr_Add();
            while (opBitShift.Contains(CurrentToken)){
                BitShift_Node s = new BitShift_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                s.Add(r);
                s.Add(Expr_Add());
                r = s;
            }
            return r;
        }
        
        public Node Expr_Add(){
            Node r = Expr_Mul();
            while (opAdd.Contains(CurrentToken)){
                Add_Node a = new Add_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                a.Add(r);
                a.Add(Expr_Mul());
                r = a;
            }
            return r;
        }
        
        public Node Expr_Mul(){
            Node r = Expr_Pow();
            while (opMul.Contains(CurrentToken)){
                Mul_Node m = new Mul_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                m.Add(r);
                m.Add(Expr_Pow());
                r = m;
            }
            return r;
        }
        
        public Node Expr_Pow(){
            Node r = Expr_Unary();
            while (CurrentToken == TokenCategory.POWER){
                Pow_Node p = new Pow_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                p.Add(r);
                p.Add(Expr_Pow());
                r = p;
            }
            return r;
        }
        
        public Node Expr_Unary(){
            Node r;
            if (opUnary.Contains(CurrentToken)){
                r = new Unary_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                r.Add(Expr_Unary());
            }else{
                r = Expr_Primary();
            }
            return r;
        }
        
        public Node Expr_Primary(){
            Node r;
            if (CurrentToken == TokenCategory.IDENTIFIER){
                Token k = Expect(TokenCategory.IDENTIFIER);
                if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                    r = new Fun_Call_Node(){
                        AnchorToken = k
                    };
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    r.Add(Expr_List());
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                }else{
                    r = new Id_Node(){
                        AnchorToken = k
                    };
                }
            }else if (litSimple.Contains(CurrentToken)){
                r = new Lit_Simple_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                
                r.AnchorToken = new Token(Convert(r.AnchorToken.Lexeme, r.AnchorToken.Category),
                                            r.AnchorToken.Category,
                                            r.AnchorToken.Row,
                                            r.AnchorToken.Column);
                                            
            }else if (CurrentToken == TokenCategory.STRING){    
                r = new Lit_Simple_Node(){
                    AnchorToken = Expect(CurrentToken)
                };
                r.AnchorToken = new Token(ProcessString(r.AnchorToken.Lexeme, r.AnchorToken),
                                            r.AnchorToken.Category,
                                            r.AnchorToken.Row,
                                            r.AnchorToken.Column);
                                            
            }else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN){
                r = Expr_Block();
            }else if (CurrentToken == TokenCategory.BRACKET_OPEN){
                Expect(TokenCategory.BRACKET_OPEN);
                r = new Array_Node();
                if (litSimple.Contains(CurrentToken)){
                    r.Add(Lit_List());
                }
                Expect(TokenCategory.BRACKET_CLOSE);
            }
            else {
                throw new SyntaxError(firstOfExprPrim,tokenStream.Current);
            }
            return r;
        }
        
        
        public string Convert(string l, TokenCategory c){
            long temp = 0;
            var b = 10;
            switch (c){
                case TokenCategory.INTBIN:
                    l = l.Substring(2);
                    b = 2;
                    break;
                case TokenCategory.INTHEX:
                    l = l.Substring(2);
                    b = 16;
                    break;
                case TokenCategory.INTOCT:
                    l = l.Substring(2);
                    b = 8;
                    break;
                case TokenCategory.CHARACTER:
                    char[] cA = l.ToCharArray();
                    if ((int)cA[1] == 92){
                        switch ((int)cA[2]){
                            case 110:
                                return "10";
                            case 114:
                                return "13";
                            case 116:
                                return "9";
                            case 34:
                                return "34";
                            case 39:
                                return "39";
                            case 92:
                                return "92";
                            case 117:
                                char [] num = new char[6];
                                num[0]=cA[3];
                                num[1]=cA[4];
                                num[2]=cA[5];
                                num[3]=cA[6];
                                num[4]=cA[7];
                                num[5]=cA[8];
                                int temp2 = 0;
                                for (int k = 0; k < 6; k++)
                                {   
                                    temp2 += (int)Math.Pow(16, k)*Char(num[5-k]);
                                }
                                return temp2.ToString();
                        }
                    }else{
                        return ((int)cA[1]).ToString();
                    }
                    
                    break;
                case TokenCategory.TRUE:
                    return "1";
                case TokenCategory.FALSE:
                    return "0";
            }
            
            char[] charArray = l.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                temp +=(long)Math.Pow(b, i)*Char(charArray[charArray.Length - i - 1]);
            }
            return temp.ToString();
            
        }
        
        public string ProcessString(string s, Token AnchorToken){
            string newS = "";
            for (int i=0; i < s.Length; i++){
                if (s[i] == '\\'){
                    i++;
                    switch(s[i]){
                        case 'n':
                            newS += (char)10;
                            break;
                        case 'r':
                            newS += (char)13;
                            break;
                        case 't':
                            newS += (char)9;
                            break;
                        case '\\':
                            newS += '\\';
                            break;
                        case (char)39:
                            newS += (char)39;
                            break;
                        case (char)34:
                            newS += (char)34;
                            break;
                        case 'u':
                            char [] num = new char[6];
                            num[0]=s[++i];
                            num[1]=s[++i];
                            num[2]=s[++i];
                            num[3]=s[++i];
                            num[4]=s[++i];
                            num[5]=s[++i];
                            int temp = 0;
                            for (int k = 0; k < 6; k++)
                            {   
                                temp += (int)Math.Pow(16, k)*Char(num[5-k]);
                            }
                            newS += (char)temp;
                            break;
                        default:
                            throw new SyntaxError(TokenCategory.STRING, AnchorToken);
                    }
                }else{
                    newS+= s[i];
                }
            }
            return newS;
        }
        public int Char(char c){
            switch (c){
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    return (int)Decimal.Parse(c.ToString());
            }
        }
    }
}