/*
 * This code is written by and belongs to DFT Games.
 * Its usage is allowed to the licensee in conjunction
 * with the package that has been licensed and also
 * in other final products produced by the licensee
 * and aimed to the end user. It's forbidden to use
 * this code as part of packages or assets 
 * aimed to be used by Unity developers other
 * than the licensee of the original package.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A simple, dirty and fast typed pooling system.
/// </summary>
public class PoolingSystem<T> where T: class {
	
	/// <summary>
	/// The out of the way position. This is the position used
	/// to set the game object out of the camera. Feel free to change it
	/// if in your project this position is not really out of the way.
	/// </summary>
	private static Vector3 outOfTheWay = new Vector3(20000f,20000f,20000f);
	
	/// <summary>
	/// The available elements list.
	/// </summary>
	private List<GameObject> availableList = new List<GameObject>();
	
	/// <summary>
	/// The elements in use list.
	/// </summary>
	private List<GameObject> inUseList = new List<GameObject>();
	
	/// <summary>
	/// The original prefab reference.
	/// </summary>
	private GameObject original = null;
	
	/// <summary>
	/// Is this a GameObject pool?
	/// </summary>
	private bool isGameObject = false;

	private Transform parentHolder;

	/// <summary>
	/// Initializes a new instance of the <see cref="PoolingSystem`1"/> class.
	/// </summary>
	/// <param name='prefab'>
	/// Prefab.
	/// </param>
	/// <param name='initialSize'>
	/// Initial size.
	/// </param>
	public PoolingSystem (GameObject prefab, int initialSize, Transform parent = null, bool isDebug=false)
	{
		GameObject temp = null;
		original = prefab;
		parentHolder = parent;
		
		// Remember if this is a GameObject type or not
		if (typeof(T) == typeof(GameObject))
			isGameObject = true;
		
        availableList.Clear();
		// Populate the initial pool
		for (int i = 0; i < initialSize; i++)
		{
			// Instantiate the object
            temp = GameObject.Instantiate(original, outOfTheWay + Vector3.right * Random.Range(0,1000), Quaternion.identity) as GameObject;
			// Set the object inactive

			//temp.SetActiveRecursively (false);

            temp.SetActive(false);

			temp.transform.parent = parentHolder;

			// Add it to the list of the available elements
			availableList.Add (temp);

            if(isDebug)
                Debug.Log(temp.GetInstanceID());
		}

        if(isDebug)
        {
            Debug.Log(availableList.Count);
            for(int i=0; i<availableList.Count; ++i)
                Debug.Log(availableList[i].GetInstanceID());
        }
	}

	/// <summary>
	/// Cleans up the memory. This method isn't really
	/// something you want to call often, but from time to time
	/// it might help in some specific use case. DO NOT use it
	/// unless profiling your app you can actually see that
	/// there is need for it. This is just a temporary memory
	/// release of the excess allocation as the objects go back and forth 
	/// between the two lists. NEVER call this on every frame!!!
	/// If you really need this you can use it every few seconds.
	/// </summary>
	public void CleanUp()
	{
		inUseList.TrimExcess ();
		availableList.TrimExcess ();
	}
	
	/// <summary>
	/// Gets the in use size.
	/// </summary>
	/// <value>
	/// The in use size.
	/// </value>
	public int InUse
	{
		get 
		{
			return inUseList.Count;
		}
	}
	
	/// <summary>
	/// Gets the available size.
	/// </summary>
	/// <value>
	/// The available size.
	/// </value>
	public int Available
	{
		get 
		{
			return availableList.Count;
		}
	}
	
	/// <summary>
	/// Releases the element. To be called instead of Destroy.
	/// </summary>
	/// <param name='element'>
	/// Element.
	/// </param>
	/// <param name='SetOutOfTheWay'>
	/// Set out of the way.
	/// </param>
	public void ReleaseElement(T element, bool SetOutOfTheWay=true)
	{
		GameObject temp;
		if (isGameObject) // Are we dealing with GameObject?
		{
			temp = element as GameObject;
		}
		else // if not...
		{
			// Get the component to get its GameObject
			Component cTemp = element as Component;
			temp = cTemp.gameObject;
		}
		
		// change the object position id the flag is true
		if (SetOutOfTheWay)
			temp.transform.position = outOfTheWay;
		// Set the object inactive

		//temp.SetActiveRecursively (false);

        temp.SetActive(false);
		temp.transform.parent = parentHolder;

		inUseList.Remove(temp);
		availableList.Add(temp);
	}
	
	/// <summary>
	/// Gets the element. To be called instead of Instantiate.
	/// </summary>
	/// <returns>
	/// The element.
	/// </returns>
	public T GetElement()
	{
		GameObject temp = null;
		// Check if the pool contains an usable element
		if (availableList.Count == 0)
		{
			// No free elements, so we create a new one.
			temp = GameObject.Instantiate(original, outOfTheWay, Quaternion.identity) as GameObject;

			//temp.SetActiveRecursively (false);

			//temp.SetActive(false);

			// add the new object to the in use list
			inUseList.Add (temp);
		}
		else // an element is available
		{
			// fetch the element
			temp = availableList[0];
			// remove it from the active list
			availableList.RemoveAt(0);
			// add the object to the in use list
			inUseList.Add(temp);
		}
		
		// Activate the object

		//temp.SetActiveRecursively (true);

		temp.SetActive (true);

		temp.transform.parent = null;

		// Return the proper object
		if (isGameObject)
			return temp as T;
		else
		{
			return temp.GetComponent(typeof(T)) as T;
		}
	}
}
