namespace Core {
    public interface IEncodable<T> {
        public T Encode();
        public void Decode(T data);
    }
}