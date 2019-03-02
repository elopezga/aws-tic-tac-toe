using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Firebase;

public class FirebaseReference : MonoBehaviour
{
    public static FirebaseReference Instance = null;
    
    private static FirebaseApp app;

    void Awake()
    {
        Instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWith( task => {
                app = SetupFirebaseApp(task);
            });

        DontDestroyOnLoad(this.gameObject);
    }

    public static FirebaseApp GetFirebaseApp()
    {
        return app;
    }

    private static FirebaseApp SetupFirebaseApp(Task<DependencyStatus> task)
    {
        DependencyStatus dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            return FirebaseApp.DefaultInstance;
        }
        else
        {
            Debug.LogWarning("Could not resolve all Firebase dependencies: " + dependencyStatus);
            return null;
        }
    }

}
