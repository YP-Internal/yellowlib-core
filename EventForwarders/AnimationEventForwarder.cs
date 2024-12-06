using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YellowPanda.EventForwarders {
    public class AnimationEventForwarder : MonoBehaviour {
        public UnityEvent[] actions;

        public void CallEvent(int index) {
            actions[index].Invoke();
        }
    }
}
