using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
}
