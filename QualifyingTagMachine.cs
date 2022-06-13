using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTLibrary {
    public class QualifyingTagMachine {
        public class Tag {
            String _match = String.Empty;
            Object? _rtn = null;
            public Tag(String match, Object rtn) {
                (this._match, this._rtn) = (match, rtn);
            }
            public Object Evaluate(String query) {
                return query.Contains(this._match) ? this._rtn ?? false : false;
            }
        }
        public class Qualifier {
            public Qualifier(Object key, Tag tag) {

            }
        }
        public QualifyingTagMachine() {

        }
    }
}
