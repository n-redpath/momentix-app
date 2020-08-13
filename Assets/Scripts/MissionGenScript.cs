using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionGenScript : MonoBehaviour
{
    // public List<string> deck; 

     void Start()
    {

    }

    // Update is called once per frame



    public void randomCards(){
        Debug.Log("Button Working"); 
    //     deck = generateCards(); 
    //    CardShuffle(deck); 
    //    print(deck[0]); 
    //     // foreach (string card in deck){
    //     //     print(card); 
    //     // }
    //     dealCard(); 
    }

//     public static List<string> generateCards(){
//         List<string> cardDeck = new List<string>(); 
//         cardDeck.Add("Card 1"); 
//         cardDeck.Add("Card 2"); 
//         cardDeck.Add("Card 3"); 
//         cardDeck.Add("Card 4"); 
//         cardDeck.Add("Card 5"); 
//         cardDeck.Add("Card 6"); 
//         cardDeck.Add("Card 7"); 
//         cardDeck.Add("Card 8"); 
//         cardDeck.Add("Card 9");
//         cardDeck.Add("Card 10"); 
//         cardDeck.Add("Card 11"); 

//         return cardDeck; 
//     }

//     void CardShuffle<T>(List<T> list)  
// {  
//     System.Random random = new System.Random();  
//     int n = list.Count;  
//     while (n > 1) {  
//         int k = random.Next(n);  
//         n--;  
//         T temp = list[k];  
//         list[k] = list[n];  
//         list[n] = temp;  
//     }

//     //pick one specific card by doing a random number generator, then start by logging it in the console
//     // then update the scripts   
// }

// //once I have that woring, repeat the process for the objects and then also the final goal, so there should be like 3 different things that are being shuffled. 

 void Update(){
    }
}
