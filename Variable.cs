using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CSP {
    public class Variable<T> : IComparable<Variable<T>> {
        public T value;
        public Domain domain;
        public int x;
        public int y;
        public bool isVertical;
        public int length;

        public Variable(T value, Domain domain, int x, int y) {
            this.value = value;
            this.domain = domain;
            this.x = x;
            this.y = y;
        }
        public Variable(T value, Domain domain, int x, int y, bool isVertical) {
            this.length = value.ToString().Length;
            this.value = default(T);
            this.domain = domain;
            this.x = x;
            this.y = y;
            this.isVertical = isVertical;
        }

        public int CompareTo([AllowNull] Variable<T> other) {
            if (this.domain.mask.Count((a) => a) > other.domain.mask.Count((a) => a)) {
                return 1;
            } else if (this.domain.mask.Count((a) => a) < other.domain.mask.Count((a) => a)) {
                return -1;
            } else return 0;
        }

        public override bool Equals(object obj) {
            if (this == obj) return false;
            return obj is Variable<T> variable &&
                   EqualityComparer<T>.Default.Equals(value, variable.value);
        }

        public override int GetHashCode() {
            return HashCode.Combine(x, y);
        }
    }
}
