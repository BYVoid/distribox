using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Distribox.Network.Communication
{    
    /// <summary>
    /// Different with TcpClient (which implements TCP, providing a *stream*), this class can split the TCP 
    /// stream, providing a service to guarantee that each message sent from one end can be received as 
    /// exactly one (and the same) message at another end.
    /// </summary>    
    /// <remarks>
    /// # Parcel 
    /// The parcel is a 4-byte "length" field and the remaining are data
    /// # Design
    /// Both send and receive is implemented by the following paradigm, we use receive for example:
    /// 1. Call BeginReceive of the TcpClient
    /// 2. When ready, in the callback function:
    ///    2.1. call EndReceive
    ///    2.2. invoke Receive event of AtomicMessageTunnel
    ///    2.3. *Call BeginReceive again*
    /// </remarks>
    class AtomicMessageTunnel 
    {
        /// <summary>
        /// Deal with finish event of send/receive asynchronously
        /// </summary>
        /// <param name="data">If a receive event happens, this field will contains received data.</param>
        /// <returns>true for success, false for other situations. Explicit retry should be made if the event returns false.</returns>
        public delegate bool MessageFinishedHandler(byte[] data);

        public event MessageFinishedHandler FinishSend;
        public event MessageFinishedHandler Receive;

        private TcpClient _client;
        /// <summary>
        /// Receive buffer of TcpClient. For AtomicMessageTunnel, its receive buffer varies by message length.
        /// But TcpClient is a stream, its message length is infinite, so we should use a buffer size manually.
        /// </summary>
        private const int RECEIVE_BUFFER_SIZE = 2 * 1024 * 1024;
        private byte[] TcpReceiveBuffer = new byte[RECEIVE_BUFFER_SIZE];
        

        private Mutex _dataLock = new Mutex();
        private Queue<byte[]> _sendQueue;

        private bool OnFinishSend(byte[] data)
        {
            if (FinishSend != null)
                return FinishSend(data);

            // Since there are no callback hooking, the return value doesn't matter
            return false;
        }

        private bool OnReceive(byte[] data)
        {
            if (Receive != null)
                return Receive(data);

            // Since there are no callback hooking, the return value doesn't matter
            return false;
        }

        private void DoFinishWrite(IAsyncResult ar)
        {
            /* 0. call EndWrite
             * 1. Dequeue
             * 2. emit an event
             * 3. send next item
             */
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            stream.EndWrite(ar);

            lock (_sendQueue)
            {
                _sendQueue.Dequeue();
            }

            OnFinishSend(new byte[0]);

            if (_sendQueue.Count > 0)
            {
                NetworkStream c_stream = _client.GetStream();
            }
        }

        private void DoFinishRead(IAsyncResult ar)
        {
            /* 0. call EndWrite
             * 1. Dequeue
             * 2. emit an event
             * 3. send next item
             */
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            int readNum = stream.EndRead(ar);

            lock (_sendQueue)
            {
                _sendQueue.Dequeue();
            }


            NetworkStream c_stream;

        }

        // TODO awkward function number 1
        private byte[] WrapMessage(byte[] raw)
        {
            int n = raw.Length;
            byte[] bagToSend = new byte[n + sizeof(int)];
            bagToSend[0] = (byte)(n % 256);
            bagToSend[1] = (byte)((n >> 8) % 256);
            bagToSend[2] = (byte)((n >> 16) % 256);
            bagToSend[3] = (byte)((n >> 24) % 256);

            raw.CopyTo(bagToSend, 4);

            return bagToSend;
        }

        // TODO awkward function number 2
        private int MessageLength(byte[] message, int offset)
        {
            int length = message[offset+3];
            length = (length<<8) + message[offset+2];
            length = (length<<8) + message[offset+1];
            length = (length<<8) + message[offset+0];

            return length;
        }

        public AtomicMessageTunnel(TcpClient client)
        {
            _client = client;

            NetworkStream stream = _client.GetStream();
            stream.BeginRead(TcpReceiveBuffer, 0, TcpReceiveBuffer.Length,
                new AsyncCallback(DoFinishRead), stream);
        }

        // FIXME
        // Safety guarantee is not strong enough...
        // More work to improve robustness, or end-to-end check should be performed
        // This procedure may sent a message twice in such situation:
        // 0. There is 1 item, A in queue currently
        // 1. lock and add 1 item, B to the queue, there are two now
        // 2. A is finished sending, so A is popped and send(B) is called (by the callback)
        // 3. _sendQueue.Count is 1 now, but send(B) is already called
        public void Send(byte[] message)
        {
            byte[] bagToSend = WrapMessage(message);

            NetworkStream stream = _client.GetStream();                                 

            lock (_sendQueue)
            {
                _sendQueue.Enqueue(bagToSend);                
            }

            // Send only if queue is empty
            // After Enqueue, there will be only one
            if (_sendQueue.Count == 1)
                stream.BeginWrite(bagToSend, 0, bagToSend.Length,
                new AsyncCallback(DoFinishWrite), stream);
        }
    }
}
