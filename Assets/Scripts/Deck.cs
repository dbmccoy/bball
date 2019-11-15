using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public List<Card> hand = new List<Card>();

    public Transform handPos;

    public int maxHandSize;
    public int currentHandSize;
    public float defaultCardOffset;
    public float handSpaceWidth;

    public void Draw() {
        var card = cards.FirstOrDefault();
        if(card == null) {
            Debug.Log("card draw fail");
            return;
        }

        hand.Add(card);
        OrderHand();
    }

    public void AddCard(Card c) {
        cards.Add(c);
    }

    public void Discard(Card c) {
        hand.Remove(c);
        OrderHand();
    }

    public void OrderHand() {
        float initOffset = handSpaceWidth / 2;
        for (int i = 0; i < hand.Count; i++) {
            hand[i].CardObj.transform.position = new Vector3((handPos.position.x - initOffset) + (hand.Count/handSpaceWidth) * defaultCardOffset, handPos.position.y, handPos.position.z);
        }
    }


}
