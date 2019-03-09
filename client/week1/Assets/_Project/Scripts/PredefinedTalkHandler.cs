using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedTalkHandler
{
    // predefined conversations
    static string[] thinking = { "It's a good thing to say, let me think.",
                            "Let me think.",
                            "Hmmm...",
                            "I will respond to you in a few seconds.",
                            "A good response takes time.",
                            "Well, I like what you said, let me think.",
                            "Responding to this takes time.",
                            "Give me a minute, To read the stars.",
                            "One seconds",
                            };

    static string[] tooClose = {"Don't push me, please.",
                        "Too close to me.",
                        "Hey",
                        "No",
                        "Please respect my personal space."
                        };

    static string[] greeting = { "Nice to see you.",
                           "Hello",
                           "I can't wait to see you.",
                            "Hey, you are too late.",
                            "Hi, how nice the weather today.",
                            "Do you bring me some icecream?"};

    public static string GetThinkingText()
    {
        return thinking[Random.Range(0, thinking.Length - 1)];
    }

    public static string GetTooClose()
    {
        return tooClose[Random.Range(0, tooClose.Length - 1)];
    }

    public static string GetGreeting()
    {
        return greeting[Random.Range(0, greeting.Length - 1)];
    }
}
