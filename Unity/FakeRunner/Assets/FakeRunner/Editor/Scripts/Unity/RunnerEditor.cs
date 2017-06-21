using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fake.FakeRunner.Unity
{
    [CustomEditor(typeof(Runner))]
    public class RunnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

       /*     Runner runner = target as Runner;

            var oldValue = runner.velocity;
            var newValue = EditorGUILayout.Vector3Field("Velocity", oldValue);
            if (oldValue != newValue)
            {
                runner.velocity = newValue;
                EditorUtility.SetDirty(runner);
            } */
        }
    }
}
