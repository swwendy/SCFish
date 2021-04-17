

namespace USocket.Messages
{
    /// <summary>
    /// Represents a message that is sent and received by server and client.
    /// </summary>
    public interface IScsMessage
    {
        /// <summary>
        /// Unique identified for this message. 
        /// </summary>
        //byte bEntry { get; set; }

        /// <summary>
        /// Unique identified for this message. 
        /// </summary>
        //uint iEntryMsgSize { get; set; }

		byte BaseMsgType{get; set;}

		uint  iMsgType {get; set;} 
		uint  iMsgSize {get; set;}
    }
}
