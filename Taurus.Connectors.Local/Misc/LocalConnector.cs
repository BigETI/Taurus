using System;
using System.Collections.Concurrent;
using Taurus.Compressors;
using Taurus.Fragmenters;

namespace Taurus.Connectors.Local
{
    /// <summary>
    /// A class that describes a local connector
    /// </summary>
    public sealed class LocalConnector : AConnector, ILocalConnector
    {
        /// <summary>
        /// Connect to local connectors
        /// </summary>
        private readonly ConcurrentQueue<LocalConnector> connectToLocalConnectors = new ConcurrentQueue<LocalConnector>();

        /// <summary>
        /// Constructs a local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer connection needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        public LocalConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) : base(onHandlePeerConnectionAttempt, fragmenter, compressor)
        {
            // ...
        }

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <returns>New local connector</returns>
        public static LocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, null, null);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <returns>New local connector</returns>
        public static LocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt, IFragmenter fragmenter) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, fragmenter, null);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New local connector</returns>
        public static LocalConnector CreateNewLocalConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt, ICompressor compressor) =>
            CreateNewLocalConnector(onHandlePeerConnectionAttempt, null, compressor);

        /// <summary>
        /// Creates a new local connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when a peer conection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        /// <returns>New local connector</returns>
        public static LocalConnector CreateNewLocalConnector
        (
            HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt,
            IFragmenter? fragmenter,
            ICompressor? compressor
        ) => new LocalConnector(onHandlePeerConnectionAttempt, fragmenter, compressor);

        /// <summary>
        /// Tries to get a local peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="localPeer">Local peer</param>
        /// <returns></returns>
        private bool TryGettingLocalPeer(IPeer peer, out ILocalPeer? localPeer)
        {
            bool ret = false;
            if ((peer is ILocalPeer local_peer) && Peers.TryGetValue(peer.PeerGUID, out IPeer current_peer) && (peer == current_peer))
            {
                ret = true;
                localPeer = local_peer;
            }
            else
            {
                localPeer = null;
            }
            return ret;
        }

        /// <summary>
        /// Handles peer disconnection request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected override void HandlePeerDisconnectionRequest(IPeer peer, EDisconnectionReason disconnectionReason)
        {
            if (TryGettingLocalPeer(peer, out ILocalPeer? local_peer))
            {
                local_peer!.TargetLocalPeer?.Disconnect(disconnectionReason);
            }
        }

        /// <summary>
        /// Handles sending peer message request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        protected override void HandleSendingPeerMessageRequest(IPeer peer, ReadOnlySpan<byte> message)
        {
            if
            (
                TryGettingLocalPeer(peer, out ILocalPeer? local_peer) &&
                (local_peer!.TargetLocalPeer != null) &&
                local_peer!.TargetLocalPeer.Connector is LocalConnector target_local_connector
            )
            {
                target_local_connector.EnqueuePeerMessageReceivedEvent(local_peer.TargetLocalPeer, message);
            }
        }

        /// <summary>
        /// Connects to the specified target local connector
        /// </summary>
        /// <param name="targetLocalConnector">Target local connector</param>
        public void ConnectToLocalConnector(ILocalConnector targetLocalConnector)
        {
            if (targetLocalConnector == null)
            {
                throw new ArgumentNullException(nameof(targetLocalConnector));
            }
            if (targetLocalConnector == this)
            {
                throw new ArgumentException("Target local connector must be of another instance.", nameof(targetLocalConnector));
            }
            if (!(targetLocalConnector is LocalConnector target_local_connector))
            {
                throw new ArgumentException($"Target local connector is not an instance of \"{nameof(LocalConnector)}\".", nameof(targetLocalConnector));
            }
            connectToLocalConnectors.Enqueue(target_local_connector);
        }

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        public override void ProcessEvents()
        {
            base.ProcessEvents();
            ProcessRequests();
            while (connectToLocalConnectors.TryDequeue(out LocalConnector connect_to_local_connector))
            {
                LocalPeer local_peer = new LocalPeer(new PeerGUID(Guid.NewGuid()), this);
                LocalPeer other_local_peer = new LocalPeer(new PeerGUID(Guid.NewGuid()), connect_to_local_connector);
                local_peer.TargetLocalPeer = other_local_peer;
                EnqueuePeerConnectionAttemptedEvent(local_peer);
                connect_to_local_connector.EnqueuePeerConnectionAttemptedEvent(other_local_peer);
            }
        }
    }
}
