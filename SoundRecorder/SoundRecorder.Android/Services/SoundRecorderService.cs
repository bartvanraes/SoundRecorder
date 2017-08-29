﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SoundRecorder.Interfaces;
using Android.Media;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using SoundRecorder.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(SoundRecorderService))]
namespace SoundRecorder.Droid.Services
{
    public class SoundRecorderService : ISoundRecorderService
    {
        public Action<bool> RecordingStateChanged;

        private static string filePath; // = "/data/data/Example_WorkingWithAudio.Example_WorkingWithAudio/files/testAudio.wav";
        private static string documentsPath;
        private byte[] audioBuffer = null;
        private AudioRecord audioRecord = null;
        private bool endRecording = false;
        private bool isRecording = false;

        public SoundRecorderService()
        {
            documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }


        public Boolean IsRecording
        {
            get { return (isRecording); }
        }

        public async Task StartRecordingAsync(Guid sessionId)
        {
            filePath = Path.Combine(documentsPath, String.Format("{0}.mp4", sessionId.ToString()));
            await StartRecorderAsync();
        }

        public void StopRecording(Guid sessionId)
        {
            endRecording = true;
            Thread.Sleep(500); // Give it time to drop out.
        }

        private async Task ReadAudioAsync()
        {
            using (var fileStream = new FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                while (true)
                {
                    if (endRecording)
                    {
                        endRecording = false;
                        break;
                    }
                    try
                    {
                        // Keep reading the buffer while there is audio input.
                        int numBytes = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
                        await fileStream.WriteAsync(audioBuffer, 0, numBytes);
                        // Do something with the audio input.
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine(ex.Message);
                        break;
                    }
                }
                fileStream.Close();
            }
            audioRecord.Stop();
            audioRecord.Release();
            isRecording = false;

            RaiseRecordingStateChangedEvent();
        }

        private void RaiseRecordingStateChangedEvent()
        {
            if (RecordingStateChanged != null)
                RecordingStateChanged(isRecording);
        }

        protected async Task StartRecorderAsync()
        {
            endRecording = false;
            isRecording = true;

            RaiseRecordingStateChangedEvent();

            audioBuffer = new Byte[100000];
            audioRecord = new AudioRecord(
                // Hardware source of recording.
                AudioSource.Mic,
                // Frequency
                11025,
                // Mono or stereo
                ChannelIn.Mono,
                // Audio encoding
                Android.Media.Encoding.Pcm16bit,
                // Length of the audio clip.
                audioBuffer.Length
            );

            audioRecord.StartRecording();

            // Off line this so that we do not block the UI thread.
            await ReadAudioAsync();
        }

        

        
    }
}