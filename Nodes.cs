

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Int64Proy{
    
    public class Node: IEnumerable<Node> {
    
        public IList<Node> children = new List<Node>();
    
        public Node this[int index] {
            get {
                return children[index];
            }
        }
    
        public Token AnchorToken { get; set; }
    
        public void Add(Node node) {
            children.Add(node);
        }
    
        public IEnumerator<Node> GetEnumerator() {
            return children.GetEnumerator();
        }
    
        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    
        public override string ToString() {
            return String.Format("{0} {1}", GetType().Name, AnchorToken);
        }
    
        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }
    
        static void TreeTraversal(Node node, string indent, StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
    
    public class Var_Def_Node: Node{} // Listo.
    public class Def_List_Node: Node{} // Listo.
    public class Id_Node: Node{} // Listo.
    public class Id_List_Node: Node{} // Listo.
    public class Fun_Def_Node: Node{} // Listo.
    public class Param_List_Node: Node{} // Listo.
    public class Var_Def_List_Node: Node{} // Listo.
    public class Stmt_List_Node: Node{} // Listo.
    public class Assign_Node: Node{} // Listo.
    public class Fun_Call_Node: Node{} // Listo.
    public class Break_Node: Node{} // Listo.
    public class Continue_Node: Node{} // Listo.
    public class Return_Node: Node{} //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Cómo saber si se está llamando dentro de una función?
    public class Empty_Node: Node{} //Listo.
    public class Expr_List_Node: Node{} //Listo.
    public class Stmt_If_Node: Node{} //Listo.
    public class Else_Node: Node{} //Listo.
    public class Stmt_Switch_Node: Node{} // Listo.
    public class Case_List_Node: Node{} // Listo.
    public class Case_Node: Node{} // Listo.
    public class Lit_List_Node: Node{} // Listo.
    public class True_Node: Node{}//Listo
    public class False_Node: Node{}//Listo
    public class Int_Lit_Node: Node{}//LISTO
    public class Int_Bin_Node: Node{}//LISTO
    public class Int_Dec_Node: Node{}//LISTO
    public class Int_Hex_Node: Node{}//LISTO
    public class Int_Oct_Node: Node{}//LISTO
    public class Character_Node: Node{}//Listo
    public class Default_Node: Node{}//Listo
    public class Stmt_While_Node: Node{} //Listo.  Contiene contador para saber si es válido hacer un break
    public class Stmt_Do_While_Node: Node{} //Listo. ^^^^
    public class Stmt_For_Node: Node{} //Listo.
    public class In_Node: Node{} // Listo.
    public class Or_Node: Node{} //LISTO
    public class And_Node: Node{}//LISTO
    public class Comp_Node: Node{}//LISTO
    public class Rel_Node: Node{} //LISTO
    public class BitOr_Node: Node{}//LISTO
    public class BitAnd_Node: Node{}//Listo
    public class BitShift_Node: Node{}//LISTO
    public class Add_Node: Node{}//LISTO
    public class Mul_Node: Node{}//listo
    public class Pow_Node: Node{}//listo
    public class Unary_Node: Node{}//LISTO
    public class Lit_Simple_Node: Node{} //Listo.
    public class Array_Node: Node{} //Listo.
    
}