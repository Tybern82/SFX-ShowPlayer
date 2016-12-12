using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kintoshmalae.Timidity.utils {
    public class Bitset : IEnumerable, ICollection {

        private BitArray store { get; set; }

        public int NBits {
            get { return store.Length; }
        }

        public Int32 Count {
            get {
                return store.Count;
            }
        }

        public Object SyncRoot {
            get {
                return store.SyncRoot;
            }
        }

        public Boolean IsSynchronized {
            get {
                return store.IsSynchronized;
            }
        }

        public Bitset(int nbits) {
            store = new BitArray(nbits, false);
        }

        public Bitset(bool[] data) {
            store = new BitArray(data);
        }

        private Bitset(BitArray data) {
            store = data;
        }

        /**
         * Clear a range of bits within a set - starting at the given bit index and clearing the given number of bits.
         */
        public void clear(int start, int nbits) {
            if (nbits == 0 || start < 0 || start >= NBits)
                return; // no bits to actually clear

            if (start + nbits > NBits)
                nbits = NBits - start;   // make sure we don't try to clear beyond the end of the set

            for (int i = 0; i < nbits; i++) {
                this[start + i] = false;
            }
        }

        /**
         * Access a single bit value from the set.
         */
         public bool this[int index] {
            get {
                return store[index];
            }
            set {
                store[index] = value;
            }
        }

        /**
         * Access a subset of bits from this set.
         */
        public Bitset this[int start, int nbits] {
            get {
                if (nbits == 0 || start < 0 || start >= NBits)
                    return new Bitset(0);

                if (start + nbits > NBits)
                    nbits = NBits - start;   // can only copy items that are actually present

                bool[] _data = new bool[nbits];
                for (int i = 0; i < nbits; i++) {
                    _data[i] = this[start + i];
                }
                return new Bitset(_data);
            }
            set {
                if (value == null) {
                    clear(start, nbits);
                } else {
                    if (nbits == 0 || start < 0 || start >= NBits)
                        return; // no actual bits to set

                    if (start + nbits > NBits)
                        nbits = NBits - start;

                    for (int i = 0; i < nbits; i++) {
                        this[start + i] = value[i];
                    }
                }
            }
        }

        public bool hasBitSet() {
            foreach (bool x in this) {
                if (x) return true;
            }
            return false;
        }

        public override String ToString() {
            string _result = "";
            for (int i = 0; i < NBits; i++) {
                _result += (this[i] ? '1' : '0');
            }
            return _result;
        }

        public override Boolean Equals(Object obj) {
            return (obj is Bitset) ? (this == (Bitset)obj) : false;
        }

        public override Int32 GetHashCode() {
            return store.GetHashCode();
        }

        public void print() {
            Console.Write(ToString());
        }

        public IEnumerator GetEnumerator() {
            return store.GetEnumerator();
        }

        public void CopyTo(Array array, Int32 index) {
            store.CopyTo(array, index);
        }

        public static bool operator ==(Bitset a, Bitset b) {
            if (a.NBits != b.NBits) return false;
            for (int i = 0; i < a.NBits; i++) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        public static bool operator !=(Bitset a, Bitset b) {
            return !(a == b);
        }

        // AND
        public static Bitset operator &(Bitset a, Bitset b) {
            return new Bitset(((BitArray)a.store.Clone()).And(b.store));
        }

        // OR
        public static Bitset operator |(Bitset a, Bitset b) {
            return new Bitset(((BitArray)a.store.Clone()).Or(b.store));
        }

        // XOR
        public static Bitset operator ^(Bitset a, Bitset b) {
            return new Bitset(((BitArray)a.store.Clone()).Xor(b.store));
        }

        // NOT
        public static Bitset operator ~(Bitset a) {
            return new Bitset(((BitArray)a.store.Clone()).Not());
        }
    }
}