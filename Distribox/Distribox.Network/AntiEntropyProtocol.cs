using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distribox.Network
{
    class AntiEntropyProtocol
    {
        /*
        #region Private Member Variables
        private LocalDataManager _manager;
        private PeerList _peers;
        private AtomicMessageTunnel _messageService;
        #endregion

        #region Private Methods
        private void PushThread();
        private void PullThread();
        #endregion

        #region Constructors
        public AntiEntropyProtocol()
        {
            _manager = new LocalDataManager();
            _peers = new PeerList();
            _messageService = new AtomicMessageService();
        }
        // This class do not need any destruction functions
        // Because the application might terminate unexpectedly
        // because of power failure. We can not expect anything done at
        // destruction.
        // public ~AntiEntropyProtocol();
        #endregion

        #region Public Methods
        public void StartAllServices()
        {
            Thread pushThread = new Thread(this.PushThread);
            pushThread.Start();

            Thread pullThread = new Thread(this.PullThread);
            pullThread.Start();
        }

        public void InviteNewPeer(PeerLocater locater);
        #endregion
         * */
    }
}
