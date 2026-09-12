using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] float _bpm;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Intervals[] _intervals;

    GameObject parentObject;

    private void Start()
    {
        parentObject = GameObject.FindGameObjectWithTag("Traps");
        int numberOfIntervals = parentObject.transform.childCount;

        // Initialize the intervals array based on the number of child objects
        //_intervals = new Intervals[numberOfIntervals];

        if (parentObject != null)
        {
            // Get all child GameObjects
            Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>();
            int childCount = allChildren.Length - 1; // Exclude the parent itself

            // Initialize the _intervals array based on the number of children
            _intervals = new Intervals[childCount];

            for (int i = 0, index = 0; i < allChildren.Length; i++)
            {
                if (allChildren[i].gameObject == parentObject) continue; // Skip the parent itself

                // Create a new Intervals object
                _intervals[index] = new Intervals();

                _intervals[index]._trigger = new UnityEvent();

                // Add listener for this specific child GameObject
                GameObject childObject = allChildren[i].gameObject;

                // Create a UnityAction to call a specific function on the child GameObject
                UnityAction action = () => ExecuteFunctionOnGameObject(childObject);

                // Assign the UnityEvent and add the listener
                _intervals[index]._trigger.AddListener(action); // Add the listener to the trigger

                Debug.Log($"Added listener for GameObject: {childObject.name}");

                index++; // Increment index for the next Interval
            }
        }
        else
        {
            Debug.LogWarning("Parent object is null. Cannot find child GameObjects.");
        }
    }
    private void Update()
    {
        foreach(Intervals interval in _intervals)
        {
            if (interval == null)
            {
                Debug.Log("no intervals!");
                continue;
            }
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
    }
    private void ExecuteFunctionOnGameObject(GameObject obj)
    {
        // Assuming each GameObject has a specific script with a method to be called
        // Replace 'YourScript' with the actual script and 'YourFunction' with the function name
        KillToBeat script = obj.GetComponent<KillToBeat>();
        if (script != null)
        {
            script.Smash(); // Call the function you want to trigger on the GameObject
        }
    }
}
[System.Serializable]
public class Intervals
{
    [SerializeField] float _steps = 1;
    public UnityEvent _trigger;
    int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }
    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}
