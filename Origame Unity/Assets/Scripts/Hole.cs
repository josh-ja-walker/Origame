using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : ProximityActivator
{
    
    void FixedUpdate() {
        Collider2D col = Physics2D.OverlapBox(transform.position + (Vector3) checkOffset, checkSize, 0f, activateLayer);

        if (col != null) {
            Ball ball = col.GetComponent<Ball>();
            
            if (ball != null) {
                Activate();
                ball.Freeze();
                return;
            }
        }

        Deactivate();
    }

}
