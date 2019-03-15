using Microsoft.CognitiveServices.Speech;
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
using AldenNet;

namespace AzureSpeechTest
{
    

    public class SpeechContent
    {
        public string content;
        public float pitch;
    }

    public enum Emotion
    {
        Natual = 0,
        Smile,
        Sad,

        Total
    }

    public static class Utility
    {
        public static Emotion ConvertEmotion(string _str)
        {
            if (_str == "natural")
                return Emotion.Natual;
            else if (_str == "happy")
                return Emotion.Smile;
            else if (_str == "sad")
                return Emotion.Sad;
            else
                return Emotion.Total;
        }

        public static string ConvertEmotion(Emotion _emo)
        {
            switch (_emo)
            {
                case Emotion.Natual:
                    return "natural";
                case Emotion.Sad:
                    return "sad";
                case Emotion.Smile:
                    return "happy";
                default:
                    return "unknown";

            }
        }
    }

    public class ReturnContent
    {
        public string response;
        public string emotion;
        public float confidence;

        public static byte[] ToByte(ReturnContent _content)
        {
            byte[] resByte = Encoding.UTF8.GetBytes(_content.response);
            Emotion emo = Utility.ConvertEmotion(_content.emotion);
            byte[] emoByte = BitConverter.GetBytes((int)emo);
            byte[] confByte = BitConverter.GetBytes((float)_content.confidence);

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
        static string accessToken;
        static NetworkLayer network;
        static ReturnContent returnContent;
        static string backendUrl = @"http://localhost:5000/listen";
        static string AzureKey = "";
        //static NetworkModule netowrkModule;
        static AldenNet.AldenNet netowrkModule;
        static void Main(string[] args)
        {
            try
            {
                LoadAzureKey();

                network = new NetworkLayer();

                // init network
                netowrkModule = new AldenNet.AldenNet();
                netowrkModule.port = 6000;
                netowrkModule.Init(true);
                netowrkModule.GetSerer().OnPeerConnected += (p) => 
                {
                    p.Output += (b) =>
                    {
                        MemoryStream ms = new MemoryStream(b);
                        byte[] cmdB = new byte[sizeof(Int32)];
                        ms.Read(cmdB, 0, sizeof(Int32));
                        int cmd = BitConverter.ToInt32(cmdB, 0);
                        Console.WriteLine(Encoding.ASCII.GetString(b));

                        //"PlayerTalking"
                        if (cmd == 1000)
                        {
                            RecognitionWithMicrophoneAsync().Wait();

                            if (string.IsNullOrEmpty(recognizedByMS))
                                return;

                            SendPitchRequestToUnity();
                        }
                        else if (cmd == 1001)
                        {
                            byte[] pitchB = new byte[sizeof(float)];
                            ms.Read(pitchB, 0, sizeof(float));
                            float pitch = BitConverter.ToSingle(pitchB, 0);
                            SpeechContent c = new SpeechContent() { content = recognizedByMS, pitch = pitch };
                            string json = JsonConvert.SerializeObject(c, Formatting.None);
                            string retJson = network.PostJson(backendUrl, json);
                            if (string.IsNullOrEmpty(retJson))
                            {
                                Console.WriteLine("Json from backend is empty!");
                                return;
                            }
                            Console.WriteLine("Json from backend = " + retJson);
                            returnContent = JsonConvert.DeserializeObject<ReturnContent>(retJson);
                            string recognizedByTheVoice = returnContent.response;
                            StartTTS(recognizedByTheVoice);
                        }
                        else if (cmd == 1002)   // predefined text, example: thinking talk
                        {
                            byte[] sizeB = new byte[sizeof(Int32)];
                            ms.Read(sizeB, 0, sizeof(Int32));
                            int size = BitConverter.ToInt32(sizeB, 0);
                            byte[] strB = new byte[size];
                            ms.Read(strB, 0, size);
                            string content = Encoding.UTF8.GetString(strB);
                            byte[] emoB = new byte[sizeof(Int32)];
                            ms.Read(emoB, 0, sizeof(Int32));
                            Emotion emotion = (Emotion)BitConverter.ToInt32(emoB, 0);
                            byte[] confB = new byte[sizeof(float)];
                            ms.Read(confB, 0, sizeof(float));
                            returnContent = new ReturnContent()
                            {
                                confidence = BitConverter.ToSingle(confB, 0),
                                response = content,
                                emotion = Utility.ConvertEmotion(emotion)
                            };
                            StartTTS(content);
                        }
                        else
                            Console.WriteLine("Unkown param:" + cmd);

                        ms.Close();
                    };
                };

                // initiazize
                AuthTTS();

                netowrkModule.GetSerer().Listen();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                Console.ReadKey();

                return;
            }



        }
        public static void LoadAzureKey()
        {
            AzureKey = File.ReadAllText("AzureKey.key");

            if (string.IsNullOrEmpty(AzureKey))
            {
                Console.WriteLine("No Azure Key");
            }
        }

    public static void AuthTTS()
        {
            Console.WriteLine("Start TextToSpeech");
            
            Authentication auth = new Authentication("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken", AzureKey);
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

        public static void StartTTS(string _content)
        {
            if (string.IsNullOrEmpty(_content))
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
                Text = _content,
                VoiceType = Gender.Female,  // useless!
                // Refer to the documentation for complete list of supported locales.
                Locale = "en-US",
                // You can also customize the output voice. Refer to the documentation to view the different
                // voices that the TTS service can output.
                VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)",
                //VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, Guy24KRUS)",
                // VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",

                // Service can return audio in different output format.
                //OutputFormat = AudioOutputFormat.Riff24Khz16BitMonoPcm,
                OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
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
            Console.WriteLine(directPlay==true? "Play audio":"Send audio to unity");
            //Console.WriteLine(args.EventData);

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
            List<AldenNet.AldenNetServer.AldenNetPeer> peers = netowrkModule.GetSerer().GetAllPeer();
            foreach(var p in peers)
                netowrkModule.GetSerer().SendToPeer(p, buffer);
            ms.Close();
        }

        static void SendPitchRequestToUnity()
        {
            // perpare stream
            MemoryStream ms = new MemoryStream();
            // write command id = 2
            ms.Write(BitConverter.GetBytes(2), 0, sizeof(int));
            byte[] bs = Encoding.UTF8.GetBytes(recognizedByMS);
            int size = bs.Length;
            ms.Write(BitConverter.GetBytes(size), 0, sizeof(int));
            ms.Write(bs, 0, size);

            // read
            byte[] buffer = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(buffer, 0, buffer.Length);
            List<AldenNet.AldenNetServer.AldenNetPeer> peers = netowrkModule.GetSerer().GetAllPeer();
            foreach (var p in peers)
                netowrkModule.GetSerer().SendToPeer(p, buffer);
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
            var config = SpeechConfig.FromSubscription(AzureKey, "westus");

            // Creates a speech recognizer using microphone as audio input.
            // SpeechRecognizer constructor without params for microphone
            using (var recognizer = new SpeechRecognizer(config))
            {
                // Starts recognizing.
                Console.WriteLine("Say something...");
                recognizedByMS = "";

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
