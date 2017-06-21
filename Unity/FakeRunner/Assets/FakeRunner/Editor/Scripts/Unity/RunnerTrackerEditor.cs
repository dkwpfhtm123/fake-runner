using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    [CustomEditor(typeof(RunnerTracker))]
    public class RunnerTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RunnerTracker runnerTracker = target as RunnerTracker;

            runnerTracker.Offset = EditorGUILayout.Vector3Field("offset", runnerTracker.Offset);
        //    runnerTracker.runner = EditorGUILayout.field("runner", runnerTracker.runner,typeof(Runner),true);
        }
    }
}
