using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CallbackManager : MonoBehaviour {
	static CallbackManager mInstance;
    private List<System.Action> listOfCallbacksForNextFrame;
    private Coroutine mCurrentCoroutine;

    class CallbackInstance
    {
        public System.Action func;
        public float timeDelay;
        public float startTime;
        public bool repeat;
		public bool isRealTime;
    }

    List<CallbackInstance> listOfCallbacks;
	CallbackInstance callbackStruct;

	public static CallbackManager Instance
    {
        get
        {
            return mInstance;
        }
	}

	void Awake()
	{
		if (mInstance == null)
			mInstance = this;

		listOfCallbacks = new List<CallbackInstance>();
		listOfCallbacksForNextFrame = new List<System.Action> ();
	}

	void Destroy()
	{
		mInstance = null;
	}

    public void clearAll()
    {
        StopAllCoroutines();
        listOfCallbacks.Clear();
		listOfCallbacksForNextFrame.Clear ();
    }

	public void doLater(System.Action func, float timeDelay, bool isRealTime=false)
	{
		if (func == null)
			return;

        if(timeDelay<=0.0f)
        {
            func();
            return;
        }

        CallbackInstance newCallback = new CallbackInstance();
        newCallback.func = func;
        newCallback.timeDelay = timeDelay;
		newCallback.startTime = isRealTime?Time.realtimeSinceStartup:Time.time;
		newCallback.isRealTime = isRealTime;
        listOfCallbacks.Add(newCallback);
	}
	
    void Update()
    {
        if (listOfCallbacks==null || listOfCallbacks.Count == 0)
            return;

        for (int i = 0; i < listOfCallbacks.Count; ++i)
        {
            callbackStruct = listOfCallbacks[i];
			if ((!callbackStruct.isRealTime && Time.time - callbackStruct.startTime > callbackStruct.timeDelay) ||
			    (callbackStruct.isRealTime && Time.realtimeSinceStartup - callbackStruct.startTime > callbackStruct.timeDelay))
            {
                callbackStruct.func();
                if(callbackStruct.repeat)
                {
                    callbackStruct.startTime = Time.time;
                }
                else
                {
                    listOfCallbacks.Remove(callbackStruct);
                }
            }
        }
    }
	
    public void repeat(System.Action func, float repeatTimeDelay)
	{
		if (func == null)
			return;

        CallbackInstance newCallback = new CallbackInstance();
        newCallback.func = func;
        newCallback.timeDelay = repeatTimeDelay;
        newCallback.startTime = Time.time;
        newCallback.repeat = true;
        listOfCallbacks.Add(newCallback);
	}
		
	public void clearAllRepeats()
	{
        if(listOfCallbacks.Count==0)
            return;

        CallbackInstance callbackStruct;
        for (int i = 0; i < listOfCallbacks.Count; ++i)
        {
            callbackStruct = listOfCallbacks[i];
            if(callbackStruct.repeat)
            {
                listOfCallbacks.Remove(callbackStruct);
            }
        }
	}

    public void playOnNextFrame(System.Action func)
    {
		if (func == null)
			return;

		listOfCallbacksForNextFrame.Add (func);

        StartCoroutine(executeAfterNextFrame(1));
    }

    public void playAfterNthFrame(System.Action func, int num)
    {
		if (func == null)
			return;

		listOfCallbacksForNextFrame.Add (func);

        StartCoroutine(executeAfterNextFrame(num));
    }

    IEnumerator executeAfterNextFrame(int num)
    {
        int i = 0;
        while (++i <= num)
        {
            yield return new WaitForEndOfFrame();
        }

		if(listOfCallbacksForNextFrame.Count>0)
		{
			for (i = 0; i < listOfCallbacksForNextFrame.Count; ++i)
			{
				listOfCallbacksForNextFrame[i]();
			}
			listOfCallbacksForNextFrame.Clear ();
		}
    }
}
