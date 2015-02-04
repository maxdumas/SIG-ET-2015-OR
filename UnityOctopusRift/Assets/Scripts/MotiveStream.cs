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
		public PlayerSphereController playerSphereController;
		public int nPlayers;
		public float zAdjust;
		public int id;
		private SocketInfo socketInfo; // The packet received/filled asychronously.
		private NatNetPkt natNetPkt; // The parsed Motive packet.
		private uint natNetPkt_SequenceNumber;
		private IPEndPoint ipEndPoint;
		private System.Threading.Thread thread = null;
		private bool stopReceive; // Do we stop receiving packets?

		// timers for calculating packet rate
		private float accum;
		private int nPackets;
		private int nFrames;

		private class SocketInfo
		{
			public ushort msgId;
			public ushort nBytes;
			public int nBytesReceived;
			public uint incomingFrame;
			public uint currentFrame;
			public Socket socket;
			public byte[] temp;
			public byte[] currentPacket;
			public byte[] inBuffer;
			public SocketInfo ()
			{
				this.currentPacket = new byte[1000];
				this.inBuffer = new byte[1000];
				this.msgId = 0;
				this.nBytes = 0;
				this.currentFrame = 0;
				this.nBytesReceived = 0;
			}
		}

		private void Start() {
			accum = 0;
			nPackets = 0;
			nFrames = 0;

			natNetPkt_SequenceNumber = 0;

			int maxMarkerSets = 20;
			int maxMarkersPerSet = 8;
			int maxOtherMarkers = 10;
			int maxRigidBodies = 10;
			int maxMarkersPerRigidBody = 8;

			natNetPkt = new NatNetPkt ();
			natNetPkt.InitMarkerSets (maxMarkerSets);
			for(int i=0; i<maxMarkerSets; i++) {
				natNetPkt.markerSets [i].InitMarkers (maxMarkersPerSet);
			}
			natNetPkt.InitOtherMarkers (maxOtherMarkers);
			natNetPkt.InitRigidBodies (maxRigidBodies);
			for(int i=0; i<maxRigidBodies; i++) {
				natNetPkt.rigidBodies [i].InitArrays (maxMarkersPerRigidBody);
			}
			thread = new System.Threading.Thread(ThreadRun);
			thread.Start ();
		}
		// Handle new thread data / invoke Unity routines outside of the socket thread.
		private void Update() {
			accum += Time.deltaTime;
			if (natNetPkt.frame != socketInfo.currentFrame) {
				loadPacket (socketInfo.currentPacket, ref natNetPkt);
			}
			
			Vector3 newPos = natNetPkt.rigidBodies [id].pos.AsVector3;
			ovrCamera.transform.rotation = natNetPkt.rigidBodies [id].rot;
			newPos.x *= -1;

			Vector3 forward = ovrCamera.transform.forward;
			forward.x *= -1;
			newPos += zAdjust * forward;

			ovrCamera.transform.position = newPos;
			ovrCamera.transform.rotation = natNetPkt.rigidBodies [id].rot;

			Vector3[] positions = new Vector3[nPlayers];
			Quaternion[] rotations = new Quaternion[nPlayers];
			for (int i = 0; i < nPlayers; i++) {
				Vector3 pos = natNetPkt.rigidBodies[i].pos.AsVector3;
				pos.x *= -1;
				positions[i] = pos;
				rotations[i] = natNetPkt.rigidBodies[i].rot;
			}
			playerSphereController.SetBodyData(positions, rotations);

			float round_accum = (float)Math.Floor(accum);
			if (round_accum > 0) {
				accum -= round_accum;
				print ("packets per second: " + ((float)nPackets / round_accum).ToString());
				print ("frames per second: " + ((float)nFrames / round_accum).ToString());
				nPackets = 0;
				nFrames = 0;
			}

			nFrames++;
		}
		// This thread handles incoming NatNet packets.
		private void ThreadRun ()
		{
			stopReceive = false;
			socketInfo = new SocketInfo ();
			Socket socket =new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			ipEndPoint = new IPEndPoint (IPAddress.Any, 1511);
			socket.Bind (ipEndPoint);
			IPAddress multicastAddr = IPAddress.Parse ("239.255.42.99");
			socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr,IPAddress.Any));
			while (true) {
				socketInfo.nBytesReceived = socket.Receive (socketInfo.inBuffer);
				if (socketInfo.nBytesReceived == 0) { continue; }
				natNetPkt_SequenceNumber++;
				nPackets++;
				IntPtr ptr = GCHandle.Alloc (socketInfo.inBuffer, GCHandleType.Pinned).AddrOfPinnedObject ();
				readPtrToObj<ushort> (ref ptr, ref socketInfo.msgId);
				readPtrToObj<ushort> (ref ptr, ref socketInfo.nBytes);
				readPtrToObj<uint> (ref ptr, ref socketInfo.incomingFrame);
				if(socketInfo.currentFrame < socketInfo.incomingFrame) {
					socketInfo.currentFrame = socketInfo.incomingFrame;
					// Swap inBuffer and currentPacket
					socketInfo.temp = socketInfo.currentPacket;
					socketInfo.currentPacket = socketInfo.inBuffer;
					socketInfo.inBuffer = socketInfo.temp;
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
			
			for (int i = 0; i < pkt.nMarkerSet; i++) {
				pkt.markerSets [i].name = Marshal.PtrToStringAnsi (ptr);
				ptr = new IntPtr (ptr.ToInt64 () + pkt.markerSets [i].name.Length + 1);
				
				readPtrToObj (ref ptr, ref pkt.markerSets [i].nMarkers);
				
				for (int j = 0; j < pkt.markerSets[i].nMarkers; j++) {
					readPtrToObj (ref ptr, ref pkt.markerSets [i].markers [j]);
				}
			}
			readPtrToObj (ref ptr, ref pkt.nOtherMarkers);
			
			for (int i = 0; i < pkt.nOtherMarkers; i++) {
				readPtrToObj (ref ptr, ref pkt.otherMarkers [i]);
			}
			readPtrToObj (ref ptr, ref pkt.nRigidBodies);			
			for (int i = 0; i < pkt.nRigidBodies; i++) {
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].ID);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].pos);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].rot);
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].nMarkers);
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
				readPtrToObj (ref ptr, ref pkt.rigidBodies [i].bodyParams);
			}
			readPtrToObj (ref ptr, ref pkt.nSkeletons);
			readPtrToObj (ref ptr, ref pkt.latency);
			readPtrToObj (ref ptr, ref pkt.timeCode);
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