using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView _treeView;
    InspectorView _inspectorView;

    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

    private BehaviourTreeSO _selectedBehaviourTree;

    [MenuItem("BehaviourTreeEditor/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("Behaviour Tree Editor");
    }

    public static void OpenWindow(BehaviourTreeSO behaviourTree)
    {
        var window = GetWindow<BehaviourTreeEditor>();
        window.titleContent = new GUIContent($"{behaviourTree.name}");
        window.Show();
        window.FocusOnTree(behaviourTree);
    }

    public void FocusOnTree(BehaviourTreeSO behaviourTree)
    {
        _selectedBehaviourTree = behaviourTree;
        
        if (_treeView != null)
        {
            _treeView.PopulateView(behaviourTree);
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BehaviourTreeSO tree = Selection.activeObject as BehaviourTreeSO;
        if (tree
            && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            _treeView.PopulateView(tree);
            titleContent = new GUIContent($"{tree.name}");
        }
    }

    private void OnNodeSelectionChanged(NodeView node)
    {
        _inspectorView.UpdateSelection(node);
    }
}
