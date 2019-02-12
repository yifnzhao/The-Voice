﻿using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesTTS;
using System.IO;
using System.Media;
using System.Threading;
using Newtonsoft.Json;
using ShuoScripts;

namespace AzureSpeechTest
{

    public class SpeechContent
    {
        public string content;
    }

    public enum Emotion
    {
        Natual = 0,
        Smile,
        Sad,

        Total
    }

    public class ReturnContent
    {
        public string response;
        public string emotion;
        public int confidence;

        public static byte[] ToByte(ReturnContent _content)
        {
            byte[] resByte = Encoding.UTF8.GetBytes(_content.response);
            Emotion emo = Emotion.Total;
            if (_content.emotion == "natural")
                emo = Emotion.Natual;
            else if (_content.emotion == "happy")
                emo = Emotion.Smile;
            else if (_content.emotion == "sad")
                emo = Emotion.Sad;
            byte[] emoByte = BitConverter.GetBytes((int)emo);
            byte[] confByte = BitConverter.GetBytes((int)_content.confidence);

            byte[] total = new byte[sizeof(int) + resByte.Length + emoByte.Length + confByte.Length];
            MemoryStream ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(resByte.Length), 0, sizeof(int));
            ms.Write(resByte, 0, resByte.Length);
            ms.Write(emoByte, 0, emoByte.Length);
            ms.Write(confByte, 0, confByte.Length);
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(total, 0, total.Length);
            ms.Close();
            return total;
        }
    }

    public class TextToUnity
    {
        public string content;
        public float duration;
    }

    class Program
    {
        static string recognizedByMS;
        static string recognizedByTheVoice;
        static string accessToken;
        static NetworkLayer network;
        static ReturnContent returnContent;
        static string backendUrl = @"http://localhost:5000/listen";

        static NetworkModule netowrkModule;
        static void Main(string[] args)
        {
            try
            {
                network = new NetworkLayer();

                // init network
                netowrkModule = new NetworkModule();
                netowrkModule.recvPort = 6000;
                netowrkModule.sendPort = 6001;
                netowrkModule.Init(true);
                netowrkModule.Recv();
                netowrkModule.Output += (b) =>
                {
                    Console.WriteLine(Encoding.ASCII.GetString(b));

                    //string str = "backStrbackStrbackStrbackStrbackStr";
                    //netowrkModule.Send(Encoding.ASCII.GetBytes(str));

                    //FileStream fs = File.Open("voice.wav", FileMode.Open);
                    //SendStreamToUnity(fs);
                    //fs.Close();
                    string cmd = Encoding.ASCII.GetString(b);
                    if (cmd == "PlayerTalking")
                    {
                        RecognitionWithMicrophoneAsync().Wait();

                        if (string.IsNullOrEmpty(recognizedByMS))
                            return;
                        SpeechContent c = new SpeechContent() { content = recognizedByMS };
                        string json = JsonConvert.SerializeObject(c, Formatting.Indented);
                        string retJson = network.PostJson(backendUrl, json);
                        Console.WriteLine("Json from backend = " + retJson);
                        returnContent = JsonConvert.DeserializeObject<ReturnContent>(retJson);
                        recognizedByTheVoice = returnContent.response;
                        StartTTS();
                    }
                    else
                        Console.WriteLine("Unkown param:" + cmd);
                };

                // initiazize
                AuthTTS();

                //do
                //{
                //    RecognitionWithMicrophoneAsync().Wait();

                //    if (string.IsNullOrEmpty(recognizedByMS))
                //        continue;
                //    SpeechContent c = new SpeechContent() { content = recognizedByMS };
                //    string json = JsonConvert.SerializeObject(c, Formatting.Indented);
                //    string retJson = network.PostJson(backendUrl, json);
                //    Console.WriteLine("Json from backend = " + retJson);
                //    ReturnContent ret = JsonConvert.DeserializeObject<ReturnContent>(retJson);
                //    recognizedByTheVoice = ret.response;
                //    StartTTS();

                //    Console.ReadKey();

                //} while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                Console.ReadKey();

                return;
            }



        }

    public static void AuthTTS()
        {
            Console.WriteLine("Start TextToSpeech");
            
            Authentication auth = new Authentication("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken", "d51c2ff78636458d91821ab43b7219d7");
            try
            {
                accessToken = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", accessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed authentication.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public static void StartTTS()
        {
            if (string.IsNullOrEmpty(recognizedByTheVoice))
            {
                Console.WriteLine("Nothing Recognized");
                return;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Authentication Failed");
                return;
            }

            string requestUri = "https://westus.tts.speech.microsoft.com/cognitiveservices/v1";
            var cortana = new Synthesize();

            cortana.OnAudioAvailable += PlayAudio;
            cortana.OnError += ErrorHandler;

            // Reuse Synthesize object to minimize latency
            cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
            {
                RequestUri = new Uri(requestUri),
                // Text to be spoken.
                Text = recognizedByTheVoice,
                VoiceType = Gender.Female,  // useless!
                // Refer to the documentation for complete list of supported locales.
                Locale = "en-US",
                // You can also customize the output voice. Refer to the documentation to view the different
                // voices that the TTS service can output.
                VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)",
                //VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Guy24KRUS)",
                // VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",

                // Service can return audio in different output format.
                OutputFormat = AudioOutputFormat.Riff24Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + accessToken,
            }).Wait();

        }

        /// <summary>
        /// This method is called once the audio returned from the service.
        /// It will then attempt to play that audio file.
        /// Note that the playback will fail if the output audio format is not pcm encoded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GenericEventArgs{Stream}"/> instance containing the event data.</param>
        private static void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            bool directPlay = false;
            //Console.WriteLine("PlayAudio");
            Console.WriteLine(args.EventData);

            if (directPlay)
            {
                // For SoundPlayer to be able to play the wav file, it has to be encoded in PCM.
                // Use output audio format AudioOutputFormat.Riff16Khz16BitMonoPcm to do that.
                SoundPlayer player = new SoundPlayer(args.EventData);
                player.PlaySync();
            }
            else
            {
                try
                {
                     SendStreamToUnity(args.EventData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            args.EventData.Dispose();
        }

        static void SendStreamToUnity(Stream _stream)
        {
            // perpare stream
            MemoryStream ms = new MemoryStream();
            // write command id = 1
            ms.Write(BitConverter.GetBytes(1), 0, sizeof(int));
            // write emotion info
            byte[] retB = ReturnContent.ToByte(returnContent);
            ms.Write(retB, 0, retB.Length);
            // write voice
            _stream.CopyTo(ms);

            // read
            byte[] buffer = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(buffer, 0, buffer.Length);
            netowrkModule.Send(buffer);
            ms.Close();
        }

        /// <summary>
        /// Handler an error when a TTS request failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenericEventArgs{Exception}"/> instance containing the event data.</param>
        private static void ErrorHandler(object sender, GenericEventArgs<Exception> e)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", e.ToString());
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
                Console.WriteLine("Say something...");

                // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,
                // so it is suitable only for single shot recognition like command or query. For long-running
                // recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Duration={result.Duration}");
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    recognizedByMS = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
            // </recognitionWithMicrophone>
        }
    }
}
