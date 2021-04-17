//using USocket.Protocols;

namespace USocket.Protocols
{
    public class MyWireProtocolFactory : IScsWireProtocolFactory
    {
        public IScsWireProtocol CreateWireProtocol()
        {
            return new MyWireProtocol();
        }
    }
}
