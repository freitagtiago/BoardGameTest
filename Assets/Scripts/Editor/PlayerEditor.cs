# if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BoardGame.Game;
namespace BoardGame.Config
{
    [CustomEditor(typeof(PlayerPiece))]
    public class PlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                return;
            }
            PlayerPiece player = (PlayerPiece)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Behaviour Trees:", EditorStyles.boldLabel);

            if (player._behaviourTrees == null)
            {
                player._behaviourTrees = new System.Collections.Generic.List<BehaviourTreeSO>();
            }

            for (int i = 0; i < player._behaviourTrees.Count; i++)
            {
                BehaviourTreeSO behaviourTree = player._behaviourTrees[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(behaviourTree, typeof(BehaviourTreeSO), false);

                if (GUILayout.Button("Edit Piece Behavior Chain", GUILayout.Width(160)))
                {
                    OpenBehaviorEditor(behaviourTree);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Behaviour Chain"))
            {
                AddNewBehaviourTree(player);
            }
        }

        private void OpenBehaviorEditor(BehaviourTreeSO behaviourTree)
        {
            BehaviourTreeEditor.OpenWindow(behaviourTree);
        }

        private void AddNewBehaviourTree(PlayerPiece player)
        {
            var newBehaviourTree = ScriptableObject.CreateInstance<BehaviourTreeSO>();
            newBehaviourTree.name = $"BehaviourTree_{player.name}_{player._behaviourTrees.Count}";

            AssetDatabase.CreateAsset(newBehaviourTree, $"Assets/Resources/ScriptableObjects/BehaviourTreeAsset/{newBehaviourTree.name}.asset");
            AssetDatabase.SaveAssets();

            player._behaviourTrees.Add(newBehaviourTree);
            EditorUtility.SetDirty(player);
        }
    } 
}
# endif