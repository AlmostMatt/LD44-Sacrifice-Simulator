using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class dirtychecker : MonoBehaviour {
    // Workaround taken from 
    // https://issuetracker.unity3d.com/issues/scene-is-instantly-marked-dirty-when-opened-if-any-object-is-affected-by-both-a-contentsizefitter-and-verticallayoutgroup

    static dirtychecker()
    {
        Undo.postprocessModifications += OnPostProcessModifications;
    }

    private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
    {
        Debug.LogWarning("Scene was marked Dirty by number of objects = " + propertyModifications.Length);
        for (int i = 0; i < propertyModifications.Length; i++)
        {
            Debug.LogWarning("currentValue.target = " + propertyModifications[i].currentValue.target);
        }
        return propertyModifications;
    }
} 