/*
  Int64 compiler - Symbol table class.
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
using System.Text;
using System.Collections.Generic;

namespace Int64Proy {

    public class TableDictionary: IEnumerable<KeyValuePair<string, SymbolTable>> {
        
        public TableDictionary(){
        }
        
        IDictionary<string, SymbolTable> data = new SortedDictionary<string, SymbolTable>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var entry in data) {
                sb.Append(entry.Value.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public SymbolTable this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, SymbolTable>> GetEnumerator() {
            return data.GetEnumerator();
        }
        
        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
