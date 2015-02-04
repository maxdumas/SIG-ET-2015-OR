using System;
using UnityEngine;
namespace AssemblyCSharp
{
	public class PlayerSphereController : MonoBehaviour
	{
		public int id;
		public int nObjects;
		public int[] playerIDs;
		public Transform playerSphere;
		public MotiveStream mStream;
		public OVRCameraController ovrCamera;
		public float zAdjust;
		private Transform[] playerSpheres;

		private void Start() {
			playerSpheres = new Transform[nObjects];
			for (int i = 0; i < nObjects; i++) {
				if (i != id) {
					playerSpheres[i] = Instantiate(playerSphere, Vector3.zero, Quaternion.identity) as Transform;
				}
			}
		}
		private void Update() {
			Vector3 cam_position = mStream.getRigidBodyPosData (id);
			Quaternion cam_rotation = mStream.getRigidBodyRotData (id);

			Vector3 newPos = cam_position;
			ovrCamera.transform.rotation = cam_rotation;
			newPos.x *= -1;
			
			Vector3 forward = ovrCamera.transform.forward;
			forward.x *= -1;
			newPos += zAdjust * forward;
			
			ovrCamera.transform.position = newPos;
			
			Vector3[] positions = new Vector3[nObjects];
			Quaternion[] rotations = new Quaternion[nObjects];
			for (int i = 0; i < nObjects; i++) {
				Vector3 pos = mStream.getRigidBodyPosData(i);
				pos.x *= -1;
				positions[i] = pos;
				rotations[i] = mStream.getRigidBodyRotData(i);
			}
			SetBodyData(positions, rotations);
		}
		public void SetBodyData(Vector3[] pos, Quaternion[] rot) {
			for (int i = 0; i < nObjects; i++) {
				if (i != id) {
					playerSpheres[i].transform.position = pos[i];
					Vector3 rot_ea = rot[i].eulerAngles;
					rot_ea.x = -rot_ea.x;
					playerSpheres[i].transform.rotation = Quaternion.Euler(rot_ea);
				}
			}
		}
	}
}

