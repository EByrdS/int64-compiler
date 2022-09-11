/*
  Int64 compiler - Semantic analyzer.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
  
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

namespace Int64Proy {

    class SemanticAnalyzer {
    
        int CycleCounter = 0; //Está bien escrito el contador de esta forma? para saber si podemos ejecutar un break o un continue.
        bool InsideFunctionDeclaration = false;
        bool DeclaringVariables = false;
        bool DeclaringParameters = false;
        public SymbolTable LocalVariableTable;

        public SymbolTable VariableTable {
            get;
            private set;
        }
        
        //-----------------------------------------------------------
        public FunctionTable FuncTable {
            get;
            private set;
        }
        
        //-----------------------------------------------------------
        public TableDictionary LocalVariableTableDictionary{
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            FuncTable = new FunctionTable("FunctionNames");
            VariableTable = new SymbolTable("GlobalVariables");
            LocalVariableTableDictionary = new TableDictionary();
            
            FuncTable["printi"] = 1;
            FuncTable["printc"] = 1;
            FuncTable["prints"] = 1;
            FuncTable["println"] = 0;
            FuncTable["readi"] = 0;
            FuncTable["reads"] = 0;
            FuncTable["new"] = 1;
            FuncTable["size"] = 1;
            FuncTable["add"] = 2;
            FuncTable["get"] = 2;
            FuncTable["set"] = 3;
            
        }


        //-----------------------------------------------------------
        //FIRST TREE SWAP TO CREATE GLOBAL VARIABLES AND FUNCTIONS
        public void VisitFirst(Def_List_Node node) { // el Nodo principal
            foreach (var n in node.children) {
                VisitFirst((dynamic) n);
            }
            if (!FuncTable.Contains("main")){
                throw new SemanticError(
                    "Program must have a 'main' method",
                    node[0].AnchorToken);
            }
            
            Visit((dynamic) node);
        }

        public void VisitFirst(Var_Def_Node node){
            VisitFirst((dynamic) node[0]); //Tiene siempre un hijo y solo uno, siendo un id_list
        }
        
        public void VisitFirst(Id_List_Node node){
            foreach (var n in node.children) {
                VisitFirst((dynamic) n); //Tiene siempre uno o muchos hijos Id_Node
            }
        }
        
        public void VisitFirst(Id_Node node){
            //Este nodo es visitado tanto dentro como fuera de funciones
            var lexeme = node.AnchorToken.Lexeme;
            if (VariableTable.Contains(lexeme)){
                throw new SemanticError(
                    "Duplicated global variable definition: " + lexeme,
                    node.AnchorToken);
            }else{
                VariableTable[lexeme] = Type.GLOBAL;
            }
        }
        
        public void VisitFirst(Fun_Def_Node node){
            var lexeme = node.AnchorToken.Lexeme;
            if (FuncTable.Contains(lexeme)){
                throw new SemanticError(
                    "Duplicated function definition: " + lexeme,
                    node.AnchorToken);
            }else{
                
                int paramNumber = 0;
                foreach (var a in node[0].children){
                    foreach (var c in a.children){
                        paramNumber++;
                    }
                }
                FuncTable[lexeme] = paramNumber;
            }
        }
        
        //-----------------------------------------------------------
        
        
        
        
        public void Visit(Def_List_Node node) { // el Nodo principal
            foreach (var n in node.children) {
                Visit((dynamic) n);
            }
            if (!FuncTable.Contains("main")){
                throw new SemanticError(
                    "Program must have a 'main' method",
                    node[0].AnchorToken); //Es correcto enviar un null en lugar de un AnchorToken?, ya que en este caso no sabría cuál anchorToken enviar.
            }
        }

        public void Visit(Var_Def_Node node){
            DeclaringVariables = true;
            Visit((dynamic) node[0]); //Tiene siempre un hijo y solo uno, siendo un id_list
            DeclaringVariables = false;
        }
        
        public void Visit(Id_List_Node node){
            foreach (var n in node.children) {
                Visit((dynamic) n); //Tiene siempre uno o muchos hijos Id_Node
            }
        }
        
        public void Visit(Id_Node node){
            //Este nodo es visitado tanto dentro como fuera de funciones
            var lexeme = node.AnchorToken.Lexeme;
            if (DeclaringVariables){
                if (InsideFunctionDeclaration){
                    if (LocalVariableTable.Contains(lexeme)){
                        throw new SemanticError(
                            "Duplicated local variable definition: " + lexeme,
                            node.AnchorToken);
                    }else{
                        LocalVariableTable[lexeme] = Type.LOCAL;
                    }
                }/*else{
                    if (VariableTable.Contains(lexeme)){
                        throw new SemanticError(
                            "Duplicated global variable definition:" + lexeme,
                            node.AnchorToken);
                    }else{
                        VariableTable[lexeme] = Type.GLOBAL;
                    }
                }*/
            }else{
                if (DeclaringParameters){
                    LocalVariableTable[lexeme] = Type.PARAM;
                }
                if (!LocalVariableTable.Contains(lexeme) && !VariableTable.Contains(lexeme)){ // revisar que la variable que se esta solicitando haya sido declarada
                    throw new SemanticError(
                        "Undefined variable: " + lexeme,
                        node.AnchorToken);
                }
            }
        }
        
        public void Visit(Fun_Def_Node node){
            InsideFunctionDeclaration = true;
            LocalVariableTable = new SymbolTable(node.AnchorToken.Lexeme);
            
            var lexeme = node.AnchorToken.Lexeme;
            DeclaringParameters = true;
            Visit((dynamic) node[0]); // el primer hijo siempre es un Param_List_Node
            DeclaringParameters = false;
            Visit((dynamic) node[1]); // el segundo hijo siempre es un Var_Def_List_Node
            Visit((dynamic) node[2]); // el tercer hijo siempre es un Stmt_List_Node
            
            InsideFunctionDeclaration = false;
            LocalVariableTableDictionary[node.AnchorToken.Lexeme] = LocalVariableTable;
        }
        
        public void Visit(Param_List_Node node){
            foreach (var n in node.children){
                Visit((dynamic) n); // si tiene un hijo será siempre Id_List_Node
            }
        }
        
        public void Visit(Var_Def_List_Node node){
            foreach (var n in node.children){
                Visit((dynamic) n); // tendrá 0 o más hijos de tipo Var_Def_Node
            }
        }
        public void Visit(Stmt_List_Node node){
            foreach (var n in node.children){
                Visit((dynamic) n); // tendrá 0 o más hijos de tipo desconocido, podrán ser nodos Empty_Node
            }
        }
        
        public void Visit(Assign_Node node){
            Visit((dynamic) node[0]); //el hijo con el nombre de la variable
            Visit((dynamic) node[1]); //el hijo con la expresión a evaluar, de tipo desconocido (o simplemente complejo de identificar ahora)
        }
        
        public void Visit(Fun_Call_Node node){
            if (!FuncTable.Contains(node.AnchorToken.Lexeme)){
                throw new SemanticError(
                    "Undefined function: " + node.AnchorToken.Lexeme,
                    node.AnchorToken);
            }
            int p = 0;
            foreach (var child in node[0].children){
                p++;
            }
            if (FuncTable[node.AnchorToken.Lexeme] != p){
                throw new SemanticError(
                    "Unvalid function call '" + node.AnchorToken.Lexeme + "': expecting " + FuncTable[node.AnchorToken.Lexeme] 
                    + " arguments but received "+ p,
                    node.AnchorToken);
            }
            Visit((dynamic) node[0]); // visitar las expresiones referentes a los parametros enviados a la llamada de la función
        }
        
        public void Visit(Break_Node node){
            if (CycleCounter == 0){
                throw new SemanticError(
                    "Cannot call break outside a loop",
                    node.AnchorToken);
            }
        }
        
        public void Visit(Continue_Node node){
            if (CycleCounter == 0){
                throw new SemanticError(
                    "Cannot call continue outside a loop",
                    node.AnchorToken);
            }
        }
        
        public void Visit(Return_Node node){
            if (!InsideFunctionDeclaration){
                throw new SemanticError(
                    "Can not call return outside a function definition",
                    node.AnchorToken);
            }
            Visit((dynamic) node[0]); // expresión
        }
        
        public void Visit(Empty_Node node){
        }
        
        public void Visit(Expr_List_Node node){
            foreach (var n in node.children) {
                Visit((dynamic) n);
            }
        }
        
        public void Visit(Stmt_If_Node node){
            foreach (var n in node.children) {
                Visit((dynamic) n); //Tendrá dos o tres hijos, el primero una expresión y los demás statements
            }
        }
        
        public void Visit(Else_Node node){
            // El Anchor Token de este node será 'else' o ':' , dependiendo de dónde fue llamado, su hijo son los statements a ejecutar en caso de if-false
            Visit((dynamic) node[0]); 
        }
        
        public void Visit(Stmt_Switch_Node node){
            Visit((dynamic) node[0]); //Expresión
            Visit((dynamic) node[2]); //Case List
            Visit((dynamic) node[1]); //Default
        }
        
        public void Visit(Case_List_Node node){
            
            List<string> literals = new List<string>();
            
            foreach (var cnode in node.children) {
                foreach (var litSimple in cnode[0].children){
                    if (literals.Contains(litSimple.AnchorToken.Lexeme)){
                        throw new SemanticError(
                            "Repeated 'case' value: " + litSimple.AnchorToken.Lexeme,
                            litSimple.AnchorToken);
                    }
                    literals.Add(litSimple.AnchorToken.Lexeme);
                }
            }
            
            foreach (var n in node.children) {
                Visit((dynamic) n); //cada uno de estos es un case diferente
            }
        }
        
        public void Visit(Case_Node node){
            Visit((dynamic) node[0]); //lit-list
            Visit((dynamic) node[1]); //stmt-list
        }
        
        public void Visit(Lit_List_Node node){
            foreach (var n in node.children) {
                Visit((dynamic) n); //cada uno de estos es un nodo de literal simple
                // true, false, int_literal, int_bin, int_hex, int_oct, int_dec, char
            }
        }
        
        public void Visit(Lit_Simple_Node node){
            // el AnchorToken de este node es una literal simple, el equivalente a los nodos de true, false, int_literal, int_bin, etc, etc o un STRING
            // muy similar al comportamiento de la función que visita Lit_List_Node
        }
        
        public void Visit(Array_Node node){
            Visit((dynamic) node[0]); //siempre tiene un hijo, siendo una lista de literales
        }
        
        public void Visit(Stmt_While_Node node){ //AnchorToken: While
            CycleCounter ++;
            Visit((dynamic) node[0]); //Expresión
            Visit((dynamic) node[1]); //Declaración
            CycleCounter--;
        }
        
        public void Visit(Stmt_Do_While_Node node){ //AnchorToken: Do
            CycleCounter++;
            Visit((dynamic) node[0]); //Declaración
            Visit((dynamic) node[1]); //Expresión
            CycleCounter--;
        }
        
        public void Visit(Stmt_For_Node node){ //AnchorToken: for
            CycleCounter++;
            Visit((dynamic) node[0]); //parte 'in'
            Visit((dynamic) node[1]); //lista de declaraciones
            CycleCounter--;
        }
        
        
        public void Visit(In_Node node){
            Visit((dynamic) node[0]); //nombre de la variable
            Visit((dynamic) node[1]); //expresión para saber dónde comenzar
        }
        
        //--------------
        
        public void Visit(Default_Node node){
            Visit((dynamic) node[0]);
        }
        
        public void Visit(True_Node node){
        }
        
        public void Visit(False_Node node){
        }
        
        public void Visit(Int_Lit_Node node){
        }
        
        public void Visit(Int_Bin_Node node){
        }
        
        public void Visit(Int_Dec_Node node){
        }
        
        public void Visit(Int_Hex_Node node){
        }
        
        public void Visit(Int_Oct_Node node){
        }
        
        public void Visit(Character_Node node){
        }
        //--------------- Arithmetic Operators--------------------------------------------
        public void Visit(Unary_Node node) {
            Visit((dynamic) node[0]);
        }
        
        public void Visit(Add_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        //----------------------------------------
        
        public void Visit(Pow_Node node){
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //----------------------------------------
        public void Visit(Mul_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //------------------------
        
        public void Visit(BitAnd_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        } 
        
        //------------------------
        
        public void Visit(BitOr_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        } 
        //------------------------
        
        public void Visit(BitShift_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //------------------------
        
        public void Visit(And_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //------------------------
        
        public void Visit(Or_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //------------Comparison and relational operators------------
        
        public void Visit(Comp_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        public void Visit(Rel_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
        }
        
        //----------Other Operators-------------- 
        
        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node.children) {
                Visit((dynamic) n);
            }
        }
    }
}
