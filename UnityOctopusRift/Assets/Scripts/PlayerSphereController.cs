using System;
using UnityEngine;
namespace AssemblyCSharp
{
	public class PlayerSphereController : MonoBehaviour
	{
		public int id;
		public int nPlayers;
		public Transform playerSphere;
		private Transform[] playerSpheres;
		private void Start() {
			playerSpheres = new Transform[nPlayers];
			for (int i = 0; i < nPlayers; i++) {
				if (i != id) {
					playerSpheres[i] = Instantiate(playerSphere, Vector3.zero, Quaternion.identity) as Transform;
				}
			}
		}
		private void Update() {

		}
		public void SetBodyData(Vector3[] pos, Quaternion[] rot) {
			for (int i = 0; i < nPlayers; i++) {
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

