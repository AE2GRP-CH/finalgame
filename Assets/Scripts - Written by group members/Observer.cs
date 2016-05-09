using UnityEngine;

public abstract class Observer:MonoBehaviour{

	protected DiscoverDispatcher subject;
	public abstract void update();
	
}
