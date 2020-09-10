using UnityEngine;
using UnityEditor;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Editor;

internal class ConfigRayComponentBaseEditor : UnityEditor.Editor
    {
        bool m_RequireSensorUpdate;

        protected void OnRayPerceptionInspectorGUI(bool is3d)
        {
            var so = serializedObject;
            so.Update();

            // Drawing the ConfigRayComponent
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;

            // Don't allow certain fields to be modified during play mode.
            // * SensorName affects the ordering of the Agent's observations
            // * The number of tags and rays affects the size of the observations.
            EditorGUI.BeginDisabledGroup(!EditorUtilities.CanUpdateModelProperties());
            {
            EditorGUILayout.PropertyField(so.FindProperty("result"), true);

            EditorGUILayout.PropertyField(so.FindProperty("m_SensorName"), true);

                EditorGUILayout.PropertyField(so.FindProperty("infoType"), true);
                EditorGUILayout.PropertyField(so.FindProperty("m_RaysPerDirection"), true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(so.FindProperty("m_MaxRayDegrees"), true);
            EditorGUILayout.PropertyField(so.FindProperty("m_SphereCastRadius"), true);
            EditorGUILayout.PropertyField(so.FindProperty("m_RayLength"), true);
            EditorGUILayout.PropertyField(so.FindProperty("m_RayLayerMask"), true);

            // Because the number of observation stacks affects the observation shape,
            // it is not editable during play mode.
            EditorGUI.BeginDisabledGroup(!EditorUtilities.CanUpdateModelProperties());
            {
                EditorGUILayout.PropertyField(so.FindProperty("m_ObservationStacks"), new GUIContent("Stacked Raycasts"), true);
            }
            EditorGUI.EndDisabledGroup();

            if (is3d)
            {
                EditorGUILayout.PropertyField(so.FindProperty("m_StartVerticalOffset"), true);
                EditorGUILayout.PropertyField(so.FindProperty("m_EndVerticalOffset"), true);
            }
        EditorGUILayout.PropertyField(so.FindProperty("rayHitColor"), true);


        EditorGUILayout.PropertyField(so.FindProperty("rayHitNonSensorColor"), true);
            EditorGUILayout.PropertyField(so.FindProperty("rayMissColor"), true);

            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                m_RequireSensorUpdate = true;
            }

            so.ApplyModifiedProperties();
            UpdateSensorIfDirty();
        }

        void UpdateSensorIfDirty()
        {
            if (m_RequireSensorUpdate)
            {
                var sensorComponent = serializedObject.targetObject as ConfigRayComponentBase;
                sensorComponent?.UpdateSensor();
                m_RequireSensorUpdate = false;
            }
        }
    }

   

    [CustomEditor(typeof(ConfigRayComponent3D))]
    [CanEditMultipleObjects]
    internal class ConfigRayComponent3DEditor : ConfigRayComponentBaseEditor
    {
        public override void OnInspectorGUI()
        {
            OnRayPerceptionInspectorGUI(true);
        }
    }
