using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using WMPLib;


namespace Civilization.Models
{

  /*  class Mp3Player
    {
        private readonly string introMP3path;
        private readonly string musicMP3path;
        //private WindowsMediaPlayer wmPlayer;
        private bool playing;
        private Task playerLoopTask;

        /* 
         * intro - ścieżka do pliku który będzie zagrany jako pierwszy, 
         *          można podac "", wtedy intro zostaniepominięte
         * music - kawałek który po odtworzeniu intra będzie odtwarzał się w pętli
         * 
         * niestety przy przejsciach pomiędzy utworami słychać krótką przerwę, 
         * nie wiem czy i jak to naprawić ale chyba nie jest to na tyle ważne 
         * żeby szukać specjalnych PRO bibliotek do odtwarzania muzyki
         * 
         * jeśli nie będzie widział WMPLib trzeba go jakoś zaimportować, jest chyba w System32
         */
/*        public Mp3Player(string intro, string music) {
            introMP3path = Path.GetFullPath(intro);//z jakiegoś powodu przy relative path lecą wyjątki
            musicMP3path = Path.GetFullPath(music);
            wmPlayer = new WindowsMediaPlayer();
            if(!introMP3path.Equals(""))
                wmPlayer.URL = introMP3path;
            else
                wmPlayer.URL = musicMP3path;
            Play();
            playerLoopTask = Task.Run(() => CheckIfFinished());
        }

        public void Pause()
        {
            wmPlayer.controls.pause();
            playing = false;
        }

        public void Play()
        {
            wmPlayer.controls.play();
            playing = true;
        }

        public void PlayNextTrack()
        {
            System.Console.WriteLine("Playing next track...");
            wmPlayer = new WindowsMediaPlayer();
            wmPlayer.URL = musicMP3path;
            Play();
        }

        public void CheckIfFinished()
        {
            while (true)
            {
                Thread.Sleep(5000);
                while (wmPlayer.controls.currentPosition>0)
                {
                    Thread.Sleep(10);
                }
                PlayNextTrack();
            }
        }
    }*/
}
