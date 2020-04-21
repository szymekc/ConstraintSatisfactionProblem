namespace CSP {
    public class Domain {
        public bool[] mask;

        public Domain(int size) {
            this.mask = new bool[size];
            for (int i = 0; i < size; i++) {
                mask[i] = true;
            }
        }
        public Domain(bool[] mask) {
            this.mask = mask;
        }
    }
}
