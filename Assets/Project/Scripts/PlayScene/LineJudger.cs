using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineJudger : MonoBehaviour
{
    [SerializeField] int lineNum = 0;
    public bool isTouched { get; private set; } = false;
    void Start() // 自オブジェクトのEvent Triggerに関数のコール情報を自動追加します
    {
        //Fetch the Event Trigger component from your GameObject
        EventTrigger trigger = GetComponent<EventTrigger>();
        //Create a new entry for the Event Trigger
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //Add a BeginDrag type event to the Event Trigger
        entry.eventID = EventTriggerType.BeginDrag;
        //call the BeginDrag function when the Event System detects dragging
        entry.callback.AddListener((data) => { BeginDrag((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        //Create a new entry for the Event Trigger
        entry = new EventTrigger.Entry();
        //Add a Drop type event to the Event Trigger
        entry.eventID = EventTriggerType.Drop;
        //call the Drop function when the Event System detects dragging
        entry.callback.AddListener((data) => { Drop((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        //Create a new entry for the Event Trigger
        entry = new EventTrigger.Entry();
        //Add a PointerDown type event to the Event Trigger
        entry.eventID = EventTriggerType.PointerDown;
        //call the PointerDown function when the Event System detects dragging
        entry.callback.AddListener((data) => { PointerDown((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        //Create a new entry for the Event Trigger
        entry = new EventTrigger.Entry();
        //Add a PointerEnter type event to the Event Trigger
        entry.eventID = EventTriggerType.PointerEnter;
        //call the PointerEnter function when the Event System detects dragging
        entry.callback.AddListener((data) => { PointerEnter((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        //Create a new entry for the Event Trigger
        entry = new EventTrigger.Entry();
        //Add a PointerUp type event to the Event Trigger
        entry.eventID = EventTriggerType.PointerUp;
        //call the PointerUp function when the Event System detects dragging
        entry.callback.AddListener((data) => { PointerUp((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        //Create a new entry for the Event Trigger
        entry = new EventTrigger.Entry();
        //Add a PointerExit type event to the Event Trigger
        entry.eventID = EventTriggerType.PointerExit;
        //call the PointerExit function when the Event System detects dragging
        entry.callback.AddListener((data) => { PointerExit(); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

    }
    public void BeginDrag(PointerEventData data)
    {
        //Debug.Log("BeginDrag: " + lineNum + "," + data.position.y + "," + data.pointerId);
        //CoordYPresever.AddCoordY(data.position.y, lineNum);
    }

    public void Drop(PointerEventData data)
    {
        //Debug.Log("Drop: " + lineNum + "," + data.position.y + "," + data.pointerId);
        int result = CoordYPresever.isFlick(data.position.y, lineNum);
        if (result == 1) // 長押しからフリック
        {
            Debug.Log("Flick Up!");
            PlaySceneProcessManager.JudgeTiming(lineNum, 5);
        }
        else // 長押し離す
        {
            PlaySceneProcessManager.JudgeTiming(lineNum, 2);
        }
        isTouched = false;
    }

    public void PointerDown(PointerEventData data)
    {
        //Debug.Log("PointerDown: " + lineNum);
        PlaySceneProcessManager.JudgeTiming(lineNum, 1);
        CoordYPresever.AddCoordY(data.position.y, lineNum);
        isTouched = true;
    }

    public void PointerUp(PointerEventData data)
    {
        int result = CoordYPresever.isFlick(data.position.y, lineNum);
        if (result == 1) // フリック
        {
            Debug.Log("Flick Up!");
            PlaySceneProcessManager.JudgeTiming(lineNum, 5);
        }
        else // 長押し離す
        {
            PlaySceneProcessManager.JudgeTiming(lineNum, 2);
        }
        isTouched = false;
    }

    public void PointerEnter(PointerEventData data)
    {
        //Debug.Log("PointerEnter: " + lineNum + "," + data.position.y + "," + data.pointerId);
        CoordYPresever.AddCoordY(data.position.y, lineNum);
        isTouched = true;
    }

    public void PointerExit()
    {
        isTouched = false;
    }
}
