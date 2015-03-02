using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Navigation.Targets;

public enum Formation
{
    CIRCLE,
    STAR,
    FREE
}

public class SelectionManager : BaseManager<SelectionManager>, ISingleSelectionHandler, IMultipleSelectionHandler
{
    /// <summary>
    /// Selected units.
    /// </summary>
    public List<GameObject> Units;

    /// <summary>
    /// Pre-selected units.
    /// </summary>
    public List<GameObject> pUnits;

    /// <summary>
    /// Max selectable units.
    /// </summary>
    public int MaxSelectedUnits = 1;

    /// <summary>
    /// Units formation when moving.
    /// </summary>
    public Formation Formation;

    void Awake()
    {
        Units = new List<GameObject>(MaxSelectedUnits);
    }

    public void SelectObject(GameObject selectedObject, int mouseButton)
    {
        switch (mouseButton)
        {
            case 0:
                SelectUnit(selectedObject);
                break;
            case 1:
                AttackUnit(selectedObject);
                break;
            default:
                break;
        }
    }

    public void SelectPosition(Vector3 position, int mouseButton)
    {
        switch (mouseButton)
        {
            case 0:
                break;
            case 1:
                MoveToPosition(position);
                break;
            default:
                break;
        }
    }

    public void SelectedObjects(List<GameObject> selectedObjects, int buttonIndex)
    {
        // LeftControl isn't pressed
        if (!Input.GetKey(KeyCode.LeftControl) && selectedObjects.Count > 0)
        {
            // Remove all selected units.
            Units.RemoveAll(
                unit =>
                {
                    unit.GetComponent<SelectableBehavior>().Deselect();
                    return true;
                }
            );
        }

        // Remove all previously selected units, while set it to Units and activate SelectionMark.
        pUnits.RemoveAll(
            unit =>
            {
                unit.GetComponent<SelectableBehavior>().Select();

                if (!Units.Contains(unit))
                {
                    Units.Add(unit);
                }

                return true;
            }
        );
    }

    public void SelectingObjects(List<GameObject> selectedObjects, int buttonIndex)
    {
        // Remove all objects from pUnits that aren't on selectedObjects

        // For each previously selected unit.
        pUnits.RemoveAll(unit =>
        {
            // Wasn't selected at this update.
            if (!selectedObjects.Contains(unit))
            {
                // We must remove it preSelected mark.

                // If it is at Units, then we just mark it as selected.
                if (Units.Contains(unit))
                {
                    unit.GetComponent<SelectableBehavior>().Select();
                }
                // If not, we should remove preSelected mark..
                else
                {
                    unit.GetComponent<SelectableBehavior>().Deselect();
                }
                return true;
            }
            // Was selected at this update.
            else
            {
                // Try to remove it from.
                selectedObjects.Remove(unit);
                return false;
            }
        }
        );

        // All units that is on selected now, are those who aren't selected yet, so add it.
        selectedObjects.ForEach(unit =>
        {
            unit.GetComponent<SelectableBehavior>().PreSelect();
            pUnits.Add(unit);
        }
        );
    }

    private void AttackUnit(GameObject selectedObject)
    {
        foreach (GameObject unit in Units)
        {
            AttackBehavior attackBehavior = unit.GetComponent<AttackBehavior>();
            if (attackBehavior) attackBehavior.Attack(selectedObject);
        }
    }

    public void SelectUnit(GameObject selectedObject)
    {
        // If leftControl is pressed.
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // And Units already contains selectedObject, then deselect it.
            if (Units.Contains(selectedObject))
            {
                selectedObject.GetComponent<SelectableBehavior>().Deselect();
                Units.Remove(selectedObject);
            }
            // Or Units doesn't contains it, then select it.
            else
            {
                Units.Add(selectedObject);
                selectedObject.GetComponent<SelectableBehavior>().Select();
            }
        }
        // LeftControl isn't pressed.
        else
        {
            // LeftControl wasn't pressed, so clear out selections.
            Units.RemoveAll(unit =>
            {
                unit.GetComponent<SelectableBehavior>().Deselect();
                return true;
            }
            );

            // And select the selectedObject
            Units.Add(selectedObject);
            selectedObject.GetComponent<SelectableBehavior>().Select();
        }
    }

    private void MoveToPosition(Vector3 position)
    {
        int order = 0;
        foreach (GameObject unit in Units)
        {
            MovementBehavior moveBehavior = unit.GetComponent<MovementBehavior>();
            if (moveBehavior)
            {
                if (order != 0)
                {
                    moveBehavior.Move(GetCircleRelativePosition(Units[0], position, order));
                }
                else
                {
                    moveBehavior.Move(position);
                }
            }
            order++;
        }
    }

    private Vector3 GetCircleRelativePosition(GameObject reference, Vector3 position, int order)
    {
        // Calculate angle
        float angle = (360f / MaxSelectedUnits) * (float)order;

        // Rotate angle with actual object rotation
        // Lets make a look at after using position
        angle += Quaternion.LookRotation(position - reference.transform.position).eulerAngles.y;

        // Save the actual rotation of the object
        Quaternion saved = reference.transform.rotation;

        // Rotate it to the angle calculated
        reference.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        // The return value its the direction where it is looking now, times the distance of positioning.
        Vector3 returnValue = position + reference.transform.forward * 3f;

        // Return it to its previous value.
        reference.transform.rotation = saved;

        // Return value.
        return returnValue;
    }
}
