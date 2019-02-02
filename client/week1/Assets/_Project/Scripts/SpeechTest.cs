using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

public class SpeechTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //RecognitionWithMicrophoneAsync().Wait();
    }


    public static async Task RecognitionWithMicrophoneAsync()
    {
        // <recognitionWithMicrophone>
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        // The default language is "en-us".
        var config = SpeechConfig.FromSubscription("d51c2ff78636458d91821ab43b7219d7", "westus");

        // Creates a speech recognizer using microphone as audio input.
        using (var recognizer = new SpeechRecognizer(config))
        {
            // Starts recognizing.
            //Console.WriteLine("Say something...");
            //Console.WriteLine("Say something...");
            Debug.Log("Say something...");

            // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,
            // so it is suitable only for single shot recognition like command or query. For long-running
            // recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                //Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                Debug.Log($"RECOGNIZED: Text={result.Text}");
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                //Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                Debug.Log($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                //Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    //Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    //Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    //Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you update the subscription info?");
                }
            }
        }
        // </recognitionWithMicrophone>
    }

    // Update is called once per framee
    void Update ()
    {
		
	}
}
