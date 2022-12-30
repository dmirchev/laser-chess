using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LaserChess
{
    [CustomEditor(typeof(PieceInfo))]
    public class PieceInfo_Inspector : Editor
    {
        PieceInfoVisualElements movePieceInfoVisualElements;
        PieceInfoVisualElements attackPieceInfoVisualElements;

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement rootVisualElement = new VisualElement();

            movePieceInfoVisualElements = new PieceInfoVisualElements();
            attackPieceInfoVisualElements = new PieceInfoVisualElements();

            PieceInfo pieceInfo = this.target as PieceInfo;

            rootVisualElement.Add(new TextField() { label = "Label", bindingPath = "label" });

            rootVisualElement.Add(new Label() { text = "Move" });
            DirGrid(rootVisualElement, 0, movePieceInfoVisualElements);
            RedrawGrid(rootVisualElement, 0, movePieceInfoVisualElements, pieceInfo.movePieceGridBehaviour.directionsOrPositions);
            SetDirectionPositionGrids(movePieceInfoVisualElements, pieceInfo.movePieceGridBehaviour.canLeap);

            rootVisualElement.Add(new Label() { text = "Attack" });
            DirGrid(rootVisualElement, 1, attackPieceInfoVisualElements);
            RedrawGrid(rootVisualElement, 1, attackPieceInfoVisualElements, pieceInfo.attackPieceGridBehaviour.directionsOrPositions);
            SetDirectionPositionGrids(attackPieceInfoVisualElements, pieceInfo.attackPieceGridBehaviour.canLeap);

            rootVisualElement.Add(new ObjectField() { label = "Mesh", bindingPath = "mesh", objectType = typeof(Mesh) });

            // InspectorElement.FillDefaultInspector(rootVisualElement, serializedObject, this);

            // Return the finished inspector UI
            return rootVisualElement;
        }

        public void DirGrid(VisualElement rootVisualElement, int fieldIndex, PieceInfoVisualElements pieceInfoVisualElements)
        {
            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(fieldIndex);
            string pieceGridBehaviourName = GetPieceGridBehaviourName(fieldIndex);

            VisualElement container = new VisualElement();
            container.style.alignItems = new StyleEnum<Align>(Align.FlexStart);
            pieceInfoVisualElements.directionsGroupBox = new GroupBox();
            SetMarginAndPaddingToZero(pieceInfoVisualElements.directionsGroupBox.style);
            container.Add(pieceInfoVisualElements.directionsGroupBox);
            int rows = 3;
            int gridSize = 1;
            var innerGroupBoxes = new List<GroupBox>(rows);
            for (int i = 0; i < rows; i++)
            {
                var innerGroupBox = new GroupBox();
                innerGroupBox.style.flexDirection = FlexDirection.Row;
                innerGroupBox.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);
                SetMarginAndPaddingToZero(innerGroupBox.style);
                innerGroupBoxes.Add(innerGroupBox);
                pieceInfoVisualElements.directionsGroupBox.Add(innerGroupBox);
            }

            int elementsCount = rows*rows;

            pieceInfoVisualElements.magnitudeIntegerFields = new List<IntegerField>(elementsCount);
            for (int i = 0; i < elementsCount; i++)
            {
                int backwardsIndex = rows-(i / rows)-1;

                int x = (i % rows) - gridSize;
                int y = gridSize - backwardsIndex;

                var newIntegerField = new IntegerField();
                SetMarginAndPaddingToZero(newIntegerField.style, 2.0f);
                newIntegerField.label = $"{x}/{y}/{i}/{fieldIndex}";
                newIntegerField.labelElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                newIntegerField.SetEnabled(!(x == 0 && y == 0));
                newIntegerField.RegisterValueChangedCallback(OnChangedMagnitudeEvent);
                
                pieceInfoVisualElements.magnitudeIntegerFields.Add(newIntegerField);
                innerGroupBoxes[backwardsIndex].Add(newIntegerField);
            }

            int index;
            Vector2Int direction;
            for (int i = 0; i < pieceGridBehaviour.directionsOrPositions.Count; i++)
            {
                direction = pieceGridBehaviour.directionsOrPositions[i];
                index = GetIndexFromPosition(
                    GetRoundedDirection(direction),
                    gridSize, rows
                );

                pieceInfoVisualElements.magnitudeIntegerFields[index].SetValueWithoutNotify(GetRoundedMagnitude(direction));
            }

            pieceInfoVisualElements.canLeapToggle = new Toggle() {label = "Can Leap", bindingPath = $"{pieceGridBehaviourName}.canLeap"};
            pieceInfoVisualElements.canLeapToggle.text = fieldIndex.ToString();
            pieceInfoVisualElements.canLeapToggle.style.color = new StyleColor(Color.clear);
            pieceInfoVisualElements.canLeapToggle.RegisterValueChangedCallback(OnChangedCanLeapEvent);

            pieceInfoVisualElements.isInfiniteToggle = new Toggle() {label = "Is Infinite", bindingPath = $"{pieceGridBehaviourName}.isInfinite"};
            pieceInfoVisualElements.isInfiniteToggle.text = fieldIndex.ToString();
            pieceInfoVisualElements.isInfiniteToggle.style.color = new StyleColor(Color.clear);
            pieceInfoVisualElements.isInfiniteToggle.RegisterValueChangedCallback(OnChangedIsInfiniteEvent);

            rootVisualElement.Add(pieceInfoVisualElements.canLeapToggle);
            rootVisualElement.Add(pieceInfoVisualElements.isInfiniteToggle);

            pieceInfoVisualElements.label = new Label();
            rootVisualElement.Add(pieceInfoVisualElements.label);
            rootVisualElement.Add(container);
        }

        private void OnChangedMagnitudeEvent(ChangeEvent<int> evt)
        {
            IntegerField integerField = evt.currentTarget as IntegerField;

            int index, fieldIndex;
            Vector2Int v = GetCoordinates(integerField.label, out index, out fieldIndex);

            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(fieldIndex);

            if (pieceGridBehaviour.isInfinite && evt.newValue > 1)
            {
                integerField.SetValueWithoutNotify(1);
                return;
            }

            pieceGridBehaviour.directionsOrPositions.Remove(v * evt.previousValue);
            if (evt.newValue != 0) pieceGridBehaviour.directionsOrPositions.Add(v * evt.newValue);
        }

        private void OnChangedCanLeapEvent(ChangeEvent<bool> evt)
        {
            Toggle toggle = evt.currentTarget as Toggle;

            PieceInfoVisualElements pieceInfoVisualElements = GetPieceInfoVisualElements(int.Parse(toggle.text));
            SetDirectionPositionGrids(pieceInfoVisualElements, evt.newValue);

            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(int.Parse(toggle.text));

            pieceGridBehaviour.directionsOrPositions.Clear();

            for (int i = 0; i < pieceInfoVisualElements.magnitudeIntegerFields.Count; i++)
                pieceInfoVisualElements.magnitudeIntegerFields[i].SetValueWithoutNotify(0);

            for (int i = 0; i < pieceInfoVisualElements.positionsToggles.Count; i++)
                pieceInfoVisualElements.positionsToggles[i].SetValueWithoutNotify(false);
        }

        void SetDirectionPositionGrids(PieceInfoVisualElements pieceInfoVisualElements, bool canLeap)
        {
            pieceInfoVisualElements.label.text = canLeap ? "Positions" : "Directions";

            pieceInfoVisualElements.directionsGroupBox.style.display = new StyleEnum<DisplayStyle>(
                canLeap ? DisplayStyle.None : DisplayStyle.Flex
            );

            pieceInfoVisualElements.positionsGroupBox.style.display = new StyleEnum<DisplayStyle>(
                canLeap ? DisplayStyle.Flex : DisplayStyle.None
            );

            pieceInfoVisualElements.isInfiniteToggle.style.display = canLeap ? DisplayStyle.None : DisplayStyle.Flex;
            pieceInfoVisualElements.positionsGridSizeIntegerField.style.display = canLeap ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnChangedIsInfiniteEvent(ChangeEvent<bool> evt)
        {
            Toggle toggle = evt.currentTarget as Toggle;

            int fieldIndex = int.Parse(toggle.text);
            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(fieldIndex);
            PieceInfoVisualElements pieceInfoVisualElements = GetPieceInfoVisualElements(fieldIndex);

            if (evt.newValue)
            {
                Vector2Int v;
                int index;
                for (int i = 0; i < pieceGridBehaviour.directionsOrPositions.Count; i++)
                {
                    v = GetRoundedDirection(pieceGridBehaviour.directionsOrPositions[i]);
                    index = GetIndexFromPosition(v, 1, 3);
                    pieceGridBehaviour.directionsOrPositions[i] = v;
                    pieceInfoVisualElements.magnitudeIntegerFields[index].SetValueWithoutNotify(1);
                }
            }
        }

        public void RedrawGrid(VisualElement rootVisualElement, int fieldIndex, PieceInfoVisualElements pieceInfoVisualElements, List<Vector2Int> vector2Ints)
        {
            int gridSize = GetLargestMagnitude(vector2Ints);;
            pieceInfoVisualElements.positionsGridSizeIntegerField = new IntegerField() { value = gridSize, maxLength = 1 };
            pieceInfoVisualElements.positionsGridSizeIntegerField.label = fieldIndex.ToString();
            pieceInfoVisualElements.positionsGridSizeIntegerField.labelElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            pieceInfoVisualElements.positionsGridSizeIntegerField.RegisterValueChangedCallback(OnChangedPositionsGridSizeEvent);

            DrawPositionGrid(gridSize, pieceInfoVisualElements, fieldIndex, vector2Ints);

            rootVisualElement.Add(pieceInfoVisualElements.positionsGridSizeIntegerField);
            rootVisualElement.Add(pieceInfoVisualElements.positionsGroupBox);
        }

        private void OnChangedPositionsGridSizeEvent(ChangeEvent<int> evt)
        {
            IntegerField integerField = evt.currentTarget as IntegerField;

            if (evt.newValue < 1)
            {
                integerField.SetValueWithoutNotify(1);
                return;
            }

            if (evt.newValue > 10)
            {
                integerField.SetValueWithoutNotify(9);
                return;
            }

            int fieldIndex = int.Parse(integerField.label);

            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(fieldIndex);
            pieceGridBehaviour.directionsOrPositions.Clear();

            PieceInfoVisualElements pieceInfoVisualElements = GetPieceInfoVisualElements(fieldIndex);

            DrawPositionGrid(evt.newValue, pieceInfoVisualElements, fieldIndex);
        }

        void DrawPositionGrid(int gridSize, PieceInfoVisualElements pieceInfoVisualElements, int fieldIndex, List<Vector2Int> vector2Ints = null)
        {
            bool isInitial = vector2Ints != null;
            if (isInitial) pieceInfoVisualElements.positionsGroupBox = new GroupBox();

            pieceInfoVisualElements.positionsGroupBox.Clear();
            SetMarginAndPaddingToZero(pieceInfoVisualElements.positionsGroupBox.style);

            int rows = (gridSize*2)+1;
            var innerGroupBoxes = new List<GroupBox>(rows);
            for (int i = 0; i < rows; i++)
            {
                var innerGroupBox = new GroupBox();
                innerGroupBox.style.flexDirection = FlexDirection.Row;
                SetMarginAndPaddingToZero(innerGroupBox.style);
                innerGroupBoxes.Add(innerGroupBox);
                pieceInfoVisualElements.positionsGroupBox.Add(innerGroupBox);
            }

            int elementsCount = rows*rows;
            pieceInfoVisualElements.positionsToggles = new List<Toggle>(elementsCount);
            Vector2Int vector2Int = Vector2Int.zero;
            for (int i = 0; i < elementsCount; i++)
            {
                int backwardsIndex = rows-(i / rows)-1;

                int x = (i % rows) - gridSize;
                int y = gridSize - backwardsIndex;

                var newToggle = new Toggle();
                SetMarginAndPaddingToZero(newToggle.style, 1.0f);
                newToggle.label = $"{x}/{y}/{i}/{fieldIndex}";
                newToggle.labelElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                if ((x == 0 && y == 0)) // isCenter
                    newToggle.SetEnabled(false);
                else
                    newToggle.SetEnabled((x == 0 || y == 0) || (x == y) || (x == -y));
                if(isInitial)
                {
                    vector2Int.x = x;
                    vector2Int.y = y;
                    newToggle.SetValueWithoutNotify(vector2Ints.Contains(vector2Int));
                }
                newToggle.RegisterValueChangedCallback(OnChangedPositionEvent);
                
                pieceInfoVisualElements.positionsToggles.Add(newToggle);
                innerGroupBoxes[backwardsIndex].Add(newToggle);
            }
        }

        void OnChangedPositionEvent(ChangeEvent<bool> evt)
        {
            Toggle toggle = evt.currentTarget as Toggle;

            int index, fieldIndex;
            Vector2Int v = GetCoordinates(toggle.label, out index, out fieldIndex);

            PieceGridBehaviour pieceGridBehaviour = GetPieceGridBehaviour(fieldIndex);

            if (evt.newValue)
                pieceGridBehaviour.directionsOrPositions.Add(v);
            else
                pieceGridBehaviour.directionsOrPositions.Remove(v);
        }

        PieceInfoVisualElements GetPieceInfoVisualElements(int fieldIndex)
        {
            return fieldIndex == 0 ? movePieceInfoVisualElements : attackPieceInfoVisualElements;
        }

        PieceGridBehaviour GetPieceGridBehaviour(int fieldIndex)
        {
            PieceInfo pieceInfo = this.target as PieceInfo;
            return fieldIndex == 0 ? pieceInfo.movePieceGridBehaviour : pieceInfo.attackPieceGridBehaviour;
        }

        string GetPieceGridBehaviourName(int fieldIndex)
        {
            return fieldIndex == 0 ? "movePieceGridBehaviour" : "attackPieceGridBehaviour";
        }

        int GetIndexFromPosition(Vector2Int v, int gridSize, int rows)
        {
            return v.x + gridSize + (v.y + gridSize) * rows;
        }

        Vector2Int GetCoordinates(string label, out int index, out int fieldIndex)
        {
            string[] separated = label.Split('/');

            int x = int.Parse(separated[0]);
            int y = int.Parse(separated[1]);
            index = int.Parse(separated[2]);
            fieldIndex = int.Parse(separated[3]);

            return new Vector2Int(x, y);
        }

        Vector2Int GetRoundedDirection(Vector2Int v)
        {
            return new Vector2Int(
                v.x == 0 ? 0 : ((int)Mathf.Sign(v.x))*v.x/v.x, 
                v.y == 0 ? 0 : ((int)Mathf.Sign(v.y))*v.y/v.y
            );
        }

        int GetRoundedMagnitude(Vector2Int v)
        {
            return Mathf.Abs(v.x == 0 ? v.y : v.x);
        }

        int GetLargestMagnitude(List<Vector2Int> vector2Ints)
        {
            int largestMagnitude = 1;
            Vector2Int vector2Int;
            for (int i = 0; i < vector2Ints.Count; i++)
            {
                vector2Int = vector2Ints[i];

                if (Mathf.Abs(vector2Int.x) > largestMagnitude)
                    largestMagnitude = Mathf.Abs(vector2Int.x);

                if (Mathf.Abs(vector2Int.y) > largestMagnitude)
                    largestMagnitude = Mathf.Abs(vector2Int.y);
            }

            return largestMagnitude;
        }

        public void SetMarginAndPaddingToZero(IStyle style, float margin = 0, float padding = 0)
        {
            StyleLength marginStyleLength = new StyleLength(margin);
            StyleLength paddingStyleLength = new StyleLength(padding);

            style.marginBottom = marginStyleLength;
            style.marginLeft = marginStyleLength;
            style.marginRight = marginStyleLength;
            style.marginTop = marginStyleLength;

            style.paddingBottom = paddingStyleLength;
            style.paddingLeft = paddingStyleLength;
            style.paddingRight = paddingStyleLength;
            style.paddingTop = paddingStyleLength;
        }

        /* private void OnBoolChangedEvent(ChangeEvent<bool> evt)
        {
            Toggle t = evt.currentTarget as Toggle;
            PieceInfo pieceInfo = this.target as PieceInfo;

            int index, fieldIndex;
            Vector2Int v = GetCoordinates(t.label, out index, out fieldIndex);

            PieceInfoVisualElements pieceInfoVisualElements = GetPieceInfoVisualElements(fieldIndex);
            int gridSize = pieceInfoVisualElements.positionsGridSizeIntegerField.value;

            int rows = (gridSize*2)+1;

            Vector2Int vDir = GetRoundedDirection(v);

            int safeCount = 0;

            if (evt.previousValue)
            {
                Vector2Int vNext = v + vDir;
                if (Mathf.Abs(vNext.x) <= gridSize && Mathf.Abs(vNext.y) <= gridSize)
                {
                    // Goint to Edge
                    if(pieceInfoVisualElements.positionsToggles[GetIndexFromPosition(vNext, gridSize, rows)].value)
                    {
                        pieceInfoVisualElements.positionsToggles[index].SetValueWithoutNotify(evt.previousValue);

                        safeCount = vNext.x != 0 ? Mathf.Abs(vNext.x) : Mathf.Abs(vNext.y);

                        while(safeCount <= gridSize)
                        {
                            index = GetIndexFromPosition(vNext, gridSize, rows);
                            pieceInfoVisualElements.positionsToggles[index].SetValueWithoutNotify(evt.newValue);
                            vNext += vDir;
                            
                            safeCount++;
                        }

                        return;
                    }
                }
            }

            safeCount = v.x != 0 ? Mathf.Abs(v.x) : Mathf.Abs(v.y);

            // Going to Center
            Vector2Int vOppDir = -vDir;

            v += vOppDir;
            safeCount--;

            while(safeCount > 0)
            {
                index = GetIndexFromPosition(v, gridSize, rows);
                pieceInfoVisualElements.positionsToggles[index].SetValueWithoutNotify(evt.newValue);
                v += vOppDir;

                safeCount--;
            }
        } */
    }

    public class PieceInfoVisualElements
    {
        public Label label;

        public GroupBox directionsGroupBox;
        public List<IntegerField> magnitudeIntegerFields;

        public IntegerField positionsGridSizeIntegerField;
        public GroupBox positionsGroupBox;
        public List<Toggle> positionsToggles;

        public Toggle canLeapToggle;
        public Toggle isInfiniteToggle;
    }
}