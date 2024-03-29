﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taurus.Collections;
using Taurus.Compressors;
using Taurus.Fragmenters;

namespace Taurus.Connectors
{
    /// <summary>
    /// An abstract class that describes a connector
    /// </summary>
    public abstract class AConnector : IConnector
    {
        /// <summary>
        /// Gets invoked when peer connection attempt needs to be handled
        /// </summary>
        private readonly HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt;

        /// <summary>
        /// Peers
        /// </summary>
        private readonly FastEnumerableDictionary<PeerGUID, IPeer> peers = new FastEnumerableDictionary<PeerGUID, IPeer>();

        /// <summary>
        /// Peer GUID to defragmenter stream lookup
        /// </summary>
        private readonly Dictionary<PeerGUID, IDefragmenterStream> peerGUIDToDefragmenterStreamLookup = new Dictionary<PeerGUID, IDefragmenterStream>();

        /// <summary>
        /// Peer connection attempted events
        /// </summary>
        private readonly ConcurrentQueue<IPeer> peerConnectionAttemptedEvents = new ConcurrentQueue<IPeer>();

        /// <summary>
        /// Peer connection handled events
        /// </summary>
        private readonly ConcurrentQueue<PeerConnection> peerConnectionHandledEvents = new ConcurrentQueue<PeerConnection>();

        /// <summary>
        /// Peer disconnection requested events
        /// </summary>
        private readonly ConcurrentQueue<PeerDisconnection> peerDisconnectionRequestedEvents = new ConcurrentQueue<PeerDisconnection>();

        /// <summary>
        /// Valid peer disconnection requested events
        /// </summary>
        private readonly ConcurrentQueue<PeerDisconnection> validPeerDisconnectionRequestedEvents = new ConcurrentQueue<PeerDisconnection>();

        /// <summary>
        /// Peer disconnected events
        /// </summary>
        private readonly ConcurrentQueue<PeerDisconnection> peerDisconnectedEvents = new ConcurrentQueue<PeerDisconnection>();

        /// <summary>
        /// Sending peer message requested events
        /// </summary>
        private readonly ConcurrentQueue<PeerMessage> sendingPeerMessageRequestedEvents = new ConcurrentQueue<PeerMessage>();

        /// <summary>
        /// Peer message sent events
        /// </summary>
        private readonly ConcurrentQueue<PeerMessage> peerMessageSentEvents = new ConcurrentQueue<PeerMessage>();

        /// <summary>
        /// Peer message received events
        /// </summary>
        private readonly ConcurrentQueue<PeerMessage> peerMessageReceivedEvents = new ConcurrentQueue<PeerMessage>();

        /// <summary>
        /// Peers
        /// </summary>
        public IReadOnlyDictionary<PeerGUID, IPeer> Peers => peers;

        /// <summary>
        /// Fragmenter
        /// </summary>
        public IFragmenter Fragmenter { get; }

        /// <summary>
        /// Compressor
        /// </summary>
        public ICompressor Compressor { get; }

        /// <summary>
        /// Gets invoked when a peer connection has been attempted
        /// </summary>
        public event PeerConnectionAttemptedDelegate? OnPeerConnectionAttempted;

        /// <summary>
        /// Gets invoked when a peer has been connected
        /// </summary>
        public event PeerConnectedDelegate? OnPeerConnected;

        /// <summary>
        /// Gets invoked when a peer connection has been denied
        /// </summary>
        public event PeerConnectionDeniedDelegate? OnPeerConnectionDenied;

        /// <summary>
        /// Gets invoked when a peer has been disconnected
        /// </summary>
        public event PeerDisconnectedDelegate? OnPeerDisconnected;

        /// <summary>
        /// Gets invoke when a peer message has been sent
        /// </summary>
        public event PeerMessageSentDelegate? OnPeerMessageSent;

        /// <summary>
        /// Gets invoked when a peer message has been received
        /// </summary>
        public event PeerMessageReceivedDelegate? OnPeerMessageReceived;

        /// <summary>
        /// Constructs a new connector
        /// </summary>
        /// <param name="onHandlePeerConnectionAttempt">Gets invoked when peer connection attempt needs to be handled</param>
        /// <param name="fragmenter">Fragmenter</param>
        /// <param name="compressor">Compressor</param>
        protected AConnector(HandlePeerConnectionAttemptDelegate onHandlePeerConnectionAttempt, IFragmenter? fragmenter, ICompressor? compressor)
        {
            Fragmenter = fragmenter ?? new NoFragmentationFragmenter();
            Compressor = compressor ?? new NoCompressionCompressor();
            this.onHandlePeerConnectionAttempt = onHandlePeerConnectionAttempt;
        }

        /// <summary>
        /// Enqueues peer connection attempted event
        /// </summary>
        /// <param name="peer">Peer</param>
        protected void EnqueuePeerConnectionAttemptedEvent(IPeer peer) => peerConnectionAttemptedEvents.Enqueue(peer);

        /// <summary>
        /// Enqueues peer disconnected event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected void EnqueuePeerDisconnectedEvent(IPeer peer, EDisconnectionReason disconnectionReason) =>
            peerDisconnectedEvents.Enqueue(new PeerDisconnection(peer, disconnectionReason));

        /// <summary>
        /// Enqueues a peer message received event
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        protected void EnqueuePeerMessageReceivedEvent(IPeer peer, ReadOnlySpan<byte> message) =>
            peerMessageReceivedEvents.Enqueue(new PeerMessage(peer, message));

        /// <summary>
        /// Processes requests
        /// </summary>
        protected void ProcessRequests()
        {
            while (validPeerDisconnectionRequestedEvents.TryDequeue(out PeerDisconnection valid_peer_disconnection_requested_event))
            {
                HandlePeerDisconnectionRequest(valid_peer_disconnection_requested_event.Peer, valid_peer_disconnection_requested_event.DisconnectionReason);
                peerDisconnectedEvents.Enqueue(valid_peer_disconnection_requested_event);
            }
            while (sendingPeerMessageRequestedEvents.TryDequeue(out PeerMessage sending_peer_message_requested_event))
            {
                HandleSendingPeerMessageRequest(sending_peer_message_requested_event.Peer, sending_peer_message_requested_event.Message.Span);
                peerMessageSentEvents.Enqueue(sending_peer_message_requested_event);
            }
        }

        /// <summary>
        /// Handles peer disconnection request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        protected abstract void HandlePeerDisconnectionRequest(IPeer peer, EDisconnectionReason disconnectionReason);

        /// <summary>
        /// Handles sending peer message request
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        protected abstract void HandleSendingPeerMessageRequest(IPeer peer, ReadOnlySpan<byte> message);

        /// <summary>
        /// Is peer contained
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <returns>"true" if peer is contained, otherwise "false"</returns>
        public bool IsPeerContained(IPeer peer) => peers.TryGetValue(peer.PeerGUID, out IPeer current_peer) && (peer == current_peer);

        /// <summary>
        /// Disconnects the specified peer
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="disconnectionReason">Disconnection reason</param>
        public void DisconnectPeer(IPeer peer, EDisconnectionReason disconnectionReason) =>
            peerDisconnectionRequestedEvents.Enqueue(new PeerDisconnection(peer, disconnectionReason));

        /// <summary>
        /// Sends a message to a peer asynchronously
        /// </summary>
        /// <param name="peer">Peer</param>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task SendMessageToPeerAsync(IPeer peer, ReadOnlyMemory<byte> message) =>
            (message.Length > 0) ?
                Task.Run(() => sendingPeerMessageRequestedEvents.Enqueue(new PeerMessage(peer, Fragmenter.Fragment(Compressor.Compress(message.Span))))) :
                Task.CompletedTask;

        /// <summary>
        /// Closes connection to all connected peers in this connector
        /// </summary>
        /// <param name="reason">Disconnection reason</param>
        public virtual void Close(EDisconnectionReason reason)
        {
            while (peers.Count > 0)
            {
                foreach (IPeer peer in peers.Values)
                {
                    DisconnectPeer(peer, reason);
                }
                ProcessEvents();
            }
        }

        /// <summary>
        /// Processes all events appeared since last call
        /// </summary>
        public virtual void ProcessEvents()
        {
            while (peerConnectionAttemptedEvents.TryDequeue(out IPeer peer))
            {
                if (peers.TryAdd(peer.PeerGUID, peer))
                {
                    peerConnectionHandledEvents.Enqueue(new PeerConnection(peer, onHandlePeerConnectionAttempt(peer)));
                    OnPeerConnectionAttempted?.Invoke(peer);
                }
            }
            PeerConnection? first_peer_connection_handled_event = null;
            while (peerConnectionHandledEvents.TryDequeue(out PeerConnection peer_connection_handled_event))
            {
                if (first_peer_connection_handled_event == null)
                {
                    first_peer_connection_handled_event = peer_connection_handled_event;
                }
                else if (first_peer_connection_handled_event.Value.Peer == peer_connection_handled_event.Peer)
                {
                    peerConnectionHandledEvents.Enqueue(peer_connection_handled_event);
                    break;
                }
                TaskStatus task_status = peer_connection_handled_event.IsPeerConnectionSuccessfulTask.Status;
                switch (task_status)
                {
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                        DisconnectPeer(peer_connection_handled_event.Peer, EDisconnectionReason.Error);
                        OnPeerConnectionDenied?.Invoke(peer_connection_handled_event.Peer, EDisconnectionReason.Error);
                        break;
                    case TaskStatus.Created:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.WaitingToRun:
                        peerConnectionHandledEvents.Enqueue(peer_connection_handled_event);
                        break;
                    case TaskStatus.RanToCompletion:
                        if (peer_connection_handled_event.IsPeerConnectionSuccessfulTask.Result)
                        {
                            OnPeerConnected?.Invoke(peer_connection_handled_event.Peer);
                        }
                        else
                        {
                            DisconnectPeer(peer_connection_handled_event.Peer, EDisconnectionReason.Denied);
                            OnPeerConnectionDenied?.Invoke(peer_connection_handled_event.Peer, EDisconnectionReason.Denied);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Task status {task_status} is not supported yet.");
                }
            }
            while (peerDisconnectionRequestedEvents.TryDequeue(out PeerDisconnection peer_disconnection_requested_event))
            {
                if (IsPeerContained(peer_disconnection_requested_event.Peer))
                {
                    validPeerDisconnectionRequestedEvents.Enqueue(peer_disconnection_requested_event);
                }
            }
            while (peerDisconnectedEvents.TryDequeue(out PeerDisconnection peer_disconnected_event))
            {
                if (IsPeerContained(peer_disconnected_event.Peer))
                {
                    peers.Remove(peer_disconnected_event.Peer.PeerGUID);
                    OnPeerDisconnected?.Invoke(peer_disconnected_event.Peer, peer_disconnected_event.DisconnectionReason);
                }
            }
            while (peerMessageSentEvents.TryDequeue(out PeerMessage peer_message_sent_event))
            {
                if (IsPeerContained(peer_message_sent_event.Peer))
                {
                    OnPeerMessageSent?.Invoke(peer_message_sent_event.Peer, peer_message_sent_event.Message.Span);
                }
            }
            while (peerMessageReceivedEvents.TryDequeue(out PeerMessage peer_message_received_event))
            {
                if (IsPeerContained(peer_message_received_event.Peer))
                {
                    if
                    (
                        !peerGUIDToDefragmenterStreamLookup.TryGetValue
                        (
                            peer_message_received_event.Peer.PeerGUID,
                            out IDefragmenterStream defragmenter_stream
                        )
                    )
                    {
                        defragmenter_stream = Fragmenter.CreateNewDefragmenterStream();
                        peerGUIDToDefragmenterStreamLookup.Add(peer_message_received_event.Peer.PeerGUID, defragmenter_stream);
                    }
                    defragmenter_stream.Write(peer_message_received_event.Message.Span);
                    while (defragmenter_stream.TryDequeuingMessage(out ReadOnlySpan<byte> message))
                    {
                        OnPeerMessageReceived?.Invoke(peer_message_received_event.Peer, Compressor.Decompress(message));
                    }
                }
            }
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose() => Close(EDisconnectionReason.Disposed);
    }
}
