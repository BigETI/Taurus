using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Taurus.Compressors;
using Taurus.Fragmenters;

/// <summary>
/// Taurus connectors local namespace
/// </summary>
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
        /// local peer GUID to target local peer lookup
        /// </summary>
        private readonly Dictionary<PeerGUID, ILocalPeer> localPeerGUIDToTargetLocalPeerLookup = new Dictionary<PeerGUID, ILocalPeer>();

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
        /// Registers a connecting peer
        /// </summary>
        /// <param name="localPeer">Local peer</param>
        private void RegisterConnectingPeer(ILocalPeer localPeer)
        {
            if (localPeer.TargetLocalConnector != this)
            {
                throw new ArgumentException("Target local connector of local peer must be the same instance as the callee.", nameof(localPeer));
            }
            if (!(localPeer.Connector is LocalConnector local_connector))
            {
                throw new ArgumentException($"Peer connector is not an instance of \"{nameof(LocalConnector)}\".", nameof(localPeer));
            }
            ILocalPeer new_local_peer = new LocalPeer(new PeerGUID(Guid.NewGuid()), this, local_connector);
            localPeerGUIDToTargetLocalPeerLookup.Add(localPeer.PeerGUID, new_local_peer);
            EnqueuePeerConnectionAttemptedEvent(new_local_peer);
            local_connector.AcknowledgeConnectionAttempt(localPeer, new_local_peer);
        }

        /// <summary>
        /// Acknowledges connection attempt
        /// </summary>
        /// <param name="localPeer">Peer</param>
        /// <param name="newLocalPeer">New peer</param>
        private void AcknowledgeConnectionAttempt(ILocalPeer localPeer, ILocalPeer newLocalPeer)
        {
            if (localPeer == null)
            {
                throw new ArgumentNullException(nameof(localPeer));
            }
            if (newLocalPeer == null)
            {
                throw new ArgumentNullException(nameof(newLocalPeer));
            }
            if (localPeer.Connector != this)
            {
                throw new ArgumentException("Connector of local peer must be the same instance as the callee.", nameof(localPeer));
            }
            if (newLocalPeer.TargetLocalConnector != this)
            {
                throw new ArgumentException("Target local connector of new local peer must be the same instance as the callee.", nameof(newLocalPeer));
            }
            if (localPeerGUIDToTargetLocalPeerLookup.TryAdd(newLocalPeer.PeerGUID, localPeer))
            {
                EnqueuePeerConnectionAttemptedEvent(localPeer);
            }
        }

        /// <summary>
        /// Handles peer disconnection request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected override void HandlePeerDisconnectionRequest(IPeer peer, EDisconnectionReason disconnectionReason)
        {
            if (localPeerGUIDToTargetLocalPeerLookup.TryGetValue(peer.PeerGUID, out ILocalPeer target_local_peer))
            {
                localPeerGUIDToTargetLocalPeerLookup.Remove(peer.PeerGUID);
                target_local_peer.Disconnect(disconnectionReason);
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
                localPeerGUIDToTargetLocalPeerLookup.TryGetValue(peer.PeerGUID, out ILocalPeer target_local_peer) &&
                (target_local_peer.Connector is LocalConnector target_local_connector)
            )
            {
                target_local_connector.EnqueuePeerMessageReceivedEvent(peer, message);
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
                connect_to_local_connector.RegisterConnectingPeer(new LocalPeer(new PeerGUID(Guid.NewGuid()), this, connect_to_local_connector));
            }
        }
    }
}
