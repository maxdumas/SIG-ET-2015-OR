using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace AssemblyCSharp
{
	class MotiveStream : MonoBehaviour
	{
		public OVRCameraController ovrCamera;
		private AsyncPkt asyncPkt; // The packet received/filled asychronously.
		private NatNetPkt natNetPkt; // The parsed Motive packet.
		private System.Object natNetPkt_lock = new System.Object();
		private bool natNetPkt_filled;
		private uint natNetPkt_parentFrame;
		public uint currentFrame;
		private UdpClient udpClient;
		private IPEndPoint remoteIPEndPoint;
		private System.Threading.Thread thread = null;
		private bool stopReceive; // Do we stop receiving packets?
		private class AsyncPkt
		{
			public ushort msgId;
			public byte[] buffer;
			public ushort nBytes;
			public int nBytesReceived;
			public AsyncPkt ()
			{
				this.msgId = 0;
				this.nBytes = 0;
				this.nBytesReceived = 0;
				buffer = new byte[10240]; // 10 KB
			}
		}
		private void Start() {
			natNetPkt_filled = false;
			natNetPkt_parentFrame = 0;
			natNetPkt = new NatNetPkt ();
			currentFrame = 0;
			thread = new System.Threading.Thread(ThreadRun);
			thread.Start ();
		}
		// Handle new thread data / invoke Unity routines outside of the socket thread.
		private void Update() {
			lock (natNetPkt_lock) {
				// check for a change in msgId
				if(natNetPkt_filled == false) {
					return;
				}
				if(natNetPkt_parentFrame != currentFrame) {
					Debug.Log (currentFrame + " | " + natNetPkt_parentFrame  + " ObjectX: " + natNetPkt.rigidBodies [0].pos.AsVector3.x);
					currentFrame = natNetPkt_parentFrame;
				}
			}

			ovrCamera.transform.position = natNetPkt.rigidBodies [0].pos.AsVector3;
			ovrCamera.transform.rotation = natNetPkt.rigidBodies [0].rot;
		}
		private void ThreadRun ()
		{
			stopReceive = false;
			udpClient = new UdpClient ();
			remoteIPEndPoint = new IPEndPoint (IPAddress.Any, 1511);
			asyncPkt = new AsyncPkt ();
			udpClient.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			udpClient.ExclusiveAddressUse = false;
			udpClient.Client.Bind (remoteIPEndPoint);
			IPAddress multicastAddr = IPAddress.Parse ("239.255.42.99");
			udpClient.JoinMulticastGroup (multicastAddr);
			udpClient.Client.ReceiveBufferSize = 512;
			while (true) {
				byte[] receivedBytes = udpClient.Receive (ref remoteIPEndPoint);
				if (receivedBytes.Length == 0) {
					continue;
				}
				if (asyncPkt.nBytes == 0) {
					asyncPkt.nBytesReceived = receivedBytes.Length;
					IntPtr ptr = GCHandle.Alloc (receivedBytes, GCHandleType.Pinned).AddrOfPinnedObject ();
					readPtrToObj<ushort> (ref ptr, ref asyncPkt.msgId);
					readPtrToObj<ushort> (ref ptr, ref asyncPkt.nBytes);
					asyncPkt.nBytes += 4;
					Array.Copy (receivedBytes, 0, asyncPkt.buffer, 0, receivedBytes.Length);
				} else {
					Array.Copy (receivedBytes, 0, asyncPkt.buffer, asyncPkt.nBytesReceived - 1, receivedBytes.Length);
					asyncPkt.nBytesReceived = receivedBytes.Length;
				}
				if (asyncPkt.nBytesReceived - asyncPkt.nBytes >= 0) {
					lock(natNetPkt_lock) {
						loadPacket (asyncPkt.buffer, ref natNetPkt);
						natNetPkt_parentFrame++;
						asyncPkt.nBytes = 0;
					}
				}
				if (stopReceive) {
					break;
				}
			}
		}
		// Read the current raw byte[] and load the values into the NatNatPkt.
		private void loadPacket (byte[] buffer, ref NatNetPkt pkt)
		{
			IntPtr ptr = GCHandle.Alloc (buffer, GCHandleType.Pinned).AddrOfPinnedObject ();
			readPtrToObj (ref ptr, ref pkt.ID);
			readPtrToObj (ref ptr, ref pkt.nBytes);
			readPtrToObj (ref ptr, ref pkt.frame);
			readPtrToObj (ref ptr, ref pkt.nMarkerSet);
			pkt.InitMarkerSets ();
			for (int i = 0; i < pkt.nMarkerSet; i++) {
				pkt.markerSets [i].name = Marshal.PtrToStringAnsi (ptr);
				ptr = new IntPtr (ptr.ToInt64 () + pkt.markerSets [i].name.Length + 1);
				
				readPtrToObj (ref ptr, ref pkt.markerSets [i].nMarkers);
				pkt.markerSets [i].InitMarkers ();
				for (int j = 0; j < pkt.markerSets[i].nMarkers; j++) {
					readPtrToObj (ref ptr, ref pkt.markerSets [i].markers [j]);
				}
			}
			readPtrToObj (ref ptr, ref pkt.nOtherMarkers);
			pkt.InitOtherMarkers ();
			for (int i = 0; i < pkt.nOtherMarkers; i++) {
				readPtrToObj (ref ptr, ref pkt.otherMarkers [i]);
			}
			readPtrToObj (ref ptr, ref pkt.nRigidBodies);
			pkt.InitRigidBodies ();
			for (int i = 0; i < pkt.nRigidBodies; i++) {
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].ID);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].pos);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].rot);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].nMarkers);
				pkt.rigidBodies [i].InitArrays ();
				for (int j = 0; j < pkt.rigidBodies[i].nMarkers; j++) {
					readPtrToObj (ref ptr, ref pkt.rigidBodies [i].Markers [j]);
				}
				for (int j = 0; j < pkt.rigidBodies[i].nMarkers; j++) {
					readPtrToObj (ref ptr, ref pkt.rigidBodies [i].MarkerIDs [j]);
				}
				for (int j = 0; j < pkt.rigidBodies[i].nMarkers; j++) {
					readPtrToObj (ref ptr, ref pkt.rigidBodies [i].MarkerSizes [j]);
				}
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].MeanError);
			}
			readPtrToObj (ref ptr, ref pkt.nSkeletons);
			readPtrToObj (ref ptr, ref pkt.latency);
			readPtrToObj (ref ptr, ref pkt.timeCode);
			natNetPkt_filled = true;
		}
		private void readPtrToObj<T> (ref IntPtr ptr, ref T obj)
		{
			obj = (T)Marshal.PtrToStructure (ptr, typeof(T));
			ptr = new IntPtr (ptr.ToInt64 () + Marshal.SizeOf (obj));
		}
		private void OnDestroy ()
		{
			stopReceive = true;
		}
	}
}