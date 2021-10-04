using System.Collections.Generic;
using System.Text;

namespace Core {
    public interface IEncodable<T> {
        public T Encode();
        public void Decode(T data);
    }
}