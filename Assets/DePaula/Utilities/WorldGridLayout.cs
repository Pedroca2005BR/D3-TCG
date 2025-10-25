using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // permite ver o resultado no Editor
[AddComponentMenu("Layout/World Grid Layout")]
public class WorldGridLayout : MonoBehaviour
{
    public Vector2 cellSize = new Vector2(1f, 1f);
    public Vector2 spacing = Vector2.zero;
    public Vector2 padding = Vector2.zero; // left/top padding stored as x/y

    public enum Corner { UpperLeft, UpperRight, LowerLeft, LowerRight }
    public Corner startCorner = Corner.UpperLeft;

    public enum StartAxis { Horizontal, Vertical }
    public StartAxis startAxis = StartAxis.Horizontal;

    public enum Constraint { Flexible, FixedColumnCount, FixedRowCount }
    public Constraint constraint = Constraint.Flexible;
    public int constraintCount = 1; // usada quando há restrição fixa

    public bool includeInactiveChildren = false;
    public bool updateInEditor = true;
    public bool updateEveryFrame = false;

    void OnEnable()
    {
        UpdateLayout();
    }

    void Update()
    {
        if (Application.isPlaying && updateEveryFrame) UpdateLayout();
#if UNITY_EDITOR
        if (!Application.isPlaying && updateInEditor) UpdateLayout();
#endif
    }

    // Chamável manualmente se quiser controlar quando re-layout
    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            if (!includeInactiveChildren && !c.gameObject.activeSelf) continue;
            children.Add(c);
        }

        int count = children.Count;
        if (count == 0) return;

        int columns = 1;
        int rows = 1;

        if (constraint == Constraint.FixedColumnCount)
        {
            columns = Mathf.Max(1, constraintCount);
            rows = Mathf.CeilToInt((float)count / columns);
        }
        else if (constraint == Constraint.FixedRowCount)
        {
            rows = Mathf.Max(1, constraintCount);
            columns = Mathf.CeilToInt((float)count / rows);
        }
        else // Flexible: tenta formar um quadrado
        {
            columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            rows = Mathf.CeilToInt((float)count / columns);
        }

        float cellW = cellSize.x;
        float cellH = cellSize.y;
        float stepX = cellW + spacing.x;
        float stepY = cellH + spacing.y;

        float totalWidth = columns * cellW + (columns - 1) * spacing.x + padding.x * 2f;
        float totalHeight = rows * cellH + (rows - 1) * spacing.y + padding.y * 2f;

        // Começa com origem centrada no transform.localPosition (ajuste abaixo conforme canto)
        float startX = -totalWidth * 0.5f + padding.x + cellW * 0.5f;
        float startY = totalHeight * 0.5f - padding.y - cellH * 0.5f;

        // Ajusta startX/startY de acordo com o Corner selecionado
        if (startCorner == Corner.UpperLeft) { /* já ok */ }
        else if (startCorner == Corner.UpperRight) { startX = totalWidth * 0.5f - padding.x - cellW * 0.5f; stepX = -stepX; }
        else if (startCorner == Corner.LowerLeft) { startY = -totalHeight * 0.5f + padding.y + cellH * 0.5f; stepY = -stepY; }
        else if (startCorner == Corner.LowerRight) { startX = totalWidth * 0.5f - padding.x - cellW * 0.5f; startY = -totalHeight * 0.5f + padding.y + cellH * 0.5f; stepX = -stepX; stepY = -stepY; }

        for (int i = 0; i < count; i++)
        {
            int row, col;
            if (startAxis == StartAxis.Horizontal)
            {
                row = i / columns;
                col = i % columns;
            }
            else // Vertical
            {
                col = i / rows;
                row = i % rows;
            }

            Vector3 pos = new Vector3(startX + col * Mathf.Abs(stepX) * Mathf.Sign(stepX), startY - row * Mathf.Abs(stepY) * Mathf.Sign(stepY), 0f);

            // Se stepX/stepY negativos (quando canto direita ou inferior), o cálculo já tratou o sinal
            children[i].localPosition = pos;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // evita perder tempo no play mode
        if (!Application.isPlaying && updateInEditor) UpdateLayout();
    }
#endif
}
