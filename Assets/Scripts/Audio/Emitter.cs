using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : Manhattan.Musician
{
    public GameObject noteObject;
    public Vector3 speed = new Vector3 (1, 15, 5);
    private float averagePitch = 72;

    // Update is called once per video frame
    void Update()
    {
        // destroy balls when they go offscreen (beneath plane)
        Transform[] balls = GetComponentsInChildren<Transform>();
        foreach (Transform t in balls)
            if (t.position.y < -16)
                Destroy(t.gameObject);
    }

    // OnNoteOn is called in the event of a new note
    protected override void OnNoteOn(Manhattan.Note note) {
        if (active) {
            // create new object to represent note
            GameObject obj = Instantiate(noteObject, transform);
            obj.SetActive(true);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) {
                // launch in direction of note pitch
                rb.velocity = new Vector3((note.pitch - averagePitch) * speed.x, speed.y, Random.Range(-speed.z, 0));

                // track average pitch (over 8 notes) to re-centre launches while preserving note-to-note differences
                averagePitch = (7 * averagePitch + note.pitch) / 8;
            }
        }
    }
}
