using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class UtitlityTest {

	[Test]
	public void RemoveDuplicateTest(){

		Collider[] colliders = new Collider[2];
		for (int i = 0; i < 2; i++) {
			ghost gh = new ghost ();
			Transform trans = gh.GetComponent<Transform> ();
			trans.position = new Vector3 (0, 0, 0);
		}


	}
}
